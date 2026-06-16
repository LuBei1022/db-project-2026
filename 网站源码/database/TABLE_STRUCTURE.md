# 数据库表结构

数据库：`manage_db_final`

## dbo.admin

| 序号 | 字段 | 类型 | 允许空 | 标识列 | 默认值 | 说明 |
|---:|---|---|---|---|---|---|
| 1 | `id` | `int` | NO | True |  |  |
| 2 | `username` | `nvarchar(50)` | NO | False |  |  |
| 3 | `password` | `nvarchar(50)` | YES | False |  |  |
| 4 | `popedom` | `nvarchar(MAX)` | YES | False |  |  |
| 5 | `lastloginip` | `nvarchar(50)` | YES | False |  |  |
| 6 | `cityid` | `int` | YES | False |  |  |
| 7 | `locks` | `int` | YES | False |  |  |
| 8 | `code` | `nvarchar(50)` | YES | False |  |  |
| 9 | `lastlogindate` | `datetime` | YES | False | (getdate()) |  |

## dbo.appeal_list

| 序号 | 字段 | 类型 | 允许空 | 标识列 | 默认值 | 说明 |
|---:|---|---|---|---|---|---|
| 1 | `id` | `bigint` | NO | True |  |  |
| 2 | `url` | `nvarchar(2500)` | NO | False |  |  |
| 3 | `info_` | `nvarchar(MAX)` | NO | False |  |  |
| 4 | `addtime` | `datetime` | NO | False |  |  |
| 5 | `status` | `int` | NO | False |  |  |
| 6 | `userid` | `int` | NO | False |  |  |

## dbo.appealimg_list

| 序号 | 字段 | 类型 | 允许空 | 标识列 | 默认值 | 说明 |
|---:|---|---|---|---|---|---|
| 1 | `appeal_id` | `bigint` | NO | False |  |  |
| 2 | `upload_pic_info` | `nvarchar(250)` | NO | False |  |  |
| 3 | `addtime` | `datetime` | NO | False |  |  |
| 4 | `orderid` | `int` | NO | False |  |  |

## dbo.Author

| 序号 | 字段 | 类型 | 允许空 | 标识列 | 默认值 | 说明 |
|---:|---|---|---|---|---|---|
| 1 | `id` | `int` | NO | True |  |  |
| 2 | `name_cn` | `nvarchar(100)` | NO | False |  |  |
| 3 | `name_en` | `nvarchar(200)` | YES | False |  |  |
| 4 | `institution` | `nvarchar(300)` | YES | False |  |  |
| 5 | `orcid` | `nvarchar(50)` | YES | False |  |  |
| 6 | `email` | `nvarchar(200)` | YES | False |  |  |
| 7 | `status` | `int` | NO | False | ((1)) |  |
| 8 | `addtime` | `datetime` | NO | False | (getdate()) |  |
| 9 | `current_institution_id` | `int` | YES | False |  |  |
| 10 | `current_institution_name` | `nvarchar(1000)` | YES | False |  |  |
| 11 | `current_institution_literature_id` | `int` | YES | False |  |  |
| 12 | `current_institution_sort_date` | `date` | YES | False |  |  |
| 13 | `current_institution_precision` | `nvarchar(20)` | YES | False |  |  |
| 14 | `identity_status` | `nvarchar(30)` | YES | False |  |  |
| 15 | `merge_group_id` | `int` | YES | False |  |  |
| 16 | `updatetime` | `datetime` | NO | False | (getdate()) |  |

## dbo.AuthorInstitutionHistory

| 序号 | 字段 | 类型 | 允许空 | 标识列 | 默认值 | 说明 |
|---:|---|---|---|---|---|---|
| 1 | `id` | `int` | NO | True |  |  |
| 2 | `author_id` | `int` | NO | False |  |  |
| 3 | `institution_name` | `nvarchar(500)` | NO | False |  |  |
| 4 | `is_current` | `int` | NO | False | ((0)) |  |
| 5 | `start_year` | `int` | YES | False |  |  |
| 6 | `end_year` | `int` | YES | False |  |  |
| 7 | `source_literature_id` | `int` | YES | False |  |  |
| 8 | `source_type` | `nvarchar(50)` | YES | False |  |  |
| 9 | `remark` | `nvarchar(500)` | YES | False |  |  |
| 10 | `status` | `int` | NO | False | ((1)) |  |
| 11 | `addtime` | `datetime` | NO | False | (getdate()) |  |
| 12 | `updatetime` | `datetime` | NO | False | (getdate()) |  |
| 13 | `institution_id` | `int` | YES | False |  |  |

## dbo.Conference

| 序号 | 字段 | 类型 | 允许空 | 标识列 | 默认值 | 说明 |
|---:|---|---|---|---|---|---|
| 1 | `id` | `int` | NO | True |  |  |
| 2 | `name_cn` | `nvarchar(300)` | NO | False | (N'') |  |
| 3 | `name_en` | `nvarchar(500)` | YES | False |  |  |
| 4 | `acronym` | `nvarchar(100)` | YES | False |  |  |
| 5 | `normalized_name` | `nvarchar(500)` | NO | False |  |  |
| 6 | `organizer` | `nvarchar(500)` | YES | False |  |  |
| 7 | `country` | `nvarchar(200)` | YES | False |  |  |
| 8 | `city` | `nvarchar(200)` | YES | False |  |  |
| 9 | `start_date` | `datetime` | YES | False |  |  |
| 10 | `end_date` | `datetime` | YES | False |  |  |
| 11 | `website` | `nvarchar(1000)` | YES | False |  |  |
| 12 | `status` | `int` | NO | False | ((1)) |  |
| 13 | `addtime` | `datetime` | NO | False | (getdate()) |  |
| 14 | `updatetime` | `datetime` | NO | False | (getdate()) |  |

## dbo.cosfile_list

| 序号 | 字段 | 类型 | 允许空 | 标识列 | 默认值 | 说明 |
|---:|---|---|---|---|---|---|
| 1 | `userid` | `int` | NO | False |  |  |
| 2 | `addtime` | `datetime` | NO | False |  |  |
| 3 | `up_filename` | `nvarchar(250)` | NO | False |  |  |

## dbo.daoru_list

| 序号 | 字段 | 类型 | 允许空 | 标识列 | 默认值 | 说明 |
|---:|---|---|---|---|---|---|
| 1 | `id` | `int` | NO | True |  |  |
| 2 | `posttime` | `datetime` | NO | False |  |  |
| 3 | `r_info` | `nvarchar(500)` | NO | False |  |  |
| 4 | `status` | `int` | NO | False |  |  |
| 5 | `type` | `int` | NO | False |  |  |

## dbo.daoruerr_list

| 序号 | 字段 | 类型 | 允许空 | 标识列 | 默认值 | 说明 |
|---:|---|---|---|---|---|---|
| 1 | `info` | `nvarchar(500)` | NO | False |  |  |
| 2 | `filename` | `nvarchar(250)` | NO | False |  |  |
| 3 | `addtime` | `datetime` | NO | False |  |  |
| 4 | `daoruid` | `int` | NO | False |  |  |

## dbo.data_list

| 序号 | 字段 | 类型 | 允许空 | 标识列 | 默认值 | 说明 |
|---:|---|---|---|---|---|---|
| 1 | `id` | `int` | NO | True |  |  |
| 2 | `name` | `nvarchar(600)` | YES | False |  |  |
| 3 | `tbclass_id` | `int` | NO | False | ((0)) |  |
| 4 | `upload_pic_img` | `nvarchar(50)` | YES | False |  |  |
| 5 | `isshow` | `int` | NO | False | ((0)) |  |
| 6 | `addtime` | `datetime` | NO | False |  |  |
| 7 | `uptime` | `datetime` | NO | False |  |  |
| 8 | `datetime` | `datetime` | NO | False |  |  |
| 9 | `orderid` | `int` | NO | False |  |  |
| 10 | `info_` | `nvarchar(MAX)` | YES | False |  |  |
| 11 | `keywords` | `nvarchar(MAX)` | YES | False |  |  |
| 12 | `description` | `nvarchar(MAX)` | YES | False |  |  |
| 13 | `title` | `nvarchar(MAX)` | YES | False |  |  |
| 14 | `istop` | `int` | NO | False |  |  |

## dbo.indexsingle_list

| 序号 | 字段 | 类型 | 允许空 | 标识列 | 默认值 | 说明 |
|---:|---|---|---|---|---|---|
| 1 | `id` | `int` | NO | True |  |  |
| 2 | `name` | `nvarchar(600)` | YES | False |  |  |
| 3 | `upload_pic_img` | `nvarchar(50)` | YES | False |  |  |
| 4 | `upload_pic_m` | `nvarchar(250)` | YES | False |  |  |
| 5 | `upload_pic_pc` | `nvarchar(250)` | YES | False |  |  |
| 6 | `isshow` | `int` | NO | False | ((0)) |  |
| 7 | `addtime` | `datetime` | NO | False |  |  |
| 8 | `uptime` | `datetime` | NO | False |  |  |
| 9 | `orderid` | `int` | NO | False |  |  |
| 10 | `info_` | `nvarchar(MAX)` | YES | False |  |  |
| 11 | `keywords` | `nvarchar(MAX)` | YES | False |  |  |
| 12 | `description` | `nvarchar(MAX)` | YES | False |  |  |
| 13 | `title` | `nvarchar(MAX)` | YES | False |  |  |
| 14 | `istop` | `int` | NO | False |  |  |
| 15 | `istype` | `int` | NO | False |  |  |
| 16 | `url` | `nvarchar(2500)` | YES | False |  |  |

## dbo.Institution

| 序号 | 字段 | 类型 | 允许空 | 标识列 | 默认值 | 说明 |
|---:|---|---|---|---|---|---|
| 1 | `id` | `int` | NO | True |  |  |
| 2 | `name_cn` | `nvarchar(300)` | NO | False | (N'') |  |
| 3 | `name_en` | `nvarchar(500)` | YES | False |  |  |
| 4 | `normalized_name` | `nvarchar(500)` | NO | False |  |  |
| 5 | `alias_names` | `nvarchar(MAX)` | YES | False |  |  |
| 6 | `country` | `nvarchar(200)` | YES | False |  |  |
| 7 | `province` | `nvarchar(200)` | YES | False |  |  |
| 8 | `city` | `nvarchar(200)` | YES | False |  |  |
| 9 | `website` | `nvarchar(1000)` | YES | False |  |  |
| 10 | `status` | `int` | NO | False | ((1)) |  |
| 11 | `addtime` | `datetime` | NO | False | (getdate()) |  |
| 12 | `updatetime` | `datetime` | NO | False | (getdate()) |  |
| 13 | `parent_id` | `int` | YES | False |  |  |

## dbo.InstitutionAlias

| 序号 | 字段 | 类型 | 允许空 | 标识列 | 默认值 | 说明 |
|---:|---|---|---|---|---|---|
| 1 | `id` | `int` | NO | True |  |  |
| 2 | `institution_id` | `int` | NO | False |  |  |
| 3 | `alias_name` | `nvarchar(500)` | NO | False |  |  |
| 4 | `normalized_alias` | `nvarchar(500)` | NO | False |  |  |
| 5 | `language` | `nvarchar(50)` | YES | False |  |  |
| 6 | `status` | `int` | NO | False | ((1)) |  |
| 7 | `addtime` | `datetime` | NO | False | (getdate()) |  |

## dbo.integrate_list

| 序号 | 字段 | 类型 | 允许空 | 标识列 | 默认值 | 说明 |
|---:|---|---|---|---|---|---|
| 1 | `id` | `int` | NO | True |  |  |
| 2 | `name` | `nvarchar(250)` | NO | False |  |  |
| 3 | `orderid` | `int` | NO | False |  |  |
| 4 | `uptime` | `datetime` | NO | False |  |  |
| 5 | `addtime` | `datetime` | NO | False |  |  |
| 6 | `upload_pic_img` | `nvarchar(250)` | YES | False |  |  |
| 7 | `about_` | `nvarchar(MAX)` | YES | False |  |  |
| 8 | `num_integrate` | `int` | NO | False |  |  |

## dbo.integrateExchangeLog_list

| 序号 | 字段 | 类型 | 允许空 | 标识列 | 默认值 | 说明 |
|---:|---|---|---|---|---|---|
| 1 | `id` | `bigint` | NO | True |  |  |
| 2 | `name` | `nvarchar(500)` | NO | False |  |  |
| 3 | `num_integrate` | `int` | NO | False |  |  |
| 4 | `codestr` | `nvarchar(250)` | NO | False |  |  |
| 5 | `addtime` | `datetime` | NO | False |  |  |
| 6 | `status` | `int` | YES | False |  |  |
| 7 | `user_id` | `int` | NO | False |  |  |
| 8 | `upload_pic_img` | `nvarchar(250)` | YES | False |  |  |
| 9 | `hexiaotime` | `nvarchar(250)` | YES | False |  |  |

## dbo.integrateLog_list

| 序号 | 字段 | 类型 | 允许空 | 标识列 | 默认值 | 说明 |
|---:|---|---|---|---|---|---|
| 1 | `id` | `int` | NO | True |  |  |
| 2 | `num_integrate` | `int` | NO | False |  |  |
| 3 | `type` | `int` | NO | False |  |  |
| 4 | `name` | `nvarchar(500)` | NO | False |  |  |
| 5 | `info_` | `nvarchar(MAX)` | YES | False |  |  |
| 6 | `addtime` | `datetime` | NO | False |  |  |
| 7 | `user_id` | `int` | NO | False |  |  |
| 8 | `adminname` | `nvarchar(250)` | YES | False |  |  |
| 9 | `orderpro_orderno` | `nvarchar(50)` | YES | False |  |  |
| 10 | `literature_id` | `int` | YES | False |  |  |

## dbo.integrateLogType_list

| 序号 | 字段 | 类型 | 允许空 | 标识列 | 默认值 | 说明 |
|---:|---|---|---|---|---|---|
| 1 | `id` | `int` | NO | False |  |  |
| 2 | `name` | `nvarchar(50)` | NO | False |  |  |
| 3 | `num_integrate` | `int` | YES | False |  |  |

## dbo.integratestatus_list

| 序号 | 字段 | 类型 | 允许空 | 标识列 | 默认值 | 说明 |
|---:|---|---|---|---|---|---|
| 1 | `id` | `int` | NO | False |  |  |
| 2 | `name` | `nvarchar(50)` | NO | False |  |  |

## dbo.IntegrationToken_list

| 序号 | 字段 | 类型 | 允许空 | 标识列 | 默认值 | 说明 |
|---:|---|---|---|---|---|---|
| 1 | `id` | `int` | NO | False |  |  |
| 2 | `adddate` | `datetime` | NO | False |  |  |
| 3 | `access_token` | `nvarchar(250)` | NO | False |  |  |
| 4 | `expires_in` | `int` | NO | False |  |  |

## dbo.Journal

| 序号 | 字段 | 类型 | 允许空 | 标识列 | 默认值 | 说明 |
|---:|---|---|---|---|---|---|
| 1 | `id` | `int` | NO | True |  |  |
| 2 | `name_cn` | `nvarchar(300)` | NO | False | (N'') |  |
| 3 | `name_en` | `nvarchar(500)` | YES | False |  |  |
| 4 | `normalized_name` | `nvarchar(500)` | NO | False |  |  |
| 5 | `issn` | `nvarchar(100)` | YES | False |  |  |
| 6 | `eissn` | `nvarchar(100)` | YES | False |  |  |
| 7 | `publisher` | `nvarchar(500)` | YES | False |  |  |
| 8 | `country` | `nvarchar(200)` | YES | False |  |  |
| 9 | `subject` | `nvarchar(500)` | YES | False |  |  |
| 10 | `website` | `nvarchar(1000)` | YES | False |  |  |
| 11 | `status` | `int` | NO | False | ((1)) |  |
| 12 | `addtime` | `datetime` | NO | False | (getdate()) |  |
| 13 | `updatetime` | `datetime` | NO | False | (getdate()) |  |

## dbo.link_list

| 序号 | 字段 | 类型 | 允许空 | 标识列 | 默认值 | 说明 |
|---:|---|---|---|---|---|---|
| 1 | `id` | `int` | NO | True |  |  |
| 2 | `name` | `nvarchar(150)` | YES | False |  |  |
| 3 | `isshow` | `int` | NO | False | ((0)) |  |
| 4 | `addtime` | `datetime` | NO | False |  |  |
| 5 | `uptime` | `datetime` | NO | False |  |  |
| 6 | `orderid` | `int` | NO | False |  |  |
| 7 | `url` | `nvarchar(600)` | YES | False |  |  |
| 8 | `type` | `int` | NO | False |  |  |
| 9 | `upload_pic_icon` | `nvarchar(50)` | YES | False |  |  |

## dbo.Literature

| 序号 | 字段 | 类型 | 允许空 | 标识列 | 默认值 | 说明 |
|---:|---|---|---|---|---|---|
| 1 | `id` | `int` | NO | True |  |  |
| 2 | `title` | `nvarchar(500)` | NO | False |  |  |
| 3 | `subtitle` | `nvarchar(500)` | YES | False |  |  |
| 4 | `doi` | `nvarchar(100)` | YES | False |  |  |
| 5 | `keywords` | `nvarchar(500)` | YES | False |  |  |
| 6 | `abstract_text` | `nvarchar(MAX)` | YES | False |  |  |
| 7 | `source_type` | `nvarchar(50)` | YES | False |  |  |
| 8 | `language` | `nvarchar(50)` | YES | False |  |  |
| 9 | `publish_year` | `int` | YES | False |  |  |
| 10 | `journal_name` | `nvarchar(300)` | YES | False |  |  |
| 11 | `conference_name` | `nvarchar(300)` | YES | False |  |  |
| 12 | `publisher` | `nvarchar(300)` | YES | False |  |  |
| 13 | `volume` | `nvarchar(50)` | YES | False |  |  |
| 14 | `issue` | `nvarchar(50)` | YES | False |  |  |
| 15 | `pages` | `nvarchar(100)` | YES | False |  |  |
| 16 | `category_id` | `int` | NO | False | ((0)) |  |
| 17 | `cover_pic` | `nvarchar(255)` | YES | False |  |  |
| 18 | `external_url` | `nvarchar(500)` | YES | False |  |  |
| 19 | `source_db` | `nvarchar(200)` | YES | False |  |  |
| 20 | `remark` | `nvarchar(1000)` | YES | False |  |  |
| 21 | `is_top` | `int` | NO | False | ((0)) |  |
| 22 | `status` | `int` | NO | False | ((1)) |  |
| 23 | `userid` | `int` | NO | False | ((0)) |  |
| 24 | `addtime` | `datetime` | NO | False | (getdate()) |  |
| 25 | `updatetime` | `datetime` | NO | False | (getdate()) |  |
| 26 | `institution` | `nvarchar(500)` | YES | False |  |  |
| 27 | `download_points` | `int` | NO | False | ((0)) |  |
| 28 | `reviewed_by` | `int` | YES | False |  |  |
| 29 | `review_time` | `datetime` | YES | False |  |  |
| 30 | `import_batch_id` | `int` | YES | False |  |  |
| 31 | `canonical_literature_id` | `int` | YES | False |  |  |
| 32 | `journal_id` | `int` | YES | False |  |  |
| 33 | `conference_id` | `int` | YES | False |  |  |
| 34 | `publish_month` | `int` | YES | False |  |  |
| 35 | `publish_day` | `int` | YES | False |  |  |
| 36 | `publish_date` | `date` | YES | False |  |  |
| 37 | `publish_date_precision` | `nvarchar(20)` | YES | False |  |  |

## dbo.LiteratureAuthorInstitutionMap

| 序号 | 字段 | 类型 | 允许空 | 标识列 | 默认值 | 说明 |
|---:|---|---|---|---|---|---|
| 1 | `id` | `int` | NO | True |  |  |
| 2 | `literature_id` | `int` | NO | False |  |  |
| 3 | `author_id` | `int` | NO | False |  |  |
| 4 | `literature_author_map_id` | `int` | NO | False | ((0)) |  |
| 5 | `institution_id` | `int` | NO | False |  |  |
| 6 | `affiliation_text` | `nvarchar(1000)` | YES | False |  |  |
| 7 | `author_order` | `int` | NO | False | ((0)) |  |
| 8 | `institution_order` | `int` | NO | False | ((0)) |  |
| 9 | `is_current_for_author` | `int` | NO | False | ((0)) |  |
| 10 | `addtime` | `datetime` | NO | False | (getdate()) |  |
| 11 | `source_type` | `nvarchar(50)` | YES | False |  |  |
| 12 | `is_confirmed` | `int` | NO | False | ((0)) |  |
| 13 | `confirm_by` | `int` | YES | False |  |  |
| 14 | `confirm_time` | `datetime` | YES | False |  |  |
| 15 | `updatetime` | `datetime` | NO | False | (getdate()) |  |

## dbo.LiteratureAuthorMap

| 序号 | 字段 | 类型 | 允许空 | 标识列 | 默认值 | 说明 |
|---:|---|---|---|---|---|---|
| 1 | `id` | `int` | NO | True |  |  |
| 2 | `literature_id` | `int` | NO | False |  |  |
| 3 | `author_id` | `int` | NO | False |  |  |
| 4 | `author_order` | `int` | NO | False | ((1)) |  |
| 5 | `is_corresponding` | `int` | NO | False | ((0)) |  |
| 6 | `addtime` | `datetime` | NO | False | (getdate()) |  |
| 7 | `affiliation_text` | `nvarchar(500)` | YES | False |  |  |
| 8 | `raw_author_text` | `nvarchar(300)` | YES | False |  |  |
| 9 | `display_author_name` | `nvarchar(300)` | YES | False |  |  |
| 10 | `author_name_raw` | `nvarchar(600)` | YES | False |  |  |
| 11 | `is_confirmed` | `int` | NO | False | ((0)) |  |
| 12 | `confirm_by` | `int` | YES | False |  |  |
| 13 | `confirm_time` | `datetime` | YES | False |  |  |
| 14 | `identity_confidence` | `decimal(5,2)` | YES | False |  |  |

## dbo.LiteratureCategory

| 序号 | 字段 | 类型 | 允许空 | 标识列 | 默认值 | 说明 |
|---:|---|---|---|---|---|---|
| 1 | `id` | `int` | NO | True |  |  |
| 2 | `parent_id` | `int` | YES | False | ((0)) |  |
| 3 | `name` | `nvarchar(100)` | NO | False |  |  |
| 4 | `name_en` | `nvarchar(200)` | YES | False |  |  |
| 5 | `code` | `nvarchar(100)` | YES | False |  |  |
| 6 | `orderid` | `int` | NO | False | ((0)) |  |
| 7 | `status` | `int` | NO | False | ((1)) |  |
| 8 | `addtime` | `datetime` | NO | False | (getdate()) |  |
| 9 | `updatetime` | `datetime` | NO | False | (getdate()) |  |

## dbo.LiteratureComment

| 序号 | 字段 | 类型 | 允许空 | 标识列 | 默认值 | 说明 |
|---:|---|---|---|---|---|---|
| 1 | `id` | `int` | NO | True |  |  |
| 2 | `literature_id` | `int` | NO | False |  |  |
| 3 | `canonical_literature_id` | `int` | YES | False |  |  |
| 4 | `userid` | `int` | NO | False |  |  |
| 5 | `parent_id` | `int` | NO | False | ((0)) |  |
| 6 | `content` | `nvarchar(MAX)` | NO | False |  |  |
| 7 | `status` | `int` | NO | False | ((0)) |  |
| 8 | `like_count` | `int` | NO | False | ((0)) |  |
| 9 | `report_count` | `int` | NO | False | ((0)) |  |
| 10 | `is_deleted` | `int` | NO | False | ((0)) |  |
| 11 | `delete_time` | `datetime` | YES | False |  |  |
| 12 | `reviewed_by` | `int` | YES | False |  |  |
| 13 | `review_time` | `datetime` | YES | False |  |  |
| 14 | `review_remark` | `nvarchar(500)` | YES | False |  |  |
| 15 | `addtime` | `datetime` | NO | False | (getdate()) |  |
| 16 | `updatetime` | `datetime` | NO | False | (getdate()) |  |

## dbo.LiteratureDownloadLog

| 序号 | 字段 | 类型 | 允许空 | 标识列 | 默认值 | 说明 |
|---:|---|---|---|---|---|---|
| 1 | `id` | `int` | NO | True |  |  |
| 2 | `literature_id` | `int` | NO | False |  |  |
| 3 | `user_id` | `int` | NO | False |  |  |
| 4 | `literature_title` | `nvarchar(500)` | YES | False |  |  |
| 5 | `file_url` | `nvarchar(255)` | YES | False |  |  |
| 6 | `download_points` | `int` | NO | False | ((0)) |  |
| 7 | `literature_user_id` | `int` | NO | False | ((0)) |  |
| 8 | `addtime` | `datetime` | NO | False | (getdate()) |  |

## dbo.LiteratureExportLog

| 序号 | 字段 | 类型 | 允许空 | 标识列 | 默认值 | 说明 |
|---:|---|---|---|---|---|---|
| 1 | `id` | `int` | NO | True |  |  |
| 2 | `export_name` | `nvarchar(200)` | NO | False |  |  |
| 3 | `export_type` | `nvarchar(50)` | NO | False |  |  |
| 4 | `file_name` | `nvarchar(255)` | YES | False |  |  |
| 5 | `record_count` | `int` | NO | False | ((0)) |  |
| 6 | `userid` | `int` | NO | False | ((0)) |  |
| 7 | `addtime` | `datetime` | NO | False | (getdate()) |  |

## dbo.LiteratureFavorite

| 序号 | 字段 | 类型 | 允许空 | 标识列 | 默认值 | 说明 |
|---:|---|---|---|---|---|---|
| 1 | `id` | `int` | NO | True |  |  |
| 2 | `literature_id` | `int` | NO | False |  |  |
| 3 | `userid` | `int` | NO | False |  |  |
| 4 | `addtime` | `datetime` | NO | False | (getdate()) |  |

## dbo.LiteratureFile

| 序号 | 字段 | 类型 | 允许空 | 标识列 | 默认值 | 说明 |
|---:|---|---|---|---|---|---|
| 1 | `id` | `int` | NO | True |  |  |
| 2 | `literature_id` | `int` | NO | False |  |  |
| 3 | `file_type` | `nvarchar(50)` | NO | False |  |  |
| 4 | `file_name` | `nvarchar(255)` | NO | False |  |  |
| 5 | `file_path` | `nvarchar(255)` | NO | False |  |  |
| 6 | `file_size` | `bigint` | YES | False |  |  |
| 7 | `mime_type` | `nvarchar(100)` | YES | False |  |  |
| 8 | `orderid` | `int` | NO | False | ((0)) |  |
| 9 | `status` | `int` | NO | False | ((1)) |  |
| 10 | `addtime` | `datetime` | NO | False | (getdate()) |  |

## dbo.LiteratureImportBatch

| 序号 | 字段 | 类型 | 允许空 | 标识列 | 默认值 | 说明 |
|---:|---|---|---|---|---|---|
| 1 | `id` | `int` | NO | True |  |  |
| 2 | `batch_name` | `nvarchar(200)` | NO | False |  |  |
| 3 | `import_type` | `nvarchar(50)` | NO | False |  |  |
| 4 | `file_name` | `nvarchar(255)` | YES | False |  |  |
| 5 | `status` | `int` | NO | False | ((0)) |  |
| 6 | `total_count` | `int` | NO | False | ((0)) |  |
| 7 | `success_count` | `int` | NO | False | ((0)) |  |
| 8 | `fail_count` | `int` | NO | False | ((0)) |  |
| 9 | `userid` | `int` | NO | False | ((0)) |  |
| 10 | `addtime` | `datetime` | NO | False | (getdate()) |  |
| 11 | `finishtime` | `datetime` | YES | False |  |  |

## dbo.LiteratureImportError

| 序号 | 字段 | 类型 | 允许空 | 标识列 | 默认值 | 说明 |
|---:|---|---|---|---|---|---|
| 1 | `id` | `int` | NO | True |  |  |
| 2 | `batch_id` | `int` | NO | False |  |  |
| 3 | `row_no` | `int` | NO | False |  |  |
| 4 | `title` | `nvarchar(500)` | YES | False |  |  |
| 5 | `error_msg` | `nvarchar(1000)` | NO | False |  |  |
| 6 | `raw_data` | `nvarchar(MAX)` | YES | False |  |  |
| 7 | `addtime` | `datetime` | NO | False | (getdate()) |  |

## dbo.LiteratureLike

| 序号 | 字段 | 类型 | 允许空 | 标识列 | 默认值 | 说明 |
|---:|---|---|---|---|---|---|
| 1 | `id` | `int` | NO | True |  |  |
| 2 | `literature_id` | `int` | NO | False |  |  |
| 3 | `userid` | `int` | NO | False |  |  |
| 4 | `addtime` | `datetime` | NO | False | (getdate()) |  |

## dbo.LiteratureTag

| 序号 | 字段 | 类型 | 允许空 | 标识列 | 默认值 | 说明 |
|---:|---|---|---|---|---|---|
| 1 | `id` | `int` | NO | True |  |  |
| 2 | `name` | `nvarchar(100)` | NO | False |  |  |
| 3 | `orderid` | `int` | NO | False | ((0)) |  |
| 4 | `status` | `int` | NO | False | ((1)) |  |
| 5 | `addtime` | `datetime` | NO | False | (getdate()) |  |

## dbo.LiteratureTagMap

| 序号 | 字段 | 类型 | 允许空 | 标识列 | 默认值 | 说明 |
|---:|---|---|---|---|---|---|
| 1 | `id` | `int` | NO | True |  |  |
| 2 | `literature_id` | `int` | NO | False |  |  |
| 3 | `tag_id` | `int` | NO | False |  |  |
| 4 | `addtime` | `datetime` | NO | False | (getdate()) |  |

## dbo.LiteratureVenueProfile

| 序号 | 字段 | 类型 | 允许空 | 标识列 | 默认值 | 说明 |
|---:|---|---|---|---|---|---|
| 1 | `id` | `int` | NO | True |  |  |
| 2 | `venue_type` | `nvarchar(30)` | NO | False |  |  |
| 3 | `venue_name` | `nvarchar(500)` | NO | False |  |  |
| 4 | `introduction` | `nvarchar(MAX)` | YES | False |  |  |
| 5 | `impact_factor` | `nvarchar(100)` | YES | False |  |  |
| 6 | `jcr_quartile` | `nvarchar(100)` | YES | False |  |  |
| 7 | `issn` | `nvarchar(100)` | YES | False |  |  |
| 8 | `conference_level` | `nvarchar(100)` | YES | False |  |  |
| 9 | `conference_cycle` | `nvarchar(100)` | YES | False |  |  |
| 10 | `location` | `nvarchar(250)` | YES | False |  |  |
| 11 | `website_url` | `nvarchar(500)` | YES | False |  |  |
| 12 | `publisher` | `nvarchar(250)` | YES | False |  |  |
| 13 | `remark` | `nvarchar(MAX)` | YES | False |  |  |
| 14 | `status` | `int` | NO | False | ((0)) |  |
| 15 | `created_by` | `int` | NO | False | ((0)) |  |
| 16 | `updated_by` | `int` | NO | False | ((0)) |  |
| 17 | `addtime` | `datetime` | NO | False | (getdate()) |  |
| 18 | `updatetime` | `datetime` | NO | False | (getdate()) |  |

## dbo.logincode_list

| 序号 | 字段 | 类型 | 允许空 | 标识列 | 默认值 | 说明 |
|---:|---|---|---|---|---|---|
| 1 | `code` | `nvarchar(50)` | NO | False |  |  |
| 2 | `val` | `nchar(10)` | NO | False |  |  |
| 3 | `addtime` | `datetime` | NO | False |  |  |
| 4 | `ip_str` | `nvarchar(50)` | NO | False |  |  |
| 5 | `type` | `int` | NO | False |  |  |

## dbo.LoginSingle_List

| 序号 | 字段 | 类型 | 允许空 | 标识列 | 默认值 | 说明 |
|---:|---|---|---|---|---|---|
| 1 | `Id` | `int` | NO | True |  |  |
| 2 | `Name` | `nvarchar(250)` | NO | False |  |  |
| 3 | `IsShow` | `int` | NO | False |  |  |
| 4 | `OrderId` | `int` | NO | False |  |  |
| 5 | `UpTime` | `datetime` | NO | False |  |  |
| 6 | `AddTime` | `datetime` | NO | False |  |  |
| 7 | `Info_` | `nvarchar(MAX)` | YES | False |  |  |

## dbo.model_list

| 序号 | 字段 | 类型 | 允许空 | 标识列 | 默认值 | 说明 |
|---:|---|---|---|---|---|---|
| 1 | `id` | `int` | NO | True |  |  |
| 2 | `m_name` | `nvarchar(255)` | YES | False |  |  |
| 3 | `m_url` | `nvarchar(255)` | YES | False |  |  |
| 4 | `page_url` | `nvarchar(255)` | YES | False |  |  |
| 5 | `orderid` | `int` | YES | False |  |  |
| 6 | `addtime` | `datetime` | YES | False |  |  |
| 7 | `upload_pic` | `nvarchar(50)` | YES | False |  |  |

## dbo.NoticeLog_List

| 序号 | 字段 | 类型 | 允许空 | 标识列 | 默认值 | 说明 |
|---:|---|---|---|---|---|---|
| 1 | `id` | `int` | NO | True |  |  |
| 2 | `info_` | `nvarchar(MAX)` | YES | False |  |  |
| 3 | `type` | `int` | NO | False |  |  |
| 4 | `addtime` | `datetime` | NO | False |  |  |
| 5 | `userid` | `int` | NO | False |  |  |
| 6 | `looktime` | `nvarchar(50)` | YES | False |  |  |
| 7 | `status` | `int` | NO | False |  |  |
| 8 | `url` | `nvarchar(500)` | YES | False |  |  |
| 9 | `name` | `nvarchar(500)` | NO | False |  |  |

## dbo.NoticeLogStatus_List

| 序号 | 字段 | 类型 | 允许空 | 标识列 | 默认值 | 说明 |
|---:|---|---|---|---|---|---|
| 1 | `id` | `int` | NO | False |  |  |
| 2 | `name` | `nvarchar(50)` | NO | False |  |  |

## dbo.NoticeLogType_List

| 序号 | 字段 | 类型 | 允许空 | 标识列 | 默认值 | 说明 |
|---:|---|---|---|---|---|---|
| 1 | `id` | `int` | NO | False |  |  |
| 2 | `name` | `nvarchar(50)` | NO | False |  |  |

## dbo.popedom

| 序号 | 字段 | 类型 | 允许空 | 标识列 | 默认值 | 说明 |
|---:|---|---|---|---|---|---|
| 1 | `id` | `int` | NO | True |  |  |
| 2 | `popedom_name` | `nvarchar(50)` | YES | False |  |  |
| 3 | `popedom_father` | `int` | YES | False |  |  |
| 4 | `popedom_url` | `nvarchar(255)` | YES | False |  |  |
| 5 | `orderid` | `int` | YES | False |  |  |
| 6 | `ishead` | `int` | YES | False |  |  |

## dbo.popedomhead

| 序号 | 字段 | 类型 | 允许空 | 标识列 | 默认值 | 说明 |
|---:|---|---|---|---|---|---|
| 1 | `id` | `int` | NO | True |  |  |
| 2 | `adminid` | `int` | YES | False |  |  |
| 3 | `headid` | `int` | YES | False |  |  |
| 4 | `popedomid` | `int` | YES | False |  |  |
## dbo.SearchHot_List

| 序号 | 字段 | 类型 | 允许空 | 标识列 | 默认值 | 说明 |
|---:|---|---|---|---|---|---|
| 1 | `id` | `int` | NO | True |  |  |
| 2 | `addtime` | `datetime` | NO | False |  |  |
| 3 | `name` | `nvarchar(500)` | NO | False |  |  |
| 4 | `url` | `nvarchar(2500)` | NO | False |  |  |
| 5 | `isshow` | `int` | NO | False |  |  |
| 6 | `orderid` | `int` | NO | False |  |  |
| 7 | `uptime` | `datetime` | YES | False |  |  |
| 8 | `num_click` | `int` | NO | False |  |  |

## dbo.ServiceLog_List

| 序号 | 字段 | 类型 | 允许空 | 标识列 | 默认值 | 说明 |
|---:|---|---|---|---|---|---|
| 1 | `id` | `int` | NO | True |  |  |
| 2 | `name` | `nvarchar(500)` | NO | False |  |  |
| 3 | `info_` | `nvarchar(MAX)` | NO | False |  |  |
| 4 | `addtime` | `datetime` | NO | False |  |  |
| 5 | `status` | `int` | NO | False |  |  |
| 6 | `userid` | `int` | NO | False |  |  |
| 7 | `uptime` | `datetime` | NO | False |  |  |
| 8 | `looktime` | `nvarchar(50)` | YES | False |  |  |

## dbo.ServiceLogInfo_List

| 序号 | 字段 | 类型 | 允许空 | 标识列 | 默认值 | 说明 |
|---:|---|---|---|---|---|---|
| 1 | `id` | `int` | NO | True |  |  |
| 2 | `ServiceLog_Id` | `int` | NO | False |  |  |
| 3 | `info_` | `nvarchar(MAX)` | NO | False |  |  |
| 4 | `type` | `int` | NO | False |  |  |
| 5 | `addtime` | `datetime` | NO | False |  |  |
| 6 | `adminname` | `nvarchar(250)` | YES | False |  |  |

## dbo.ServiceLogStatus_List

| 序号 | 字段 | 类型 | 允许空 | 标识列 | 默认值 | 说明 |
|---:|---|---|---|---|---|---|
| 1 | `id` | `int` | NO | False |  |  |
| 2 | `name` | `nvarchar(50)` | NO | False |  |  |

## dbo.sysdiagrams

| 序号 | 字段 | 类型 | 允许空 | 标识列 | 默认值 | 说明 |
|---:|---|---|---|---|---|---|
| 1 | `name` | `sysname` | NO | False |  |  |
| 2 | `principal_id` | `int` | NO | False |  |  |
| 3 | `diagram_id` | `int` | NO | True |  |  |
| 4 | `version` | `int` | YES | False |  |  |
| 5 | `definition` | `varbinary(MAX)` | YES | False |  |  |

## dbo.tbl_class

| 序号 | 字段 | 类型 | 允许空 | 标识列 | 默认值 | 说明 |
|---:|---|---|---|---|---|---|
| 1 | `id` | `int` | NO | True |  |  |
| 2 | `parentid` | `int` | NO | False |  |  |
| 3 | `children` | `nvarchar(MAX)` | YES | False |  |  |
| 4 | `classname` | `nvarchar(250)` | NO | False |  |  |
| 5 | `model` | `int` | NO | False |  |  |
| 6 | `orderid` | `int` | NO | False |  |  |
| 7 | `isurl` | `int` | NO | False |  |  |
| 8 | `classurl` | `nvarchar(250)` | YES | False |  |  |
| 9 | `about` | `nvarchar(MAX)` | YES | False |  |  |
| 10 | `info_` | `nvarchar(MAX)` | YES | False |  |  |
| 11 | `isshow` | `int` | NO | False | ((0)) |  |
| 12 | `adddate` | `datetime` | NO | False |  |  |
| 13 | `isfoot` | `int` | NO | False |  |  |
| 14 | `istop` | `int` | NO | False | ((0)) |  |
| 15 | `description` | `nvarchar(MAX)` | YES | False |  |  |
| 16 | `keywords` | `nvarchar(MAX)` | YES | False |  |  |
| 17 | `upload_pic_m` | `nvarchar(250)` | YES | False |  |  |
| 18 | `title` | `nvarchar(MAX)` | YES | False |  |  |
| 19 | `urlnamebtn` | `nvarchar(250)` | NO | False |  |  |
| 20 | `upload_pic_pc` | `nvarchar(250)` | YES | False |  |  |

## dbo.telcode_list

| 序号 | 字段 | 类型 | 允许空 | 标识列 | 默认值 | 说明 |
|---:|---|---|---|---|---|---|
| 1 | `tel` | `nvarchar(150)` | NO | False |  |  |
| 2 | `type` | `int` | NO | False |  |  |
| 3 | `code` | `nvarchar(50)` | NO | False |  |  |
| 4 | `addtime` | `datetime` | NO | False |  |  |
| 5 | `img_x` | `int` | NO | False |  |  |
| 6 | `img_y` | `int` | NO | False |  |  |

## dbo.TopUpType_List

| 序号 | 字段 | 类型 | 允许空 | 标识列 | 默认值 | 说明 |
|---:|---|---|---|---|---|---|
| 1 | `id` | `int` | NO | True |  |  |
| 2 | `money` | `int` | NO | False |  |  |
| 3 | `isshow` | `int` | NO | False |  |  |

## dbo.user_list

| 序号 | 字段 | 类型 | 允许空 | 标识列 | 默认值 | 说明 |
|---:|---|---|---|---|---|---|
| 1 | `id` | `int` | NO | True |  |  |
| 2 | `name` | `nvarchar(150)` | YES | False |  |  |
| 3 | `tel` | `nvarchar(150)` | NO | False |  |  |
| 4 | `addtime` | `datetime` | NO | False |  |  |
| 5 | `uptime` | `datetime` | YES | False |  |  |
| 6 | `isshow` | `int` | NO | False |  |  |
| 7 | `logintime` | `nvarchar(50)` | YES | False |  |  |
| 8 | `loginip` | `nvarchar(50)` | YES | False |  |  |
| 9 | `code` | `nvarchar(50)` | YES | False |  |  |
| 10 | `email` | `nvarchar(250)` | YES | False |  |  |
| 11 | `upload_pic_avatar` | `nvarchar(250)` | YES | False |  |  |

## dbo.user_login

| 序号 | 字段 | 类型 | 允许空 | 标识列 | 默认值 | 说明 |
|---:|---|---|---|---|---|---|
| 1 | `id` | `int` | NO | True |  |  |
| 2 | `username` | `nvarchar(50)` | YES | False |  |  |
| 3 | `time` | `datetime` | YES | False |  |  |
| 4 | `ip` | `nvarchar(50)` | YES | False |  |  |
| 5 | `password` | `nvarchar(50)` | YES | False |  |  |
| 6 | `type` | `int` | YES | False |  |  |
| 7 | `content` | `nvarchar(MAX)` | YES | False |  |  |

## dbo.userfile_list

| 序号 | 字段 | 类型 | 允许空 | 标识列 | 默认值 | 说明 |
|---:|---|---|---|---|---|---|
| 1 | `userid` | `int` | NO | False |  |  |
| 2 | `addtime` | `datetime` | NO | False |  |  |
| 3 | `up_filename` | `nvarchar(250)` | NO | False |  |  |

## dbo.userimg_list

| 序号 | 字段 | 类型 | 允许空 | 标识列 | 默认值 | 说明 |
|---:|---|---|---|---|---|---|
| 1 | `userid` | `int` | NO | False |  |  |
| 2 | `addtime` | `datetime` | NO | False |  |  |
| 3 | `upload_pic_img` | `nvarchar(250)` | NO | False |  |  |

## dbo.userpaylog_list

| 序号 | 字段 | 类型 | 允许空 | 标识列 | 默认值 | 说明 |
|---:|---|---|---|---|---|---|
| 1 | `user_id` | `int` | NO | False |  |  |
| 2 | `out_trade_no` | `nvarchar(250)` | NO | False |  |  |
| 3 | `add_time` | `datetime` | NO | False |  |  |
| 4 | `pay_type` | `int` | NO | False |  |  |
| 5 | `pay_status` | `int` | NO | False |  |  |
| 6 | `up_time` | `nvarchar(250)` | YES | False |  |  |
| 7 | `payer_total` | `decimal(18,2)` | YES | False |  |  |

## dbo.userpayloginfo_list

| 序号 | 字段 | 类型 | 允许空 | 标识列 | 默认值 | 说明 |
|---:|---|---|---|---|---|---|
| 1 | `id` | `bigint` | NO | True |  |  |
| 2 | `appid` | `nvarchar(250)` | YES | False |  |  |
| 3 | `mchid` | `nvarchar(250)` | YES | False |  |  |
| 4 | `out_trade_no` | `nvarchar(250)` | NO | False |  |  |
| 5 | `transaction_id` | `nvarchar(250)` | NO | False |  |  |
| 6 | `trade_type` | `nvarchar(50)` | YES | False |  |  |
| 7 | `trade_state` | `nvarchar(50)` | YES | False |  |  |
| 8 | `trade_state_desc` | `nvarchar(350)` | YES | False |  |  |
| 9 | `bank_type` | `nvarchar(50)` | YES | False |  |  |
| 10 | `success_time` | `nvarchar(150)` | NO | False |  |  |
| 11 | `payer_total` | `decimal(18,2)` | YES | False |  |  |
| 12 | `pay_type` | `int` | NO | False |  |  |
| 13 | `add_time` | `datetime` | NO | False |  |  |
| 14 | `user_id` | `int` | NO | False |  |  |

## dbo.websiteinfo_list

| 序号 | 字段 | 类型 | 允许空 | 标识列 | 默认值 | 说明 |
|---:|---|---|---|---|---|---|
| 1 | `id` | `int` | NO | False |  |  |
| 2 | `companyname` | `nvarchar(250)` | YES | False |  |  |
| 3 | `wangzhanbeian` | `nvarchar(250)` | YES | False |  |  |
| 4 | `wangzhanbeianurl` | `nvarchar(250)` | YES | False |  |  |
| 5 | `gonganbeian` | `nvarchar(250)` | YES | False |  |  |
| 6 | `gonganbeianurl` | `nvarchar(250)` | YES | False |  |  |
| 7 | `banquan` | `nvarchar(250)` | YES | False |  |  |
| 8 | `upload_pic_logotop` | `nvarchar(50)` | YES | False |  |  |
| 9 | `upload_pic_favicon` | `nvarchar(50)` | YES | False |  |  |
| 10 | `title` | `nvarchar(500)` | YES | False |  |  |
| 11 | `keywords` | `nvarchar(500)` | YES | False |  |  |
| 12 | `description` | `nvarchar(500)` | YES | False |  |  |
| 13 | `emailnum` | `nvarchar(250)` | YES | False |  |  |
| 14 | `emailpasswd` | `nvarchar(250)` | YES | False |  |  |
| 15 | `email_to` | `nvarchar(250)` | YES | False |  |  |
| 16 | `smtpserverport` | `nvarchar(250)` | YES | False |  |  |
| 17 | `host` | `nvarchar(250)` | YES | False |  |  |
| 18 | `emailname` | `nvarchar(250)` | YES | False |  |  |
| 19 | `upload_pic_indexbj` | `nvarchar(50)` | YES | False |  |  |
| 20 | `upload_pic_indexbj_m` | `nvarchar(50)` | YES | False |  |  |
| 21 | `money_integrate` | `int` | YES | False |  |  |
| 22 | `integrate_donate` | `int` | YES | False |  |  |
| 23 | `integrate_buy` | `int` | YES | False |  |  |
| 24 | `integrate_fare` | `int` | YES | False |  |  |
| 25 | `integrate_allocation` | `int` | YES | False |  |  |

