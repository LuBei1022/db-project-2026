using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;

namespace LiteratureManager.Common
{
    public class MakePic
    {
        /**/
        /// <summary>
        /// 生成缩略图
        /// </summary>
        /// <param name="originalImagePath">源图路径（物理路径）</param>
        /// <param name="thumbnailPath">缩略图路径（物理路径）</param>
        /// <param name="width">缩略图宽度</param>
        /// <param name="height">缩略图高度</param>
        /// <param name="mode">生成缩略图的方式</param>    
        public static bool MakeThumbnail(string originalImagePath, string thumbnailPath, int width, int height, string mode)
        {
            bool isyes = false;
            try
            {
                System.Drawing.Image originalImage = System.Drawing.Image.FromFile(originalImagePath);

                //HttpContext.Current.Response.Write(thumbnailPath);
                //HttpContext.Current.Response.End();

                int towidth = width;
                int toheight = height;

                int x = 0;
                int y = 0;
                int ow = originalImage.Width;
                int oh = originalImage.Height;

                switch (mode)
                {
                    case "HW"://指定高宽缩放（可能变形）                
                        break;
                    case "W"://指定宽，高按比例                    
                        toheight = originalImage.Height * width / originalImage.Width;
                        break;
                    case "H"://指定高，宽按比例
                        towidth = originalImage.Width * height / originalImage.Height;
                        break;
                    case "Cut"://指定高宽裁减（不变形）    

                        if ((double)originalImage.Width / (double)originalImage.Height > (double)towidth / (double)toheight)
                        {
                            oh = originalImage.Height;
                            ow = originalImage.Height * towidth / toheight;
                            y = 0;
                            x = (originalImage.Width - ow) / 2;
                        }
                        else
                        {
                            ow = originalImage.Width;
                            oh = originalImage.Width * height / towidth;
                            x = 0;
                            y = (originalImage.Height - oh) / 2;
                        }
                        break;
                    default:
                        break;
                }

                //新建一个bmp图片
                System.Drawing.Image bitmap = new System.Drawing.Bitmap(towidth, toheight);

                //新建一个画板
                System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(bitmap);

                //设置高质量插值法
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;

                //设置高质量,低速度呈现平滑程度
                g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;


                //清空画布并以透明背景色填充
                g.Clear(System.Drawing.Color.Transparent);

                //在指定位置并且按指定大小绘制原图片的指定部分
                g.DrawImage(originalImage, new System.Drawing.Rectangle(0, 0, towidth, toheight),
                    new System.Drawing.Rectangle(x, y, ow, oh),
                    System.Drawing.GraphicsUnit.Pixel);

                try
                {
                    //以jpg格式保存缩略图
                    bitmap.Save(thumbnailPath, System.Drawing.Imaging.ImageFormat.Png);
                    isyes = true;
                }
                catch (System.Exception e)
                {
                    throw e;
                }
                finally
                {
                    originalImage.Dispose();
                    bitmap.Dispose();
                    g.Dispose();
                }
            }
            catch (Exception)
            {

                throw;
            }
            return isyes;
        }


        public static void WaterMark(string old_Path, string thumb_Path, string txt, int txtWidth, string water_pic)
        {
            //define a string of text to use as the Copyright message
            string Copyright = txt;

            //create a image object containing the photograph to watermark
            Image imgPhoto = Image.FromFile(old_Path);
            int phWidth = imgPhoto.Width;
            int phHeight = imgPhoto.Height;

            //create a Bitmap the Size of the original photograph
            Bitmap bmPhoto = new Bitmap(phWidth, phHeight, PixelFormat.Format24bppRgb);

            bmPhoto.SetResolution(imgPhoto.HorizontalResolution, imgPhoto.VerticalResolution);

            //load the Bitmap into a Graphics object 
            Graphics grPhoto = Graphics.FromImage(bmPhoto);

            //create a image object containing the watermark

            //------------------------------------------------------------
            //Step #1 - Insert Copyright message
            //------------------------------------------------------------

            //Set the rendering quality for this Graphics object
            grPhoto.SmoothingMode = SmoothingMode.AntiAlias;

            //Draws the photo Image object at original size to the graphics object.
            grPhoto.DrawImage(
                imgPhoto,                               // Photo Image object
                new Rectangle(0, 0, phWidth, phHeight), // Rectangle structure
                0,                                      // x-coordinate of the portion of the source image to draw. 
                0,                                      // y-coordinate of the portion of the source image to draw. 
                phWidth,                                // Width of the portion of the source image to draw. 
                phHeight,                               // Height of the portion of the source image to draw. 
                GraphicsUnit.Pixel);                    // Units of measure 

            //-------------------------------------------------------
            //to maximize the size of the Copyright message we will 
            //test multiple Font sizes to determine the largest posible 
            //font we can use for the width of the Photograph
            //define an array of point sizes you would like to consider as possiblities
            if (txt != "" && txtWidth != 0)
            {

                int[] sizes = new int[] { 16, 14, 12, 10, 8, 6, 4 };

                Font crFont = null;
                SizeF crSize = new SizeF();

                //Loop through the defined sizes checking the length of the Copyright string
                //If its length in pixles is less then the image width choose this Font size.
                for (int i = 0; i < 7; i++)
                {
                    //set a Font object to Arial (i)pt, Bold
                    //crFont = new Font("arial", sizes[i], FontStyle.Bold); //自适应图片大小定义水印文字大小
                    crFont = new Font("黑体", 8, FontStyle.Bold);
                    //Measure the Copyright string in this Font
                    crSize = grPhoto.MeasureString(Copyright, crFont);

                    if ((ushort)crSize.Width < (ushort)phWidth)
                        break;
                }

                //Since all photographs will have varying heights, determine a 
                //position 5% from the bottom of the image
                int yPixlesFromBottom = (int)(phHeight * .05);

                //Now that we have a point size use the Copyrights string height 
                //to determine a y-coordinate to draw the string of the photograph
                float yPosFromBottom = ((phHeight - yPixlesFromBottom) - (crSize.Height / 2));

                //下面文字水印距左间距
                float xCenterOfImg = phWidth - txtWidth;

                //Define the text layout by setting the text alignment to centered
                StringFormat StrFormat = new StringFormat();
                StrFormat.Alignment = StringAlignment.Near;

                //define a Brush which is semi trasparent black (Alpha set to 30)
                SolidBrush semiTransBrush2 = new SolidBrush(Color.FromArgb(30, 0, 0, 0));

                //Draw the Copyright string
                grPhoto.DrawString(Copyright,                 //string of text
                    crFont,                                   //font
                    semiTransBrush2,                           //Brush
                    new PointF(xCenterOfImg + 1, yPosFromBottom + 1),  //Position
                    StrFormat);

                //define a Brush which is semi trasparent white (Alpha set to 30)
                SolidBrush semiTransBrush = new SolidBrush(Color.FromArgb(30, 255, 255, 255));

                //Draw the Copyright string a second time to create a shadow effect
                //Make sure to move this text 1 pixel to the right and down 1 pixel
                grPhoto.DrawString(Copyright,                 //string of text
                    crFont,                                   //font
                    semiTransBrush,                           //Brush
                    new PointF(xCenterOfImg, yPosFromBottom),  //Position
                    StrFormat);                               //Text alignment

                //Replace the original photgraphs bitmap with the new Bitmap
                imgPhoto = bmPhoto;
                grPhoto.Dispose();

                //save new image to file system.
                imgPhoto.Save(thumb_Path, ImageFormat.Jpeg);
                imgPhoto.Dispose();

            }


            if (water_pic != "" && water_pic != null)
            {
                //------------------------------------------------------------
                //Step #2 - Insert Watermark image
                //------------------------------------------------------------
                Image imgWatermark = new Bitmap(water_pic);
                int wmWidth = imgWatermark.Width;
                int wmHeight = imgWatermark.Height;
                //Create a Bitmap based on the previously modified photograph Bitmap
                Bitmap bmWatermark = new Bitmap(bmPhoto);
                bmWatermark.SetResolution(imgPhoto.HorizontalResolution, imgPhoto.VerticalResolution);
                //Load this Bitmap into a new Graphic Object
                Graphics grWatermark = Graphics.FromImage(bmWatermark);

                //To achieve a transulcent watermark we will apply (2) color 
                //manipulations by defineing a ImageAttributes object and 
                //seting (2) of its properties.
                ImageAttributes imageAttributes = new ImageAttributes();

                //The first step in manipulating the watermark image is to replace 
                //the background color with one that is trasparent (Alpha=0, R=0, G=0, B=0)
                //to do this we will use a Colormap and use this to define a RemapTable
                ColorMap colorMap = new ColorMap();

                //My watermark was defined with a background of 100% Green this will
                //be the color we search for and replace with transparency
                colorMap.OldColor = Color.FromArgb(60, 0, 0, 0);
                colorMap.NewColor = Color.FromArgb(60, 255, 255, 255);

                ColorMap[] remapTable = { colorMap };

                imageAttributes.SetRemapTable(remapTable, ColorAdjustType.Bitmap);

                //The second color manipulation is used to change the opacity of the 
                //watermark.  This is done by applying a 5x5 matrix that contains the 
                //coordinates for the RGBA space.  By setting the 3rd row and 3rd column 
                //to 0.3f we achive a level of opacity
                float[][] colorMatrixElements = {
new float[] {1.0f,  0.0f,  0.0f,  0.0f, 0.0f},
new float[] {0.0f,  1.0f,  0.0f,  0.0f, 0.0f},
new float[] {0.0f,  0.0f,  1.0f,  0.0f, 0.0f},
new float[] {0.0f,  0.0f,  0.0f,  0.3f, 0.0f},
new float[] {0.0f,  0.0f,  0.0f,  0.0f, 1.0f}};
                ColorMatrix wmColorMatrix = new ColorMatrix(colorMatrixElements);

                imageAttributes.SetColorMatrix(wmColorMatrix, ColorMatrixFlag.Default,
                    ColorAdjustType.Bitmap);

                //For this example we will place the watermark in the upper right
                //hand corner of the photograph. offset down 10 pixels and to the 
                //left 10 pixles

                int xPosOfWm = ((phWidth - wmWidth) - 10);
                int yPosOfWm = 10;

                grWatermark.DrawImage(imgWatermark,
                    new Rectangle(xPosOfWm, yPosOfWm, wmWidth, wmHeight),  //Set the detination Position
                    0,                  // x-coordinate of the portion of the source image to draw. 
                    0,                  // y-coordinate of the portion of the source image to draw. 
                    wmWidth,            // Watermark Width
                    wmHeight,		    // Watermark Height
                    GraphicsUnit.Pixel, // Unit of measurment
                    imageAttributes);   //ImageAttributes Object

                //Replace the original photgraphs bitmap with the new Bitmap
                imgPhoto = bmWatermark;
                grWatermark.Dispose();

                //save new image to file system.
                imgPhoto.Save(thumb_Path, ImageFormat.Jpeg);
                imgPhoto.Dispose();
                imgWatermark.Dispose();
            }


        }

        public static bool WaterMarkText(string path, string Addtit, string newpath, int fontsize, int x, int y)
        {
            bool str = false;
            try
            {
                System.Drawing.Image image = System.Drawing.Image.FromFile(path);
                Graphics g = Graphics.FromImage(image);
                g.DrawImage(image, 0, 0, image.Width, image.Height);
                Font f = new Font("Verdana", fontsize);
                Brush b = new SolidBrush(Color.Black);
                g.DrawString(Addtit, f, b, x, y);
                g.Dispose();
                image.Save(newpath);
                image.Dispose();
                str = true;
            }
            catch (Exception)
            {
            }
            finally
            {
                if (!path.Contains("zhengshu.jpg"))
                {
                    if (File.Exists(path))
                    {
                        File.Delete(path);
                    }
                }
            }
            return str;

        }



        public static bool WaterMarkImg(string path, string shuiyinpath, string newpath, int x, int y, int shuiyinw, int shuiyinh)
        {
            bool str = false;
            try
            {


                System.Drawing.Image image = System.Drawing.Image.FromFile(path);
                FileStream pFileStream = new FileStream(shuiyinpath, FileMode.Open, FileAccess.Read);
                System.Drawing.Image copyImage = Image.FromStream(pFileStream);
                Graphics g = Graphics.FromImage(image);

                g.DrawImage(copyImage, x, y, shuiyinw, shuiyinh);
                g.Dispose();
                image.Save(newpath);
                image.Dispose();
                copyImage.Dispose();
                str = true;
            }
            catch (Exception ex)
            {
                ImportDataLog.WriteLog(LogType.Error, ex.Message + "-" + ex.StackTrace);
            }
            finally
            {
                //if (File.Exists(path))
                //{
                //    File.Delete(path);
                //}
            }
            return str;


            //bool str = false;
            //try
            //{
            //    System.Drawing.Image image = System.Drawing.Image.FromFile(path);
            //    System.Drawing.Image copyImage = System.Drawing.Image.FromFile(shuiyinpath);
            //    Graphics g = Graphics.FromImage(image);

            //    g.DrawImage(copyImage, x, y, 224, 340);
            //    g.Dispose();
            //    image.Save(newpath);
            //    image.Dispose();
            //    str = true;
            //}
            //catch (Exception exs)
            //{
            //    ImportDataLog.WriteLog(LogType.Error, exs.Message + "-" + exs.StackTrace);
            //}
            //finally
            //{
            //    if (File.Exists(path))
            //    {
            //        File.Delete(path);
            //    }
            //}
            //return str;

        }
        public static bool CopyImg(string path, string newpath)
        {
            bool str = false;
            try
            {
                System.Drawing.Image image = System.Drawing.Image.FromFile(path);
                image.Save(newpath);
                image.Dispose();
                str = true;
            }
            catch (Exception exs)
            {
                ImportDataLog.WriteLog(LogType.Error, exs.Message + "-" + exs.StackTrace);
            }
            finally
            {
                //if (!path.Contains("zs_"))
                //{
                //    if (File.Exists(path))
                //    {
                //        File.Delete(path);
                //    }
                //}
            }
            return str;

        }

        public static bool WaterMarkTextS(string path, string[] Addtit, int[] Addx, int[] Addy, int[] Addfontsize, string newpath)
        {
            bool str = false;
            try
            {
                System.Drawing.Image image = System.Drawing.Image.FromFile(path);
                Graphics g = Graphics.FromImage(image);
                g.DrawImage(image, 0, 0, image.Width, image.Height);
                Brush b = new SolidBrush(Color.Black);
                for (int i = 0; i < Addtit.Length; i++)
                {
                    g.DrawString(Addtit[i], new Font("NSimSun", Addfontsize[i], FontStyle.Bold), b, Addx[i], Addy[i]);
                }
                g.Dispose();
                image.Save(newpath);
                image.Dispose();
                str = true;
            }
            catch (Exception exs)
            {
                ImportDataLog.WriteLog(LogType.Error, exs.Message + "-" + exs.StackTrace);
            }
            finally
            {
                //if (!path.Contains("zs_"))
                //{
                //    if (File.Exists(path))
                //    {
                //        File.Delete(path);
                //    }
                //}
            }
            return str;

        }
    }
}
