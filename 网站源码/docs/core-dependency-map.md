# 核心依赖图

```mermaid
flowchart TD
    U["用户端页面"] --> UC["Web/Inc/UserCommon.ashx.cs"]
    UC --> CU["LiteratureManager.Common/CommonUserFunc.cs"]
    CU --> M["Model"]
    CU --> B["BLLBase<T>"]
    B --> D["DALCommon<T> / DBHelper"]
    D --> SQL["SQL Server manage_db"]

    LI["Web/LiteratureInfo.aspx.cs"] --> LC["LiteratureComment"]
    LS["Web/LiteratureSearch.aspx.cs"] --> LC
    ALI["Web/admin/Admin_LiteratureInfo.aspx.cs"] --> LC
    ALL["Web/admin/Admin_LiteratureList.aspx.cs"] --> LC
    ACL["Web/admin/Admin_LiteratureCommentList.aspx"] --> LC

    LC --> L["Literature"]
    LC --> UTable["user_list"]
    LC -.兼容未迁移旧评论.-> SL["ServiceLog_List"]
    SL --> SLI["ServiceLogInfo_List"]

    CU --> Login["登录注册/短信验证码"]
    CU --> Points["积分/充值/兑换"]
    CU --> React["文献点赞/收藏"]
    CU --> Notice["NoticeLog_List"]
```

## 当前必须保护的核心链路

- 登录注册：`UserCommon.ashx.cs` -> `CommonUserFunc.GetUserLoginFunc` / `GetAddCodeFunc` / `GetSmsCodeFunc` -> `user_list` / `telcode_list`。
- 文献详情：`LiteratureInfo.aspx.cs` -> `Literature` / `LiteratureFile` / `LiteratureComment`。
- 评论：`LiteratureCommentAdd/Delete` -> `CommonUserFunc` -> `LiteratureComment`。
- 点赞收藏：`LiteratureReactionToggle` -> `LiteratureLike` / `LiteratureFavorite`。
- 积分充值兑换：`CommonUserFunc` -> `integrateLog_list` / `integrateExchangeLog_list` / `userpaylog_list`。
- 后台审核：`admin\Admin_LiteratureList.aspx.cs`、`admin\Admin_LiteratureCommentList.aspx.cs`。
