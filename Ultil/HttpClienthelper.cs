using System.Net;
using System.Reflection;

namespace Ultil
{
    public class HttpClientHelper
    {
    
        public static string Post(string url, Dictionary<string,string> paramObject)
        {
            //for ssl
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
            HttpClient client = new HttpClient();
            var content = new FormUrlEncodedContent(paramObject);
            var response = client.PostAsync(url, content).Result; 
            return response.Content.ReadAsStringAsync().Result;
           
        }       

    }
}