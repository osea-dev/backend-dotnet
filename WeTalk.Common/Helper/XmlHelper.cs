using System;
using System.IO;
using System.Reflection;
using System.Xml;
using System.Xml.Serialization;

namespace WeTalk.Common.Helper
{
	public class XmlHelper
	{
		/// <summary>
		/// 转换对象为JSON格式数据
		/// </summary>
		/// <typeparam name="T">类</typeparam>
		/// <param name="obj">对象</param>
		/// <returns>字符格式的JSON数据</returns>
		public static string GetXML<T>(object obj)
		{
			try
			{
				XmlSerializer xs = new XmlSerializer(typeof(T));

				using (TextWriter tw = new StringWriter())
				{
					xs.Serialize(tw, obj);
					return tw.ToString();
				}
			}
			catch (Exception)
			{
				return string.Empty;
			}
		}

        /// <summary>
        /// Xml格式字符转换为T类型的对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="xml"></param>
        /// <returns></returns>
        public static T ParseFormByXml<T>(string strXML) where T : class
        {
            try
            {
                using (StringReader sr = new StringReader(strXML))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(T),new XmlRootAttribute("xml"));
                    return serializer.Deserialize(sr) as T;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        /// <summary>
        /// var xml = new XmlDocument();XmlConvertClass(xml,obj)
        /// </summary>
        /// <param name="parentNode"></param>
        /// <param name="obj"></param>
        public static void XmlConvertClass(XmlNode parentNode, object obj)
        {
			XmlNodeList nodeList = parentNode.ChildNodes;
			foreach (XmlNode item in nodeList) {
				PropertyInfo info = obj.GetType().GetProperty(item.Name);
				if (item.FirstChild != null && item.FirstChild.NodeType == XmlNodeType.Element)
				{
					XmlConvertClass(item, info.GetValue(obj));
				}
				else {
					info.SetValue(obj, Convert.ChangeType(item.InnerText, info.PropertyType));
				}
            }
        }
       
    }
}
