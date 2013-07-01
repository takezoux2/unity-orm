using System;

namespace UnityORM
{
	
	public class KeyAttribute : Attribute{
		public bool AutoIncrement;
	}
	
	
	public class MetaInfoAttirbute : Attribute{
			
		public string NameInJSON;
		public string NameInTable;
		
	}
	
	public class IgnoreAttribute : Attribute{
	}
			
}

