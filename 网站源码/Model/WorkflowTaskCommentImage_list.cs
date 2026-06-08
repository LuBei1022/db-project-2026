using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
     public class WorkflowTaskCommentImage_list
    {
            private long _WorkflowTaskComment_Id;
            /// <summary>
            /// 
            /// </summary>
            public long WorkflowTaskComment_Id
        {
                set { _WorkflowTaskComment_Id = value; }
                get { return _WorkflowTaskComment_Id; }
            }
            private string _upload_pic_info;
            /// <summary>
            /// 
            /// </summary>
            public string upload_pic_info
            {
                set { _upload_pic_info = value; }
                get { return _upload_pic_info; }
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
            private int _orderid;
            /// <summary>
            /// 
            /// </summary>
            public int orderid
            {
                set { _orderid = value; }
                get { return _orderid; }
            }
        }
    }
