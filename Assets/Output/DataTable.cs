using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;


namespace UnityORM
{
	public class DataRow : Dictionary<string, object>
	{
	    public new object this[string column]
	    {
	        get
	        {
	            if (ContainsKey(column))
	            {
	                return base[column];
	            }
	            
	            return null;
	        }
	        set
	        {
	            if (ContainsKey(column))
	            {
	                base[column] = value;
	            }
	            else
	            {
	                Add(column, value);
	            }
	        }
	    }
		
		public DateTime GetAsDateTime(string column){
			return DateTime.FromBinary(GetAsLong(column));
		}
		
		public string GetAsString(string column){
			object v = this[column];
			if(v == null) return null;
			else return v.ToString();
		}
		public long GetAsLong(string column){
			object v = this[column];
			if(v == null){
				return 0;
			}else if(v is long){
				return (long)v;
			}else if( v is int){
				return (long)(int)v;
			}else if(v is double){
				return (long)(double)v;
			}else if(v is string){
				return long.Parse((string)v);
			}else{
				return int.Parse(v.ToString());
			}
		}
		
		public int GetAsInt(string column){
			return GetAsInt(column,0);
		}
		
		public int GetAsInt(string column,int defaultValue){
			object v = this[column];
			if(v == null){
				return defaultValue;
			}else if(v is long){
				return (int)(long)v;
			}else if( v is int){
				return (int)v;
			}else if(v is double){
				return (int)(double)v;
			}else if(v is string){
				return int.Parse((string)v);
			}else{
				return int.Parse(v.ToString());
			}
		}
		
		public double GetAsDouble(string column){
			return GetAsDouble(column,0);
		}
		
		public double GetAsDouble(string column,double defaultValue){
			object v = this[column];
			if(v == null){
				return defaultValue;
			}else if(v is long){
				return (double)(long)v;
			}else if( v is int){
				return (double)(int)v;
			}else if(v is double){
				return (double)v;
			}else if(v is string){
				return double.Parse((string)v);
			}else{
				return double.Parse(v.ToString());
			}
		}
	}
	
	public class DataTable
	{
	    public DataTable()
	    {
	        Columns = new List<string>();
	        Rows = new List<DataRow>();
	    }
	    
	    public List<string> Columns { get; set; }
	    public List<DataRow> Rows { get; set; }
	    
	    public DataRow this[int row]
	    {
	        get
	        {
	            return Rows[row];
	        }
	    }
	    
	    public void AddRow(object[] values)
	    {
	        if (values.Length != Columns.Count)
	        {
	            throw new IndexOutOfRangeException("The number of values in the row must match the number of column");
	        }
	        
	        var row = new DataRow();
	        for (int i = 0; i < values.Length; i++)
	        {
	            row[Columns[i]] = values[i];
	        }
	        
	        Rows.Add(row);
	    }
	}
	public static class SQLExtension{
		
		public static string SQLFormat(this string str,params object[] ps){
			
			var v = from p in ps select Convert(p);
			
			return string.Format(str, v.ToArray());
		}
		
		public static string Convert(object v){
			if(v == null){
				return "NULL";
			}else if(v is DateTime){
				return ((DateTime)v).ToBinary().ToString();
			}else if( v is string){
				return "'" + Escape((string)v) + "'";
			}else{
				return v.ToString();
			}
		}
		
		public static string Escape(string v){
			return v.Replace("\\","\\\\").Replace("\"","\\\"").Replace("'","\\'");
		}
	}
}