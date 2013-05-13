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
		
		Dictionary<Type,object> descriptions = new Dictionary<Type, object>();
		
		public DBMapper (SqliteDatabase database)
		{
			this.database = database;
		}
		
		
		public T[] Read<T>(string sql) {
			var result = database.ExecuteQuery(sql);
			var desc = Registory.GetClassDesc<T>();
			return ResultReader.Get<T>(result,desc);
		}
		
		public int ReadTo<T>(string sql,T[] objects){
			return ReadTo<T>(sql,objects,0,objects.Length);
		}
		
		public int ReadTo<T>(string sql,T[] objects,int offset,int size){
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
			string delete = SQLMaker.GenerateDeleteSQL<T>(desc);
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
		
		
	}
}

