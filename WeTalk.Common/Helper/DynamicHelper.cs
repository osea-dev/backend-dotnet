using System;
using System.Linq;
using System.Reflection;

namespace WeTalk.Common.Helper
{
	/// <summary>
	/// 动态编译类
	/// </summary>
	public class DynamicHelper
	{
		/// <summary>
		/// 动态赋值
		/// </summary>
		/// <param name="obj">对象</param>
		/// <param name="fieldName">字段名</param>
		/// <param name="value">字段值</param>
		public static void SetValue(object obj, string fieldName, object value, BindingFlags attr = BindingFlags.Public | BindingFlags.IgnoreCase | BindingFlags.Instance)
		{
			PropertyInfo info = null;
			object obj_f = obj;//字段上级对象
			if (fieldName.Contains("."))
			{
				//取出最终子节点的值 
				string[] arr = fieldName.Split('.');
				if (arr.Length > 0)
				{
					info = obj.GetType().GetProperty(arr[0], attr);
					for (int i = 1; i < arr.Length; i++)
					{
						if (string.IsNullOrEmpty(arr[i])) continue;
						obj_f = info.GetValue(obj_f);
						info = info.PropertyType.GetProperty(arr[i], attr);
					}
				}
			}
			else
			{
				info = obj.GetType().GetProperty(fieldName, attr);
			}
			if (info != null)
			{
				info.SetValue(obj_f, value);
			}

		}
		/// <summary>
		/// 泛型动态赋值
		/// </summary>
		/// <typeparam name="T">值的类型</typeparam>
		/// <param name="obj">对象</param>
		/// <param name="fieldName">字段名</param>
		/// <param name="value">字段值</param>
		public static void SetValue<T>(object obj, string fieldName, T value, BindingFlags attr = BindingFlags.Public | BindingFlags.IgnoreCase | BindingFlags.Instance)
		{
			PropertyInfo info = null;
			object obj_f = obj;//字段上级对象
			if (fieldName.Contains("."))
			{
				//取出最终子节点的值 
				string[] arr = fieldName.Split('.');
				if (arr.Length > 0)
				{
					info = obj.GetType().GetProperty(arr[0], attr);
					for (int i = 1; i < arr.Length; i++)
					{
						if (string.IsNullOrEmpty(arr[i])) continue;
						obj_f = info.GetValue(obj_f);
						info = info.PropertyType.GetProperty(arr[i], attr);
					}
				}
			}
			else
			{
				info = obj.GetType().GetProperty(fieldName, attr);
			}
			if (info != null)
			{
				Type outTpye = info.PropertyType;
				var outVal = Convert.ChangeType(value, info.PropertyType);
				info.SetValue(obj_f, outVal);
			}
		}

		/// <summary>
		/// 动态取值
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="fieldName"></param>
		/// <returns></returns>
		public static object GetValue(object obj, string fieldName, BindingFlags attr = BindingFlags.Public | BindingFlags.IgnoreCase | BindingFlags.Instance)
		{
			PropertyInfo info;
			object obj_val = obj;
			string count = "";
			string[] arr = fieldName.Split('.');
			for (int i = 0; i < arr.Length; i++)
			{
				if (string.IsNullOrEmpty(arr[i])) continue;
				string key = arr[i];

				if (count != "")
				{
					//GetCustomAttributes：返回在该成员上定义、由类型标识的自定义属性数组，如果没有该类型的自定义属性，则返回空数组。返回object[]
					String indexerName = ((DefaultMemberAttribute)obj_val.GetType()
					 .GetCustomAttributes(typeof(DefaultMemberAttribute), true)[0]).MemberName;
					PropertyInfo pi2 = obj_val.GetType().GetProperty(indexerName);
					obj_val = pi2.GetValue(obj_val, new Object[] { int.Parse(count) });
					count = "";

					//上次循环取出数组，这次要取数组的具体属性
					info = obj_val.GetType().GetProperty(key, attr);
					obj_val = info.GetValue(obj_val);
				}
				else
				{
					if (key.Contains("[") && key.Contains("]"))
					{
						info = obj_val.GetType().GetProperty(key.Split('[').FirstOrDefault(), attr);
					}
					else
					{
						info = obj_val.GetType().GetProperty(key, attr);
					}
					obj_val = info.GetValue(obj_val);
				}
				if (key.Contains("[") && key.Contains("]"))
				{
					count = key.Split('[').LastOrDefault().Split(']').FirstOrDefault();
				}
			}

			return obj_val;
		}
		/// <summary>
		/// 动态取值泛型
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="obj"></param>
		/// <param name="fieldName"></param>
		/// <returns></returns>
		public static T GetValue<T>(object obj, string fieldName, BindingFlags attr = BindingFlags.Public | BindingFlags.IgnoreCase | BindingFlags.Instance)
		{
			//fieldName = fieldName.Split('.').LastOrDefault().Replace("_", "");
			//PropertyInfo info = obj.GetType().GetProperty(fieldName, attr);
			//return (T)info.GetValue(obj);

			PropertyInfo info;
			object obj_val = obj;
			string[] arr = fieldName.Split('.');
			for (int i = 0; i < arr.Length; i++)
			{
				if (string.IsNullOrEmpty(arr[i])) continue;
				info = obj_val.GetType().GetProperty(arr[i], attr);
				obj_val = info.GetValue(obj_val);
			}
			return (T)obj_val;
		}



		#region 创建对象实例

		/// <summary>
		/// 创建对象实例
		/// </summary>
		/// <typeparam name="T">对象类型</typeparam>
		/// <param name="assemblyName">程序集名称</param>
		/// <param name="nameSpace">命名空间</param>
		/// <param name="className">类名</param>
		/// <returns></returns>
		public static T CreateInstance<T>(string assemblyName, string nameSpace, string className)
		{
			try
			{
				//命名空间.类名,程序集
				string path = nameSpace + "." + className + "," + assemblyName;
				//加载类型
				Type type = Type.GetType(path);
				//根据类型创建实例
				object obj = Activator.CreateInstance(type, true);
				//类型转换并返回
				return (T)obj;
			}
			catch
			{
				//发生异常时，返回类型的默认值。
				return default(T);
			}
		}

		#endregion

		#region 调用方法实例

		/// <summary>
		/// 调用方法实例
		/// </summary>
		/// <typeparam name="T">对象类型</typeparam>
		/// <param name="assemblyName">程序集名称</param>
		/// <param name="nameSpace">命名空间</param>
		/// <param name="className">类名</param>
		/// <returns></returns>
		public static T GetInvokeMethod<T>(string assemblyName, string nameSpace, string className, string methodName, object[] paras)
		{
			try
			{
				//命名空间.类名,程序集
				string path = nameSpace + "." + className + "," + assemblyName;
				//加载类型
				Type type = Type.GetType(path);
				//根据类型创建实例
				object obj = Activator.CreateInstance(type, true);
				//加载方法参数类型及方法
				MethodInfo method = null;
				if (paras != null && paras.Length > 0)
				{
					//加载方法参数类型
					Type[] paratypes = new Type[paras.Length];
					for (int i = 0; i < paras.Length; i++)
					{
						paratypes[i] = paras[i].GetType();
					}
					//加载有参方法
					method = type.GetMethod(methodName, paratypes);
				}
				else
				{
					//加载无参方法
					method = type.GetMethod(methodName);
				}
				//类型转换并返回
				return (T)method.Invoke(obj, paras);
			}
			catch
			{
				//发生异常时，返回类型的默认值。
				return default(T);
			}
		}
		#endregion
	}
}