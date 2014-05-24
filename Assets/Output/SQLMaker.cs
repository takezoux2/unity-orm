using System;
using System.Text;
using System.Collections.Generic;

namespace UnityORM
{
    public class SQLMaker
    {
        public readonly static DateTime UnixTime = new DateTime (1970, 1, 1);

        public SQLMaker ()
        {
        }

        public string GenerateCreateTableSQL<T> (ClassDesc desc)
        {
            var builder = new StringBuilder (); 
            builder.Append ("CREATE TABLE IF NOT EXISTS " + desc.Name + "(");
            foreach (var f in desc.FieldDescs)
            {
                builder.Append (f.nameInTable + " " + ConvertToSQLType (f.FieldType));
                if (f == desc.KeyField)
                {
                    builder.Append (" PRIMARY KEY");
                }
                if (desc.AutoIncrement)
                {
                    builder.Append (" AUTOINCREMENT");
                }
                builder.Append (",");
            }
            builder.Remove (builder.Length - 1, 1);
            builder.Append (");");
            return builder.ToString ();
        }

        private string ConvertToSQLType (Type t)
        {
            if (t == typeof(int) ||
                t == typeof(long))
            {
                return "INTEGER";
            }
            else if (t == typeof(double))
            {
                return "FLOAT";
            }
            else if (t == typeof(byte[]))
            {
                return "BLOB";
            }
            else if (t == typeof(DateTime))
            {
                return "INTEGER";
            }
            else
            {
                return "TEXT";
            }
        }

        public string GenerateSelectAllSQL<T> (ClassDesc desc)
        {
            return "SELECT * FROM " + desc.Name + ";";
        }

        public string GenerateSelectSQL<T> (ClassDesc desc, object key)
        {
            if (desc.KeyField == null)
            {
                throw new Exception ("Class " + desc.Name + " hasn't key field");
            }
            return "SELECT * FROM " + desc.Name + " WHERE " + desc.KeyField.nameInTable + " = " + ValueToBlock (key) + ";";
        }

        public string GenerateDeleteAllSQL<T> (ClassDesc desc)
        {
            return "DELETE FROM " + desc.Name + ";";
        }

        public string GenerateDeleteSQL<T> (ClassDesc desc, object key)
        {
            if (desc.KeyField == null)
            {
                throw new Exception ("Class " + desc.Name + " hasn't key field");
            }
            return "DELETE FROM " + desc.Name + " WHERE " + desc.KeyField.nameInTable + " = " + ValueToBlock (key) + ";";
        }

        public string GenerateInsertSQL<T> (ClassDesc desc, T obj)
        {
            var columnsBuilder = new StringBuilder (); 
            var valuesBuilder = new StringBuilder (); 
            columnsBuilder.Append ("INSERT INTO " + desc.Name + " (");

            var fields = desc.FieldDescs;
            if (desc.AutoIncrement)
            {
                fields = new List<UnityORM.FieldDesc> (fields);
                fields.Remove (desc.KeyField);
            }

            foreach (var f in fields)
            {
                // columns
                columnsBuilder.Append (f.nameInTable + ",");
                // values
                object v = f.GetForDb (obj);
                valuesBuilder.Append (ValueToBlock (v) + ",");
            }

            columnsBuilder.Remove (columnsBuilder.Length - 1, 1);
            columnsBuilder.Append (") VALUES (");

            valuesBuilder.Remove (valuesBuilder.Length - 1, 1);
            valuesBuilder.Append (");");
            columnsBuilder.Append (valuesBuilder);

            return columnsBuilder.ToString ();
        }

        public string GenerateUpdateSQL<T> (ClassDesc desc, T obj)
        {
            if (desc.KeyField == null)
            {
                throw new Exception ("Class " + desc.Name + " hasn't key field");
            }
            var builder = new StringBuilder (); 
            builder.Append ("UPDATE " + desc.Name + " SET ");
            foreach (var f in desc.FieldDescs)
            {
                object v = f.GetForDb (obj);
                builder.Append (f.nameInTable + "=" + ValueToBlock (v) + ",");
            }
            builder.Remove (builder.Length - 1, 1);
            builder.Append (" WHERE " + desc.KeyField.nameInTable + " = " + ValueToBlock (desc.KeyField.GetForDb (obj)) + ";");
            
            return builder.ToString ();
        }

        private string ValueToBlock (object v)
        {
            if (v == null)
            {
                return "NULL";
            }
            else
            {
                return "'" + Escape (v.ToString ()) + "'";
            }
        }

        private string Escape (string str)
        {
            return str.Replace ("'", "''");
        }
    }
}
