using System;
using System.Net;
using System.Text.RegularExpressions;
using System.Timers;
using PushbulletSharp;
using PushbulletSharp.Models.Requests;
using PushbulletSharp.Models.Responses;
using HtmlAgilityPack;
using System.Collections.Generic;

namespace ConsoleApp1
{
    class Program
    {
        private string sid=string.Empty;
        private string server=string.Empty;
        private string crediti=string.Empty;
        private string username=string.Empty;
        private string password=string.Empty;
        
        private int c=0;
        PushbulletClient client;
        public string Sid { get => server; set => server = value; }

        public string Server { get => sid; set => sid = value; }

        public int C { get => c; set => c = value; }

        public string Crediti { get => crediti; set => crediti = value; }
        
        public string Username { get => username; set => username = value; }
        
        public string Password { get => password; set => password = value; }
        
        static void Main(string[] args)
        {
            Program a = new Program();
            a.login();
            a.check();
            Timer t = new Timer(TimeSpan.FromMinutes(5).TotalMilliseconds); // Set the time (5 mins in this case)
            t.AutoReset = true;
            t.Elapsed += new System.Timers.ElapsedEventHandler(a.check);
            t.Start();
            Console.ReadLine();

    
            // Timer myTimer = new Timer();
        
            
            /*  // Tell the timer what to do when it elapses
             myTimer.Interval = 10000;
             myTimer.Elapsed += new ElapsedEventHandler(a.check);
             // Set it to go off every five seconds
             // And start it        
             myTimer.Enabled = true;*/

        }
        
        void login(){
           // Console.WriteLine("PushBullet API Key");
           //String apikey=Console.ReadLine();
            client= new PushbulletClient("o.Tl8ptIfzt4UP2Je0vIb0NQ2vnlRFuvXu");
            Console.WriteLine("server");
            Server= Console.ReadLine();
            Console.WriteLine("sid");
            Sid= Console.ReadLine();   
            Console.WriteLine("username");
            Username= Console.ReadLine();      
            Console.WriteLine("password");
            Password= Console.ReadLine();
            Console.WriteLine(DateTime.Now.ToString("HH:mm:ss ")+"Logging in...");
    }

    void push(int i){                     //0=inactivty 1=session lost
       try{
           var currentUserInformation = client.CurrentUsersInformation();

        if (currentUserInformation != null)
        {
            PushNoteRequest request;
            if (i == 0)
            {
                request = new PushNoteRequest
                {

                    Email = currentUserInformation.Email,
                    Title = "INACTIVE",
                    Body = DateTime.Now.ToString("HH:mm:ss ")+"Bot has been inactive for 5 minutes!"
                };
                Console.WriteLine(DateTime.Now.ToString("HH:mm:ss ")+"Inactive");
            }else if (i == 1)
            {
                request = new PushNoteRequest
                {

                    Email = currentUserInformation.Email,
                    Title = "SESSION LOST",
                    Body = DateTime.Now.ToString("HH:mm:ss ")+"Session not found!"
                };
                Console.WriteLine(DateTime.Now.ToString("HH:mm:ss ")+"Session not found(SID expired)");
            }
            else
            {
                request = new PushNoteRequest
                {

                    Email = currentUserInformation.Email,
                    Title = "ERROR",
                    Body = DateTime.Now.ToString("HH:mm:ss ")+"Unknown error!"
                };
            }

            PushResponse response = client.PushNote(request);
        }
       }catch(Exception ex){
           Console.WriteLine(DateTime.Now.ToString("HH:mm:ss ")+"API key not found. Go here to generate your api key and sync your accounts: https://www.pushbullet.com/#settings/account");
           Console.WriteLine(ex.StackTrace);
       }
    }
    void check(){
        var shows = GetSourceForMyShowsPage();
        var doc = new HtmlAgilityPack.HtmlDocument();
        doc.LoadHtml( shows );
        if(shows!=""){
            shows= doc.DocumentNode.SelectSingleNode("(//div[@id=\"header_credits\"])").InnerText; 
            shows=shows.Replace(" ",string.Empty);
            shows = Regex.Replace(shows, @"\n", "");
            if(C==0)
                Console.WriteLine(DateTime.Now.ToString("HH:mm:ss ")+"Logged in");
            Console.WriteLine(DateTime.Now.ToString("HH:mm:ss ")+shows);
            shows=shows.Replace(".",string.Empty);
            if(C==0)
                Crediti=shows;
            else{
                if(string.Compare(Crediti,shows)==0){
                    //notifica cell per inattività
                    push(0);
                }else{
                    crediti=shows;
                }
            }
            C++;
        }else{
            //notifica cell perchè sloggato
            push(1);
        }
    }
    void check(object source, ElapsedEventArgs e){
        var shows = GetSourceForMyShowsPage();
        var doc = new HtmlAgilityPack.HtmlDocument();
        doc.LoadHtml( shows );
        if(shows!=""){
            shows= doc.DocumentNode.SelectSingleNode("(//div[@id=\"header_credits\"])").InnerText;
            shows=shows.Replace(" ",string.Empty);
            shows = Regex.Replace(shows, @"\n", "");
            if(C==0)
                Console.WriteLine(DateTime.Now.ToString("HH:mm:ss ")+"Logged in");
            Console.WriteLine(DateTime.Now.ToString("HH:mm:ss ")+shows);
            shows=shows.Replace(".",string.Empty);
            if(C==0)
                Crediti=shows;
            else{
                if(string.Compare(Crediti,shows)==0){
                    //notifica cell per inattività
                    push(0);
                }else{
                    crediti=shows;
                }
            }
            C++;
        }else{
            //notifica cell perchè sloggato
            push(1);
        }
    }
     string  GetSourceForMyShowsPage()
    {
         

        using (var wb = new WebClient())
        {

            // Download desired page
            wb.Headers.Add(HttpRequestHeader.Cookie, "dosid="+Sid);
            string ok="";
            try
            {
                //Console.WriteLine("COOKIES:"+wb.Headers.Get("Cookie"));
                ok+=wb.DownloadString("https://www.darkorbit.com");
                // Console.WriteLine("DOSID USCITA"+wb.ResponseHeaders.Get("Cookie"));
                /*  for(int i = 0; i < wb.ResponseHeaders.Count; i++)
                  {
                      String header = wb.ResponseHeaders.GetKey(i);
                      String[] values = 
                          wb.ResponseHeaders.GetValues(header);
                      if(values.Length > 0) 
                      {
                          Console.WriteLine("The values of {0} header are : "
                                          , header);
                          for(int j = 0; j < values.Length; j++) 
                              Console.WriteLine("\t{0}", values[j]);
                      }
                      else
                          Console.WriteLine("There is no value associated" +
                              "with the header");
                  }*/
                ok += wb.DownloadString("https://" + Server + ".darkorbit.com/indexInternal.es?action=internalStart");
                //wb.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
                var doc1 = new HtmlAgilityPack.HtmlDocument();
                doc1.LoadHtml(ok);
                wb.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
                if (getBetween(doc1.Text, "html", "header_credits") == ""){
                    Console.WriteLine(DateTime.Now.ToString("HH:mm:ss ")+"Session expired! Relogging in with the same SID");
                    var link2 = doc1.DocumentNode.SelectSingleNode("(//form[@action])[1]");
                    var a = link2.Attributes["action"].Value;
                    a = a.Replace("&amp;", "&");
                    ok+= wb.UploadString(a, "username="+Username+"&password="+Password);
                }
                wb.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
                  string HtmlResult = wb.UploadString("https://"+Server+".darkorbit.com/ajax/nanotechFactory.php", "command=nanoTechFactoryShowBuff&key=RPM&level=1");
                        string inProduzione = getBetween(HtmlResult, "result", "button_build_inactive");
                        if (inProduzione == "")
                        {
                            var doc = new HtmlAgilityPack.HtmlDocument();
                            doc.LoadHtml( HtmlResult );
                            var link = doc.DocumentNode.SelectSingleNode("(//a[@href])[2]");
                            HtmlResult = link.Attributes["href"].Value;
                            HtmlResult = HtmlResult.Remove(0, 2);
                            HtmlResult = HtmlResult.Remove(HtmlResult.Length-2);
                            ok += wb.DownloadString("https://" + Server + ".darkorbit.com/" + HtmlResult);
                            Console.WriteLine(DateTime.Now.ToString("HH:mm:ss ")+"Precision Targeter tech has been started");
                        }
                      
            }catch(Exception ex)
            {
                Console.WriteLine(DateTime.Now.ToString("HH:mm:ss ")+"Connection not available/Wrong SID or Server");
                Console.WriteLine(ex.StackTrace);
                ok = "";
            }
                        return ok;
                    }
            
        }
        public static string getBetween(string strSource, string strStart, string strEnd)
{
    int Start, End;
    if (strSource.Contains(strStart) && strSource.Contains(strEnd))
    {
        Start = strSource.IndexOf(strStart, 0) + strStart.Length;
        End = strSource.IndexOf(strEnd, Start);
        return strSource.Substring(Start, End - Start);
    }
    else
    {
        return "";
    }
}
           
    
}

    
}
