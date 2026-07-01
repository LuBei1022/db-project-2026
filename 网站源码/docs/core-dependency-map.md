# 核心依赖图

```mermaid
flowchart TD
    U["用户端页面"] --> UC["Web/Inc/UserCommon.ashx.cs"]
    UC --> CU["LiteratureManager.Common/CommonUserFunc.cs"]
    CU --> M["Model"]
    CU --> B["BLLBase<T>"]
    B --> D["DALCommon<T> / DBHelper"]
    D --> SQL["SQL Server manage_db_final"]

    LI["Web/LiteratureInfo.aspx.cs"] --> L["Literature"]
    LI --> LC["LiteratureComment"]
    LI --> LF["LiteratureFile"]
    LI --> LA["LiteratureAuthorMap"]
    LA --> A["Author"]
    LA --> AIM["LiteratureAuthorInstitutionMap"]
    AIM --> I["Institution"]

    LS["Web/LiteratureSearch.aspx.cs"] --> L
    ALI["Web/admin/Admin_LiteratureInfo.aspx.cs"] --> L
    ALE["Web/admin/Admin_LiteratureEdit.aspx.cs"] --> L
    ACL["Web/admin/Admin_LiteratureCommentList.aspx"] --> LC
    AAL["Web/admin/Admin_AuthorList.aspx.cs"] --> A
    AIL["Web/admin/Admin_InstitutionList.aspx.cs"] --> I
    AJL["Web/admin/Admin_JournalList.aspx.cs"] --> J["Journal"]
    ACF["Web/admin/Admin_ConferenceList.aspx.cs"] --> C["Conference"]

    LC --> L
    LC --> UTable["user_list"]
    L --> J
    L --> C

    CU --> Login["登录注册/短信验证码"]
    CU --> Points["积分/充值/兑换"]
    CU --> React["文献点赞/收藏"]
    CU --> Notice["NoticeLog_List / ServiceLog_List"]
```

## 当前必须保护的核心链路

- 登录注册：`UserCommon.ashx.cs` -> `CommonUserFunc.GetUserLoginFunc` / `GetAddCodeFunc` / `GetSmsCodeFunc` -> `user_list` / `telcode_list`。
- 文献详情：`LiteratureInfo.aspx.cs` -> `Literature` / `LiteratureFile` / `LiteratureComment` / `LiteratureAuthorMap` / `LiteratureAuthorInstitutionMap`。
- 评论：`LiteratureCommentAdd/Delete` -> `CommonUserFunc` -> `LiteratureComment`。
- 点赞收藏：`LiteratureReactionToggle` -> `LiteratureLike` / `LiteratureFavorite`。
- 作者机构：后台作者、机构和文献编辑页面 -> `Author` / `Institution` / `AuthorInstitutionHistory` / `LiteratureAuthorMap` / `LiteratureAuthorInstitutionMap`。
- 期刊会议：后台期刊、会议和文献编辑页面 -> `Journal` / `Conference`；上传/导入时由 `LiteratureVenueSync` 按文献类型同步主数据。
- 积分充值兑换：`CommonUserFunc` -> `integrateLog_list` / `integrateExchangeLog_list` / `userpaylog_list`。
- 后台审核：`admin/Admin_LiteratureList.aspx.cs`、`admin/Admin_LiteratureCommentList.aspx.cs`。
