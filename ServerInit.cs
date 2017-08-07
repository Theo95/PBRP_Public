using GrandTheftMultiplayer.Server.API;
using GrandTheftMultiplayer.Server.Constant;
using GrandTheftMultiplayer.Server.Elements;
using GrandTheftMultiplayer.Shared;
using GrandTheftMultiplayer.Shared.Math;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading;

namespace PBRP
{
    public class ServerInit : Script
    {
        public static Ped SkinImagePed { get; set; }
        public delegate void onSecond();
        public delegate void onIGMinute();
        public delegate void onMinute();
        public delegate void onMilli();
        public static event onMilli OnMilli = delegate { };
        public static event onSecond OnSecond = delegate { };
        public static event onMinute OnMinute = delegate { };
        public static event onIGMinute OnIGMinute = delegate { };

        public static int ServerYear { get; set; }
        public static int ServerMonth { get; set; }
        public static int ServerDay { get; set; }

        public static int ServerHour { get; set; }
        public static int ServerMinute { get; set; }

        private Thread OnSecondTick;
        private Thread IGMinuteTick;
        private Thread MilliTick;

        public ServerInit()
        {
            API.onResourceStart += OnResourceStart;
            OnIGMinute += OnIGMinuteHandler;
            API.onResourceStop += API_onResourceStop;
        }

        private void API_onResourceStop()
        {
            foreach(Client c in API.getAllPlayers())
            {
                API.triggerClientEventForAll("onPlayerDisconnect");
                c.FadeIn(100);
                c.kick("Server Restart");
            }
        }

        private async void LoadWeatherData()
        {
            using (var httpClient = new HttpClient())
            {
                try
                {
                    var json = await httpClient.GetStringAsync("http://api.wunderground.com/api/41f62bac4c7f23ef/hourly/q/zmw:92101.1.99999.json");

                    dynamic weatherData = JsonConvert.DeserializeObject(json);

                    var weather = weatherData.hourly_forecast;
                    Weather.WeatherData.Clear();
                    for (int i = 0; i < 24; i++)
                    {
                        Weather newWeather = new Weather
                        {
                            Hour = weather[i].FCTTIME.hour,
                            TempF = weather[i].temp.english,
                            TempC = weather[i].temp.metric,
                            WindSpeed = weather[i].wspd.english / 18,
                            Description = weather[i].condition,
                            RainPercentage = weather[i].pop,
                            WeatherType = 2,
                            CloudOpacity = 0.1f,
                            SnowActive = false,
                            DeepPedTracks = false,
                            DeepVehicleTracks = false
                        };


                        Console.WriteLine("{0}: Temp = {1}C {2} {3}", newWeather.Hour, newWeather.TempC, newWeather.Description, newWeather.WindSpeed);

                        if (newWeather.Description.Contains("Rain") || newWeather.Description.Contains("Drizzle"))
                        {
                            if (newWeather.Description.Contains("Light"))
                            {

                            }
                            else if (newWeather.Description.Contains("Heavy"))
                            {

                            }
                            else
                            {

                            }
                        }
                        else if (newWeather.Description.Contains("Haze"))
                        {
                            newWeather.WeatherType = 3;
                        }
                        else if (newWeather.Description.Contains("Fog"))
                        {
                            newWeather.WeatherType = 4;
                        }
                        else if (newWeather.Description.Contains("Snow"))
                        {
                            if (newWeather.Description.Contains("Light"))
                            {

                            }
                            else if (newWeather.Description.Contains("Heavy"))
                            {

                            }
                            else
                            {

                            }
                            newWeather.SnowActive = weather[i].snow.english > 0.3 ? true : false;
                            newWeather.DeepPedTracks = newWeather.DeepVehicleTracks = weather[i].snow.engish > 2 ? true : false;
                        }
                        else if (newWeather.Description.Contains("Cloudy"))
                        {
                            newWeather.WeatherType = 2;
                            newWeather.CloudOpacity = newWeather.Description.Contains("Mostly") ? 1f : 0f;
                        }
                        else if (newWeather.Description.Contains("Thunderstorm"))
                        {
                            if (newWeather.Description.Contains("Heavy"))
                            {

                            }
                            else
                            {

                            }
                        }
                        else
                        {
                            newWeather.WeatherType = API.RandomNumber(0, 1);
                        }
                        Weather.WeatherData.Add(newWeather);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.InnerException);
                    
                }
            }

            Weather we = Weather.WeatherData?.FirstOrDefault(w => w.Hour == Server.Date.Hour);
            if (we == null) return;
            API.setWeather(we.WeatherType);
            API.sendNativeToAllPlayers(Hash.SET_WIND_SPEED, we.WindSpeed);
            API.sendNativeToAllPlayers(Hash._SET_RAIN_FX_INTENSITY, we.RainLevel);
            API.sendNativeToAllPlayers(0xF36199225D6D8C86, we.RainLevel);
            API.triggerClientEventForAll("activateSnow", we.SnowActive, we.DeepPedTracks, we.DeepVehicleTracks);
        }

        private void OnIGMinuteHandler() {
            if (ServerMinute == 59)
            {
                LoadWeatherData();
                if (ServerHour == 23)
                {
                    ServerHour = 0;
                    ServerDay++;
                    
                    var numOfDays = 31;
                    switch(ServerMonth)
                    {
                        case 2:
                            numOfDays = 28;
                            break;
                        case 4:
                        case 6:
                        case 9:
                        case 11:
                            numOfDays = 30;
                            break;                       
                    }
                    if(ServerDay >= numOfDays)
                    {
                        ServerMonth++;
                        if(ServerMonth >= 12)
                        {
                            ServerMonth = 1;
                            ServerYear++;
                        }
                        ServerDay = 1;
                    }                    
                }
                ServerHour++;
                ServerMinute = 0;
            }
            ServerMinute++;


            Server.Date = new DateTime(ServerYear, ServerMonth, ServerDay, ServerHour, ServerMinute, 0);
            API.sendNativeToAllPlayers(Hash.ADVANCE_CLOCK_TIME_TO, Server.Date.Hour, Server.Date.Minute, 0);

            Console.WriteLine("{0}", Server.Date.ToString("dd/MM/yy HH:mm"));
            ServerRepository.UpdateAsync();

            //PrisonManager.TickPrisonSentences();
        }

        private async void OnResourceStart()
        {
            API.setGamemodeName("pbrp");

            Server serverData = ServerRepository.LoadServerData();

            Server.Date = serverData.DateTime;

            Globals.MAX_PLAYERS = API.getMaxPlayers();

            LoadWeatherData();

            Player.IDs = new Client[Globals.MAX_PLAYERS];
            Player.IDs.Initialize();

            Vehicle.IDs = new GrandTheftMultiplayer.Server.Elements.Vehicle[2000];
            Vehicle.IDs.Initialize();

            // -- Dropped Weapons
            Weapon.DroppedWeapons = WeaponRepository.GetAllDroppedWeapons();
            foreach (Weapon w in Weapon.DroppedWeapons)
            {
                string weaponName = NameToHash.Weapons.Keys.Where(k => NameToHash.Weapons[k] == (WeaponHash)w.Model).First();
                w.DroppedObj = API.createObject(NameToHash.WeaponObjects[weaponName], w.DroppedPos, w.DroppedRot, w.DroppedDimension);
            }

            // --- Dropped Items
            Inventory.DroppedItems = await InventoryRepository.GetAllDroppedInventoryItems();
            foreach (Inventory i in Inventory.DroppedItems)
            {
                i.DroppedObj = API.createObject(Inventory.GetObjectForItem[i.Type], i.DroppedPos, i.DroppedRot, i.DroppedDimension);
            }

            SkinImagePed = API.createPed(PedHash.Barry, new Vector3(-697.0174, 5803.262, 17.33096), 90, 0);

            System.Globalization.CultureInfo.DefaultThreadCurrentCulture = new System.Globalization.CultureInfo("en-US");

            API.removeIpl("bnkheist_apt_dest");
            API.removeIpl("bnkheist_apt_dest_vfx");
            API.removeIpl("bnkheist_apt_dest_lod");
            API.requestIpl("bnkheist_apt_norm");
            API.requestIpl("bnkheist_apt_norm_lod");
            API.removeIpl("CS1_02_cf_offmission");
            API.requestIpl("CS1_02_cf_onmission1");
            API.requestIpl("CS1_02_cf_onmission2");
            API.requestIpl("CS1_02_cf_onmission3");
            API.requestIpl("CS1_02_cf_onmission4");

            API.removeIpl("fakeint");
            API.requestIpl("shr_int");

            API.createObject(1539730305, new Vector3(-810.866821, -210.795883, 340.32489), new Vector3(0, 0, 14.6697245));
            API.createObject(1539730305, new Vector3(-809.520752, -210.520264, 341.861816), new Vector3(89.0096512, -2.6680425e-08, -163.269455));
            API.createObject(1539730305, new Vector3(-810.520752, -208.520264, 341.861816), new Vector3(89.0096512, 4.00206375e-08, -78.2687149));
            API.createObject(1539730305, new Vector3(-810.520752, -212.520264, 341.861816), new Vector3(89.0096512, -2.134434e-07, 101.731239));

            //API.requestIpl("shr_int_lod");

            var doorManager = API.exported.doormanager;

            // -- Base Doors
            doorManager.setDoorState(doorManager.registerDoor(-1666470363, new Vector3(-109.65, 6462.11, 31.98499)), false, 0); // PB Bank Right Door 1(1)
            doorManager.setDoorState(doorManager.registerDoor(-353187150,  new Vector3(-111.48, 6463.94, 31.98499)), false, 0); // PB Bank Left Door 2 (2)

            doorManager.setDoorState(doorManager.registerDoor(-1501157055, new Vector3(-442.66, 6015.222, 31.86633)), true, 0); // PB PD Left Door (3)
            doorManager.setDoorState(doorManager.registerDoor(-1501157055, new Vector3(-444.66, 6017.060, 31.86633)), true, 0); // PB PD Right Door (3)
                
            doorManager.setDoorState(doorManager.registerDoor(1417577297, new Vector3(-37.33113, -1108.873, 26.7198)), true, 0); // Simeon Garage Door 1 (5)
            doorManager.setDoorState(doorManager.registerDoor(2059227086, new Vector3(-39.13366, -1108.218, 26.7198)), true, 0); // Simeon Garage Door 2 (6)
            doorManager.setDoorState(doorManager.registerDoor(2059227086, new Vector3(-59.89302, -1092.952, 26.88362)), true, 0); // Simeon Garage Door 3 (7)
            doorManager.setDoorState(doorManager.registerDoor(1417577297, new Vector3(-60.54582, -1094.749, 26.88872)), true, 0); // Simeone Garage Door 4 (8)

            doorManager.setDoorState(doorManager.registerDoor(-116041313, new Vector3(127.9552, -1298.503, 29.41962)), true, 0); // LS Strip Club Door 1 (9)
            doorManager.setDoorState(doorManager.registerDoor(668467214, new Vector3(96.09197, -1284.854, 29.43878)), true, 0); // LS Strip Club Door 2 (10)

            // --- PD INTERIOR DOORS

            doorManager.setDoorState(doorManager.registerDoor(-1033001619, new Vector3(463.4782, -1003.538, 25.00599)), true, 0); // LS PD Door 1 (11)
            doorManager.setDoorState(doorManager.registerDoor(749848321, new Vector3(461.2865, -985.3206, 30.83926)), true, 0); // LS PD Door 2 (12)
            doorManager.setDoorState(doorManager.registerDoor(749848321, new Vector3(453.0793, -983.1895, 30.83926)), true, 0); // LS PD Door 3 (Armoury) (13)
            
            PDManager.AddCell(631614199, new Vector3(464.5601, -992.6381, 25.0649), true);  // LS PD Cell 1
            PDManager.AddCell(631614199, new Vector3(461.8293, -994.4047, 25.0649), true);  // LS PD Cell 2
            PDManager.AddCell(631614199, new Vector3(461.8293, -998.6381, 25.0649), true);  // LS PD Cell 3
            PDManager.AddCell(631614199, new Vector3(461.8293, -1002.6381, 25.0649), true); // LS PD Cell 4

            // -- Property Doors are handled in Property Manager

            ServerDay = Server.Date.Day;
            ServerMonth = Server.Date.Month;
            ServerYear = Server.Date.Year;

            ServerHour = Server.Date.Hour;
            ServerMinute = Server.Date.Minute;

            API.delay(1200, true, OnServerTick);
        }

        public void OnServerTick()
        {
            long t = 0;
            IGMinuteTick = new Thread(new ThreadStart(() =>
            {
                while (true)
                {
                    Thread.Sleep(50000);
                    OnIGMinute();
                }
            }));

            MilliTick = new Thread(new ThreadStart(() =>
            {
                while (true)
                {
                    Thread.Sleep(1);
                    OnMilli();
                }
            }));

            OnSecondTick = new Thread(() =>
            {
                while (true)
                {
                    Thread.Sleep(1000);
                    //HuntingManager.Wildlife_Update();
                    OnSecond();

                    if (t % 60 == 0)
                    {
                        OnMinute();
                        t = 0;
                    }
                    t++;
                }
            });

            IGMinuteTick.Start();
            OnSecondTick.Start();
        }
    }
}


