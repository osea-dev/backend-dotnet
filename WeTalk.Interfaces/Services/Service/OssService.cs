using Aliyun.OSS;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using WeTalk.Common.Helper;
using WeTalk.Interfaces.Base;

namespace WeTalk.Interfaces.Services
{
    public partial class OssService : BaseService, IOssService
    {
        private String httpMethod;
        private String httpURL;
        private String httpProtocolVersionString;
        private String httpBody;
        private Hashtable httpHeadersDict = new Hashtable();
        private String strAuthorizationRequestBase64 = "";
        private byte[] byteAuthorizationRequest;
        private String strPublicKeyURLBase64 = "";
        private String strPublicKeyBase64 = "";
        private String strPublicKeyContentBase64 = "";
        private String strPublicKeyContentXML = "";
        private String strAuthSourceForMD5 = "";
        private const int BUF_SIZE = 4096;
        //private Stream streamRequest;
        private StreamWriter swResponse;
        private StreamReader srPostData;
        private string bucketName = "";

        public int MAX_POST_SIZE { get; set; } = 10 * 1024 * 1024;  // 10MB 

        // 请填写您的AccessKeyId。
        public static string accessKeyId = "";
        // 请填写您的AccessKeySecret。
        public static string accessKeySecret = "";
        // host的格式为 bucketname.endpoint ，请替换为您的真实信息。
        public static string host = "";
        // callbackUrl为 上传回调服务器的URL，请将下面的IP和Port配置为您自己的真实信息。
        public static string callbackUrl = "";
        public static int expireTime = 30;

        private readonly ILogger<OssService> _logger;
        private readonly IHttpContextAccessor _accessor;
        private readonly IUserManage _userManage;
        private readonly ICommonService _commonService;
        public OssService(IPubConfigBaseService pubConfigBaseService, ILogger<OssService> logger, IHttpContextAccessor accessor, IUserManage userManage, ICommonService commonService)
        {
            _logger = logger;
            _accessor = accessor;
            _userManage = userManage;
            var dic_config = pubConfigBaseService.GetConfigs("alibaba_accesskey,alibaba_secret,alibaba_oss_bucket,alibaba_oss_endpoint,alibaba_oss_callback");
            accessKeyId = dic_config.ContainsKey("alibaba_accesskey") ? dic_config["alibaba_accesskey"] : "";
            accessKeySecret = dic_config.ContainsKey("alibaba_secret") ? dic_config["alibaba_secret"] : "";
            bucketName = dic_config.ContainsKey("alibaba_oss_bucket") ? dic_config["alibaba_oss_bucket"] : "";
            host = $"https://{(dic_config.ContainsKey("alibaba_oss_bucket") ? dic_config["alibaba_oss_bucket"] : "")}.{(dic_config.ContainsKey("alibaba_oss_endpoint") ? dic_config["alibaba_oss_endpoint"] : "")}.aliyuncs.com";
            callbackUrl = dic_config.ContainsKey("alibaba_oss_callback") ? dic_config["alibaba_oss_callback"] : "";
            _commonService = commonService;
        }

        /// <summary>
        /// 获取授权签名
        /// </summary>
        /// <param name="UploadDir">用户上传文件时指定的前缀</param>
        /// <returns></returns>
        public string GetPolicyToken(string UploadDir="")
        {
            var expireDateTime = DateTime.Now.AddSeconds(expireTime);
            //policy
            var config = new PolicyConfig();
            config.expiration = FormatIso8601Date(expireDateTime);
            config.conditions = new List<List<Object>>();
            config.conditions.Add(new List<Object>());
            config.conditions[0].Add("content-length-range");
            config.conditions[0].Add(0);
            config.conditions[0].Add(1048576000);
            config.conditions.Add(new List<Object>());
            config.conditions[1].Add("starts-with");
            config.conditions[1].Add("$key");
            config.conditions[1].Add(UploadDir);

            var policy = JsonConvert.SerializeObject(config);
            var policy_base64 = EncodeBase64("utf-8", policy);
            var signature = ComputeSignature(accessKeySecret, policy_base64);

            //callback
            var callback = new CallbackParam();
            callback.callbackUrl = callbackUrl;
            callback.callbackBody = "filename=${object}&size=${size}&mimeType=${mimeType}&height=${imageInfo.height}&width=${imageInfo.width}";
            callback.callbackBodyType = "application/x-www-form-urlencoded";

            //callback.callbackBody = "{\"filename\":${object},\"size\":${size},\"mimeType\":${mimeType}\"height\":${imageInfo.height},\"width\":${imageInfo.width}}";
            //callback.callbackBodyType = "application/json";

            var callback_string = JsonConvert.SerializeObject(callback);
            var callback_string_base64 = EncodeBase64("utf-8", callback_string);

            var policyToken = new PolicyToken();

            policyToken.accessid = accessKeyId;
            policyToken.host = host;
            policyToken.policy = policy_base64;
            policyToken.signature = signature;
            policyToken.expire = ToUnixTime(expireDateTime);
            policyToken.callback = callback_string_base64;
            policyToken.dir = UploadDir;

            return JsonConvert.SerializeObject(policyToken);
        }

        public string DoPost()
        {
            foreach (var head in _accessor.HttpContext.Request.Headers)
            {
                httpHeadersDict[head.Key] = head.Value;
                Console.WriteLine($"{head.Key}:{head.Value}");
            }
            var bodyDict = _accessor.HttpContext.Request.Form.ToDictionary(u=>u.Key,u=>u.Value);
            httpBody = String.Join("&", bodyDict.Select(u => UrlEncode(u.Key) + "=" + UrlEncode(u.Value)).ToList());
            //httpBody = "filename=test2cap5xZbAB.png&size=16724&mimeType=image%2Fpng&height=83&width=91";
            //Console.WriteLine("httpBody=" + httpBody);
                       
            this.httpURL = UrlHelper.UrlDecode(_accessor.HttpContext.Request.Path);
            
            string strResponseBody = "";
            // Verify Signature
            try
            {
                int width = bodyDict.ContainsKey("width") ? (!string.IsNullOrEmpty(bodyDict["width"]) ? int.Parse(bodyDict["width"]) : 0) : 0;
                int height = bodyDict.ContainsKey("height") ? (!string.IsNullOrEmpty(bodyDict["height"]) ? int.Parse(bodyDict["height"]) : 0) : 0;
                var mimeType = bodyDict.ContainsKey("mimeType") ? bodyDict["mimeType"].ToString() : "";
                int size = bodyDict.ContainsKey("size") ? int.Parse(bodyDict["size"]) : 0;
                if (this.VerifySignature())
                {
                    //Console.WriteLine("\nVerifySignature Successful . \n");

                    // do something accoding to callback_body ... 
                    //filename=testhcfGzx4YET.png&size=9690&mimeType=image/png&height=108&width=108
                    strResponseBody = "{\"Code\":0,\"Status\":\"OK\",\"file\":\""+ bodyDict["filename"] + "\",\"size\":"+ size + ",\"mimeType\":\"" + mimeType + "\",\"width\":" + width + ",\"height\":" + height + "}";
                    //this.HttpResponseSuccess();
                }
                else
                {
                    //Console.WriteLine("\nVerifySignature Failed . \n");
                    //this.HttpResponseFailure();
                    strResponseBody = "{\"Code\":-1,\"Status\":\"OK\",\"file\":\"" + bodyDict["filename"] + "\",\"size\":" + size + ",\"mimeType\":\"" + mimeType + "\",\"width\":" + width + ",\"height\":" + height + "}";

                    //strResponseBody = "{\"Message\":\"VerifySignature Failed .\"}";
                }
            }
            catch(Exception ex)
            {
                //Console.WriteLine("\nVerifySignature Failed . \n");
                //this.HttpResponseFailure();
                strResponseBody = "{\"Message\":\"VerifySignature Failed .\",\"ErrorMsg\":\""+ ex.Message + "\"}";
            }
            return strResponseBody;
        }

        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="Img"></param>
        /// <returns></returns>
        public bool DelFile(string img)
        {
            // yourEndpoint填写Bucket所在地域对应的Endpoint。以华东1（杭州）为例，Endpoint填写为https://oss-cn-hangzhou.aliyuncs.com。
        
            // 阿里云账号AccessKey拥有所有API的访问权限，风险很高。强烈建议您创建并使用RAM用户进行API访问或日常运维，请登录RAM控制台创建RAM用户。
       
            // 填写Bucket名称，例如examplebucket。

            // 填写Object完整路径，完整路径中不能包含Bucket名称，例如exampledir/exampleobject.txt。
            var objectName = _commonService.ResourceClear(img);
            // 创建OSSClient实例。
            var client = new OssClient(host, accessKeyId, accessKeySecret);
            try
            {
                // 删除文件。
                if(client.DoesObjectExist(bucketName, objectName))
                    client.DeleteObject(bucketName, objectName);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Delete object failed. {0}", ex.Message);
                return false;
            }
        }
        //**************************************************************************************************************************************

        private string UrlEncode(string content, bool needUpper = true)
        {
            if (string.IsNullOrEmpty(content))
            {
                return string.Empty;
            }

            if (!needUpper)
            {
                return UrlHelper.UrlEncode(content);
            }

            var result = new StringBuilder();

            foreach (var per in content)
            {
                var temp = UrlHelper.UrlEncode(per.ToString());
                if (temp.Length > 1)
                {
                    result.Append(temp.ToUpper());
                    continue;
                }

                result.Append(per);
            }

            return result.ToString();
        }

        private string StreamReadLine(Stream inputStream)
        {
            if (inputStream == null) return "";
            int next_char;
            string data = "";
            while (true)
            {
                next_char = inputStream.ReadByte();
                if (next_char == '\n') { break; }
                if (next_char == '\r') { continue; }
                if (next_char == -1) { Thread.Sleep(1); continue; };
                data += Convert.ToChar(next_char);
            }
            return data;
        }
        private static string FormatIso8601Date(DateTime dtime)
        {
            return dtime.ToUniversalTime().ToString("yyyy-MM-dd'T'HH:mm:ss.fff'Z'",
                               CultureInfo.CurrentCulture);
        }

        private static string EncodeBase64(string code_type, string code)
        {
            string encode = "";
            byte[] bytes = Encoding.GetEncoding(code_type).GetBytes(code);
            try
            {
                encode = Convert.ToBase64String(bytes);
            }
            catch
            {
                encode = code;
            }
            return encode;
        }

        private static string ComputeSignature(string key, string data)
        {
            using (var algorithm = new HMACSHA1())
            {
                algorithm.Key = Encoding.UTF8.GetBytes(key.ToCharArray());
                return Convert.ToBase64String(
                    algorithm.ComputeHash(Encoding.UTF8.GetBytes(data.ToCharArray())));
            }
        }

        private static string ToUnixTime(DateTime dtime)
        {
            const long ticksOf1970 = 621355968000000000;
            var expires = ((dtime.ToUniversalTime().Ticks - ticksOf1970) / 10000000L)
                .ToString(CultureInfo.InvariantCulture);

            return expires;
        }

        private string RSAPublicKeyString2XML(string publicKey)
        {
            RsaKeyParameters publicKeyParam = (RsaKeyParameters)PublicKeyFactory.CreateKey(Convert.FromBase64String(publicKey));
            return string.Format("<RSAKeyValue><Modulus>{0}</Modulus><Exponent>{1}</Exponent></RSAKeyValue>",
                Convert.ToBase64String(publicKeyParam.Modulus.ToByteArrayUnsigned()),
                Convert.ToBase64String(publicKeyParam.Exponent.ToByteArrayUnsigned()));
        }
        private static byte[] byteMD5Encrypt32(string password)
        {
            string cl = password;
            MD5 md5 = MD5.Create();
            byte[] s = md5.ComputeHash(Encoding.UTF8.GetBytes(cl));
            return s;
        }
        private static bool validateServerCertificate(object sender, System.Security.Cryptography.X509Certificates.X509Certificate certificate, X509Chain chain, System.Net.Security.SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }

        private void HttpResponseSuccess()
        {
            this.swResponse.WriteLine("HTTP/1.0 200 OK");
            this.swResponse.WriteLine("Content-Type: application/json"); //Not "Content-Type: text/html");
            this.swResponse.WriteLine("Content-Length: 15");
            this.swResponse.WriteLine("Connection: close");
            this.swResponse.WriteLine("");
            string strResponseBody = "{\"Status\":\"OK\"}";
            this.swResponse.WriteLine(strResponseBody);
        }

        private void HttpResponseFailure()
        {
            this.swResponse.WriteLine("HTTP/1.0 404 File not found");
            this.swResponse.WriteLine("Connection: close");
            this.swResponse.WriteLine("");
        }
        private bool VerifySignature()
        {
            // Get the Authorization Base64 from Request
            if (this.httpHeadersDict["authorization"] != null)
            {
                this.strAuthorizationRequestBase64 = this.httpHeadersDict["authorization"].ToString();
            }
            else if (this.httpHeadersDict["Authorization"] != null)
            {
                this.strAuthorizationRequestBase64 = this.httpHeadersDict["Authorization"].ToString();
            }
            if (this.strAuthorizationRequestBase64 == "")
            {
                _logger.LogInformation("authorization property in the http request header is null. ");
                return false;
            }
            Console.WriteLine("strAuthorizationRequestBase64:" + strAuthorizationRequestBase64);
            // Decode the Authorization from Request
            this.byteAuthorizationRequest = Convert.FromBase64String(this.strAuthorizationRequestBase64);

            // Decode the URL of PublicKey
            this.strPublicKeyURLBase64 = this.httpHeadersDict["x-oss-pub-key-url"].ToString();
            var bytePublicKeyURL = Convert.FromBase64String(this.strPublicKeyURLBase64);
            var strAsciiPublickeyURL = Encoding.ASCII.GetString(bytePublicKeyURL);

            // Get PublicKey from the URL
            ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(validateServerCertificate);
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(strAsciiPublickeyURL);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            StreamReader srPublicKey = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
            this.strPublicKeyBase64 = srPublicKey.ReadToEnd();
            response.Close();
            srPublicKey.Close();
            this.strPublicKeyContentBase64 = this.strPublicKeyBase64.Replace("-----BEGIN PUBLIC KEY-----\n", "").Replace("-----END PUBLIC KEY-----", "").Replace("\n", "");
            this.strPublicKeyContentXML = this.RSAPublicKeyString2XML(this.strPublicKeyContentBase64);
            Console.WriteLine("strPublicKeyContentBase64:" + this.strPublicKeyContentBase64);
            // Generate the New Authorization String according to the HttpRequest
            String[] arrURL;
            if (this.httpURL.Contains('?'))
            {
                arrURL = this.httpURL.Split('?');
                this.strAuthSourceForMD5 = String.Format("{0}?{1}\n{2}", System.Web.HttpUtility.UrlDecode(arrURL[0]), arrURL[1], this.httpBody);
            }
            else
            {
                this.strAuthSourceForMD5 = String.Format("{0}\n{1}", System.Web.HttpUtility.UrlDecode(this.httpURL), this.httpBody);
            }

            // MD5 hash bytes from the New Authorization String 
            var byteAuthMD5 = byteMD5Encrypt32(this.strAuthSourceForMD5);

            // Verify Signature
            RSACryptoServiceProvider RSA = new RSACryptoServiceProvider();
            try
            {
                _logger.LogInformation("byteAuthMD5:" + byteAuthMD5);
                RSA.FromXmlString(this.strPublicKeyContentXML);
            }
            catch (ArgumentNullException e)
            {
                throw new ArgumentNullException(String.Format("VerifySignature Failed : RSADeformatter.VerifySignature get null argument : {0} .", e));
            }
            catch (CryptographicException e)
            {
                throw new CryptographicException(String.Format("VerifySignature Failed : RSA.FromXmlString Exception : {0} .", e));
            }
            _logger.LogInformation("strPublicKeyContentXML:" + strPublicKeyContentXML);
            RSAPKCS1SignatureDeformatter RSADeformatter = new RSAPKCS1SignatureDeformatter(RSA);
            RSADeformatter.SetHashAlgorithm("MD5");

            var bVerifyResult = false;
            try
            {
                bVerifyResult = RSADeformatter.VerifySignature(byteAuthMD5, this.byteAuthorizationRequest);
            }
            catch (System.ArgumentNullException e)
            {
                throw new ArgumentNullException(String.Format("VerifySignature Failed : RSADeformatter.VerifySignature get null argument : {0} .", e));
            }
            catch (System.Security.Cryptography.CryptographicUnexpectedOperationException e)
            {
                throw new System.Security.Cryptography.CryptographicUnexpectedOperationException(String.Format("VerifySignature Failed : RSADeformatter.VerifySignature Exception : {0} .", e));
            }

            _logger.LogInformation("004:"+ bVerifyResult);
            return bVerifyResult;
        }
    }

    internal class PolicyConfig
    {
        public string expiration { get; set; }
        public List<List<Object>> conditions { get; set; }
    }

    internal class PolicyToken
    {
        public string accessid { get; set; }
        public string policy { get; set; }
        public string signature { get; set; }
        public string dir { get; set; }
        public string host { get; set; }
        public string expire { get; set; }
        public string callback { get; set; }
    }

    internal class CallbackParam
    {
        public string callbackUrl { get; set; }
        public string callbackBody { get; set; }
        public string callbackBodyType { get; set; }
    }

}