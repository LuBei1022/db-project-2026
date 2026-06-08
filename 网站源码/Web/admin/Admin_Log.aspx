<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Admin_Log.aspx.cs" Inherits="Web.admin.Admin_Log" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
</head>
<body>
<%@ Register TagPrefix="LiteratureManager" TagName="Inc" Src="Inc.ascx" %>
<%@ Register TagPrefix="LiteratureManager" TagName="class_menu" Src="class_menu.ascx" %>
     <%if (isLoading)
    {  %>
<LiteratureManager:Inc ID="Inc1" runat="server" />
<LiteratureManager:class_menu ID="class_menu" runat="server" />
<!--begin::App Content-->
<div class="app-content">
    <!--begin::Container-->
    <div class="container-fluid">
        <div class="col-md-12">
            <!-- /.card -->
            <div class="card mb-12">
                <div class="card-body p-0">
                    <table class="table table-sm">
                        <thead>
                            <tr>
                                <th>选择</th>
                                <th>管理姓名</th>
                                <th>操作日期</th>
                                <th>IP地址</th>
                                <th class="textAlignL">操作记录</th>
                            </tr>
                        </thead>
                        <tbody>
                            <asp:Repeater ID="Repeater1" runat="server">
                                <itemtemplate>
                                    <tr class="hover">
                                        <td >
                                            <input name="id" type="checkbox" class="checkbox" value="<%#Eval("id")%>"></td>
                                        <td><%#Function.HtmlDiscodeWeb(Eval("UserName").ToString())%></td>
                                        <td><%#Eval("Time")%></td>
                                        <td><%#Eval("Ip")%></td>
                                        <td class="textAlignL"><%#Function.HtmlDiscodeWeb(Eval("Content").ToString())%></td>
                                    </tr>
                                </itemtemplate>
                            </asp:Repeater>
                            <asp:Panel ID="DivNull" runat="server" Visible="true">
                                <tr>
                                    <td colspan="10" style="text-align: center;">无相关数据!</td>
                                </tr>
                            </asp:Panel>
                        </tbody>
                    </table>
                    <div class="msdn">
                        <div></div>
                        <Webdiyer:AspNetPager ID="AspNetPager1" runat="server" CurrentPageButtonClass="current" FirstPageText="Home" PrevPageText="Prev" NextPageText="Next" LastPageText="End"
                            ShowDisabledButtons="true" OnPageChanged="AspNetPager1_PageChanged" UrlPaging="true" PageIndexBoxClass="input_page" PageIndexBoxType="TextBox" SubmitButtonClass="go" SubmitButtonText="GO" ShowPageIndexBox="Always">
                        </Webdiyer:AspNetPager>
                    </div>
                </div>
                <!-- /.card-body -->
            </div>
            <!-- /.card -->
        </div>
    </div>
    <!--end::Container-->
</div>
<!--end::App Content-->
      <%} %>
    </body>
</html>