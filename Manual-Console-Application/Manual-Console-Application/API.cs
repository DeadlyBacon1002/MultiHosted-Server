using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json.Linq;

namespace Manual_Console_Application
{
    class API
    {
        public string result;

        public async Task<bool> checkServerStatus()
        {
            var client = new HttpClient();
            HttpResponseMessage response = await client.GetAsync("https://mcapi.us/server/status?ip=BigPPBoys.ooguy.com");
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadAsStringAsync();
            JObject json = JObject.Parse(result);
            if (json.ContainsKey("online"))
            {
                result = (string)json.GetValue("online");
            }
            if(result == "True")
            {
                return true;
            }
            else
            {
                return false;
            }
            
        }

        public async Task<string> UpdateDNSP1()//get IP and call update api
        {
            var client = new HttpClient();
            HttpResponseMessage response = await client.GetAsync("http://api.ipify.org/");
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadAsStringAsync();
            result = "http://api.dynu.com/nic/update?hostname=BigPPBoys.ooguy.com&myip=" + result + "&myipv6=no&password=aac699232386fc400bc756468f9baa95";
            result = await UpdateDNSP2(result);
            return result;
        }

        private async Task<string> UpdateDNSP2(string url)
        {
            var client = new HttpClient();
            HttpResponseMessage response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadAsStringAsync();
            return result;
        }
    }
}
