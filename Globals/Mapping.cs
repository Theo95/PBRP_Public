using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GTANetworkServer;
using GTANetworkShared;

namespace PBRP
{
    class Mapping : Script
    {
        public Mapping()
        {
            API.onResourceStart += OnResourceStart;
        }

        public void OnResourceStart()
        {
            API.createObject(-1169914483, new Vector3(-423.344177, 5964.35645, 30.52303), new Vector3(0, -0, 150.999252));
            API.createObject(1898279756, new Vector3(-390.3321, 6026.975, 30.5631866), new Vector3(0, 0, 47.9999123));
            API.createObject(1898279756, new Vector3(-232.7916, 6167.66357, 30.4297237), new Vector3(0, 0, 40.99993));
            API.createObject(1898279756, new Vector3(-102.199089, 6296.32275, 30.5044518), new Vector3(0, 0, 43.9999275));
            API.createObject(1898279756, new Vector3(130.706726, 6554.333, 30.62526), new Vector3(0, 0, 0));
            API.createObject(1898279756, new Vector3(140.798447, 6552.608, 30.5526161), new Vector3(0, 0, 0));
            API.createObject(-879318991, new Vector3(-411.194153, 5937.84131, 31.0872459), new Vector3(0, 0, -30.9999733));
            API.createObject(-879318991, new Vector3(-433.416229, 5910.04248, 31.640173), new Vector3(0, 0, -40.99995));
            API.createObject(-1169914483, new Vector3(379.850952, 6558.179, 26.80328), new Vector3(0, -0, -91.999855));
            API.createObject(-879318991, new Vector3(333.39035, 6586.95752, 27.78068), new Vector3(0, 0, 89.99987));
            API.createObject(-879318991, new Vector3(367.179169, 6586.836, 27.0468769), new Vector3(0, 0, 71.99987));
        }
    }
}
