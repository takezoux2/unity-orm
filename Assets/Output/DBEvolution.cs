using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityORM
{
    /// <summary>
    /// Schme management tool.
    /// Execute only diff sqls.
    /// </summary>
    public class DBEvolution
    {
        private string dbFilePath;
        private SqliteDatabase database;

        public SqliteDatabase Database {
            get {
                if (database == null)
                {
                    database = new SqliteDatabase ();
                    
                    database.Open (dbFilePath);
                    CreateMetaTable ();
                }
                return database;
            }
        }

        public bool RecreateTableIfHashDiffers{ get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="com.geishatokyo.DB.DBEvolution"/> class.
        /// </summary>
        /// <param name='dbFilePath'>
        /// Db file path.
        /// </param>
        public DBEvolution (string dbFilePath)
        {
            this.dbFilePath = dbFilePath;
            RecreateTableIfHashDiffers = false;
        }

        /// <summary>
        /// Evolute the specified tableName with sqls.
        /// </summary>
        /// <param name='tableName'>
        /// Table name.
        /// </param>
        /// <param name='sqls'>
        /// Sqls.
        /// </param>
        public void Evolute (string tableName, List<string> sqls)
        {
            TableVersion version = GetTableVersion (tableName);
            var db = this.Database;
            if (RecreateTableIfHashDiffers && version.QueryHash != null)
            {
                string hash;
                if (sqls.Count > version.QueryIndex)
                {
                    hash = GetHash (sqls.GetRange (0, version.QueryIndex + 1));
                }
                else
                {
                    hash = string.Empty;
                }
                if (hash != version.QueryHash)
                {
                    Debug.Log (string.Format ("Table hash error.OnDB:{0} Passed:{1}", version.QueryHash, hash));
                    DropTable (tableName);
                    version.QueryIndex = -1;
                    version.QueryHash = null;
                }
            }

            // TODO: use updated?
            //bool updated = false;
            int successIndex = -1;
            bool error = false;

            for (int i = version.QueryIndex + 1; i < sqls.Count; i++)
            {
                Debug.Log (string.Format ("Execute query of {0}:{1}", tableName, i));
                //updated = true;
                try
                {
                    db.ExecuteNonQuery (sqls [i]);
                    successIndex = i;
                }
                catch (SqliteException e)
                {
                    error = true;
                    Debug.LogError ("Error SQL:" + sqls [i]);
                    Debug.LogException (e);
                    break;
                }
            }

            if (successIndex >= 0)
            {
                sqls = sqls.GetRange (0, successIndex + 1);
                if (version.QueryHash == null)
                {
                    string hashCode = GetHash (sqls);
                    CreateRecord (tableName, sqls.Count - 1, hashCode);
                }
                else
                {
                    string hashCode = GetHash (sqls);
                    UpdateRecord (tableName, sqls.Count - 1, hashCode);
                }
            }

            if (error)
            {
                Debug.LogError ("There are some errors.Please fix your sql");
            }
        }

        public void Close ()
        {
            if (database != null)
            {
                database.Close ();
                database = null;
            }
        }

        private string GetHash (List<string> sqls)
        {
            string join = string.Join ("_", sqls.ToArray ());
            return join.GetHashCode ().ToString ();
        }

        void CreateRecord (string tableName, int index, string queryHash)
        {
            string sql = string.Format (
                @"INSERT INTO MetaTable VALUES('{0}',{1},'{2}');", tableName, index, queryHash);
            Database.ExecuteNonQuery (sql);
        }

        void UpdateRecord (string tableName, int index, string queryHash)
        {
            string sql = string.Format (
                @"UPDATE MetaTable SET queryIndex={0},queryHash='{1}' WHERE tableName='{2}';",
                index, queryHash, tableName);
            Database.ExecuteNonQuery (sql);
        }

        private void DropTable (string tableName)
        {
            Debug.Log ("DROP TABLE IF EXISTS " + tableName);
            string sql = string.Format (@"DROP TABLE IF EXISTS {0};", tableName);
            Database.ExecuteNonQuery (sql);

            string metaSql = string.Format (@"DELETE FROM MetaTable WHERE tableName='{0}';", tableName);
            Database.ExecuteNonQuery (metaSql);
        }

        private TableVersion GetTableVersion (string tableName)
        {
            string sql = @"SELECT * FROM MetaTable WHERE tableName = '" + tableName + "';";
            DataTable dt = Database.ExecuteQuery (sql);
            
            TableVersion tv = new TableVersion ();
            if (dt.Rows.Count > 0)
            {
                tv.TableName = tableName;
                var row = dt.Rows [0];
                tv.QueryIndex = row.GetAsInt ("queryIndex");
                tv.QueryHash = row ["queryHash"].ToString ();
            }
            else
            {
                tv.TableName = tableName;
                tv.QueryIndex = -1;
                tv.QueryHash = null;
            }
            return tv;
        }

        private bool CreateMetaTable ()
        {
            string createTable = @"CREATE TABLE IF NOT EXISTS MetaTable(
tableName VARCHAR(100) NOT NULL,
queryIndex Int NOT NULL,
queryHash VARCHAR(100) NOT NULL);";
            Database.ExecuteNonQuery (createTable);
            // TODO fix to check table is really created.
            return true;
        }
    }

    struct TableVersion
    {
        public string TableName;
        public int QueryIndex;
        public string QueryHash;
    }
}
