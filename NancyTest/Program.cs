using System;
using Nancy;
using Nancy.Hosting.Self;
using Nancy.ModelBinding;

namespace NancyTest
{
    //http://blog.darkthread.net/post-2016-10-17-nancyfx.aspx
    class Program
    {
        static void Main(string[] args)
        {
            using (var host = new NancyHost(
                new Uri("http://localhost:9527")))
            {
                host.Start();
                Console.WriteLine("Please do not close the window during Nancy.Hosting.Self testing.");
                Console.Read();
                host.Stop();
            }
        }
    }

    class BindParams
    {
        public string A { get; set; }
        public string B { get; set; }
    }

    public class NancyModuleTest : NancyModule
    {
        public NancyModuleTest()
        {
            //http://localhost:9527
            Get["/"] = (p) =>
            {
                return Guid.NewGuid().ToString();
            };

            //1.由URL路由取得參數
            //http://localhost:9527/route/test
            Get["route/{blah}"] = (p) =>
            {
                return "param=" + p.blah.ToString();
            };

            //2.由QueryString或Form讀取參數
            //http://localhost:9527/getpost?a=apple&b=banana
            Get["GetPost"] = Post["GetPost"] = (p) =>
            {
                //Query, Form是dynamic，取屬性時可寫成Query.A或Query["A"]
                //Query.Blah傳回型別為DynamicDictionaryValue, 有HasValue及Value
                return $"GetPost : {Request.Query.A.Value ?? Request.Form.A.Value ?? string.Empty} {Request.Query["B"].Value ?? Request.Form["B"].Value ?? string.Empty}";
            };

            //3.Model Binding
            //http://localhost:9527/Model?a=apple&b=banana
            Get["Model"] = Post["Model"] = (p) =>
            {
                var param = this.Bind<BindParams>();
                return string.Format("Model : {0} {1}", param.A, param.B);
            };

            //4.POST JSON 及回傳 JSON
            //http://localhost:9527/Json
            //header : Content-Type application/json
            //raw JSON
            //{ A:"Apple",B:"Banana" }
            Post["Json"] = (p) =>
            {
                var param = this.Bind<BindParams>();
                return Response.AsJson(new
                {
                    Resullt = string.Format("{0} {1}", param.A, param.B)
                });
            };

        }
    }

}
