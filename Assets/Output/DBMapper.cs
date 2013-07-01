using System;

using System.Collections.Generic;
using UnityEngine;

namespace UnityORM
{
	public class DBMapper
	{
		SqliteDatabase database;
		public SQLResultReader ResultReader = new SQLResultReader();
		public ClassDescRepository Registory = ClassDescRepository.Instance;
		public SQLMaker SQLMaker = new SQLMaker();
		
		public SqliteDatabase Database{get{return database;}}
		
		public DBMapper (SqliteDatabase database)
		{
			this.database = database;
		}
		
		public T ReadByKey<T>(object key) where T : class{
			var desc = Registory.GetClassDesc<T>();
			var result = database.ExecuteQuery(
				SQLMaker.GenerateSelectSQL<T>(desc,key));
			T[] r = ResultReader.Get<T>(result,desc);
			if(r.Length > 0){
				return r[0];
			}else{
				return null;
			}
		}
		
		public T[] ReadAll<T>(){
			var desc = Registory.GetClassDesc<T>();
			return Read<T>("SELECT * FROM " + desc.Name);
		}
		
		public T[] Read<T>(string sql) {
			var result = database.ExecuteQuery(sql);
			var desc = Registory.GetClassDesc<T>();
			return ResultReader.Get<T>(result,desc);
		}
		
		public int ReadTo<T>(T[] objects,string sql){
			return ReadTo<T>(objects,0,objects.Length,sql);
		}
		
		public int ReadTo<T>(T[] objects,int offset,int size,string sql){
			var result = database.ExecuteQuery(sql);
			
			ClassDesc<T> desc = Registory.GetClassDesc<T>();
			
			return ResultReader.SetTo(result,0,desc,objects,offset,size);
		}
		
		/// <summary>
		/// Delete all record then insert.
		/// 
		/// </summary>
		/// <param name='objects'>
		/// Objects.
		/// </param>
		/// <typeparam name='T'>
		/// The 1st type parameter.
		/// </typeparam>
		public void ReplaceAll<T>(T[] objects){
			var desc = Registory.GetClassDesc<T>();
			string delete = SQLMaker.GenerateDeleteAllSQL<T>(desc);
			database.ExecuteNonQuery(delete);
			
			foreach(T obj in objects){
				string insert = SQLMaker.GenerateInsertSQL(desc,obj);
				database.ExecuteNonQuery(insert);
			}
		}
		
		public void UpdateAll<T>(T[] objects){
			var desc = Registory.GetClassDesc<T>();
			foreach(T obj in objects){
				string update = SQLMaker.GenerateUpdateSQL(desc,obj);
				database.ExecuteNonQuery(update);
			}
		}
		public void InsertAll<T>(T[] objects){
			var desc = Registory.GetClassDesc<T>();
			foreach(T obj in objects){
				string insert = SQLMaker.GenerateInsertSQL(desc,obj);
				database.ExecuteNonQuery(insert);
			}
		}
		public void UpdateOrInsertAll<T>(T[] objects){
			foreach(T obj in objects){
				UpdateOrInsert<T>(obj);
			}
		}
	
		public void UpdateOrInsert<T>(T obj){
			var desc = Registory.GetClassDesc<T>();
			string update = SQLMaker.GenerateUpdateSQL(desc,obj);
			try{
				int effectedRows = database.ExecuteNonQuery(update);
				if(effectedRows == 0){
					Debug.Log("No updates.Insert!");
					string insert = SQLMaker.GenerateInsertSQL(desc,obj);
					int r2 = database.ExecuteNonQuery(insert);
					Debug.Log("ResultCode = " + r2);	
				}
			}catch(SqliteException e){
				Debug.Log("Fail to update.Insert!");
				string insert = SQLMaker.GenerateInsertSQL(desc,obj);
				database.ExecuteNonQuery(insert);
			}
		}
		
		public void DeleteAll<T>(){
			var desc = Registory.GetClassDesc<T>();
			database.ExecuteNonQuery(SQLMaker.GenerateDeleteAllSQL<T>(desc));
		}
		
		public void DeleteByKey<T>(object key){
			var desc = Registory.GetClassDesc<T>();
			database.ExecuteNonQuery(SQLMaker.GenerateDeleteSQL<T>(desc,key));
		}
		
		
		
		
		
	}
}

