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
		
		
		
		public virtual ClassDesc<T> listUp<T>(){
			Type t = typeof(T);
			ClassDesc<T> classInfo = new ClassDesc<T>();
			classInfo.Name = t.Name;
			
			bool keyFieldIsSetByAttribute = false;
			
			List<FieldDesc> infoList = new List<FieldDesc>();
			var props = t.GetProperties();
			foreach(var prop in props){
				if(prop.CanRead && prop.CanWrite){
					FieldDesc info = new FieldDesc();
					info.Prop = prop;
					
					if(info.GetAttribute<IgnoreAttribute>() == null){
						SetNames(info);
						
						infoList.Add(info);
						
						if(info.GetAttribute<KeyAttribute>() != null){
							classInfo.KeyField = info;
							keyFieldIsSetByAttribute = true;
						}else if(info.Name.ToLowerInvariant() == "id" && !keyFieldIsSetByAttribute){
							classInfo.KeyField = info;
						} 
					}
					
				}
			}
			var fields = t.GetFields();
			foreach(var f in fields){
				if(f.IsPublic && !f.IsStatic && !f.IsInitOnly){
					FieldDesc info = new FieldDesc();
					info.Field = f;
					if(info.GetAttribute<IgnoreAttribute>() == null){
						SetNames(info);
						
						if(info.GetAttribute<KeyAttribute>() != null){
							classInfo.KeyField = info;
							keyFieldIsSetByAttribute = true;
						}else if(info.Name.ToLowerInvariant() == "id" && !keyFieldIsSetByAttribute){
							classInfo.KeyField = info;
						}
						infoList.Add(info);
					}
				}
			}
			
			classInfo.FieldDescs = infoList;
			
			return classInfo;
		}
		
		protected virtual void SetNames(FieldDesc desc){
			MetaInfoAttirbute att = desc.GetAttribute<MetaInfoAttirbute>();
			
			if(att != null && !string.IsNullOrEmpty(att.NameInJSON)){
				desc.NameInJSON = att.NameInJSON;
			}else{
				desc.NameInJSON = desc.Name;
			}
			if(att != null && !string.IsNullOrEmpty(att.NameInTable)){
				desc.NameInTable = att.NameInTable;
			}else{
				desc.NameInTable = desc.Name.ToLower(); 
			}
		}
	}
	
	public class KeyAttribute : Attribute{
	}
	
	public class MetaInfoAttirbute : Attribute{
			
		public string NameInJSON;
		public string NameInTable;
		
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
		
		
		public T GetAttribute<T>() where T : Attribute{
			if(Field != null){
				var att = Attribute.GetCustomAttributes(Field,typeof(T),true);
				if(att.Length > 0) return att[0] as T;
				else return null;
			}else if(Prop != null){
				var att = Attribute.GetCustomAttributes(Prop,typeof(T),true);
				if(att.Length > 0) return att[0] as T;
				else return null;
			}
			return null;
		}
		
	}
}

