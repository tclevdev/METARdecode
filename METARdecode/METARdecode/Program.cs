using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace METARdecode
{
    class Program
    {
        static string GetMetar(string code)
        {
            string responseString = string.Empty;

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Add("Authorization", "Token yrUG0HlkEFgXd6Hjo_gRbOC4Hb6U8oZlR4L1Fr4H4tw");
                var response = client.GetAsync($"https://avwx.rest/api/metar/{code}").Result;

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = response.Content;
                    string rData = responseContent.ReadAsStringAsync().Result;
                    JObject jsonData = JObject.Parse(rData);
                    responseString = jsonData.SelectToken("raw").ToString();
                }
            }

            return responseString;
        }

        static void Main(string[] args)
        {
            Decoder decoder = new Decoder();
            METAR metar = new METAR();

            //if (args[0].Length == 4)
            //{
                // Just an ICAO Code, look up the METAR via AVWX API
                string rawMetar = GetMetar("KSFO");
                metar = decoder.ProcessMetar(rawMetar);
            //}
            //else
            //{
                // Use command line argument
            //    metar = decoder.ProcessMetar(args[0]);
            //}

            string report = decoder.BuildReport(metar);

            Console.WriteLine(report);
            Console.ReadLine();
        }
    }
}
