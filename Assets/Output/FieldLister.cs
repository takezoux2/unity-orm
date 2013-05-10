using System;

using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace UnityORM
{
	
	public class FieldLister
	{
		public FieldLister ()
		{
		}	
		
		
		
		public ClassDesc<T> listUp<T>(){
			Type t = typeof(T);
			ClassDesc<T> classInfo = new ClassDesc<T>();
			classInfo.Name = t.Name;
			
			
			List<FieldDesc> infoList = new List<FieldDesc>();
			var props = t.GetProperties();
			foreach(var prop in props){
				if(prop.CanRead && prop.CanWrite){
					FieldDesc info = new FieldDesc();
					info.Prop = prop;
					SetNames(info);
					
					infoList.Add(info);
					
					if(info.Name.ToLowerInvariant() == "id"){
						classInfo.KeyField = info;
					}
					
				}
			}
			var fields = t.GetFields();
			foreach(var f in fields){
				if(f.IsPublic && !f.IsStatic && !f.IsInitOnly){
					FieldDesc info = new FieldDesc();
					info.Field = f;
					SetNames(info);
					
					if(info.Name.ToLowerInvariant() == "id"){
						classInfo.KeyField = info;
					}
					infoList.Add(info);
				}
			}
			
			classInfo.FieldDescs = infoList;
			
			return classInfo;
		}
		
		void SetNames(FieldDesc desc){
			desc.NameInJSON = desc.Name;
			desc.NameInTable = desc.Name.ToLower(); 
		}
	}
	
	public class KeyAttribute : Attribute{
	}
	
	public class MetaInfoAttirbute : Attribute{
			
	}
	
	public class IgnoreAttribute : Attribute{
	}
			
	public class ClassDesc<T>{
		
		public string Name;
		public FieldDesc KeyField;
		public List<FieldDesc> FieldDescs;
		
		public override string ToString ()
		{
			var toS = from f in FieldDescs select f.ToString();
			var s = string.Join(",",toS.ToArray());
			return string.Format ("Name:{0} Key:{1} Fields:{2}",Name,KeyField != null ? KeyField.Name : "none",s);
		}
		
	}
	
	public class FieldDesc{
		
		public PropertyInfo Prop;
		public FieldInfo Field;
		
		public string Name{
			get{
				if(Prop != null)return Prop.Name;
				else return Field.Name;
			}
		}
		
		public Type FieldType{
			get{
				if(Prop != null)return Prop.PropertyType;
				else return Field.FieldType;
			}
		}
		
		public string NameInTable;
		public string NameInJSON;
		
		public object GetValue(object obj){
			if(Prop != null) return Prop.GetValue(obj,new object[0]);
			else return Field.GetValue(obj);
		}
		
		
		
		public void SetValue(object obj,object v){
			if(Prop != null){
				Prop.SetValue(obj,CastIfNeeded(Prop.PropertyType,v),new object[0]);
			}else{
				Field.SetValue(obj,CastIfNeeded(Field.FieldType,v));
			}
		}
		public object CastIfNeeded(Type fieldType,object v){
			if(v is long){
				if(fieldType == typeof(int)){
					return (int)(long)v;
				}else{
					return v;
				}
			}else{
				return v;
			}
		}
		
		
		public override string ToString ()
		{
			return string.Format("Name:{0}",Name);
		}
		
		
	}
}

