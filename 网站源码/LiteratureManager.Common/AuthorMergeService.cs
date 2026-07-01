using DAL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace LiteratureManager.Common
{
    public static class AuthorMergeService
    {
        public static void MergeAuthors(int masterAuthorId, int duplicateAuthorId, int adminId, string remark)
        {
            if (masterAuthorId <= 0 || duplicateAuthorId <= 0 || masterAuthorId == duplicateAuthorId)
            {
                throw new ArgumentException("masterAuthorId and duplicateAuthorId must be different valid author ids.");
            }

            using (SqlConnection connection = new SqlConnection(DBHelper.ConnectionString))
            {
                connection.Open();
                using (SqlTransaction transaction = connection.BeginTransaction())
                {
                    try
                    {
                        EnsureAuthorsExist(transaction, masterAuthorId, duplicateAuthorId);
                        MoveAuthorMaps(transaction, masterAuthorId, duplicateAuthorId);
                        MoveInstitutionMaps(transaction, masterAuthorId, duplicateAuthorId);
                        MoveInstitutionHistory(transaction, masterAuthorId, duplicateAuthorId);
                        MarkDuplicateAuthor(transaction, masterAuthorId, duplicateAuthorId);
                        InsertMergeLog(transaction, masterAuthorId, duplicateAuthorId, adminId, remark);
                        transaction.Commit();
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }

            LiteratureRelationSync.RecalculateCurrentInstitutionForAuthors(new[] { masterAuthorId, duplicateAuthorId });
        }

        private static void EnsureAuthorsExist(SqlTransaction transaction, int masterAuthorId, int duplicateAuthorId)
        {
            int count = ExecuteScalar(transaction, @"
select count(1)
from dbo.Author
where status<>-1 and id in(@master_author_id,@duplicate_author_id)",
                Param("@master_author_id", masterAuthorId),
                Param("@duplicate_author_id", duplicateAuthorId));
            if (count != 2)
            {
                throw new InvalidOperationException("Master or duplicate author does not exist, or has already been deleted/merged.");
            }
        }

        private static void MoveAuthorMaps(SqlTransaction transaction, int masterAuthorId, int duplicateAuthorId)
        {
            ExecuteNonQuery(transaction, @"
update aim
set literature_author_map_id=masterMap.id,
    author_id=@master_author_id,
    updatetime=getdate()
from dbo.LiteratureAuthorInstitutionMap aim
inner join dbo.LiteratureAuthorMap dupMap on dupMap.id=aim.literature_author_map_id
inner join dbo.LiteratureAuthorMap masterMap on masterMap.literature_id=dupMap.literature_id and masterMap.author_id=@master_author_id
where dupMap.author_id=@duplicate_author_id;

delete dupMap
from dbo.LiteratureAuthorMap dupMap
inner join dbo.LiteratureAuthorMap masterMap on masterMap.literature_id=dupMap.literature_id and masterMap.author_id=@master_author_id
where dupMap.author_id=@duplicate_author_id;

update dbo.LiteratureAuthorMap
set author_id=@master_author_id
where author_id=@duplicate_author_id;",
                Param("@master_author_id", masterAuthorId),
                Param("@duplicate_author_id", duplicateAuthorId));
        }

        private static void MoveInstitutionMaps(SqlTransaction transaction, int masterAuthorId, int duplicateAuthorId)
        {
            ExecuteNonQuery(transaction, @"
update dbo.LiteratureAuthorInstitutionMap
set author_id=@master_author_id,
    updatetime=getdate()
where author_id=@duplicate_author_id;

;with duplicated as
(
    select id,
           row_number() over(partition by literature_id, author_id, institution_id, isnull(literature_author_map_id,0), isnull(affiliation_text,N'') order by id) as rn
    from dbo.LiteratureAuthorInstitutionMap
    where author_id=@master_author_id
)
delete from duplicated where rn>1;",
                Param("@master_author_id", masterAuthorId),
                Param("@duplicate_author_id", duplicateAuthorId));
        }

        private static void MoveInstitutionHistory(SqlTransaction transaction, int masterAuthorId, int duplicateAuthorId)
        {
            ExecuteNonQuery(transaction, @"
update dbo.AuthorInstitutionHistory
set author_id=@master_author_id,
    updatetime=getdate()
where author_id=@duplicate_author_id;

;with duplicated as
(
    select id,
           row_number() over(partition by author_id, institution_name, isnull(institution_id,0), isnull(source_literature_id,0), isnull(source_type,N'') order by status desc, is_current desc, id) as rn
    from dbo.AuthorInstitutionHistory
    where author_id=@master_author_id and status<>-1
)
update duplicated set status=-1 where rn>1;",
                Param("@master_author_id", masterAuthorId),
                Param("@duplicate_author_id", duplicateAuthorId));
        }

        private static void MarkDuplicateAuthor(SqlTransaction transaction, int masterAuthorId, int duplicateAuthorId)
        {
            bool hasMergedToColumn = ExecuteScalar(transaction, "select count(1) from sys.columns where object_id=object_id(N'dbo.Author') and name=N'merged_to_author_id'") > 0;
            string mergedToSql = hasMergedToColumn ? "merged_to_author_id=@master_author_id," : string.Empty;
            ExecuteNonQuery(transaction, @"
update masterAuthor
set name_cn=case when nullif(masterAuthor.name_cn,N'') is null then dup.name_cn else masterAuthor.name_cn end,
    name_en=case when nullif(masterAuthor.name_en,N'') is null then dup.name_en else masterAuthor.name_en end,
    orcid=case when nullif(masterAuthor.orcid,N'') is null then dup.orcid else masterAuthor.orcid end,
    email=case when nullif(masterAuthor.email,N'') is null then dup.email else masterAuthor.email end,
    updatetime=getdate()
from dbo.Author masterAuthor
inner join dbo.Author dup on dup.id=@duplicate_author_id
where masterAuthor.id=@master_author_id;

update dbo.Author
set status=-1,
    identity_status=N'merged',
    " + mergedToSql + @"
    merge_group_id=@master_author_id,
    updatetime=getdate()
where id=@duplicate_author_id;",
                Param("@master_author_id", masterAuthorId),
                Param("@duplicate_author_id", duplicateAuthorId));
        }

        private static void InsertMergeLog(SqlTransaction transaction, int masterAuthorId, int duplicateAuthorId, int adminId, string remark)
        {
            if (ExecuteScalar(transaction, "select count(1) from sys.objects where object_id=object_id(N'dbo.AuthorMergeLog') and type=N'U'") <= 0)
            {
                return;
            }

            ExecuteNonQuery(transaction, @"
insert into dbo.AuthorMergeLog(master_author_id, duplicate_author_id, admin_id, remark, addtime)
values(@master_author_id, @duplicate_author_id, @admin_id, @remark, getdate());",
                Param("@master_author_id", masterAuthorId),
                Param("@duplicate_author_id", duplicateAuthorId),
                Param("@admin_id", adminId),
                new SqlParameter("@remark", SqlDbType.NVarChar, 1000) { Value = (object)(remark ?? string.Empty) });
        }

        private static SqlParameter Param(string name, int value)
        {
            return new SqlParameter(name, SqlDbType.Int) { Value = value };
        }

        private static int ExecuteScalar(SqlTransaction transaction, string sql, params SqlParameter[] parameters)
        {
            using (SqlCommand command = new SqlCommand(sql, transaction.Connection, transaction))
            {
                command.Parameters.AddRange(parameters);
                object value = command.ExecuteScalar();
                return value == null || value == DBNull.Value ? 0 : Convert.ToInt32(value);
            }
        }

        private static void ExecuteNonQuery(SqlTransaction transaction, string sql, params SqlParameter[] parameters)
        {
            using (SqlCommand command = new SqlCommand(sql, transaction.Connection, transaction))
            {
                command.Parameters.AddRange(parameters);
                command.ExecuteNonQuery();
            }
        }
    }
}
