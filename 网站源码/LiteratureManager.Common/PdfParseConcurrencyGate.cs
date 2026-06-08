using System;
using System.Configuration;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace LiteratureManager.Common
{
    public static class PdfParseConcurrencyGate
    {
        private static readonly SemaphoreSlim LocalGate = new SemaphoreSlim(UploadPolicy.MaxPdfParseConcurrent);

        public static IDisposable TryEnter()
        {
            IDisposable lease;
            if (UploadPolicy.RedisEnabled && TryEnterRedis(out lease))
            {
                return lease;
            }

            if (!LocalGate.Wait(0))
            {
                return null;
            }

            return new ReleaseAction(delegate { LocalGate.Release(); });
        }

        public static bool TryAcquireThrottle(string operationKey, int seconds)
        {
            if (!UploadPolicy.RedisEnabled)
            {
                return true;
            }

            string key = UploadPolicy.RedisKeyPrefix + ":throttle:" + (operationKey ?? string.Empty);
            try
            {
                return string.Equals(
                    RedisCommand.Execute("SET", key, "1", "NX", "EX", Math.Max(1, seconds).ToString()),
                    "OK",
                    StringComparison.OrdinalIgnoreCase);
            }
            catch
            {
                return true;
            }
        }

        private static bool TryEnterRedis(out IDisposable lease)
        {
            lease = null;
            string token = Guid.NewGuid().ToString("N");
            try
            {
                for (int slot = 1; slot <= UploadPolicy.MaxPdfParseConcurrent; slot++)
                {
                    string key = UploadPolicy.RedisKeyPrefix + ":pdfparse:slot:" + slot;
                    string response = RedisCommand.Execute(
                        "SET", key, token, "NX", "EX", UploadPolicy.PdfParseLeaseSeconds.ToString());
                    if (string.Equals(response, "OK", StringComparison.OrdinalIgnoreCase))
                    {
                        lease = new RedisLease(key, token);
                        return true;
                    }
                }

                // Redis responded successfully, but all distributed slots are busy.
                return true;
            }
            catch
            {
                // Keep the feature usable during a temporary Redis outage.
                return false;
            }
        }

        private sealed class RedisLease : IDisposable
        {
            private readonly string key;
            private readonly string token;
            private int disposed;

            public RedisLease(string key, string token)
            {
                this.key = key;
                this.token = token;
            }

            public void Dispose()
            {
                if (Interlocked.Exchange(ref disposed, 1) != 0)
                {
                    return;
                }

                try
                {
                    RedisCommand.Execute(
                        "EVAL",
                        "if redis.call('get', KEYS[1]) == ARGV[1] then return redis.call('del', KEYS[1]) else return 0 end",
                        "1",
                        key,
                        token);
                }
                catch
                {
                    // The expiry releases an abandoned lease if Redis is unreachable here.
                }
            }
        }

        private sealed class ReleaseAction : IDisposable
        {
            private readonly Action release;
            private int disposed;

            public ReleaseAction(Action release)
            {
                this.release = release;
            }

            public void Dispose()
            {
                if (Interlocked.Exchange(ref disposed, 1) == 0)
                {
                    release();
                }
            }
        }

        private static class RedisCommand
        {
            public static string Execute(params string[] arguments)
            {
                using (TcpClient client = Connect())
                using (NetworkStream stream = client.GetStream())
                {
                    stream.ReadTimeout = UploadPolicy.RedisTimeoutMs;
                    stream.WriteTimeout = UploadPolicy.RedisTimeoutMs;

                    if (!string.IsNullOrWhiteSpace(UploadPolicy.RedisPassword))
                    {
                        Write(stream, "AUTH", UploadPolicy.RedisPassword);
                        EnsureOk(Read(stream));
                    }

                    if (UploadPolicy.RedisDatabase > 0)
                    {
                        Write(stream, "SELECT", UploadPolicy.RedisDatabase.ToString());
                        EnsureOk(Read(stream));
                    }

                    Write(stream, arguments);
                    return Read(stream);
                }
            }

            private static TcpClient Connect()
            {
                TcpClient client = new TcpClient();
                IAsyncResult connect = client.BeginConnect(UploadPolicy.RedisHost, UploadPolicy.RedisPort, null, null);
                try
                {
                    if (!connect.AsyncWaitHandle.WaitOne(UploadPolicy.RedisTimeoutMs))
                    {
                        client.Close();
                        throw new TimeoutException("Redis connection timed out.");
                    }

                    client.EndConnect(connect);
                    return client;
                }
                finally
                {
                    connect.AsyncWaitHandle.Close();
                }
            }

            private static void Write(Stream stream, params string[] arguments)
            {
                StringBuilder command = new StringBuilder();
                command.Append("*");
                command.Append(arguments.Length);
                command.Append("\r\n");
                foreach (string argument in arguments)
                {
                    byte[] bytes = Encoding.UTF8.GetBytes(argument ?? string.Empty);
                    command.Append("$");
                    command.Append(bytes.Length);
                    command.Append("\r\n");
                    command.Append(argument ?? string.Empty);
                    command.Append("\r\n");
                }

                byte[] payload = Encoding.UTF8.GetBytes(command.ToString());
                stream.Write(payload, 0, payload.Length);
            }

            private static string Read(Stream stream)
            {
                int marker = stream.ReadByte();
                if (marker < 0)
                {
                    throw new IOException("Redis closed the connection.");
                }

                string line = ReadLine(stream);
                if (marker == '+')
                {
                    return line;
                }
                if (marker == '-')
                {
                    throw new IOException("Redis error: " + line);
                }
                if (marker == ':')
                {
                    return line;
                }
                if (marker == '$')
                {
                    int length;
                    if (!int.TryParse(line, out length) || length < 0)
                    {
                        return null;
                    }

                    byte[] buffer = new byte[length];
                    int read = 0;
                    while (read < length)
                    {
                        int current = stream.Read(buffer, read, length - read);
                        if (current <= 0)
                        {
                            throw new IOException("Redis response was incomplete.");
                        }
                        read += current;
                    }
                    ReadLine(stream);
                    return Encoding.UTF8.GetString(buffer);
                }

                throw new IOException("Unsupported Redis response.");
            }

            private static string ReadLine(Stream stream)
            {
                StringBuilder line = new StringBuilder();
                int current;
                while ((current = stream.ReadByte()) >= 0)
                {
                    if (current == '\r')
                    {
                        if (stream.ReadByte() != '\n')
                        {
                            throw new IOException("Redis response line was invalid.");
                        }
                        return line.ToString();
                    }
                    line.Append((char)current);
                }

                throw new IOException("Redis response ended unexpectedly.");
            }

            private static void EnsureOk(string response)
            {
                if (!string.Equals(response, "OK", StringComparison.OrdinalIgnoreCase))
                {
                    throw new IOException("Redis command failed.");
                }
            }
        }
    }
}
