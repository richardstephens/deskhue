using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using DeskHue.Entities;
using RestSharp;

namespace DeskHue
{
    class HueDiscoveryService
    {
        public static async Task<List<HueIpAddress>> discover()
        {
            var client = new RestClient("https://discovery.meethue.com/");

            var request = new RestRequest("/");
            var resp = await client.ExecuteAsync<List<HueIpAddress>>(request, Method.GET);
            return resp.Data;
        }
    }
}
