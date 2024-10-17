using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace WeTalk.Common.Helper
{
	public class JsonHelper
	{


		#region "Json特符字符过滤"
		/// <summary>
		/// Json特符字符过滤，参见http://www.json.org/
		/// </summary>
		/// <param name="sourceStr">要过滤的源字符串</param>
		/// <returns>返回过滤的字符串</returns>
		public static string JsonCharFilter(string sourceStr)
		{
			if (string.IsNullOrEmpty(sourceStr)) return "";
			sourceStr = sourceStr.Replace("\\", "\\\\");
			sourceStr = sourceStr.Replace("\b", "\\b");
			sourceStr = sourceStr.Replace("\t", "\\t");
			sourceStr = sourceStr.Replace("\n", "\\n");
			sourceStr = sourceStr.Replace("\n", "\\n");
			sourceStr = sourceStr.Replace("\f", "\\f");
			sourceStr = sourceStr.Replace("\r", "\\r");
			return sourceStr.Replace("\"", "\\\"");
		}
		#endregion

		/// <summary>
		/// 对象序列化
		/// </summary>
		/// <param name="obj">对象</param>
		/// <param name="isUseTextJson">是否使用textjson</param>
		/// <returns>返回json字符串</returns>
		public static string ObjToJson(object obj, bool isUseTextJson = false)
		{
			if (isUseTextJson)
			{
				return System.Text.Json.JsonSerializer.Serialize(obj);
			}
			else
			{
				return Newtonsoft.Json.JsonConvert.SerializeObject(obj);
			}
		}
		/// <summary>
		/// json反序列化obj
		/// </summary>
		/// <typeparam name="T">反序列类型</typeparam>
		/// <param name="strJson">json</param>
		/// <param name="isUseTextJson">是否使用textjson</param>
		/// <returns>返回对象</returns>
		public static T JsonToObj<T>(string strJson, bool isUseTextJson = false)
		{
			if (isUseTextJson)
			{
				return System.Text.Json.JsonSerializer.Deserialize<T>(strJson);
			}
			else
			{
				return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(strJson);
			}
		}
		/// <summary>
		/// 转换对象为JSON格式数据
		/// </summary>
		/// <typeparam name="T">类</typeparam>
		/// <param name="obj">对象</param>
		/// <returns>字符格式的JSON数据</returns>
		public static string GetJSON<T>(object obj)
		{
			string result = String.Empty;
			try
			{
				System.Runtime.Serialization.Json.DataContractJsonSerializer serializer =
				new System.Runtime.Serialization.Json.DataContractJsonSerializer(typeof(T));
				using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
				{
					serializer.WriteObject(ms, obj);
					result = System.Text.Encoding.UTF8.GetString(ms.ToArray());
				}
			}
			catch (Exception)
			{
				throw;
			}
			return result;
		}
		/// <summary>
		/// 转换List<T>的数据为JSON格式
		/// </summary>
		/// <typeparam name="T">类</typeparam>
		/// <param name="vals">列表值</param>
		/// <returns>JSON格式数据</returns>
		public string JSON<T>(List<T> vals)
		{
			System.Text.StringBuilder st = new System.Text.StringBuilder();
			try
			{
				System.Runtime.Serialization.Json.DataContractJsonSerializer s = new System.Runtime.Serialization.Json.DataContractJsonSerializer(typeof(T));

				foreach (T city in vals)
				{
					using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
					{
						s.WriteObject(ms, city);
						st.Append(System.Text.Encoding.UTF8.GetString(ms.ToArray()));
					}
				}
			}
			catch (Exception)
			{
			}
			return st.ToString();
		}
		/// <summary>
		/// JSON格式字符转换为T类型的对象
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="jsonStr"></param>
		/// <returns></returns>
		public static T ParseFormByJson<T>(string jsonStr)
		{
			T obj = Activator.CreateInstance<T>();
			using (System.IO.MemoryStream ms =
			new System.IO.MemoryStream(System.Text.Encoding.UTF8.GetBytes(jsonStr)))
			{
				System.Runtime.Serialization.Json.DataContractJsonSerializer serializer =
				new System.Runtime.Serialization.Json.DataContractJsonSerializer(typeof(T));
				return (T)serializer.ReadObject(ms);
			}
		}

		public string JSON1<SendData>(List<SendData> vals)
		{
			System.Text.StringBuilder st = new System.Text.StringBuilder();
			try
			{
				System.Runtime.Serialization.Json.DataContractJsonSerializer s = new System.Runtime.Serialization.Json.DataContractJsonSerializer(typeof(SendData));

				foreach (SendData city in vals)
				{
					using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
					{
						s.WriteObject(ms, city);
						st.Append(System.Text.Encoding.UTF8.GetString(ms.ToArray()));
					}
				}
			}
			catch (Exception)
			{
			}

			return st.ToString();
		}

		private static bool IsJsonStart(ref string json)
		{
			if (!string.IsNullOrEmpty(json))
			{
				json = json.Trim('\r', '\n', ' ');
				if (json.Length > 1)
				{
					char s = json[0];
					char e = json[json.Length - 1];
					return (s == '{' && e == '}') || (s == '[' && e == ']');
				}
			}
			return false;
		}
		public static bool IsJson(string json)
		{
			int errIndex;
			return IsJson(json, out errIndex);
		}
		public static bool IsJson(string json, out int errIndex)
		{
			errIndex = 0;
			if (IsJsonStart(ref json))
			{
				CharState cs = new CharState();
				char c;
				for (int i = 0; i < json.Length; i++)
				{
					c = json[i];
					if (SetCharState(c, ref cs) && cs.childrenStart)//设置关键符号状态。
					{
						string item = json.Substring(i);
						int err;
						int length = GetValueLength(item, true, out err);
						cs.childrenStart = false;
						if (err > 0)
						{
							errIndex = i + err;
							return false;
						}
						i = i + length - 1;
					}
					if (cs.isError)
					{
						errIndex = i;
						return false;
					}
				}

				return !cs.arrayStart && !cs.jsonStart;
			}
			return false;
		}

		/// <summary>
		/// 获取值的长度（当Json值嵌套以"{"或"["开头时）
		/// </summary>
		private static int GetValueLength(string json, bool breakOnErr, out int errIndex)
		{
			errIndex = 0;
			int len = 0;
			if (!string.IsNullOrEmpty(json))
			{
				CharState cs = new CharState();
				char c;
				for (int i = 0; i < json.Length; i++)
				{
					c = json[i];
					if (!SetCharState(c, ref cs))//设置关键符号状态。
					{
						if (!cs.jsonStart && !cs.arrayStart)//json结束，又不是数组，则退出。
						{
							break;
						}
					}
					else if (cs.childrenStart)//正常字符，值状态下。
					{
						int length = GetValueLength(json.Substring(i), breakOnErr, out errIndex);//递归子值，返回一个长度。。。
						cs.childrenStart = false;
						cs.valueStart = 0;
						//cs.state = 0;
						i = i + length - 1;
					}
					if (breakOnErr && cs.isError)
					{
						errIndex = i;
						return i;
					}
					if (!cs.jsonStart && !cs.arrayStart)//记录当前结束位置。
					{
						len = i + 1;//长度比索引+1
						break;
					}
				}
			}
			return len;
		}

		/// <summary>
		/// 设置字符状态(返回true则为关键词，返回false则当为普通字符处理）
		/// </summary>
		private static bool SetCharState(char c, ref CharState cs)
		{
			cs.CheckIsError(c);
			switch (c)
			{
				case '{'://[{ "[{A}]":[{"[{B}]":3,"m":"C"}]}]
					#region 大括号
					if (cs.keyStart <= 0 && cs.valueStart <= 0)
					{
						cs.keyStart = 0;
						cs.valueStart = 0;
						if (cs.jsonStart && cs.state == 1)
						{
							cs.childrenStart = true;
						}
						else
						{
							cs.state = 0;
						}
						cs.jsonStart = true;//开始。
						return true;
					}
					#endregion
					break;
				case '}':
					#region 大括号结束
					if (cs.keyStart <= 0 && cs.valueStart < 2 && cs.jsonStart)
					{
						cs.jsonStart = false;//正常结束。
						cs.state = 0;
						cs.keyStart = 0;
						cs.valueStart = 0;
						cs.setDicValue = true;
						return true;
					}
					// cs.isError = !cs.jsonStart && cs.state == 0;
					#endregion
					break;
				case '[':
					#region 中括号开始
					if (!cs.jsonStart)
					{
						cs.arrayStart = true;
						return true;
					}
					else if (cs.jsonStart && cs.state == 1)
					{
						cs.childrenStart = true;
						return true;
					}
					#endregion
					break;
				case ']':
					#region 中括号结束
					if (cs.arrayStart && !cs.jsonStart && cs.keyStart <= 2 && cs.valueStart <= 0)//[{},333]//这样结束。
					{
						cs.keyStart = 0;
						cs.valueStart = 0;
						cs.arrayStart = false;
						return true;
					}
					#endregion
					break;
				case '"':
				case '\'':
					#region 引号
					if (cs.jsonStart || cs.arrayStart)
					{
						if (cs.state == 0)//key阶段,有可能是数组["aa",{}]
						{
							if (cs.keyStart <= 0)
							{
								cs.keyStart = (c == '"' ? 3 : 2);
								return true;
							}
							else if ((cs.keyStart == 2 && c == '\'') || (cs.keyStart == 3 && c == '"'))
							{
								if (!cs.escapeChar)
								{
									cs.keyStart = -1;
									return true;
								}
								else
								{
									cs.escapeChar = false;
								}
							}
						}
						else if (cs.state == 1 && cs.jsonStart)//值阶段必须是Json开始了。
						{
							if (cs.valueStart <= 0)
							{
								cs.valueStart = (c == '"' ? 3 : 2);
								return true;
							}
							else if ((cs.valueStart == 2 && c == '\'') || (cs.valueStart == 3 && c == '"'))
							{
								if (!cs.escapeChar)
								{
									cs.valueStart = -1;
									return true;
								}
								else
								{
									cs.escapeChar = false;
								}
							}

						}
					}
					#endregion
					break;
				case ':':
					#region 冒号
					if (cs.jsonStart && cs.keyStart < 2 && cs.valueStart < 2 && cs.state == 0)
					{
						if (cs.keyStart == 1)
						{
							cs.keyStart = -1;
						}
						cs.state = 1;
						return true;
					}
					// cs.isError = !cs.jsonStart || (cs.keyStart < 2 && cs.valueStart < 2 && cs.state == 1);
					#endregion
					break;
				case ',':
					#region 逗号 //["aa",{aa:12,}]

					if (cs.jsonStart)
					{
						if (cs.keyStart < 2 && cs.valueStart < 2 && cs.state == 1)
						{
							cs.state = 0;
							cs.keyStart = 0;
							cs.valueStart = 0;
							//if (cs.valueStart == 1)
							//{
							//    cs.valueStart = 0;
							//}
							cs.setDicValue = true;
							return true;
						}
					}
					else if (cs.arrayStart && cs.keyStart <= 2)
					{
						cs.keyStart = 0;
						//if (cs.keyStart == 1)
						//{
						//    cs.keyStart = -1;
						//}
						return true;
					}
					#endregion
					break;
				case ' ':
				case '\r':
				case '\n'://[ "a",\r\n{} ]
				case '\0':
				case '\t':
					if (cs.keyStart <= 0 && cs.valueStart <= 0) //cs.jsonStart && 
					{
						return true;//跳过空格。
					}
					break;
				default: //值开头。。
					if (c == '\\') //转义符号
					{
						if (cs.escapeChar)
						{
							cs.escapeChar = false;
						}
						else
						{
							cs.escapeChar = true;
							return true;
						}
					}
					else
					{
						cs.escapeChar = false;
					}
					if (cs.jsonStart || cs.arrayStart) // Json 或数组开始了。
					{
						if (cs.keyStart <= 0 && cs.state == 0)
						{
							cs.keyStart = 1;//无引号的
						}
						else if (cs.valueStart <= 0 && cs.state == 1 && cs.jsonStart)//只有Json开始才有值。
						{
							cs.valueStart = 1;//无引号的
						}
					}
					break;
			}
			return false;
		}


		#region 获取Json字符串某节点的值(字串处理)
		/// <summary>
		/// 获取Json字符串某节点的值
		/// </summary>
		public static string GetJsonValue(string jsonStr, string key)
		{
			string result = string.Empty;
			if (!string.IsNullOrEmpty(jsonStr))
			{
				key = "\"" + key.Trim('"') + "\"";
				int index = jsonStr.IndexOf(key) + key.Length + 1;
				if (index > key.Length + 1)
				{
					//先截逗号，若是最后一个，截“｝”号，取最小值
					int end = jsonStr.IndexOf(',', index);
					if (end == -1)
					{
						end = jsonStr.IndexOf('}', index);
					}

					result = jsonStr.Substring(index, end - index);
					result = result.Trim(new char[] { '"', ' ', '\'' }); //过滤引号或空格
				}
			}
			return result;
		}
		#endregion

		#region 压缩/格式化JSON字串
		/// <summary>
		/// 格式化JSON字串
		/// </summary>
		/// <param name="str"></param>
		/// <returns></returns>
		public static string FormatJsonString(string str)
		{
			//格式化json字符串  
			try
			{
				JsonSerializer serializer = new JsonSerializer();
				TextReader tr = new StringReader(str);
				JsonTextReader jtr = new JsonTextReader(tr);
				object obj = serializer.Deserialize(jtr);
				if (obj != null)
				{
					StringWriter textWriter = new StringWriter();
					JsonTextWriter jsonWriter = new JsonTextWriter(textWriter)
					{
						Formatting = Newtonsoft.Json.Formatting.Indented,
						Indentation = 4,
						IndentChar = ' '
					};
					serializer.Serialize(jsonWriter, obj);
					return textWriter.ToString();
				}
				else
				{
					return str;
				}
			}
			catch
			{
				return str;
			}
		}
		/// <summary>
		/// 压缩JSON字串
		/// </summary>
		/// <param name="str"></param>
		/// <returns></returns>
		public static string CompressJsonString(string str)
		{
			try
			{
				var obj = JsonConvert.DeserializeObject(str);
				str = JsonConvert.SerializeObject(obj);
			}
			catch
			{

			}
			return str;
		}
		#endregion

		#region "操作JSON对象"
		/// <summary>
		/// 移除指定对象的属性，返回新对象
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="FieldNames"></param>
		/// <returns></returns>
		public static object RemoveFields(object obj, string FieldNames)
		{
			var arr = FieldNames.Split(',');
			foreach (var FieldName in arr)
			{
				obj = RemoveField(obj, FieldName);
			}
			return obj;
		}

		/// <summary>
		/// 移除指定的节点属性
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="FieldName"></param>
		/// <param name="count">如果是数组，此项填数组下标</param>
		/// <returns></returns>
		public static object RemoveField(object obj, string FieldName, string count = "")
		{
			string json = JsonConvert.SerializeObject(obj);
			//数组
			if (count != "")
			{
				JArray jarray = (JArray)JsonConvert.DeserializeObject(json);
				jarray[int.Parse(count)] = (JObject)RemoveField(jarray[int.Parse(count)], FieldName);
				obj = jarray;
			}
			else
			{
				JObject o = (JObject)JsonConvert.DeserializeObject(json);
				if (FieldName.Contains("."))
				{
					var keys = FieldName.Split('.');
					if (keys[0].Contains("["))
					{
						//data.content[0].data.resourceData[0].id
						string key = keys[0].Split('[').FirstOrDefault();
						count = keys[0].Split(']').FirstOrDefault().Split('[').LastOrDefault();
						if (o[key] != null)//o["content"]
						{
							o[key] = (JArray)RemoveField(o[key], FieldName.Remove(0, FieldName.IndexOf(".") + 1), count);
						}
						count = "";
					}
					else
					{
						if (o[keys[0]] != null) o[keys[0]] = (JObject)RemoveField(o[keys[0]], FieldName.Remove(0, FieldName.IndexOf(".") + 1));
					}
				}
				else
				{
					o.Remove(FieldName);
				}
				obj = o;
			}
			return obj;
		}

		/// <summary>
		/// 通过JSON对象取节点属性值 
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="o"></param>
		/// <param name="FieldName"></param>
		/// <returns></returns>
		public static T GetField<T>(JObject o, string FieldName)
		{
			object obj = o.SelectToken(FieldName);
			if (obj != null)
			{
				return (T)obj;
			}
			else
			{
				return default(T);
			}
		}

		/// <summary>
		/// 过滤取出实体对象中的部份属性值
		/// </summary>
		/// <param name="obj">实体对象</param>
		/// <param name="FieldNames">多个字段属性</param>
		/// <returns></returns>
		public static object FilterFields(object obj, string FieldNames)
		{
			JObject o = new JObject();

			string[] arr = FieldNames.Split(',');
			for (int i = 0; i < arr.Length; i++)
			{
				if (string.IsNullOrEmpty(arr[i])) continue;
				var val = DynamicHelper.GetValue(obj, arr[i]);
				CreateFields(o, arr[i], val);
			}
			return o;
		}

		/// <summary>
		/// 在指定的json对象中添改相应属性值
		/// </summary>
		/// <param name="o">json对象</param>
		/// <param name="FieldNames">属性KEY</param>
		/// <param name="val">属性值</param>
		/// <returns></returns>
		public static JToken CreateFields(JObject o, string FieldNames, object val, string count = "")
		{
			string[] arr = FieldNames.Split(',');
			//JObject o = all;
			if (o == null) o = new JObject();

			//数组
			if (!string.IsNullOrEmpty(count))
			{
				o.Add(new JProperty("_count", count));
			}
			if (FieldNames.Contains("."))
			{
				string[] attrs = FieldNames.Split('.');
				if (attrs.Length > 0)
				{
					//节点不存在则添加新的
					if (attrs[0].Contains("["))
					{
						string key = attrs[0].Split('[').FirstOrDefault();
						count = attrs[0].Split(']').FirstOrDefault().Split('[').LastOrDefault();
						if (o[key] == null)
						{
							//data.content[0].data.resourceData[3].id
							o.Add(new JProperty(key, new object[] { CreateFields((JObject)o[key], FieldNames.Remove(0, FieldNames.IndexOf(".") + 1), val, count) }));
						}
						else
						{
							var tmp_count = ((JArray)o[key]).FirstOrDefault(u => u["_count"].ToString() == count);
							if (tmp_count != null)
							{
								//数组已经存在了
								CreateFields((JObject)tmp_count, FieldNames.Remove(0, FieldNames.IndexOf(".") + 1), val);
							}
							else
							{
								//数组不存在
								//o.Add(new JProperty(key, new object[] { CreateField((JObject)o[key], FieldNames.Remove(0, FieldNames.IndexOf(".") + 1), val, count) }));
								((JArray)o[key]).Add(CreateFields(null, FieldNames.Remove(0, FieldNames.IndexOf(".") + 1), val, count));
							}
						}
						count = "";
					}
					else
					{
						if (o[attrs[0]] == null)
						{
							o.Add(attrs[0], CreateFields((JObject)o[attrs[0]], FieldNames.Remove(0, FieldNames.IndexOf(".") + 1), val));
						}
						else
						{
							CreateFields((JObject)o[attrs[0]], FieldNames.Remove(0, FieldNames.IndexOf(".") + 1), val);
						}
					}
				}
			}
			else
			{
				o.Add(new JProperty(FieldNames, val));
			}
			return (JToken)o;
		}
		#endregion
	}
	/// <summary>
	/// 字符状态
	/// </summary>
	public class CharState
	{
		internal bool jsonStart = false;//以 "{"开始了...
		internal bool setDicValue = false;// 可以设置字典值了。
		internal bool escapeChar = false;//以"\"转义符号开始了
		/// <summary>
		/// 数组开始【仅第一开头才算】，值嵌套的以【childrenStart】来标识。
		/// </summary>
		internal bool arrayStart = false;//以"[" 符号开始了
		internal bool childrenStart = false;//子级嵌套开始了。
		/// <summary>
		/// 【0 初始状态，或 遇到“,”逗号】；【1 遇到“：”冒号】
		/// </summary>
		internal int state = 0;

		/// <summary>
		/// 【-1 取值结束】【0 未开始】【1 无引号开始】【2 单引号开始】【3 双引号开始】
		/// </summary>
		internal int keyStart = 0;
		/// <summary>
		/// 【-1 取值结束】【0 未开始】【1 无引号开始】【2 单引号开始】【3 双引号开始】
		/// </summary>
		internal int valueStart = 0;
		internal bool isError = false;//是否语法错误。

		internal void CheckIsError(char c)//只当成一级处理（因为GetLength会递归到每一个子项处理）
		{
			if (keyStart > 1 || valueStart > 1)
			{
				return;
			}
			//示例 ["aa",{"bbbb":123,"fff","ddd"}] 
			switch (c)
			{
				case '{'://[{ "[{A}]":[{"[{B}]":3,"m":"C"}]}]
					isError = jsonStart && state == 0;//重复开始错误 同时不是值处理。
					break;
				case '}':
					isError = !jsonStart || (keyStart != 0 && state == 0);//重复结束错误 或者 提前结束{"aa"}。正常的有{}
					break;
				case '[':
					isError = arrayStart && state == 0;//重复开始错误
					break;
				case ']':
					isError = !arrayStart || jsonStart;//重复开始错误 或者 Json 未结束
					break;
				case '"':
				case '\'':
					isError = !(jsonStart || arrayStart); //json 或数组开始。
					if (!isError)
					{
						//重复开始 [""",{"" "}]
						isError = (state == 0 && keyStart == -1) || (state == 1 && valueStart == -1);
					}
					if (!isError && arrayStart && !jsonStart && c == '\'')//['aa',{}]
					{
						isError = true;
					}
					break;
				case ':':
					isError = !jsonStart || state == 1;//重复出现。
					break;
				case ',':
					isError = !(jsonStart || arrayStart); //json 或数组开始。
					if (!isError)
					{
						if (jsonStart)
						{
							isError = state == 0 || (state == 1 && valueStart > 1);//重复出现。
						}
						else if (arrayStart)//["aa,] [,]  [{},{}]
						{
							isError = keyStart == 0 && !setDicValue;
						}
					}
					break;
				case ' ':
				case '\r':
				case '\n'://[ "a",\r\n{} ]
				case '\0':
				case '\t':
					break;
				default: //值开头。。
					isError = (!jsonStart && !arrayStart) || (state == 0 && keyStart == -1) || (valueStart == -1 && state == 1);//
					break;
			}
			//if (isError)
			//{

			//}
		}
	}
}
