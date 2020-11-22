using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DeskHue.Entities;
using RestSharp;

namespace DeskHue.Services
{
    class HueDeviceService
    {
        private HueConfig config;

        public HueDeviceService(HueConfig config)
        {
            this.config = config;
        }

        public async Task<List<LightEntity>> ListLights()
        {
            var client = new RestClient("http://" + config.ip);

            var request = new RestRequest("/api/" + config.username + "/lights");
            var response = await client.ExecuteAsync<Dictionary<String, LightEntity>>(request, Method.GET);

            List<LightEntity> result = new List<LightEntity>();
            foreach (var entry in response.Data)
            {
                entry.Value.id = entry.Key;
                result.Add(entry.Value);
            }

            return result;
        }
    }
}
