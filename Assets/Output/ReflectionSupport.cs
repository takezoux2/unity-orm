using System;

using System.Collections.Generic;

namespace UnityORM
{
	public class ReflectionSupport
	{
		
		public static T[] CreateNewInstances<T>(int count){
			var constructor = typeof(T).GetConstructor(new Type[0]);
			T[] objects = new T[count];
			object[] args = new object[0];
			for(int i = 0;i < count;i++){
				objects[i] = (T)constructor.Invoke(args);
			}
			return objects;
		}
		
		public static object CreateNewInstance(Type t){
			var constructor = t.GetConstructor(new Type[0]);
			return constructor.Invoke(new object[0]);
		}
		
		public ReflectionSupport ()
		{
		}
	}
}

