using System;

using System.Collections.Generic;
using UnityEngine;

namespace UnityORM
{
	public class SQLResultReader
	{
		public SQLResultReader ()
		{
		}
		
		public T[] Get<T>(DataTable result,ClassDesc desc){
			T[] objects = ReflectionSupport.CreateNewInstances<T>(result.Rows.Count);
			SetTo(result,0,desc,objects,0,objects.Length);
			return objects;
		}
		
		public int SetTo<T>(DataTable result,ClassDesc desc,T[] objects){
			return SetTo<T>(result,0,desc,objects,0,objects.Length);
		}
		
		public int SetTo<T>(DataTable result,int resultOffset,ClassDesc desc,T[] objects,int offset,int length){
			int readCount = Math.Min(result.Rows.Count - resultOffset,length);
			if(length < 0){
				readCount = result.Rows.Count - resultOffset;
			}
			for(int i = resultOffset;i < readCount;i++){
				var row = result.Rows[i + resultOffset];
				object obj = objects[i + offset];
				
				foreach(var f in desc.FieldDescs){
					if(row.ContainsKey(f.NameInTable)){
						f.SetFromDb(obj,row[f.NameInTable]);
					}else if(row.ContainsKey(f.NameInTable.ToLower())){
						f.SetFromDb(obj,row[f.NameInTable.ToLower()]);
					}
				}
				
			}
			return readCount;
		}
		
	}
}

