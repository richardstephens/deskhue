using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeskHue.Entities
{
    class HueIpAddress
    {
        public string id { get; set; }
        public string internalipaddress { get; set; }
        public override string ToString()
        {
            return internalipaddress;
        }
    }

    class NewUserRequest
    {
        public string devicetype;
    }

    class NewUserErrorEntity
    {
        public Int32 type { get; set; }
        public string address { get; set; }
        public string description { get; set; }
    }

    class NewUserSuccessEntity
    {
        public string username { get; set; }
    }

    class NewUserResponse
    {
        public NewUserErrorEntity error { get; set; }
        public NewUserSuccessEntity success { get; set; }
    }

    class LightState
    {
        public bool on { get; set; }
        public Int32 bri { get; set; }
        public Int32 hue { get; set; }
        public bool reachable { get; set; }
    }
    class LightEntity
    {
        public LightState state { get; set; }
        public string name { get; set; }
        public string id { get; set; }

        public override string ToString()
        {
            return id + ": " + name;
        }
    }

    class HueStateUpdate
    {
        public Int32 bri { get; set; }
    }
}

