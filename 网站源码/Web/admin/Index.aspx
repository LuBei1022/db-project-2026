<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Index.aspx.cs" Inherits="Web.admin.Index" %>
<!doctype html>
<html lang="en">

<head>
  <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
  <title>文献管理后台</title>
  <meta name="viewport" content="width=device-width, initial-scale=1.0" />
  <link rel="stylesheet" href="css/index.css" />
  <link rel="stylesheet" href="css/overlayscrollbars.min.css" />
  <link rel="stylesheet" href="css/bootstrap-icons.min.css" />
  <link rel="stylesheet" href="css/adminlte.css" />
  <link rel="stylesheet" href="css/layui.css" />
  <script src="../Inc/jquery.min.js"></script>
</head>

<body class="layout-fixed sidebar-expand-lg bg-body-tertiary">
  <div class="app-wrapper">
    <!--begin::Header-->
    <nav class="app-header navbar navbar-expand bg-body">
      <!--begin::Container-->
      <div class="container-fluid">
        <!--begin::Start Navbar Links-->
        <ul class="navbar-nav">
          <li class="nav-item">
            <a class="nav-link" data-lte-toggle="sidebar" href="#" role="button">
              <i class="bi bi-list"></i>
            </a>
          </li>
        </ul>
        <!--end::Start Navbar Links-->
        <!--begin::End Navbar Links-->
        <ul class="navbar-nav ms-auto">
          <li class="nav-item  user-menu">
            <a href="/" target="_blank" class="nav-link dropdown-toggle">
              <span class="d-none d-md-inline">访问首页</span>
            </a>
          </li>
          <li class="nav-item  user-menu">
            <a class="nav-link dropdown-toggle">
              <span class="d-none d-md-inline"><%=LiteratureManager.Common.Cookie.GetCookie("LMS_AdminName")%></span>
            </a>
          </li>
          <li class="nav-item  user-menu">
            <a href="Admin_AdminUpPwd.aspx?MenuId=587" class="nav-link dropdown-toggle" hidefocus="true" target="main">
              <span class="d-none d-md-inline">修改密码</span>
            </a>
          </li>
          <li class="nav-item  user-menu">
            <a href="Right.aspx?MenuID=Right&Action=logout" class="nav-link dropdown-toggle">
              <span class="d-none d-md-inline">退出登录</span>
            </a>
          </li>
        </ul>
        <!--end::End Navbar Links-->
      </div>
      <!--end::Container-->
    </nav>
    <!--end::Header-->
    <!--begin::Sidebar-->
    <aside class="app-sidebar bg-body-secondary shadow" data-bs-theme="dark">
      <!--begin::Sidebar Brand-->
      <div class="sidebar-brand">
        <!--begin::Brand Link-->
        <a href="index.aspx" class="brand-link">
          <span class="brand-text fw-light" style="margin-left: 0px;">文献管理后台</span>
        </a>
        <!--end::Brand Link-->
      </div>
      <!--end::Sidebar Brand-->
      <!--begin::Sidebar Wrapper-->
      <div class="sidebar-wrapper">
        <nav class="mt-2" id="leftmenu">
          
        </nav>
      </div>
      <!--end::Sidebar Wrapper-->
    </aside>
    <!--end::Sidebar-->
    <!--begin::App Main-->
    <main class="app-main" id="mainframes">
     <iframe src="Admin_LiteratureList.aspx?MenuId=1723" id="main" name="main" onload="" width="100%" height="100%" frameborder="0" scrolling="yes" style="overflow: visible;display:"></iframe>
    </main>
    <!--end::App Main-->
    <!--begin::Footer-->
    <footer class="app-footer"></footer>
    <!--end::Footer-->
  </div>
<script type="text/JavaScript">
    $.ajaxSetup({ cache: false });
    $("#leftmenu").load("Left.aspx");
    parent.main.location = "Admin_LiteratureList.aspx?MenuId=1723";
</script>
  <script src="js/overlayscrollbars.browser.es6.min.js"></script>
  <script src="js/popper.min.js"></script>
  <script src="js/bootstrap.min.js"></script>
  <!-- <script src="js/layui.js"></script> -->
</body>

</html>
