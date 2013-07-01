using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Text;


namespace UnityORM.Helper{

	public static class JSONHelper{
		
		public static object Array(params object[] values){
			return values;
		}
		
		public static Dictionary<string,object> Dict(params object[] values){
			Dictionary<string,object> d = new Dictionary<string, object>();
			for(int i = 1;i < values.Length;i += 2){
				d.Add(values[i-1].ToString(),values[i]);
			}
			return d;
		}
		
		public static string Jasonize( params object[] dictValues){
			
			Dictionary<string,object> d = new Dictionary<string, object>();
			for(int i = 1;i < dictValues.Length;i += 2){
				d.Add(dictValues[i-1].ToString(),dictValues[i]);
			}
			return UnityORM.Json.Serialize(d);
		}
		
		
		public static T GetGet<T>(this IDictionary<string,object> dict,string key1,string key2){
			IDictionary d;
			try{
				d = (IDictionary)dict[key1];
				if(d == null){
					Debug.Log("Key:" + key1 + " not found");
					return default(T);
				}
			}catch(InvalidCastException e){
				Debug.Log("Wrong cast at key:" + key1 + ";" + e.Message);
			    return default(T);
			}
		    return (T)d[key2];
		}
		public static T Get<T>(this IDictionary<string,object> dict,string key){
		    return (T)dict[key];
		}
		
		public static IList<object> GetList(this IDictionary<string,object> dict, string key){
			return (IList<object>)dict[key];
		}
		public static IDictionary<string,object> GetDict(this IDictionary<string,object> dict,string key){
			return (IDictionary<string,object>)dict[key];	
		}
		
	}
}