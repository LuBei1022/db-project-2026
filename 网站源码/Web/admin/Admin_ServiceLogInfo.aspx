<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Admin_ServiceLogInfo.aspx.cs" Inherits="Web.admin.Admin_ServiceLogInfo" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
</head>
<body>
    <%@ Register TagPrefix="LiteratureManager" TagName="Inc" Src="Inc.ascx" %>
    <%@ Register TagPrefix="LiteratureManager" TagName="class_menu" Src="class_menu.ascx" %>
    <%if (isLoading)
        {  %>
    <LiteratureManager:Inc ID="Inc2" runat="server" />
    <LiteratureManager:class_menu ID="class_menu" runat="server" />
    <style>
        
.record-list{
    border-top: 1px solid #e8e8e8;
    padding-top: 15px;
}
.record-item{
        display: flex;
    justify-content: space-between;
    padding: 10px 0px;
}
.record-img{
    width: 30px;
    height: 30px;
}
.record-img img{
    width: 100%;
    height: 100%;
    object-fit: cover;
    border-radius: 100%;
}
.record-text{
    width: calc(100% - 40px);
}
.record-text h4{
    line-height: 30px;
    font-size: 14px;
    color: #666666;
    font-weight: 400;
}
.record-text h4 span{
    margin-left: 20px;
    font-size: 14px;
    color: #999999;
}
.record-text p{
font-size: 14px;
    color: #333;
    line-height: 24px;
    margin-top: 10px;
}
.record-text img{
    max-width: 100%;
    height: auto;
}
.record-form{
        padding-top: 20px;
    padding-bottom: 50px;
}
.record-extare{
    margin-bottom: 20px;
}
.record-extare textarea{
display: block;
    width: 100%;
    height: 250px;
    padding: 20px;
    border: none;
    resize: none;
    background: #f3f3f3;
    border-radius: 10px;
  }
  .comment-audit-panel{
      margin: 18px 0 8px;
      padding: 16px;
      border: 1px solid #d9e7ff;
      border-left: 4px solid #0d6efd;
      border-radius: 8px;
      background: #f8fbff;
  }
  .comment-audit-title{
      font-weight: 700;
      font-size: 16px;
      color: #1f2937;
      margin-bottom: 8px;
  }
  .comment-audit-status{
      color: #4b5563;
      margin-bottom: 12px;
  }
  .comment-audit-actions{
      display: flex;
      flex-wrap: wrap;
      gap: 8px;
  }
  .comment-audit-actions a{
      text-decoration: none;
  }
    </style>
    <form id="form2" runat="server">
        <!--begin::App Content-->
        <div class="app-content">
            <asp:Panel ID="AddUp" runat="server" >
                <%=Function.KindEditor("info_", 1)%>
                <!--begin::Container-->
                <div class="container-fluid">
                    <!--begin::Row-->
                    <div class="row">
                        <!--begin::Col-->
                        <div class="col-md-6 offset-md-3">
                            <!--begin::Quick Example-->
                            <div class="card card-primary card-outline mb-4">
                                <!--begin::Header-->
                                <div class="card-header">
                                    <div class="card-title">
                                        <asp:Label ID="Txt_Title" runat="server"></asp:Label>
                                    </div>
                                </div>
                                <!--end::Header-->
                                <!--begin::Form-->
                                <!--begin::Body-->
                                <div class="card-body">
                                    <div class="mb-6 ">
                                        <div class="record-list">
                                            <h4><%=Function.HtmlDiscode(ServiceLog_List.name) %></h4>
                                            <%if (!string.IsNullOrWhiteSpace(ServiceLog_List.info_))
                                                {  %>
                                            <div class="record-item">
                                                <div class="record-img">
                                                    <img src="<%=upload_pic_avatar %>" />
                                                </div>
                                                <div class="record-text">
                                                    <h4><%=user_name %> <span><%=ServiceLog_List.addtime.ToString("yyyy-MM-dd HH:mm:ss") %></span></h4>
                                                    <div><%=Function.Replace_Content(ServiceLog_List.info_) %></div>
                                                 </div>
                                             </div>
                                             <%} %>
                                             <%=LiteratureCommentAuditHtml %>
                                             <asp:Repeater ID="DataList" runat="server">
                                                <ItemTemplate>
                                                    <div class="record-item">
                                                        <div class="record-img">
                                                            <img src="<%#(Eval("type").ToString()=="1"?upload_pic_avatar:(Eval("type").ToString()=="2"?"/images/kefu.jpg":"null")) %>" />
                                                        </div>
                                                        <div class="record-text">
                                                            <h4><%#(Eval("type").ToString()=="1"?Function.HtmlDiscode(user_name):(Eval("type").ToString()=="2"?"客服":"null")) %> <span><%#Function.ConvertTo<DateTime>(Eval("addtime").ToString(),DateTime.MinValue).ToString("yyyy-MM-dd HH:mm:ss") %></span></h4>
                                                            <div><%#Function.Replace_Content(Eval("info_").ToString()) %></div>
                                                        </div>
                                                    </div>
                                                </ItemTemplate>
                                            </asp:Repeater>
                                        </div>
                                    </div>
                                </div>

                                <div class="card-body">
                                    <div class="mb-3">
                                        <asp:TextBox ID="info_" placeholder="回 复" TextMode="MultiLine" runat="server" Style="visibility: hidden;width:100%;height:300px;"></asp:TextBox>
                                    </div>
                                </div>
                                <!--end::Body--> 
                                <!--begin::Footer-->
                                <div class="card-footer">
                                    <asp:Button ID="Button3" Text=" 回 复 " CssClass="btn btn-primary" runat="server" OnClick="OnClick_AddUp" />
                                    <a id="button" class="btn submit-but" href="<%=Server.HtmlEncode(BackUrl) %>"> 返 回 </a>
                                </div>
                                <!--end::Footer-->
                                <!--end::Form-->
                            </div>
                            <!--end::Quick Example-->
                        </div>
                        <!--end::Col-->
                    </div>
                    <!--end::Row-->
                </div>
                <!--end::Container-->
            </asp:Panel>
        </div>
        <!--end::App Content-->
    </form>
    <%} %>
</body>
</html>
