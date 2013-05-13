using System;

using System.Collections.Generic;

namespace UnityORM
{
	public class ClassDescRepository
	{
		static ClassDescRepository instance;
		public static ClassDescRepository Instance{
			get{
				if(instance == null){
					instance = new ClassDescRepository();
				}
				return instance;
			}
		}
		
		Dictionary<Type,object> descriptions = new Dictionary<Type, object>();
		
		
		public FieldLister Lister = new FieldLister();
		
		private ClassDescRepository ()
		{
		}
		
		
		public ClassDesc<T> GetClassDesc<T>(){
			
			ClassDesc<T> desc;
			if(descriptions.ContainsKey(typeof(T))){
				desc = descriptions[typeof(T)] as ClassDesc<T>;
			}else{
				desc = Lister.listUp<T>();
				descriptions[typeof(T)] = desc;
			}
			return desc;
		}
		
	}
}

