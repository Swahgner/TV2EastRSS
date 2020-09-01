using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using static TV2EastRSS.Json;
using static TV2EastRSS.xml;
using Console = Colorful.Console;

namespace TV2EastRSS
{
    class Program
    {
        static async Task Main(string[] args)
        {
            List<string> channelItems = new List<string>();

            string feed = null;
            string url = "https://www.tv2east.dk/rss";

            bool firstIte = true;

            string[] keywords = new string[] {
                "ulykke",
                "uheld",
                "spærret",
                "politi ",
                "politi-",
                "politiet",
                "aks",
                "udrykning",
                "fastklæmt",
                "skud",
                "færdselsuheld",
                "brand",
                "voldsom"
            };

            while (true)
            { 
                try
                { 
                    using (var client = new HttpClient())
                    {
                        try
                        {
                            feed = await client.GetStringAsync(url);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.ToString());
                        }
                    }

                    if (feed != null)
                    {
                        DateTime now = DateTime.Now;
                        string filename = now.Year.ToString() + now.Month.ToString() + now.Day.ToString() + "-" + now.Hour.ToString() + now.Minute.ToString();

                        if (!Directory.Exists(@"\RSS\log\"))
                            Directory.CreateDirectory(@"\RSS\log\");

                        using (StreamWriter writer = new StreamWriter(@"\RSS\log\" + filename + ".json"))
                        {
                            writer.Write(feed);
                        }

                        XmlSerializer serializer = new XmlSerializer(typeof(rss));
                        StringReader sr = new StringReader(feed);
                        rss content = (rss)serializer.Deserialize(sr);

                        string baseUrl = @"\RSS\";

                        foreach (rssChannelItem item in content.channel.item)
                        {
                            DateTime pub = DateTime.Parse(item.pubDate);
                            if (!Directory.Exists(baseUrl + item.category + @"\"))
                                Directory.CreateDirectory(baseUrl + item.category + @"\");

                            bool doesExist = false;

                            if (channelItems.Contains(item.guid))
                                doesExist = true;
                            else
                                Console.WriteLine("New story: " + item.title, Color.Red);

                            if (!doesExist)
                            {
                                channelItems.Add(item.guid);
                                if (channelItems.Count > 1000)
                                    channelItems.RemoveAt(0);

                                bool exciting = false;
                                foreach (string kw in keywords)
                                {
                                    if (item.title.Contains(kw) || firstIte)
                                    {
                                        exciting = true;
                                        Console.WriteLine("Sending notification...", Color.FromArgb(255, 100, 100));

                                        var parameters = new NameValueCollection {
                                            { "token", "agneeuvoupztvd18us6kqhveaudz8n" },
                                            { "user", "ubv377xnnrxujede3efpmxm3v6r5j4" },
                                            { "message", item.title + "\n" + item.description },
                                            { "title", "ALARM TV2EAST" },
                                            { "priority", "1" },
                                            { "url", item.link }
                                        };

                                        using (var client = new WebClient())
                                        {
                                            if (!firstIte)
                                                client.UploadValues("https://api.pushover.net/1/messages.json", parameters);
                                        }
                                        break;
                                    }
                                }

                                if (!exciting)
                                    Console.WriteLine("Not exciting...", Color.CadetBlue);

                                using (StreamWriter sw = new StreamWriter(baseUrl + item.category + @"\" + pub.Year + pub.Month + pub.Day + "-" + item.guid.Remove(0, 33) + ".txt"))
                                {
                                    sw.WriteLine("Title:\n" + item.title + "\n");
                                    sw.WriteLine("Published:\n" + item.pubDate + "\n");
                                    sw.WriteLine("Category:\n" + item.category + "\n");
                                    sw.WriteLine("Link:\n" + item.link);
                                }
                            }
                        }

                        if (firstIte)
                        {
                            Console.WriteLine("First iteration done", Color.Yellow);
                            firstIte = false;
                        }
                    }

                    //½Console.WriteLine("Iteration done. Pausing for 5 seconds...", Color.Green);
                    Thread.Sleep(5000);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString(), Color.Red);
                    break;
                }
            }

            Console.WriteLine("\nPress any key to close...");
            Console.ReadKey();
        }
    }
}
