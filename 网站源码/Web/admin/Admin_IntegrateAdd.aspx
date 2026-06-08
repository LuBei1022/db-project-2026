<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Admin_IntegrateAdd.aspx.cs" Inherits="Web.admin.Admin_IntegrateAdd" %>

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
        .search-collapse, .select-table {
    background: #fff;
    border-radius: 6px;
    margin-top: 10px;
    padding: 5px 15px 20px 15px;
    box-shadow: 1px 1px 3px rgba(0, 0, 0, .2);
}.form-group {
    color: #333;
    margin: 5px 15px 5px 0px;
    display: flex;
    justify-content: flex-start;
    margin-bottom: 15px;
    align-items: center;
}.form-control {
    background: #FFFFFF;
    border: 1px solid #e5e6e7;
    border-radius: 4px;
    color: #333;
    padding: 3px 6px 4px;
    width: 100%;
    height: 31px;
    font-size: 14px;
}.btn-xs {
    padding: 1px 5px;
    font-size: 12px;
    line-height: 1.5;
    border-radius: 3px;
    margin: 0 5px;
    padding: 5px 10px;
    justify-content: center;
    align-items: center;
    display: flex;
    width: max-content;
    cursor: pointer;
    background: #F0F0EE;
}.btn-success {
    background-color: #1c84c6;
    border-color: #1c84c6;
    color: #FFFFFF;
    /* padding: 5px 10px; */
}.btn-list  input {
    outline: none;
    border: none;
}.button-group {
    /* width: calc(70% + 30px); */
    text-align: center;
    padding: 30px 0 30px calc(20% + 30px);
    display: flex;
    justify-content: flex-start;
}
.btn-list {
    display: flex;
    /* justify-content: center; */
    align-items: center;
}
    </style>
    <form id="form2" runat="server">
        <!--begin::App Content-->
        <div class="app-content">
            <asp:Panel ID="Main" runat="server">

                <!--begin::Container-->
                <div class="container-div" style="display: flex;
    justify-content: space-around;
    margin-bottom: 15px;
    align-items: center;">
                    <!---->
                    <div class="search-collapse" style="width:45%;">
                        <div class="edit-title">
                            <asp:Literal ID="Txt_Title" runat="server"></asp:Literal>
                        </div>
                        <div class="form-group">
                            <label class="control-label is-required">上传附件：</label>
                            <asp:FileUpload ID="FileUpload1" Style="border: 0px; padding: 0;" runat="server" accept="application/vnd.openxmlformats-officedocument.spreadsheetml.sheet, application/vnd.ms-excel" />
                        </div>
                        <div class="form-group">
                            <label class="control-label is-required">示例模板：</label>
                            <a href="excel_.xlsx" target="_blank" style="color: blue;">（点击下载模板）</a>
                        </div>
                        <div class=" btn-list button-group">
                            <asp:Button ID="Button1" Text=" 一键发放 " CssClass="btn-xs btn-success" runat="server" OnClick="OnClick_AddBatch" />
                        </div>
                    </div>
                    
                    <div class="search-collapse" style="width:45%;">
                        <div class="edit-title">
                            <asp:Literal ID="Literal1" runat="server"></asp:Literal>
                        </div>
                        <div class="form-group">
                            <label class="control-label is-required">电话：</label>
                           <asp:TextBox ID="user_tel" TextMode="SingleLine" runat="server" Width="450" CssClass="txt form-control" autocomplete="off"></asp:TextBox>
                        </div>
                        <div class="form-group">
                            <label class="control-label is-required">积分：</label>
                           <asp:TextBox ID="num_integrate" TextMode="SingleLine" runat="server" Width="450" CssClass="txt form-control" autocomplete="off"></asp:TextBox>
                        </div>
                        <div class=" btn-list button-group">
                            <asp:Button ID="Button2" Text=" 发 放 " CssClass="btn-xs btn-success" runat="server" OnClick="OnClick_AddUp" />
                        </div>
                    </div>
                    <!---->
                </div>
                <!--end::Container-->
                <!--begin::Container-->
                <div class="container-fluid">
                    <div class="col-md-12">
                        <!-- /.card -->
                        <div class="card mb-12">
                            <div class="card-body p-0">
                                <table class="table table-sm">
                                    <thead>
                                        <tr>
                                            <th>序号</th>
                                            <th>请求时间</th>
                                            <th>返回结果</th>
                                            <th>请求状态</th>
                                            <th>操作</th>
                                        </tr>
                                    </thead>
                                    <tbody>
                                        <asp:Repeater ID="Repeater1" runat="server">
                                            <ItemTemplate>
                                                <tr class="hover">
                                                    <td><%#Eval("xuhao").ToString()%></td>
                                                    <td><%#Function.HtmlDiscodeWeb(Eval("posttime").ToString())%></td>
                                                    <td>
                                                        <div class="teshutxt" style="<%#(Eval("status").ToString()=="-1"?"color:red": "")%>"><%#Function.HtmlDiscodeWeb(Eval("r_info").ToString())%></div>
                                                    </td>
                                                    <td><%#(Eval("status").ToString()=="1"?"任务执行完成":(Eval("status").ToString()=="-1"?"<span style=\"color:red\">任务执行失败</span>":"<span style=\"color:#0539f7\">任务执行中</span>"))%></td>
                                                    <td><%#(Eval("status").ToString()=="-1"?"<a class=\"badge text-bg-primary \" onclick=\"PopupFunc(this)\" data-url=\"Admin_DaoRuInfo.aspx?daoruid="+Eval("id").ToString()+"\" data-name=\"错误详情\">查看错误</a>":"")%></td>
                                                </tr>
                                            </ItemTemplate>
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
            </asp:Panel>
        </div>
        <!--end::App Content-->
    </form>
    <%} %>
</body>
</html>
