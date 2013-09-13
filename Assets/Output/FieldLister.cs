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
			converters.Add(typeof(string),new StringValueConverter());
			converters.Add(typeof(int),new IntValueConverter());
			converters.Add(typeof(long),new LongValueConverter());
			converters.Add(typeof(float),new FloatValueConverter());
			converters.Add(typeof(double),new DoubleValueConverter());
			converters.Add(typeof(bool),new BoolValueConverter());
			converters.Add(typeof(DateTime),new DateTimeValueConverter());
			converters.Add(typeof(Dictionary<string,object>),new DictionaryValueConverter());
		
		}	
		
		/// <summary>
		/// Control modify names for table and json according to naming rule or not.
		/// </summary>
		public bool ModifyAccordingToNamingRule = true;
		
		Dictionary<Type,ValueConverter> converters = new Dictionary<Type, ValueConverter>();
		
		ValueConverter GetConverter(Type t){
			
			if(converters.ContainsKey(t)){
				return converters[t];
			}else{
				ListUp(t);
				return converters[t];
			}
		}
		
		public virtual ClassDesc ListUp<T>(){
			return ListUp(typeof(T));
		}
		public virtual ClassDesc ListUp(Type t){
			ClassDesc classInfo;// = new ClassDesc(t);
			
			if(t.IsArray){
				classInfo = new ClassDesc(t.GetElementType());
				if(!converters.ContainsKey(t)){
					converters.Add(t,new ArrayValueConverter(classInfo));
				}
			}else{
				classInfo = new ClassDesc(t);
				if(!converters.ContainsKey(t)){
					converters.Add(t,new ClassValueConverter(classInfo));
				}
			}
			classInfo.Name = t.Name;
			
			bool keyFieldIsSetByAttribute = false;
			
			List<FieldDesc> infoList = new List<FieldDesc>();
			var props = t.GetProperties();
			foreach(var prop in props){
				if(prop.CanRead && prop.CanWrite){
					FieldDesc info = new FieldDesc();
					info.Prop = prop;
					info.ValueConverter = GetConverter(info.FieldType);
					
					if(info.GetAttribute<IgnoreAttribute>() == null){
						SetNames(info);
						
						infoList.Add(info);
						
						var keyAttribute = info.GetAttribute<KeyAttribute>();
						if(keyAttribute != null){
							classInfo.KeyField = info;
							keyFieldIsSetByAttribute = true;
							classInfo.AutoIncrement = keyAttribute.AutoIncrement;
						}else if(info.Name.ToLowerInvariant() == "id" && !keyFieldIsSetByAttribute){
							classInfo.KeyField = info;
							classInfo.AutoIncrement = false;
						} 
					}
					
				}
			}
			var fields = t.GetFields();
			foreach(var f in fields){
				if(f.IsPublic && !f.IsStatic && !f.IsInitOnly){
					FieldDesc info = new FieldDesc();
					info.Field = f;
					info.ValueConverter = GetConverter(info.FieldType);
					
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
				if(ModifyAccordingToNamingRule && !string.IsNullOrEmpty(desc.Name) && char.IsUpper( desc.Name[0])){
					desc.NameInJSON = char.ToLower(desc.Name[0]) + desc.Name.Substring(1);
				}
			}
			if(att != null && !string.IsNullOrEmpty(att.NameInTable)){
				desc.NameInTable = att.NameInTable;
//			}else if(ModifyAccordingToNamingRule){
//				desc.NameInTable = desc.Name.ToLower();
			}else{
				desc.NameInTable = desc.Name;
			}
		}
	}
	
	public class ClassDesc{
		
		public Type ClassType;
		public string Name;
		public bool AutoIncrement = false;
		public FieldDesc KeyField;
		public List<FieldDesc> FieldDescs;
		
		public ClassDesc(Type classType){
			this.ClassType = classType;
		}
		
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
		public ValueConverter ValueConverter;
		
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
		
		public object GetForJson(object obj){
			return ValueConverter.ToJson(GetValue(obj));	
		}
		public object GetForDb(object obj){
			return ValueConverter.ToDb(GetValue(obj));
		}
		
		object GetValue(object obj){
			if(Prop != null) return Prop.GetValue(obj,new object[0]);
			else return Field.GetValue(obj);
		}
		
		public void SetFromJson(object obj,object v){
			SetValue(obj,ValueConverter.FromJson(v));
		}
		public void SetFromDb(object obj,object v){
			SetValue(obj,ValueConverter.FromDb(v));
		}
		
		void SetValue(object obj,object v){
			if(Prop != null){
				Prop.SetValue(obj,v,new object[0]);
			}else{
				Field.SetValue(obj,v);
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

