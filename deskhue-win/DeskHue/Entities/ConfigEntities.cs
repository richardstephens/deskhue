using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeskHue.Entities
{
    class HueConfig
    {
        public string ip;
        public string username;
        public string lightId;
        public bool bound;
        public bool autobound;
        public string deviceId;
        public int offset;
    }
}
