<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Left.aspx.cs" Inherits="Web.admin.Left" %>
<!--begin::Sidebar Menu-->
          <ul class="nav sidebar-menu flex-column" data-lte-toggle="treeview" role="menu" data-accordion="false" >
              <asp:repeater id="myRepeater" runat="server">
<ItemTemplate> 
                <li class="nav-item">
                    <a class="nav-link">
                        <i class="nav-icon bi bi-box-arrow-in-right"></i>
                        <p>
                            <%#Function.HtmlDiscode(Eval("popedom_name").ToString()) %>
                            <i class="nav-arrow bi bi-chevron-right"></i>
                        </p>
                    </a>  
                    <%#GetPopedomChildren2(Eval("id").ToString()) %>
                </li>
</ItemTemplate>
</asp:repeater>   
          </ul>
          <!--end::Sidebar Menu-->
  <script src="js/adminlte.js"></script>