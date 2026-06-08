using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class WorkflowTaskReaction_list
    {
            private long _task_comment_id;
            /// <summary>
            /// 
            /// </summary>
            public long task_comment_id
        {
                set { _task_comment_id = value; }
                get { return _task_comment_id; }
            }
            private int _user_id;
            /// <summary>
            /// 
            /// </summary>
            public int user_id
            {
                set { _user_id = value; }
                get { return _user_id; }
            }
            private DateTime _addtime;
            /// <summary>
            /// 
            /// </summary>
            public DateTime addtime
            {
                set { _addtime = value; }
                get { return _addtime; }
            }
        }
    }

