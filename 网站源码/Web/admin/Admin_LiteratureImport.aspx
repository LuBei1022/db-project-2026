<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Admin_LiteratureImport.aspx.cs" Inherits="Web.admin.Admin_LiteratureImport" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
</head>
<body>
    <%@ Register TagPrefix="LiteratureManager" TagName="Inc" Src="Inc.ascx" %>
    <%@ Register TagPrefix="LiteratureManager" TagName="class_menu" Src="class_menu.ascx" %>
    <% if (isLoading) { %>
    <LiteratureManager:Inc ID="Inc2" runat="server" />
    <LiteratureManager:class_menu ID="class_menu" runat="server" />

    <form id="form2" runat="server">
        <div class="app-content">
            <asp:Panel ID="Main" runat="server">
                <div class="container-fluid">
                    <div class="card mb-4">
                        <div class="card-header">
                            <div class="card-title">&#25209;&#37327;&#23548;&#20837;&#25991;&#29486;</div>
                        </div>
                        <div class="card-body">
                            <div class="mb-3">
                                <label class="form-label">CSV &#25991;&#20214;</label>
                                <asp:FileUpload ID="import_file" runat="server" accept=".csv" />
                                <asp:Button ID="ButtonImport" runat="server" Text=" &#24320; &#22987; &#23548; &#20837; " CssClass="btn btn-primary" OnClick="OnClick_Import" style="margin-left: 10px;" />
                            </div>
                            <div class="alert alert-info" style="margin-bottom: 0;">
                                <div><strong>&#23548;&#20837;&#35828;&#26126;</strong></div>
                                <div style="margin-top: 8px;">&#30446;&#21069;&#25903;&#25345; CSV&#65292;&#31532;&#19968;&#34892;&#20026;&#34920;&#22836;&#12290;</div>
                                <div style="margin-top: 8px;">&#25512;&#33616;&#23383;&#27573;&#65306;<code>title,subtitle,author_names,institution,doi,keywords,abstract_text,source_type,language,publish_year,journal_name,conference_name,publisher,volume,issue,pages,category_id,category_name,tag_names,external_url,source_db,remark,status,is_top</code></div>
                            </div>
                        </div>
                    </div>

                    <div class="card mb-12">
                        <div class="card-header">
                            <div class="card-title">&#26368;&#36817;&#23548;&#20837;&#25209;&#27425;</div>
                        </div>
                        <div class="card-body p-0">
                            <table class="table table-sm">
                                <thead>
                                    <tr>
                                        <th>ID</th>
                                        <th>&#25209;&#27425;&#21517;&#31216;</th>
                                        <th>&#25991;&#20214;&#21517;</th>
                                        <th>&#24635;&#25968;</th>
                                        <th>&#25104;&#21151;</th>
                                        <th>&#22833;&#36133;</th>
                                        <th>&#29366;&#24577;</th>
                                        <th>&#23548;&#20837;&#26102;&#38388;</th>
                                        <th class="textAlignC">&#25805;&#20316;</th>
                                    </tr>
                                </thead>
                                <tbody>
                                    <asp:Repeater ID="Repeater1" runat="server">
                                        <ItemTemplate>
                                            <tr class="hover">
                                                <td><%# Eval("id") %></td>
                                                <td><%# Function.HtmlDiscodeWeb(Eval("batch_name").ToString()) %></td>
                                                <td><%# Function.HtmlDiscodeWeb(Eval("file_name").ToString()) %></td>
                                                <td><%# Eval("total_count") %></td>
                                                <td><%# Eval("success_count") %></td>
                                                <td><%# Eval("fail_count") %></td>
                                                <td><%# GetBatchStatus(Eval("status")) %></td>
                                                <td><%# Function.ConvertTo<DateTime>(Eval("addtime").ToString(),DateTime.MinValue).ToString("yyyy-MM-dd HH:mm:ss") %></td>
                                                <td class="textAlignC">
                                                    <a class="badge text-bg-info" href='Admin_LiteratureImportError.aspx?MenuId=<%=MenuId %>&BatchId=<%# Eval("id") %>'>&#38169;&#35823;&#26126;&#32454;</a>
                                                </td>
                                            </tr>
                                        </ItemTemplate>
                                    </asp:Repeater>
                                    <asp:Panel ID="DivNull" runat="server" Visible="true">
                                        <tr>
                                            <td colspan="9" style="text-align: center;">&#26080;&#23548;&#20837;&#35760;&#24405;!</td>
                                        </tr>
                                    </asp:Panel>
                                </tbody>
                            </table>
                        </div>
                    </div>
                </div>
            </asp:Panel>
        </div>
    </form>
    <% } %>
</body>
</html>
