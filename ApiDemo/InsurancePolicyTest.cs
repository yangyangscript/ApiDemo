using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ApiDemo.TestConfig;
using Newtonsoft.Json.Linq;
using Xunit;
using Xunit.Abstractions;

namespace ApiDemo
{
    /// <summary>
    /// 获取保单(单个)
    /// </summary>
    [Collection("MyCollection")]
    public class InsurancePolicyTest
    {
        private ITestOutputHelper _outputHelper;
        private MyHttpClient _httpClient;

        /// <summary>
        /// 注入测试配置
        /// </summary>
        /// <param name="iTestOutputHelper"></param>
        /// <param name="httpClient"></param>
        public InsurancePolicyTest(ITestOutputHelper iTestOutputHelper, MyHttpClient httpClient)
        {
            _outputHelper = iTestOutputHelper;
            _httpClient = httpClient;
        }

        /// <summary>
        /// 组织Sign
        /// </summary>
        /// <param name="bdh"></param>
        /// <returns></returns>
        private string GetSign(string bdh)
        {
            var partBdh = bdh.PadLeft(16, '0');
            var plainKey = $"{partBdh.Substring(bdh.Length - 16, 16)}{_httpClient.Timestamp}{_httpClient.Key}";
            return _httpClient.MD5Sign(plainKey);
        }

        /// <summary>
        /// 测试调用 
        /// </summary>
        /// <param name="bdh">保单号</param>
        [Theory]
        [InlineData("203043403202019007054")]
        [InlineData("6010102080320190007883")]
        [InlineData("6010102080720190021224")]       
        public void SearchCarInsurance(string bdh)
        {
            var url = $"Service/CustomerInterface/SearchCarInsurance.ashx?Bdh={bdh}&Timestamp={_httpClient.Timestamp}&Sign={GetSign(bdh)}";
            var ret = _httpClient.MyClient.GetAsync(url);
            ret.Result.EnsureSuccessStatusCode();
            var jsonRet = JObject.Parse(ret.Result.Content.ReadAsStringAsync().Result);
            if (Convert.ToBoolean(jsonRet["SuccessStatus"]))
            {
                //Result详情 见 [Result对象.xlsx(SearchCarInsurance)] 
                _outputHelper.WriteLine(jsonRet["Result"].ToString());
            }
            else
            {
                _outputHelper.WriteLine(jsonRet["ErrorMessage"].ToString());
            }
        }
    }
}
