<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Admin_MsgList.aspx.cs" Inherits="Web.admin.Admin_MsgList" %>

<%@ Register TagPrefix="LiteratureManager" TagName="Inc" Src="Inc.ascx" %>
<%@ Register TagPrefix="LiteratureManager" TagName="class_menu" Src="class_menu.ascx" %>
<form id="form2" runat="server">
    <LiteratureManager:Inc ID="Inc1" runat="server" />
    <LiteratureManager:class_menu ID="class_menu" runat="server" />
    <!--begin::App Content-->
    <div class="app-content">
        <!--begin::Container-->
        <div class="container-fluid">
            <div class="col-md-12">
                <!-- /.card -->
                <div class="card mb-12">
                    <div class="card-header cardList">
                        <div class="cardItem">
                            <label class="col-form-label">时间选择</label>
                            <div class="layui-inline" id="test6">
                                <div class="col-form-input">
                                    <asp:TextBox ID="SearchStartTime" TextMode="SingleLine" autocomplete="off" CssClass="form-control" runat="server"></asp:TextBox>
                                </div>
                                <div class="layui-form-mid">-</div>
                                <div class="col-form-input">
                                    <asp:TextBox ID="SearchEndTime" TextMode="SingleLine" autocomplete="off" CssClass="form-control" runat="server"></asp:TextBox>
                                </div>
                            </div>
                        </div>
                        <div class="cardItem">
                            <asp:Button ID="Button2" runat="server" OnClick="OnClick_Search" Text="搜索" CssClass="btn btn-success" />
                        </div>
                    </div>
                    <div class="card-body p-0">
                        <table class="table table-sm">
                            <thead>
                                <tr>
                                    <th>序号</th>
                                    <th>姓名</th>
                                    <th>电话</th>
                                    <th>邮箱</th>
                                    <th>公司名</th>
                                    <th>咨询问题</th>
                                    <th>来源</th>
                                    <th>栏目</th>
                                    <th>留言日期</th>
                                </tr>
                            </thead>
                            <tbody>
                                <asp:Repeater ID="Repeater1" runat="server">
                                    <itemtemplate>
                                        <tr class="hover">
                                            <td><%#Eval("xuhao").ToString()%></td>
                                            <td><%#Function.HtmlDiscodeWeb(Eval("name").ToString())%></td>
                                            <td><%#Function.HtmlDiscodeWeb(Eval("tel").ToString())%></td>
                                            <td><%#Function.HtmlDiscodeWeb(Eval("email").ToString())%></td>
                                            <td><%#Function.HtmlDiscodeWeb(Eval("companyname").ToString())%></td>
                                            <td><%#Function.HtmlDiscodeWeb(Eval("info").ToString())%></td>
                                            <td><%#(Eval("type").ToString() == "1" ? "中国站" : (Eval("type").ToString() =="2"?"英文站":(Eval("type").ToString() =="3"?"日文站":"")))%></td>
                                            <td><%#Function.HtmlDiscodeWeb(Eval("tbl_classname").ToString())+(!string.IsNullOrWhiteSpace(Eval("classname").ToString())?" - "+Function.HtmlDiscode(Eval("classname").ToString()) :"")%></td>
                                            <td><%#Eval("addtime").ToString()%></td>
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
                            <Webdiyer:AspNetPager ID="AspNetPager1" runat="server" CurrentPageButtonClass="current" FirstPageText="Home" PrevPageText="Prev" NextPageText="Next" LastPageText="End"
                                ShowDisabledButtons="true" OnPageChanged="AspNetPager1_PageChanged" UrlPaging="true" PageIndexBoxClass="input_page" PageIndexBoxType="TextBox" SubmitButtonClass="go" SubmitButtonText="GO" ShowPageIndexBox="Always">
                            </Webdiyer:AspNetPager>
                        </div>
                    </div>
                    <!-- /.card-body -->
                    <link rel="stylesheet" href="css/layui.css" />
                    <script src="js/layui.js"></script>
                    <script>
                    layui.use('laydate', function(){
                        var laydate = layui.laydate;
                          laydate.render({
                            elem: '#test6',
                            range: ['#SearchStartTime', '#SearchEndTime']
                        });
                    })
                    </script>
                </div>
                <!-- /.card -->
            </div>
        </div>
        <!--end::Container-->
    </div>
    <!--end::App Content-->
</form>
