using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Timers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PushbulletSharp;
using PushbulletSharp.Models.Requests;



namespace ConsoleApp1{
    
    public class Settings{

        private int damagePercentage;

        public int DamagePercentage{
            get => damagePercentage;
            set => damagePercentage = value;
        }

        public bool BuildTech{
            get => buildTech;
            set => buildTech = value;
        }

        private bool buildTech;

        private bool buildSkylab;

        public bool BuildSkylab
        {
            get => buildSkylab;
            set => buildSkylab = value;
        }
        public Settings(int damagePercentage,bool buildTech, bool buildSkylab){
            DamagePercentage = damagePercentage;
            BuildTech = buildTech;
            BuildSkylab = buildSkylab;
        }

        


    }
    public class Account{
        
        private PushbulletClient client;
        
        private Settings settings;
        
        private string sid=string.Empty;
        private string server=string.Empty;
        private string crediti=string.Empty;
        private string username=string.Empty;
        private string password=string.Empty;

        private int extraEnergy = 0;
         
        private int c=0;
        

        public Account(string apiKey, string server, string sid, string username, string password){
            client= new PushbulletClient(apiKey);
            Server = server;
            Sid = sid;
            Username = username;
            Password = password;
        }

        public void AddSettings(int damagePercentage,bool buildTech,bool buildSkylab){
            settings = new Settings(damagePercentage,buildTech,buildSkylab);
            Console.Title = (DateTime.Now.ToString("HH:mm:ss") + " | " + Username + " | " + Server);
        }
        
         private void SendNotification(int i){                     //0=inactivty 1=session lost
            try{
                var currentUserInformation = client.CurrentUsersInformation();
    
                if (currentUserInformation != null){
                    
                    PushNoteRequest request;
                    
                    switch (i){
                        case 0:
                            request = new PushNoteRequest{
                                Email = currentUserInformation.Email,
                                Title = "INACTIVE",
                                Body = DateTime.Now.ToString("HH:mm:ss ")+Username+" | "+"Bot has been inactive for 5 minutes!"
                            };
                            Console.WriteLine(DateTime.Now.ToString("HH:mm:ss ")+Username+" | "+"Inactive");
                            break;
                        case 1:
                            request = new PushNoteRequest{
                                Email = currentUserInformation.Email,
                                Title = "SESSION LOST",
                                Body = DateTime.Now.ToString("HH:mm:ss ")+Username+" | "+"Session not found!"
                            };
                            Console.WriteLine(DateTime.Now.ToString("HH:mm:ss ")+Username+" | "+"Session not found(SID expired)");
                            break;
                        case 2:
                            request = new PushNoteRequest{
                                Email = currentUserInformation.Email,
                                Title = "DRONE REPAIR ERROR",
                                Body = DateTime.Now.ToString("HH:mm:ss ")+Username+" | "+"Error when repairing drones!!"
                            };
                            
                            break;
                        default:
                            request = new PushNoteRequest{
                                Email = currentUserInformation.Email,
                                Title = "ERROR",
                                Body = DateTime.Now.ToString("HH:mm:ss ")+Username+" | "+"Unknown error!"
                            };
                            break;
                    }
                    
                    client.PushNote(request);
                }
                
            }catch(Exception ex){
                Console.WriteLine(DateTime.Now.ToString("HH:mm:ss ")+Username+" | "+"API key not found. Go here to generate" +
                                 " your api key and sync your accounts: https://www.pushbullet.com/#settings/account");
                Console.WriteLine(ex);
           }
        }
         
         public void CheckActivity(){
             var htmlResult = GetHtmlSource();
             
             var doc = new HtmlAgilityPack.HtmlDocument();
             doc.LoadHtml( htmlResult );
             if(htmlResult!=""){
                 var credits= doc.DocumentNode.SelectSingleNode("(//div[@id=\"header_credits\"])").InnerText; 
                 var uridium = doc.DocumentNode.SelectSingleNode("(//a[@id=\"header_uri\"])").InnerText;

                 credits =credits.Replace(" ",string.Empty);
                 credits = Regex.Replace(credits, @"\n", "");

                uridium = uridium.Replace(" ", string.Empty);
                uridium = Regex.Replace(uridium, @"\n", "");            
                

                if (C == 0){
                     Console.WriteLine(DateTime.Now.ToString("HH:mm:ss ")+Username+" | Credits: "+credits+" - Uridium: "+uridium+ " - Extra Energy: "+ExtraEnergy);
                     credits=credits.Replace(".",string.Empty);
                     Crediti=credits;        
                     C++;
                 }else{
                     Console.WriteLine(DateTime.Now.ToString("HH:mm:ss ")+Username+ " | Credits: " +credits+ " - Uridium: " +uridium+" - Extra Energy: "+ExtraEnergy);
                     credits=credits.Replace(".",string.Empty);       
                     if(string.Compare(Crediti,credits)==0)
                         SendNotification(0);
                     else
                         Crediti=credits;                
                 }
             }else{
                 //notifica cell perchè sloggato
                 SendNotification(1);
             }
         }
         
         public void CheckActivity(object source, ElapsedEventArgs e){
            var htmlResult = GetHtmlSource();

            var doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(htmlResult);
            if (htmlResult != "")
            {
                var credits = doc.DocumentNode.SelectSingleNode("(//div[@id=\"header_credits\"])").InnerText;
                var uridium = doc.DocumentNode.SelectSingleNode("(//a[@id=\"header_uri\"])").InnerText;

                credits = credits.Replace(" ", string.Empty);
                credits = Regex.Replace(credits, @"\n", "");

                uridium = uridium.Replace(" ", string.Empty);
                uridium = Regex.Replace(uridium, @"\n", "");


                if (C == 0)
                {
                    Console.WriteLine(DateTime.Now.ToString("HH:mm:ss ") + Username + " | Credits: " + credits + " - Uridium: " + uridium + " - Extra Energy: " + ExtraEnergy);
                    credits = credits.Replace(".", string.Empty);
                    Crediti = credits;
                    C++;
                }
                else
                {
                    Console.WriteLine(DateTime.Now.ToString("HH:mm:ss ") + Username + " | Credits: " + credits + " - Uridium: " + uridium + " - Extra Energy: " + ExtraEnergy);
                    credits = credits.Replace(".", string.Empty);
                    if (string.Compare(Crediti, credits) == 0)
                        SendNotification(0);
                    else
                        Crediti = credits;
                }
            }
            else
            {
                //notifica cell perchè sloggato
                SendNotification(1);
            }
        }
         
         private string  GetHtmlSource(){
             
            using (var wc = new WebClient()){
                var xmlResult = "";
                var htmlResult = "";
                try{
                    htmlResult += Login(wc);
                    var doc1 = new HtmlAgilityPack.HtmlDocument();
                    doc1.LoadHtml(htmlResult);
                     
                    if (GetBetween(doc1.Text, "html", "header_credits") == "")
                        htmlResult += LoginWhenExpired(wc,doc1);
                        doc1.LoadHtml(htmlResult);

                    if ((C % 72)==0)                    // Checks at first runtime and every 6 hours
                        htmlResult += DailyLoginBonus(wc);

                    if(settings.BuildSkylab)
                        htmlResult += BuildSkyLab(wc);

                    if(settings.BuildTech)
                        htmlResult += BuildPrecisionTargeter(wc);
                    
                    htmlResult += RepairDrones(wc);

                    var userId = doc1.DocumentNode.SelectSingleNode("//div[contains(@class, 'header_item_wrapper')]//span").InnerText;
                    xmlResult += wc.DownloadString("https://" + Server + ".darkorbit.com/flashinput/galaxyGates.php?userID=" + userId + "&action=init&sid=" + Sid);
                    doc1.LoadHtml(xmlResult);
                    ExtraEnergy = int.Parse(doc1.DocumentNode.SelectSingleNode("//samples").InnerText);

                }
                catch(Exception ex){
                    Console.WriteLine(DateTime.Now.ToString("HH:mm:ss ")+Username+" | "+"Connection not available/Wrong SID or Server");
                    Console.WriteLine(ex);
                    return "";
                }
                return htmlResult;
             }
                
         }

         private string DailyLoginBonus(WebClient wc){
             var htmlResult = string.Empty;

             htmlResult += wc.DownloadString("https://" + Server + ".darkorbit.com/flashAPI/dailyLogin.php?doBook");
             if(GetBetween(htmlResult,"{","true")!= "")
                Console.Write(DateTime.Now.ToString("HH:mm:ss ")+Username+" | "+"Received daily login bonus!\n");
             else if(GetBetween(htmlResult,"{","error")!= "")
                 Console.Write(DateTime.Now.ToString("HH:mm:ss ")+Username+" | "+"Daily login bonus already received!\n");
             else
                 Console.Write(DateTime.Now.ToString("HH:mm:ss ")+Username+" | "+"Unknown error on daily login bonus!\n");
    
             return htmlResult;
         }

         private string RepairDrones(WebClient wc){
             var htmlResult = string.Empty;

             var activeHangarId = GetActiveHangarId(wc);
             var drones = GetDronesOver90Damage(wc, activeHangarId);

             var length = drones.Count;
             var lootId = string.Empty;
             var repairPrice = string.Empty;
             var itemId = string.Empty;
             var repairCurrency = string.Empty;
             var droneLevel = string.Empty;
             var notificationSent = false;
             
             Thread.Sleep(4000);
             
             for (var i = 0; i < drones.Count; i++){

                 if (string.Compare(drones[i][0], "2") == 0)
                     lootId = "drone_iris";
                 else if (string.Compare(drones[i][0], "3") == 0)
                     lootId = "drone_apis";
                 else if (string.Compare(drones[i][0], "4") == 0)
                     lootId = "drone_zeus";
                 else if (string.Compare(drones[i][0], "1") == 0)
                     lootId = "drone_flax";


                 var encodedString =
                     "{\"action\":\"repairDrone\",\"lootId\":\"" + lootId + "\",\"repairPrice\":" + drones[i][1] +
                     ",\"params\":{\"hi\":" + activeHangarId + "}," +
                     "\"itemId\":\"" + drones[i][2] + "\",\"repairCurrency\":\"" + drones[i][3] +
                     "\",\"quantity\":1,\"droneLevel\":" + drones[i][4] + "}";

                 var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(encodedString);
                 var decodedString = System.Convert.ToBase64String(plainTextBytes);

                 wc.Headers.Add("Content-Type", "application/x-www-form-urlencoded");

                 htmlResult = wc.UploadString("https://" + Server + ".darkorbit.com/flashAPI/inventory.php",
                     "action=repairDrone&params=" + decodedString);

                 byte[] data = Convert.FromBase64String(htmlResult);
                 decodedString = Encoding.UTF8.GetString(data);
                 decodedString = decodedString.Replace("\"", "\'");

                 if (GetBetween(decodedString, "'isError':0", "'data'")!=""){
                     Console.WriteLine(DateTime.Now.ToString("HH:mm:ss ") + Username + " | " + "Drone repaired (over " +
                                       settings.DamagePercentage + " damage)");
                 }else{
                     Console.WriteLine(DateTime.Now.ToString("HH:mm:ss ") + Username + " | " +
                                       "Drone repair error. Check your account's uridium/ " +
                                       "Make sure you haven't set repair percentage at 0 in the .txt");
                     if (notificationSent == false){
                         SendNotification(2);
                         notificationSent = true;
                     }
                 }

                 Thread.Sleep(5000);
             }

             return htmlResult;
         }

         private List<List<string>> GetDronesOver90Damage(WebClient wc, string activeHangarId){
             var htmlResult = String.Empty;
             
             var encodedString="{\"params\":{\"hi\":"+activeHangarId+"}}";
             var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(encodedString);
             var decodedString = System.Convert.ToBase64String(plainTextBytes);
             
             wc.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
             htmlResult += wc.UploadString("https://"+Server+".darkorbit.com/flashAPI/inventory.php",
                 "action=getHangar&params="+decodedString);
             
             byte[] data = Convert.FromBase64String(htmlResult);
             decodedString = Encoding.UTF8.GetString(data);
             
             decodedString = decodedString.Replace("\"", "\'");
             
             var result = JsonConvert.DeserializeObject<dynamic>(decodedString);
             
             var hangar =string.Empty;
             result = result.SelectToken("data.ret.hangars");
             
             List<List<String>> drones = new List<List<String>>();
             int i = 0;
             
             if (GetBetween(decodedString, "'hangars':[", "hangarID") != ""){
                 Console.ReadLine();
                 
                 for (i = 0; i < result.Count; i++){
                     if (result[i].hangar_is_active == true){
                         result = result.SelectToken("["+i+"].general.drones");
                         Console.WriteLine(i );
                     }
                 }
             }
             else{

                 foreach (JProperty prop in result.Properties()){
                     hangar = prop.Name;
                 }

                 result = result.SelectToken("['" + hangar + "'].general.drones");
             }

             i = 0;
             foreach (JObject item in result){
                var tmp = (string) item.GetValue("HP");
                tmp = tmp.Remove(tmp.Length - 1);
                int tmp2 = Int32.Parse(tmp);
                if (tmp2 >= settings.DamagePercentage){
                    drones.Add(new List<String>());
                    drones[i].Add((string) item.GetValue("L"));
                    drones[i].Add((string) item.GetValue("repair"));
                    drones[i].Add((string) item.GetValue("I"));
                    drones[i].Add((string) item.GetValue("currency"));
                    drones[i].Add((string) item.GetValue("LV"));
                    i++;
                }
             }

             return drones;
         }

         private string GetActiveHangarId(WebClient wc){
             var htmlResult = string.Empty;
             wc.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
             htmlResult += wc.UploadString("https://" + Server + ".darkorbit.com/flashAPI/inventory.php",
                 "action=getHangarList&params=e30%3D");

             byte[] data = Convert.FromBase64String(htmlResult);
             string decodedString = Encoding.UTF8.GetString(data);

             decodedString = decodedString.Replace("\"", "\'");
             
             dynamic result = JsonConvert.DeserializeObject(decodedString);
             result = result.data.ret.hangars;
             
             if (GetBetween(decodedString, "'hangars':[", "hangarID") != ""){
                 
                 for (var i = 0; i < result.Count; i++){
                     if (result[i].hangar_is_active == true){
                         htmlResult = result[i].hangarID;
                     }
                 }

             }else{
                 
                 foreach (var hangars in result){
                     foreach (var hangar in hangars){
                         var activeHangar = hangar.GetValue("hangar_is_active");
                         if (activeHangar == true)
                             htmlResult = hangar.GetValue("hangarID").ToString();
                     }
                 }
             }
 
         return htmlResult;
         }
         private string Login(WebClient wc){
             var htmlResult = string.Empty;
             
             wc.Headers.Add(HttpRequestHeader.Cookie, "dosid="+Sid);
             htmlResult+=wc.DownloadString("https://" + Server + ".darkorbit.com/indexInternal.es?action=internalStart");
             
             return htmlResult;
         }

         private string LoginWhenExpired(WebClient wc, HtmlAgilityPack.HtmlDocument doc){
             var htmlResult = string.Empty;
             
             wc.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
             Console.WriteLine(DateTime.Now.ToString("HH:mm:ss ")+Username+" | "+"Session expired! Relogging in with the same SID");
             
             var link2 = doc.DocumentNode.SelectSingleNode("(//form[@action])[1]");
             var a = link2.Attributes["action"].Value;
             a = a.Replace("&amp;", "&");
             
             htmlResult+= wc.UploadString(a, "username="+Username+"&password="+Password);
             return htmlResult;
         }

         private string BuildPrecisionTargeter(WebClient wc){
             var htmlResult = string.Empty;
                        
             wc.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
             var result = wc.UploadString("https://"+Server+".darkorbit.com/ajax/nanotechFactory.php", "command=nanoTechFactoryShowBuff&key=RPM&level=1");
             var inProduzione = GetBetween(result, "result", "button_build_inactive");
             if (inProduzione == ""){           
                 var doc = new HtmlAgilityPack.HtmlDocument();
                 doc.LoadHtml( result );
                 
                 var link = doc.DocumentNode.SelectSingleNode("(//a[@href])[2]");
                 result = link.Attributes["href"].Value;
                 
                 result = result.Remove(0, 2);
                 result = result.Remove(result.Length-2);
                 
                 htmlResult += wc.DownloadString("https://" + Server + ".darkorbit.com/" + result);
                 Console.WriteLine(DateTime.Now.ToString("HH:mm:ss ")+Username+" | "+"Precision Targeter tech has been started");
             }
            

             return htmlResult;
         }
         
         private string BuildSkyLab(WebClient wc){
             var htmlResult = string.Empty;

             var result = wc.DownloadString("https://" + Server + ".darkorbit.com/indexInternal.es?action=internalSkylab");
             
             var doc = new HtmlAgilityPack.HtmlDocument();
             doc.LoadHtml( result );
             var upgrades = doc.DocumentNode.SelectNodes("//a[contains(@href,\"subaction=upgrade\")]");

             var transport = doc.DocumentNode.SelectSingleNode("//input[@name=\"reloadToken\"]");
             var token = transport.Attributes["value"].Value;

             wc.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
             Thread.Sleep(3000);
             //htmlResult+= wc.UploadString("https://" + Server + ".darkorbit.com/indexInternal.es",
             //    "reloadToken="+token+"&reloadToken="+token+"&action=internalSkylab&subaction=startTransport&mode=normal&construction=TRANSPORT_MODULE&count_prometium=0&count_endurium=0&count_terbium=0&count_prometid=0&count_duranium=0&count_xenomit=0&count_promerium="+100+"&count_seprom=0");
             
             if (upgrades == null)
                 return htmlResult;
             
             foreach (var upgrade in upgrades){
                 result = upgrade.Attributes["href"].Value;
                 result = result.Replace("&amp;", "&");
                 Thread.Sleep(2000);
                 wc.DownloadString("https://" + Server + ".darkorbit.com/"+result);
             }
             
            

             return htmlResult;
         }
         
         private static string GetBetween(string strSource, string strStart, string strEnd){
    
             int Start, End;
             if (strSource.Contains(strStart) && strSource.Contains(strEnd))
             {
                 Start = strSource.IndexOf(strStart, 0) + strStart.Length;
                 End = strSource.IndexOf(strEnd, Start);
                 return strSource.Substring(Start, End - Start);
             }else{
                 return "";
             }
         }       
        
        public string Sid { get; set; }
        public string Server { get; set ; }
        public int C { get; set; }
        public string Crediti { get; set; }   
        public string Username { get; set; }     
        public string Password { get; set; }
        public int ExtraEnergy { get; set; }
    }
    
    
    public class Program{
       
        static void Main(string[] args){
            
            string[] accounts;
            var lines = 0;
            var i = 0;
            Console.WriteLine("v1.3.6");
            Console.WriteLine(DateTime.Now.ToString("HH:mm:ss ")+"Logging in...");
            try{
                accounts = System.IO.File.ReadAllLines(@"settings.txt");
                // settings.txt must have 1 account per line with this specifications
                // apikey;server;sid;username;password;repairDronePercentage;buildTech;buildSkylab
                //                                         
                
                lines = accounts.Length;
                string[] settings = new string[lines];
                Account[] a = new Account[lines];

                foreach (string account in accounts){

                    settings = account.Split(";");
                    if (settings.Length == 8){
                        settings[5] = Regex.Replace(settings[5], "[^0-9]", "");
                        if (settings[2].Length == 32 && int.Parse(settings[5]) >= 0 && int.Parse(settings[5]) <= 99 && (settings[6] == "true" || settings[6] == "false") && (settings[7] == "true" || settings[7] == "false")){
                            a[i] = new Account(settings[0], settings[1], settings[2], settings[3], settings[4]);

                            a[i].AddSettings(int.Parse(settings[5]), bool.Parse(settings[6]), bool.Parse(settings[7]));

                            a[i].CheckActivity();

                            StartTimer(a[i]);
                        }else
                            Console.WriteLine("Incorrect account format at line:" + (i + 1) +
                                              "\tCorrect Format is:\n" +
                                              "apikey;server;sid;username;password;repairDronePercentage;buildTech(true/false);buildSkylab(true/false)");
                    }else 
                        Console.WriteLine("Incorrect account format at line:" + (i + 1) +
                                          "\tCorrect Format is:\n" +
                                          "apikey;server;sid;username;password;repairDronePercentage;buildTech(true/false);buildSkylab(true/false)");
                    i++;
                }
                
                while (true){
                    var sid=Console.ReadLine();
                    var answer = string.Empty;
                    var newSid = string.Empty;
                    var oldSid = string.Empty;
                    
                    if (sid == "sid"){
                        Console.WriteLine("Which account would you like to change? Insert number (starting from 1)");
                        var accountNumber=int.Parse(Console.ReadLine());
                        var accountName = a[accountNumber - 1].Username;
                        
                        do{
                            Console.WriteLine("Do you want to change " + accountName + " sid? y/n");
                            answer = Console.ReadLine();
                        } while (answer!="Y"&&answer!="y"); 
                        
                        do{
                            Console.WriteLine("Write the new sid!");
                            newSid = Console.ReadLine();
                        } while (newSid.Length != 32);
            
                        oldSid = a[accountNumber - 1].Sid;
                        a[accountNumber - 1].Sid = newSid;

                        accounts[accountNumber - 1] = accounts[accountNumber - 1].Replace(oldSid,newSid);
                        System.IO.File.WriteAllLines(@"settings.txt", accounts);
                        Console.WriteLine(a[accountNumber - 1].Username+" new sid: "+a[accountNumber - 1].Sid);
                        

                    }
                
                }
            }catch(Exception ex){ 
                Console.WriteLine(ex);
            }

           

            void StartTimer(Account acc){
                System.Timers.Timer t = new System.Timers.Timer(TimeSpan.FromMinutes(5).TotalMilliseconds)
                {
                    AutoReset = true
                }; // Set the time (5 mins in this case)
                t.Elapsed += (acc.CheckActivity);
                t.Start();
            }    
        } 
    }
}
