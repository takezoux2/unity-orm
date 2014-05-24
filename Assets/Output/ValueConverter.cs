using System;

using System.Collections.Generic;

namespace UnityORM
{
    public abstract class ValueConverter
    {
        public abstract object FromJson (object o);

        public virtual object FromDb (object o)
        {
            return FromJson (o);
        }

        public virtual object ToJson (object o)
        {
            return o;
        }

        public virtual object ToDb (object o)
        {
            return ToJson (o);
        }
    }

    public class StringValueConverter : ValueConverter
    {
        public override object FromJson (object o)
        {
            if (o == null)
            {
                return null;
            }
            else if (o is string)
            {
                return (string)o;
            }
            else
            {
                return o.ToString ();
            }
        }
    }

    public class IntValueConverter : ValueConverter
    {
        public override object FromJson (object o)
        {
            if (o == null)
            {
                return 0;
            }
            else if (o is long)
            {
                return (int)(long)o;
            }
            else if (o is int)
            {
                return (int)o;
            }
            else if (o is double)
            {
                return (int)(double)o;
            }
            else if (o is float)
            {
                return (int)(float)o;
            }
            else
            {
                return int.Parse (o.ToString ());
            }
        }
    }

    public class LongValueConverter : ValueConverter
    {
        public override object FromJson (object o)
        {
            if (o == null)
            {
                return 0L;
            }
            else if (o is long)
            {
                return (long)o;
            }
            else if (o is int)
            {
                return (long)(int)o;
            }
            else if (o is double)
            {
                return (long)(double)o;
            }
            else if (o is float)
            {
                return (long)(float)o;
            }
            else
            {
                return long.Parse (o.ToString ());
            }
        }
    }

    public class DoubleValueConverter : ValueConverter
    {
        public override object FromJson (object o)
        {
            if (o == null)
            {
                return 0.0;
            }
            else if (o is long)
            {
                return (double)(long)o;
            }
            else if (o is int)
            {
                return (double)(long)(int)o;
            }
            else if (o is double)
            {
                return (double)o;
            }
            else if (o is float)
            {
                return (double)(float)o;
            }
            else
            {
                return double.Parse (o.ToString ());
            }
        }
    }

    public class FloatValueConverter : ValueConverter
    {
        public override object FromJson (object o)
        {
            if (o == null)
            {
                return 0.0f;
            }
            else if (o is long)
            {
                return (float)(long)o;
            }
            else if (o is int)
            {
                return (float)(int)o;
            }
            else if (o is double)
            {
                return (float)(double)o;
            }
            else if (o is float)
            {
                return (float)o;
            }
            else
            {
                return long.Parse (o.ToString ());
            }
        }
    }

    public class DateTimeValueConverter : ValueConverter
    {
        public override object FromJson (object o)
        {
            if (o == null)
            {
                return new DateTime (0);
            }
            else if (o is long)
            {
                return new DateTime (SQLMaker.UnixTime.Ticks + (long)o * 1000 * 1000 * 10);
            }
            else if (o is int)
            {
                return new DateTime (SQLMaker.UnixTime.Ticks + ((long)(int)o) * 1000 * 1000 * 10);
            }
            else if (o is double)
            {
                return new DateTime (SQLMaker.UnixTime.Ticks + (long)((double)o * 1000 * 1000 * 10));
            }
            else if (o is float)
            {
                return new DateTime (SQLMaker.UnixTime.Ticks + (long)((float)o * 1000 * 1000 * 10));
            }
            else
            {
                try
                {
                    return new DateTime (SQLMaker.UnixTime.Ticks + long.Parse (o.ToString ()) * 1000 * 10);
                }
                catch (Exception e)
                {
                    UnityEngine.Debug.LogError (e.Message);
                    return DateTime.Parse (o.ToString ());
                }
            }
        }

        public override object ToJson (object o)
        {
            return (long)((((DateTime)o) - SQLMaker.UnixTime).TotalSeconds);
        }

        public override object FromDb (object o)
        {
            return new DateTime (SQLMaker.UnixTime.Ticks + (long)o * 1000 * 10);
        }

        public override object ToDb (object o)
        {
            return (long)((((DateTime)o) - SQLMaker.UnixTime).TotalMilliseconds);
        }
    }

    public class BoolValueConverter : ValueConverter
    {
        public override object FromJson (object o)
        {
            if (o == null)
            {
                return false;
            }
            else if (o is long)
            {
                return (long)o > 0;
            }
            else if (o is int)
            {
                return (int)o > 0;
            }
            else if (o is double)
            {
                return (double)o > 0;
            }
            else if (o is float)
            {
                return (float)o > 0;
            }
            else if (o is bool)
            {
                return (bool)o;
            }
            else
            {
                return bool.Parse (o.ToString ());
            }
        }
    }

    public class DictionaryValueConverter : ValueConverter
    {
        public override object FromJson (object o)
        {
            if (o is string)
            {
                return Json.Deserialize (o as string);
            }
            else
            {
                throw new Exception (string.Format ("Fail to cast from {0} to Dictionary<string,object>", o.GetType ().Name));
            }
        }

        public override object ToJson (object o)
        {
            return Json.Serialize (o);
        }
    }

    public class ClassValueConverter : ValueConverter
    {
        public ClassDesc desc;
        
        public ClassValueConverter (ClassDesc desc)
        {
            this.desc = desc;
        }

        public override object FromJson (object o)
        {
            if (o == null)
            {
                return null;
            }
            else
            {
                var obj = JSONMapper.DefaultJsonMapper.ReadFromJSONObject (desc.ClassType, o);
 
                return obj [0];
            }
        }

        public override object FromDb (object o)
        {
            if (o == null)
            {
                return null;
            }
            else if (o is string)
            {
                var obj = JSONMapper.DefaultJsonMapper.Read (desc.ClassType, o as string);

                return obj [0];
            }
            else
            {
                throw new Exception (string.Format ("Fail to cast from {0} to {1}", o.GetType ().Name, desc.ClassType.Name));
            }
        }

        public override object ToJson (object o)
        {
            return JSONMapper.DefaultJsonMapper.ToJsonObject (o);
        }

        public override object ToDb (object o)
        {
            return JSONMapper.DefaultJsonMapper.Write (o);
        }
    }

    public class ArrayValueConverter : ValueConverter
    {
        public ClassDesc desc;
        
        public ArrayValueConverter (ClassDesc desc)
        {
            this.desc = desc;
        }

        public override object FromJson (object o)
        {
            if (o == null)
            {
                return null;
            }
            else
            {
                
                var obj = JSONMapper.DefaultJsonMapper.ReadFromJSONObject (desc.ClassType, o);
                
                return obj;
            }
        }

        public override object FromDb (object o)
        {
            if (o == null)
            {
                return null;
            }
            else if (o is string)
            {
                var obj = JSONMapper.DefaultJsonMapper.Read (desc.ClassType, o as string);
                
                return obj;
            }
            else
            {
                throw new Exception (string.Format ("Fail to cast from {0} to {1}", o.GetType ().Name, desc.ClassType.Name));
            }
        }

        public override object ToJson (object o)
        {
            if (o.GetType ().IsArray)
            {
                return JSONMapper.DefaultJsonMapper.ToJsonObject ((object[])o);
            }
            else
            {
                return JSONMapper.DefaultJsonMapper.ToJsonObject (o);
            }
        }

        public override object ToDb (object o)
        {
            return JSONMapper.DefaultJsonMapper.Write (o);
        }
    }
}
