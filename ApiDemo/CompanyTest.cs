using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using ApiDemo.TestConfig;
using Newtonsoft.Json.Linq;
using Xunit;
using Xunit.Abstractions;

namespace ApiDemo
{
    /// <summary>
    /// 获取保险公司(集合)
    /// </summary>
    [Collection("MyCollection")]
    public class CompanyTest
    {
        private ITestOutputHelper _outputHelper;
        private MyHttpClient _httpClient;

        /// <summary>
        /// 注入测试配置
        /// </summary>
        /// <param name="iTestOutputHelper"></param>
        /// <param name="httpClient"></param>
        public CompanyTest(ITestOutputHelper iTestOutputHelper, MyHttpClient httpClient)
        {
            _outputHelper = iTestOutputHelper;
            _httpClient = httpClient;
        }

        /// <summary>
        /// 组织Sign
        /// </summary>
        /// <returns></returns>
        private string GetSign()
        {
            var partTime = _httpClient.Timestamp.ToString().PadRight(16, '0');
            var plainKey = $"{partTime}{_httpClient.Timestamp}{_httpClient.Key}";
            return _httpClient.MD5Sign(plainKey);
        }

        /// <summary>
        /// 测试调用
        /// </summary>
        [Fact]
        private void SearchCompanies()
        {
            var url = $"Service/CustomerInterface/SearchCompanies.ashx?Timestamp={_httpClient.Timestamp}&Sign={GetSign()}";
            var ret=_httpClient.MyClient.GetAsync(url);
            ret.Result.EnsureSuccessStatusCode();
            var jsonRet = JObject.Parse(ret.Result.Content.ReadAsStringAsync().Result);
            if (Convert.ToBoolean(jsonRet["SuccessStatus"]))
            {
                //Result详情 见 [Result对象.xlsx(Company)] 
                _outputHelper.WriteLine(jsonRet["Result"].ToString());
            }
            else
            {
                _outputHelper.WriteLine(jsonRet["ErrorMessage"].ToString());
            }          
        }
    }
}
