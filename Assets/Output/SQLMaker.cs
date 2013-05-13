using System;
using System.Text;


namespace UnityORM
{
	public class SQLMaker
	{
		
		public readonly static DateTime UnixTime = new DateTime(1970,1,1);
		
		public SQLMaker ()
		{
		}
		
		public string GenerateCreateTableSQL<T>(ClassDesc<T> desc){
			
			var builder = new StringBuilder();	
			builder.Append("CREATE TABLE IF NOT EXISTS " + desc.Name + "(");
			foreach(var f in desc.FieldDescs){
				builder.Append(f.NameInTable + " " + ConvertToSQLType(f.FieldType ));
				if(f == desc.KeyField){
					builder.Append(" PRIMARY KEY");
				}
				builder.Append(",");
			}
			builder.Remove(builder.Length - 1,1);
			builder.Append(");");
			return builder.ToString();
		}
		string ConvertToSQLType(Type t){
			if(t == typeof(int) ||
				t == typeof(long)){
				return "INTEGER";
			}else if(t == typeof(double)){
				return "FLOAT";
			}else if(t == typeof(byte[])){
				return "BLOB";
			}else if(t == typeof(DateTime)){
				return "INTEGER";
			}else{
				return "TEXT";
			}
		}
		
		public string GenerateSelectSQL<T>(ClassDesc<T> desc,object key){
			if(desc.KeyField == null) throw new Exception("Class " + desc.Name + " hasn't key field");
			return "SELECT * FROM " + desc.Name + " WHERE " + desc.KeyField.NameInTable + " = " + ValueToBlock(key) + ";";
		}
		
		public string GenerateDeleteSQL<T>(ClassDesc<T> desc){
			return "DELETE FROM " + desc.Name + ";";
		}
		
		public string GenerateInsertSQL<T>(ClassDesc<T> desc,T obj){
			var builder = new StringBuilder();	
			builder.Append("INSERT INTO " + desc.Name + " (");
			foreach( var f in desc.FieldDescs){
				builder.Append(f.NameInTable + ",");
			}
			builder.Remove(builder.Length - 1,1);
			builder.Append(") VALUES (");
			
			foreach( var f in desc.FieldDescs){
				object v = f.GetValue(obj);
				builder.Append(ValueToBlock(v) + ",");
			}
			builder.Remove(builder.Length - 1,1);
			builder.Append(");");
			
			return builder.ToString();
		}
		
		public string GenerateUpdateSQL<T>(ClassDesc<T> desc,T obj){
			if(desc.KeyField == null) throw new Exception("Class " + desc.Name + " hasn't key field");
			var builder = new StringBuilder();	
			builder.Append("UPDATE " + desc.Name + " SET ");
			foreach(var f in desc.FieldDescs){
				object v = f.GetValue(obj);
				builder.Append(f.NameInTable + "=" + ValueToBlock(v) + ",");
			}
			builder.Remove(builder.Length -1 ,1);
			builder.Append(" WHERE " + desc.KeyField.NameInTable + " = " + ValueToBlock(desc.KeyField.GetValue(obj)) + ";");
			
			return builder.ToString();
		}
		
		
		string ValueToBlock(object v){
			if(v == null){
				return "NULL";
			}else{
				if( v is DateTime){
					return ((long)(((DateTime)v) - UnixTime).TotalMilliseconds).ToString();
				}else{
					return "'" + Escape(v.ToString()) + "'";
				}
			}
		}
		
		public string Escape(string str){
			return str.Replace("\\","\\\\").Replace("'","\\'").Replace("%","\\%");
		}
		
		
	}
}

