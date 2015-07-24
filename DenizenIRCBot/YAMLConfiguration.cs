﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using YamlDotNet.Serialization;

namespace DenizenIRCBot
{
    public class YAMLConfiguration
    {
        public YAMLConfiguration(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                Logger.Output(LogType.DEBUG, "Empty YAML config");
                Data = new Dictionary<string, dynamic>();
            }
            else
            {
                Deserializer des = new Deserializer();
                Data = des.Deserialize<Dictionary<string, dynamic>>(new StringReader(input));
                Logger.Output(LogType.DEBUG, "YAML Config with " + Data.Keys.Count + " root keys");
            }
        }

        public YAMLConfiguration(Dictionary<string, dynamic> datas)
        {
            Data = datas;
        }

        public Dictionary<string, dynamic> Data;

        public List<string> ReadList(string path)
        {
            try
            {
                string[] data = path.Split('.');
                int i = 0;
                dynamic obj = Data;
                while (i < data.Length - 1)
                {
                    dynamic nobj = obj.ContainsKey(data[i]) ? obj[data[i]] : null;
                    if (nobj == null || !(nobj is Dictionary<string, dynamic> || nobj is Dictionary<string, object> || nobj is Dictionary<object, object>))
                    {
                        return null;
                    }
                    obj = nobj;
                    i++;
                }
                if (!obj.ContainsKey(data[i]) || !(obj[data[i]] is List<string> || obj[data[i]] is List<object>))
                {
                    return null;
                }
                if (obj[data[i]] is List<object>)
                {
                    List<object> objs = (List<object>)obj[data[i]];
                    List<string> nstr = new List<string>();
                    for (int x = 0; x < objs.Count; x++)
                    {
                        nstr.Add(objs[x] + "");
                    }
                    return nstr;
                }
                return (List<string>)obj[data[i]];
            }
            catch (Exception ex)
            {
                Logger.Output(LogType.DEBUG, "Caught exception while reading YAML: " + ex.ToString());
            }
            return null;
        }

        public string Read(string path, string def)
        {
            try
            {
                string[] data = path.Split('.');
                int i = 0;
                dynamic obj = Data;
                while (i < data.Length - 1)
                {
                    dynamic nobj = obj.ContainsKey(data[i]) ? obj[data[i]] : null;
                    if (nobj == null || !(nobj is Dictionary<string, dynamic> || nobj is Dictionary<string, object> || nobj is Dictionary<object, object>))
                    {
                        return def;
                    }
                    obj = nobj;
                    i++;
                }
                if (!obj.ContainsKey(data[i]))
                {
                    return def;
                }
                return obj[data[i]].ToString();
            }
            catch (Exception ex)
            {
                Logger.Output(LogType.DEBUG, "Caught exception while reading YAML: " + ex.ToString());
            }
            return def;
        }

        public bool HasKey(string path, string key)
        {
            return GetKeys(path).Contains(key);
        }

        public List<string> GetKeys(string path)
        {
            try
            {
                if (string.IsNullOrEmpty(path))
                {
                    return new List<string>(Data.Keys);
                }
                string[] data = path.Split('.');
                int i = 0;
                dynamic obj = Data;
                while (i < data.Length - 1)
                {
                    dynamic nobj = obj.ContainsKey(data[i]) ? obj[data[i]] : null;
                    if (nobj == null || !(nobj is Dictionary<string, dynamic> || nobj is Dictionary<string, object> || nobj is Dictionary<object, object>))
                    {
                        return new List<string>();
                    }
                    obj = nobj;
                    i++;
                }
                if (!obj.ContainsKey(data[i]))
                {
                    return new List<string>();
                }
                dynamic tobj = obj[data[i]];
                if (tobj is Dictionary<object, object>)
                {
                    List<object> objs = tobj.Keys;
                    List<string> toret = new List<string>();
                    for (int x = 0; x < objs.Count; x++)
                    {
                        toret.Add(objs[i] + "");
                    }
                    return toret;
                }
                if (!(tobj is Dictionary<string, dynamic> || tobj is Dictionary<string, object>))
                {
                    return new List<string>();
                }
                return new List<string>(tobj.Keys);
            }
            catch (Exception ex)
            {
                Logger.Output(LogType.DEBUG, "Caught exception while reading YAML: " + ex.ToString());
            }
            return new List<string>();
        }

        public YAMLConfiguration GetConfigurationSection(string path)
        {
            try
            {
                if (string.IsNullOrEmpty(path))
                {
                    return new YAMLConfiguration(Data);
                }
                string[] data = path.Split('.');
                int i = 0;
                dynamic obj = Data;
                while (i < data.Length - 1)
                {
                    dynamic nobj = obj.ContainsKey(data[i]) ? obj[data[i]] : null;
                    if (nobj == null || !(nobj is Dictionary<string, dynamic> || nobj is Dictionary<string, object> || nobj is Dictionary<object, object>))
                    {
                        return null;
                    }
                    obj = nobj;
                    i++;
                }
                if (!obj.ContainsKey(data[i]))
                {
                    return null;
                }
                dynamic tobj = obj[data[i]];
                if (tobj is Dictionary<object, object>)
                {
                    Dictionary<object, object> dict = (Dictionary<object, object>)tobj;
                    Dictionary<string, object> ndict = new Dictionary<string, object>();
                    foreach (object fobj in dict.Keys)
                    {
                        ndict.Add(fobj + "", dict[fobj]);
                    }
                    return new YAMLConfiguration(ndict);
                }
                if (!(tobj is Dictionary<string, dynamic> || tobj is Dictionary<string, object>))
                {
                    return null;
                }
                return new YAMLConfiguration(tobj);
            }
            catch (Exception ex)
            {
                Logger.Output(LogType.DEBUG, "Caught exception while reading YAML: " + ex.ToString());
            }
            return null;
        }

        public void Set(string path, object val)
        {
            string[] data = path.Split('.');
            int i = 0;
            dynamic obj = Data;
            while (i < data.Length - 1)
            {
                dynamic nobj = obj.ContainsKey(data[i]) ? obj[data[i]] : null;
                if (nobj == null || !(nobj is Dictionary<string, dynamic> || nobj is Dictionary<string, object> || nobj is Dictionary<object, object>))
                {
                    nobj = new Dictionary<dynamic, dynamic>();
                    obj[data[i]] = nobj;
                }
                obj = nobj;
                i++;
            }
            if (val == null)
            {
                obj.Remove(data[i]);
            }
            else
            {
                obj[data[i]] = val;
            }
        }

        public string SaveToString()
        {
            Serializer ser = new Serializer();
            StringWriter sw = new StringWriter();
            ser.Serialize(sw, Data);
            return sw.ToString();
        }
    }
}
