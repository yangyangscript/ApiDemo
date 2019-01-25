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
    /// 获取保单(集合)
    /// </summary>
    [Collection("MyCollection")]
    public class InsurancePolicyCollectionTest
    {
        private ITestOutputHelper _outputHelper;
        private MyHttpClient _httpClient;
        /// <summary>
        /// 注入测试配置
        /// </summary>
        /// <param name="iTestOutputHelper"></param>
        /// <param name="httpClient"></param>
        public InsurancePolicyCollectionTest(ITestOutputHelper iTestOutputHelper, MyHttpClient httpClient)
        {
            _outputHelper = iTestOutputHelper;
            _httpClient = httpClient;
        }
        /// <summary>
        /// 组织Sign
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="star"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        private string GetSign(int companyId,string star,string end)
        {
            var partParam = $"{companyId}{star}{end}";
            var plainKey = $"{partParam.Substring(0, 16)}{_httpClient.Timestamp}{_httpClient.Key}";
            return _httpClient.MD5Sign(plainKey);
        }

        /// <summary>
        /// 测试调用
        /// </summary>
        /// <param name="companyId">保险公司Id(通过保险公司接口获取)</param>
        /// <param name="star">开始时间(yyyyMMdd)</param>
        /// <param name="end">结束时间(yyyyMMdd)</param>
        /// <param name="PageIndex">页码(1开始)</param>
        /// <param name="PageSise">页面大小(最大500)</param>
        [Theory]
        [InlineData(91,"20190125", "20190125",1,500)]
        [InlineData(15, "20190125", "20190125", 1, 500)]
        [InlineData(34, "20190125", "20190125", 1, 500)]
        public void SearchCarInsuranceList(int companyId, string star, string end,int PageIndex,int PageSise)
        {
            var url = $"Service/CustomerInterface/SearchCarInsuranceList.ashx?CompanyId={companyId}&TimeStr={star}{end}&Timestamp={_httpClient.Timestamp}&Sign={GetSign(companyId,star,end)}&PageIndex={PageIndex}&PageSise={PageSise}";
            var ret = _httpClient.MyClient.GetAsync(url);
            ret.Result.EnsureSuccessStatusCode();
            var jsonRet = JObject.Parse(ret.Result.Content.ReadAsStringAsync().Result);
            if (Convert.ToBoolean(jsonRet["SuccessStatus"]))
            {
                //Result详情 见 [Result对象.xlsx(CarInsurance)] 
                _outputHelper.WriteLine(jsonRet["Result"].ToString());
            }
            else
            {
                _outputHelper.WriteLine(jsonRet["ErrorMessage"].ToString());
            }
        }
    }
}
