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
		
		
		public ClassDesc GetClassDesc<T>(){
			return GetClassDesc(typeof(T));
		}
			
		public ClassDesc GetClassDesc(Type t){
				
			
			ClassDesc desc;
			if(descriptions.ContainsKey(t)){
				desc = descriptions[t] as ClassDesc;
			}else{
				desc = Lister.ListUp(t);
				descriptions[t] = desc;
			}
			return desc;
		}
		
	}
}

