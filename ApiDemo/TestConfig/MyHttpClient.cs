using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ApiDemo.TestConfig
{
    public class MyHttpClient: IDisposable
    {
        /// <summary>
        /// HttpClient
        /// </summary>
        public HttpClient MyClient { get; private set; }
        /// <summary>
        /// 预定key
        /// </summary>
        public string Key { get; private set; }
        /// <summary>
        /// 时间戳
        /// </summary>
        public long Timestamp { get; private set; }

        public MyHttpClient()
        {
            //配置读取KEY和URL
            var config = ConfigurationManager.OpenExeConfiguration(Assembly.GetExecutingAssembly().Location);
            var url = config.AppSettings.Settings["URL"].Value;
            MyClient = new HttpClient();
            MyClient.BaseAddress = new Uri(url);
            Key = config.AppSettings.Settings["KEY"].Value;
            Timestamp = GetTimestamp();
        }

        /// <summary>
        /// 时间戳获取方法
        /// </summary>
        /// <returns></returns>
        private long GetTimestamp()
        {
            //https://tool.lu/timestamp/
            return (DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000000;
        }

        /// <summary>
        /// MD5签名
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public string MD5Sign(string str)
        {
            var sb = new StringBuilder();
            var md5 = MD5.Create();
            byte[] bytes = md5.ComputeHash(Encoding.UTF8.GetBytes(str));
            foreach (var b in bytes)
            {
                sb.Append(b.ToString("X").PadLeft(2, '0'));
            }
            return sb.ToString();
        }

        public void Dispose()
        {
            this.MyClient.Dispose();
        }
    }
}
