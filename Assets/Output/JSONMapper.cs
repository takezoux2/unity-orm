using System;

using System.Collections.Generic;
using UnityORM.Helper;
using System.Collections;

namespace UnityORM
{
	public class JSONMapper
	{
		public static JSONMapper DefaultJsonMapper = new JSONMapper();
		
		public static readonly DateTime UnixTime = new DateTime(1970,1,1);
		
		ClassDescRepository Repository = ClassDescRepository.Instance;
		
		
		public JSONMapper ()
		{
		}
		
		
		public T[] Read<T>(string json){
			var jObjs = Json.Deserialize(json);
			return ReadFromJSONObject<T>(jObjs);
		}
		
		public object[] Read(Type t , string json){
			
			object jObjs = Json.Deserialize(json);
			return ReadFromJSONObject(t,jObjs);
		}
		
		public object[] ReadFromJSONObject(Type t , object jObjs){
			int size;
			
			if(jObjs is Dictionary<string,object>)
			{
				size = 1;
			}else if(jObjs is System.Collections.IList){
				size = (jObjs as System.Collections.IList).Count;
			}else if(jObjs.GetType().IsArray){
				size = (jObjs as object[]).Length;
			}else{
				throw new Exception(
					@"Wrong json object format.Must be List<Dictionary<stirng,object>> or Dictionary<string,object>.But was " + jObjs.GetType().Name);
			}
			
			Array objects =  Array.CreateInstance(t,size);
			for(int i=0;i<size;i++){
				objects.SetValue(ReflectionSupport.CreateNewInstance(t),i);
			}
			
			LoadFromJSONObject(t,jObjs,(object[])objects,0,size);
			return (object[])objects;
		}
		
		public T[] ReadFromJSONObject<T>(object jObjs){
			int size;
			
			if(jObjs is Dictionary<string,object>)
			{
				size = 1;
			}else if(jObjs is System.Collections.IList){
				size = (jObjs as System.Collections.IList).Count;
			}else if(jObjs.GetType().IsArray){
				size = (jObjs as object[]).Length;
			}else{
				throw new Exception(
					@"Wrong json object format.Must be List<Dictionary<stirng,object>> or Dictionary<string,object>.But was " + jObjs.GetType().Name);
			}
			
			T[] objects = ReflectionSupport.CreateNewInstances<T>(size);
			LoadFromJSONObject<T>(jObjs,objects,0,size);
			return objects;
		}
		
		public int Load<T>(string json, T[] objects){
			var jObjs = Json.Deserialize(json);
			return LoadFromJSONObject<T>(jObjs,objects,0,objects.Length);
		}
		
		public int LoadFromJSONObject(Type t , object jsonObj, object[] objects,int offset,int size){
			if(size == 0) return 0;
			if(jsonObj is Dictionary<string,object>){
				var jObj = jsonObj as Dictionary<string,object>;
				var obj = objects[offset];
				LoadObj(jObj,obj);
				
				return 1;
			}else if(jsonObj.GetType().IsArray){
				var jobjs = jsonObj as object[];
				
				int readSize = Math.Min(size,jobjs.Length);
				if(size < 0){
					readSize = Math.Min(jobjs.Length,objects.Length - offset);
				}
				for(int i = 0;i < readSize;i++){
					var jobj = jobjs[i] as Dictionary<string,object>;
					if(jobj == null) throw new Exception("Wrong json object format.Must be List<Dictionary<stirng,object>>");
					LoadObj(jobj,objects[offset + i]);
					
				}
				return readSize;
				
			}else if(jsonObj is IList){
				var jobjs = jsonObj as IList;
				
				int readSize = Math.Min(size,jobjs.Count);
				if(size < 0){
					readSize = Math.Min(jobjs.Count,objects.Length - offset);
				}
				for(int i = 0;i < readSize;i++){
					var jobj = jobjs[i] as Dictionary<string,object>;
					if(jobj == null) throw new Exception("Wrong json object format.Must be List<Dictionary<stirng,object>>");
					LoadObj(jobj,objects[offset + i]);
					
				}
				return readSize;
			}else{
				throw new Exception("Wrong json object format.Must be List<Dictionary<stirng,object>> or Dictionary<string,object>");
			}
		}
		
		public int LoadFromJSONObject<T>(object jsonObj, T[] objects,int offset,int size){
			object[] temp = new object[objects.Length];
			for(int i=0;i<objects.Length;i++){
				temp[i] =objects[i];
			}
			return LoadFromJSONObject(typeof(T),jsonObj,temp,offset,size);
		}
				
		void LoadObj(Dictionary<string,object> jObj,object target){
			var classDesc = Repository.GetClassDesc(target.GetType());
			foreach(var field in classDesc.FieldDescs){
				if(jObj.ContainsKey(field.NameInJSON)){
					field.SetFromJson(target,jObj[field.NameInJSON]);
				}
			}
		}
		
		public object ToJsonObject(params object[] objects){
			
			List<Dictionary<string,object>> jsonObjs = new List<Dictionary<string, object>>();
			foreach(var obj in objects){
				Dictionary<string,object> dict = new Dictionary<string, object>();
				if(obj == null) continue;
				
				var classDesc = Repository.GetClassDesc(obj.GetType());
				foreach(var field in classDesc.FieldDescs){
					dict.Add(field.NameInJSON,field.GetForJson(obj) );
				}
				jsonObjs.Add(dict);
			}
			if(jsonObjs.Count ==1){
				return jsonObjs[0];
			}else{
				return jsonObjs;
			}
		}
		
		public object ToJsonObject<T>(params T[] objects){
			
			var classDesc = Repository.GetClassDesc<T>();
			
			List<Dictionary<string,object>> jsonObjs = new List<Dictionary<string, object>>();
			
			foreach(T obj in objects){
				Dictionary<string,object> dict = new Dictionary<string, object>();
				
				foreach(var field in classDesc.FieldDescs){
					dict.Add(field.NameInJSON,field.GetForJson(obj) );
				}
				jsonObjs.Add(dict);
			}
			
			if(jsonObjs.Count ==1){
				return jsonObjs[0];
			}else{
				return jsonObjs;
			}
		}
		
		public string Write(params object[] objects){
			return Json.Serialize(ToJsonObject(objects));
		}
		
		public string Write<T>(params T[] objects){
			
			object jsonObjs = ToJsonObject<T>(objects);
			
			return Json.Serialize(jsonObjs);
		}
		
	}
}

