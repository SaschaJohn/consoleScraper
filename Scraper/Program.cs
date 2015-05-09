using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using HtmlAgilityPack;

namespace Scraper
{
    class Program
    {
        private static String user = "";
        private static String pass = "";

        static void Main(string[] args)
        {

            Task.Run(async () =>
            {
                await DoWork();
            }).Wait();
           

            // Manually add the missing cookie.
            //Cookie cookie = new Cookie("Name", response.Cookies["Name"].Value);
            //cookie.Secure = true;
            //cookie.Domain = "www.domain.com";
            //cookieContainer.Add(cookie);

            //cookieContainer.Add(new Uri("https://www.domain.com"), response.Cookies);

            //DoHttpPost(cookieContainer, client);
            //DoHttpGet(cookieContainer, client);

        }

        private static async Task<HttpResponseMessage> DoWork()
        {
            var handler = new HttpClientHandler();
            handler.UseDefaultCredentials = true;
            handler.AllowAutoRedirect = true;
            handler.UseCookies = true;
            HttpClient client = new HttpClient(handler);
            
            client.DefaultRequestHeaders.Add("User-Agent","Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.2;WOW64; Trident/6.0)");

            CookieContainer cookieContainer = new CookieContainer();
            handler.CookieContainer = cookieContainer;

            var response = await DoHttpGet(cookieContainer, client);
            //var content = await response.Content.ReadAsStringAsync();
            
            //Console.Write(content);
//            var responseCookies = cookieContainer.GetCookies(new Uri("https://secure.moneyou.de"));
//            foreach (Cookie cookie in responseCookies)
//            {
//                string cookieName = cookie.Name;
//                string cookieValue = cookie.Value;
//                Console.WriteLine(cookieName+":"+cookieValue);
//            }
            var response2 = await DoHttpPost(cookieContainer, client);
            var content = await response2.Content.ReadAsStringAsync();

            HtmlAgilityPack.HtmlDocument htmlDoc = new HtmlAgilityPack.HtmlDocument();

            // There are various options, set as needed
            htmlDoc.OptionFixNestedTags = true;

            // filePath is a path to a file containing the html
            htmlDoc.LoadHtml(content);

            var nodes = htmlDoc.DocumentNode.SelectNodes("//span[contains(@class,'amount')]");

            foreach (HtmlNode span in nodes)
            {
                Console.Write(span.InnerText);
            }

            

            Console.ReadKey();
            return response;
        }


        private static async Task<HttpResponseMessage> DoHttpGet(CookieContainer cookieContainer, HttpClient client)
        {
            var response = await client.GetAsync("https://secure.moneyou.de/thc/policyenforcer/pages/loginB2C.jsf");
            return response;
        }

        private static async Task<HttpResponseMessage> DoHttpPost(CookieContainer cookieContainer, HttpClient client)
        {
            var data = "loginForm=loginForm&j_username_pwd="+user+"&j_password_pwd="+pass+"&storeLoginSwitch_dig=on&btnNext=Anmelden&TX_CLIENT_NUMBER=0&javax.faces.ViewState=j_id1:j_id2";
            StringContent queryString = new StringContent(data, System.Text.Encoding.UTF8, "application/x-www-form-urlencoded");

            var response =
                await client.PostAsync("https://secure.moneyou.de/thc/policyenforcer/pages/loginB2C.jsf", queryString);
            
            return response;
        }
    }
}
