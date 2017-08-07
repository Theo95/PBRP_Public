using System;
using System.Linq;
using GrandTheftMultiplayer.Server.Elements;
using GrandTheftMultiplayer.Server.API;

namespace PBRP
{
    public static class Globals
    {
        public static int MAX_PLAYERS { get; set; }
        public static DateTime ServerTime { get; set; }

        public async static void AssignBETAItems(Player player)
        {
            ////Give them a new phone

            Phone phone = new Phone()
            {
                BatteryLevel = 100,
                IMEI = await PhoneManager.GenerateImeiAsync(),
                PoweredOn = true,
                WallpaperId = 2

            };
            await PhoneRepository.AddNew(phone);

            PhoneApp phoneApp = new PhoneApp()
            {
                AppId = 0,
                Installed = true,
                PhoneId = phone.Id,
                Purchased = true,
                Position = "51"
            };

            await PhoneAppRepository.AddNew(phoneApp);

            PhoneApp messagesApp = new PhoneApp()
            {
                AppId = 1,
                Installed = true,
                PhoneId = phone.Id,
                Purchased = true,
                Position = "52"
            };

            await PhoneAppRepository.AddNew(messagesApp);

            PhoneApp settingsApp = new PhoneApp()
            {
                AppId = 2,
                Installed = true,
                PhoneId = phone.Id,
                Purchased = true,
                Position = "44"
            };

            await PhoneAppRepository.AddNew(settingsApp);

            SimCard sim = new SimCard()
            {
                Number = await PhoneManager.GeneratePhoneNumberAsync(),
                Credit = 2000,
                IsBlocked = false
            };

            await SimCardRepository.AddNew(sim);

            phone.InstalledSim = sim.Id;

            PhoneRepository.UpdateAsync(phone);

        }

        public static bool IsBETA(this string socialClub)
        {
            switch(socialClub.ToLower())
            {
                case "titancraftz":
                case "smally-":
                case ".g0dzilla":
                case ".m99t.":
                case "nathaniel_lann":
                case "boz_83_boz":
                case "schmople":
                case "booothz":
                case "cradboard":
                case "bkarner93":
                case "dftheo":
                case "speakybutton":
                case "mrwonanother":
                case "meli-malta":
                case "soapstility":
                case "agentafrica":
                case "_deadwire":
                case "tookyourtime":
                case "rufiocaslione":
                case "triple_c1987":
                case "togshark":
                    return true;
                default: return false;
            }
        }

        public struct Colour {
            public int a;
            public int r;
            public int g;
            public int b;

            public Colour(int t_r, int t_g, int t_b, int t_a = 255) {
                a = t_a;
                r = t_r;
                g = t_g;
                b = t_b;
            }

            //public Colour(string hexCode) {

            //}

            //public string ToHexcode() {
            //    string hex = "";
            //    return hex;
            //}

            //public int ToInt() {
            //    int num = 0;
            //    return num;
            //}
        }

        #region GetClosestVehicle Methods
        public static GrandTheftMultiplayer.Server.Elements.Vehicle GetClosestVehicle(Client sender, int distance)
        {
            GrandTheftMultiplayer.Server.Elements.Vehicle closest = null;
            foreach (Vehicle v in Vehicle.VehicleData.Values)
            {
                if (v.Entity.position.DistanceTo(sender.position) < distance)
                {
                    if (closest == null ||
                        Vehicle.VehicleData[closest].Entity.position.DistanceTo(sender.position) > v.Entity.position.DistanceTo(sender.position))
                            closest = v.Entity;
                }
            }
            return closest;
        }

        public static GrandTheftMultiplayer.Server.Elements.Vehicle GetClosestFactionVehicle(Client sender, int factionId, int distance)
        {
            GrandTheftMultiplayer.Server.Elements.Vehicle closest = null;
            foreach (Vehicle v in Vehicle.VehicleData.Values)
            {
                if (v.IsAdminVehicle) continue;
                if (v.IsDealerVehicle) continue;
                if (v.Entity.position.DistanceTo(sender.position) < distance)
                {
                    if (closest == null ||
                        Vehicle.VehicleData[closest].Entity.position.DistanceTo(sender.position) > v.Entity.position.DistanceTo(sender.position))
                        if (v.FactionId == factionId)
                            closest = v.Entity;
                }
            }
            return closest;
        }

        public static GrandTheftMultiplayer.Server.Elements.Vehicle GetClosestOwnedOrFactionVehicle(Player sender, int factionId, int distance)
        {
            GrandTheftMultiplayer.Server.Elements.Vehicle closest = null;
            foreach (Vehicle v in Vehicle.VehicleData.Values)
            {
                if (v.IsAdminVehicle) continue;
                if (v.IsDealerVehicle) continue;
                if (!(v.Entity.position.DistanceTo(sender.Client.position) < distance)) continue;
                if (closest != null &&
                    !(Vehicle.VehicleData[closest].Entity.position.DistanceTo(sender.Client.position) >
                      v.Entity.position.DistanceTo(sender.Client.position))) continue;
                if (v.FactionId == factionId || sender.Inventory.Count(k => k.Value == v.Key) == 1)
                    closest = v.Entity;
            }
            return closest;
        }
        #endregion

        public static string GetUniqueString()
        {
            return Convert.ToBase64String(Guid.NewGuid().ToByteArray());
        }

        private static readonly Random random = new Random();
        public static string GenerateLicensePlate(string format)
        {
            string finalPlate = "";
            string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";


            foreach (char c in format)
            {
                if(c == 'A')
                {
                    finalPlate += chars[random.Next(0, chars.Length - 1)];
                }
                else if(c == ' ')
                {
                    finalPlate += c;
                }
                else
                {
                    finalPlate += random.Next(0, 9);
                }
            }

            return finalPlate;
        }

        public static int GetPlayerID(Client player)
        {
            for(int i = 0; i < MAX_PLAYERS; i++)
            {
                if(Player.IDs[i] == player)
                {
                    return i;
                }
            }
            return -1;
        }
        public static int GetVehicleID(GrandTheftMultiplayer.Server.Elements.Vehicle vehicle)
        {
            for(int i = 0; i < 2000; i++)
            {
                if(Vehicle.IDs[i] == vehicle)
                {
                    return i;
                }
            }
            return -1;
        }

        public static int RandomNumber(this ServerAPI api, int max)
        {
            return random.Next(max);
        }

        public static int RandomNumber(this ServerAPI api, int min, int max)
        {
            return random.Next(min, max);
        }

        public static VehicleTrunkPosition GetTrunkPosition(int hash)
        {
            switch(hash)
            {
                case -1030275036:
                case -616331036:
                case -311022263:
                case 2053223216:
                case 904750859:
                case -1050465301:
                case -2052737935:
                case 2112052861:
                case 1747439474:
                case -214455498:
                case -344943009:
                case 1039032026:
                case -591651781:
                case 1549126457:
                case -1130810103:
                case 1682114128:
                case -1177863319:
                case -431692672:
                case -1450650718:
                case 841808271:
                case 330661258:
                case -5153954:
                case -591610296:
                case -391594584:
                case -89291282:
                case -624529134:
                case 1348744438:
                case -511601230:
                case 1349725314:
                case 873639469:
                case 1581459400:
                case -1930048799:
                case -1122289213:
                case -1193103848:
                case 1127131465:
                case -1647941228:
                case 469291905:
                case 2046537925:
                case -1627000575:
                case 1912215274:
                case -1973172295:
                case -1536924937:
                case -1779120616:
                case 741586030:
                case -1683328900:
                case 1922257928:
                case -2107990196:
                case -823509173:
                case 630371791:
                case 321739290:
                case -2140431165:
                case -1842748181:
                case 55628203:
                case -1289178744:
                case 743478836:
                case -1205801634:
                case -682211828:
                case -1013450936:
                case 349605904:
                case -1361687965:
                case 784565758:
                case 80636076:
                case -915704871:
                case 723973206:
                case -2119578145:
                case -1790546981:
                case -2039755226:
                case -1800170043:
                case 349315417:
                case 37348240:
                case 2068293287:
                case 525509695:
                case 1896491931:
                case -1943285540:
                case -2095439403:
                case 1507916787:
                case -589178377:
                case -227741703:
                case 941494461:
                case -1685021548:
                case 223258115:
                case 729783779:
                case 1119641113:
                case 1923400478:
                case -401643538:
                case 972671128:
                case -825837129:
                case -498054846:
                case -899509638:
                case 16646064:
                case 2006667053:
                case 523724515:
                case -48031959:
                case -1269889662:
                case -1590337689:
                case -1435919434:
                case -1479664699:
                case -1237253773:
                case 2071877360:
                case 92612664:
                case -2064372143:
                case -1207771834:
                case -2045594037:
                case -1189015600:
                case 989381445:
                case -1809822327:
                case -1807623979:
                case -1903012613:
                case 906642318:
                case 704435172:
                case -2030171296:
                case -604842630:
                case -685276541:
                case -1883002148:
                case -1241712818:
                case 1909141499:
                case 75131841:
                case -1289722222:
                case 886934177:
                case -1883869285:
                case -1150599089:
                case -2040426790:
                case -14495224:
                case 627094268:
                case -1255452397:
                case -888242983:
                case 1922255844:
                case -1477580979:
                case 1723137093:
                case -1961627517:
                case 970598228:
                case 1123216662:
                case -1894894188:
                case -1008861746:
                case 1373123368:
                case 1777363799:
                case 1283517198:
                case -305727417:
                case -713569950:
                case -2072933068:
                case -956048545:
                case 1917016601:
                case -1255698084:
                case 767087018:
                case -1041692462:
                case 1274868363:
                case -304802106:
                case 736902334:
                case 237764926:
                case 2072687711:
                case 196747873:
                case -566387422:
                case -1995326987:
                case 499169875:
                case 2016857647:
                case 544021352:
                case -1372848492:
                case 410882957:
                case 482197771:
                case -142942670:
                case -631760477:
                case -377465520:
                case -1934452204:
                case 1737773231:
                case -1485523546:
                case 1489967196:
                case -746882698:
                case -1757836725:
                case 384071873:
                case 1102544804:
                case 941800958:
                case -1566741232:
                case 1051415893:
                case -1660945322:
                case -2124201592:
                case 1830407356:
                case 1078682497:
                case 1545842587:
                case 464687292:
                case 1531094468:
                case 1762279763:
                case -2033222435:
                case -1797613329:
                case -1558399629:
                case -295689028:
                case -808831384:
                case 142944341:
                case 1878062887:
                case 634118882:
                case 470404958:
                case 666166960:
                case 850565707:
                case 2006918058:
                case -789894171:
                case 683047626:
                case 1177543287:
                case -394074634:
                case -1137532101:
                case -1775728740:
                case -1543762099:
                case 884422927:
                case 486987393:
                case 1269098716:
                case 914654722:
                case -748008636:
                case -808457413:
                case -1651067813:
                case 1645267888:
                case 1933662059:
                case 2136773105:
                case 1221512915:
                case 1337041428:
                case 1203490606:
                case -432008408:
                case -599568815:
                case 734217681:
                case 2132890591:
                case -16948145:
                case 2072156101:
                case 1739845664:
                case 1069929536:
                case -310465116:
                case -1126264336:
                case 1475773103:
                case 699456151:
                case -1311240698:
                    return VehicleTrunkPosition.Back;
                case 1026149675:
                case 1126868326:
                case -349601129:
                case -1045541610:
                case -2022483795:
                case -1297672541:
                case -1106353882:
                case 1032823388:
                case 1887331236:
                case -433375717:
                case -1696146015:
                case -1291952903:
                case 418536135:
                case 272929391:
                case 338562499:
                    return VehicleTrunkPosition.Front;
                default:
                    return VehicleTrunkPosition.Back;
            }
        }

        public static FuelType GetFuelType(int hash)
        {
            switch(hash)
            {
                case 771711535:
                case -1043459709:
                case -1066334226:
                case 908897389:
                case 1070967343:
                case 276773164:
                case 509498602:
                case 867467158:
                case 1033245328:
                case 231083307:
                case 437538602:
                case 861409633:
                case 400514754:
                case 290013743:
                case 1448677353:
                case -1030275036:
                case -616331036:
                case -311022263:
                case -431692672:
                case -1450650718:
                case 841808271:
                case -591651781:
                case -344943009:
                case 1039032026:
                case 1549126457:
                case -1130810103:
                case 1682114128:
                case -1177863319:
                case -5153954:
                case 330661258:
                case 1581459400:
                case -1930048799:
                case -391594584:
                case -89291282:
                case -591610296:
                case -624529134:
                case 1348744438:
                case -511601230:
                case 873639469:
                case 1349725314:
                case -1122289213:
                case -1193103848:
                case -1779120616:
                case 1127131465:
                case -1627000575:
                case -1647941228:
                case 469291905:
                case 741586030:
                case -1536924937:
                case 456714581:
                case 1922257928:
                case 1912215274:
                case 2046537925:
                case -1683328900:
                case -1973172295:
                case -34623805:
                case 321739290:
                case 1672195559:
                case -1670998136:
                case 1753414259:
                case 1836027715:
                case -1353081087:
                case -618617997:
                case -1606187161:
                case -2115793025:
                case 301427732:
                case -159126838:
                case 1491277511:
                case -1523428744:
                case -1453280962:
                case 788045382:
                case 86520421:
                case 11251904:
                case 6774487:
                case -405626514:
                case -114291515:
                case -891462355:
                case 2035069708:
                case -1289178744:
                case -1842748181:
                case 627535535:
                case -757735410:
                case -893578776:
                case -609625092:
                case -634879114:
                case -239841468:
                case 1790834270:
                case 55628203:
                case 640818791:
                case 822018448:
                case 1265391242:
                case -255678177:
                case -909201658:
                case -140902153:
                case -2140431165:
                case 390201602:
                case -1404136503:
                case 2006142190:
                case 741090084:
                case 1873600305:
                case 743478836:
                case -1009268949:
                case -570033273:
                case -682211828:
                case -1013450936:
                case 2068293287:
                case -498054846:
                case -1800170043:
                case -667151410:
                case -589178377:
                case 349315417:
                case 1507916787:
                case 525509695:
                case 1896491931:
                case -1685021548:
                case 223258115:
                case -401643538:
                case 1923400478:
                case 972671128:
                case -825837129:
                case 523724515:
                case 2006667053:
                case 16646064:
                case -899509638:
                case -326143852:
                case 723973206:
                case -1943285540:
                case -2095439403:
                case -227741703:
                case 941494461:
                case 784565758:
                case -1205801634:
                case 349605904:
                case -1361687965:
                case -915704871:
                case 80636076:
                case 37348240:
                case 833469436:
                case 729783779:
                case 1119641113:
                case -2119578145:
                case -1790546981:
                case -2039755226:
                case -1237253773:
                case -349601129:
                case -1661854193:
                case 1126868326:
                case -827162039:
                case -312295511:
                case 534258863:
                case 1770332643:
                case -1435919434:
                case 92612664:
                case -2064372143:
                case 1233534620:
                case -1479664699:
                case 2071877360:
                case -2128233223:
                case -1590337689:
                case -48031959:
                case -1269889662:
                case -440768424:
                case -663299102:
                case 989381445:
                case -845961253:
                case 101905590:
                case -1883002148:
                case -1241712818:
                case -685276541:
                case -1150599089:
                case -2040426790:
                case -1961627517:
                case 1777363799:
                case 75131841:
                case -1255452397:
                case 1922255844:
                case -888242983:
                case -114627507:
                case 627094268:
                case 1909141499:
                case -1809822327:
                case -1807623979:
                case -1883869285:
                case -14495224:
                case -2030171296:
                case -604842630:
                case 704435172:
                case 906642318:
                case 1123216662:
                case -1903012613:
                case 886934177:
                case 970598228:
                case -1008861746:
                case -1477580979:
                case -1289722222:
                case 1373123368:
                case 1723137093:
                case -956048545:
                case 767087018:
                case 196747873:
                case -566387422:
                case -1995326987:
                case 1489967196:
                case -1485523546:
                case -746882698:
                case 384071873:
                case -674927303:
                case -1041692462:
                case -304802106:
                case 736902334:
                case 237764926:
                case 1102544804:
                case -1071380347:
                case -631760477:
                case -142942670:
                case -1934452204:
                case 1737773231:
                case -1757836725:
                case 1886268224:
                case 1074745671:
                case -1297672541:
                case -1106353882:
                case 1274868363:
                case 2072687711:
                case 544021352:
                case 108773431:
                case 2016857647:
                case -1372848492:
                case 410882957:
                case -1089039904:
                case 1887331236:
                case -377465520:
                case 1032823388:
                case -1461482751:
                case -777172681:
                case 482197771:
                case -1045541610:
                case -2022483795:
                case 499169875:
                case -831834716:
                case -2124201592:
                case 117401876:
                case -602287871:
                case -1566741232:
                case -1660945322:
                case -2033222435:
                case 464687292:
                case 1531094468:
                case 1762279763:
                case -1797613329:
                case -1558399629:
                case 1051415893:
                case 1545842587:
                case -2098947590:
                case 1011753235:
                case 941800958:
                case 1078682497:
                case -433375717:
                case 75889561:
                case 1830407356:
                case -282946103:
                case -1232836011:
                case 633712403:
                case 819197656:
                case -1311154784:
                case 408192225:
                case 2123327359:
                case -295689028:
                case -1758137366:
                case -1291952903:
                case 418536135:
                case 1987142870:
                case 234062309:
                case 272929391:
                case 338562499:
                case -1403128555:
                case -1829802492:
                case -2048333973:
                case -482719877:
                case 1663218586:
                case 2067820283:
                case -1216765807:
                case 1034187331:
                case 1093792632:
                case -1696146015:
                case 1426219628:
                case 2006918058:
                case -394074634:
                case 1177543287:
                case 1337041428:
                case -432008408:
                case 1203490606:
                case -1543762099:
                case -748008636:
                case 914654722:
                case 1221512915:
                case -1775728740:
                case 1645267888:
                case 1933662059:
                case 884422927:
                case 486987393:
                case -1137532101:
                case -808831384:
                case 142944341:
                case 470404958:
                case 1878062887:
                case 634118882:
                case 666166960:
                case 850565707:
                case -808457413:
                case 2136773105:
                case 683047626:
                case -1651067813:
                case 1560980623:
                case 1491375716:
                case 1783355638:
                case 1445631933:
                case 1641462412:
                case -599568815:
                case 734217681:
                case -1311240698:
                case 699456151:
                case -16948145:
                case 1739845664:
                case 2072156101:
                case 1488164764:
                case -1776615689:
                case 1162065741:
                case 1475773103:
                case 65402552:
                case 1026149675:
                case -119658072:
                case 943752001:
                case 1069929536:
                case 728614474:
                case -310465116:
                case -1126264336:
                case -120287622:
                    return FuelType.Petrol;
                case -488123221:
                case -2100640717:
                case -214455498:
                case 1747439474:
                case 850991848:
                case 1518533038:
                case -2137348917:
                case -1649536104:
                case -2052737935:
                case -1050465301:
                case 904750859:
                case 569305213:
                case 2112052861:
                case 2053223216:
                case 1171614426:
                case -1205689942:
                case 1938952078:
                case -2007026063:
                case -947761570:
                case 48339065:
                case -1006919392:
                case 444583674:
                case 1886712733:
                case -2130482718:
                case -784816453:
                case 475220373:
                case -1705304628:
                case 1353720154:
                case -2107990196:
                case 782665360:
                case -823509173:
                case 630371791:
                case 1074326203:
                case -1860900134:
                case -2045594037:
                case -1207771834:
                case -2096818938:
                case 1180875963:
                case -2103821244:
                case -1189015600:
                case 1283517198:
                case -713569950:
                case -2072933068:
                case -1098802077:
                case 1941029835:
                case -305727417:
                case -1255698084:
                case 1917016601:
                case -789894171:
                case 1269098716:
                case 516990260:
                case 887537515:
                case 2132890591:
                case -884690486:
                case -845979911:
                case -2076478498:
                case -1700801569:
                case -1323100960:
                case -442313018:
                case 682434785:
                case -1987130134:
                case -233098306:
                case 121658888:
                case 444171386:
                case 1951180813:
                case -1743316013:
                case -1346687836:
                case -907477130:
                case 296357396:
                case 893081117:
                case 1132262048:
                case -1745203402:
                case -810318068:
                case 1876516712:
                    return FuelType.Diesel;
                case -1894894188:
                case 989294410:
                case -1622444098:
                case -537896628:
                case 1147287684:
                    return FuelType.Electric;
                case -82626025:
                case -1660661558:
                case 353883353:
                case 710198397:
                case -1671539132:
                case -339587598:
                case 1075432268:
                case -1600252419:
                case 1543134283:
                case -1845487887:
                case 1044954915:
                case 744705981:
                case 1949211328:
                case 745926877:
                case 788747387:
                case 837858166:
                case -50547061:
                case 1394036463:
                case 1621617168:
                case 2025593404:
                case 368211810:
                case 1058115860:
                case 1981688531:
                case -613725916:
                case -150975354:
                case 621481054:
                case -1214293858:
                case 165154707:
                case -1295027632:
                case -1214505995:
                case 1341619767:
                case -1746576111:
                case -1281684762:
                case 1077420264:
                case -901163259:
                case 970385471:
                case 1824333165:
                case -644710429:
                case 970356638:
                case -2122757008:
                case -1673356438:
                    return FuelType.Aviation;
                default: return FuelType.Petrol;
            }
        }
    }
}
