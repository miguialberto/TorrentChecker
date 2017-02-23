using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace WebTorrentChecker
{
    class Program
    {

        const string FILE_PROVIDERS = "providers.txt";
        const string FILE_LAST = "last.txt";
        const string FILE_NEWS = "news.txt";

        static void Main(string[] args)
        {

            // read list of providers
            string providersPath = Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar + FILE_PROVIDERS;
            if (!File.Exists(providersPath))
            {
                Console.WriteLine("No providers file: " + providersPath);
                Environment.Exit(0);
            }
            HashSet<string> providers = LoadProviders(providersPath);

            string lastFilePath = Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar + FILE_LAST;
            string changedFilePath = Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar + FILE_NEWS;
            HashSet<string> lastTorrents = new HashSet<string>();
            HashSet<string> newsTorrents = new HashSet<string>();
            HashSet<string> changedTorrents = new HashSet<string>();

            // load last torrents
            if (File.Exists(lastFilePath))
            {
                string[] lastFileLines = File.ReadAllLines(lastFilePath);
                foreach(string lastFileLine in lastFileLines)
                {
                    if (lastFileLine != null && lastFileLine.Length > 0 && !lastFileLine.StartsWith("#"))
                    {
                        lastTorrents.Add(lastFileLine);
                    }
                }
            }


            // read torrents from web
            foreach(string provider in providers)
            {
                HtmlWeb web = new HtmlWeb();
                HtmlDocument doc = web.Load(provider);
                List<string> hrefTags = new List<string>();
                foreach (HtmlNode link in doc.DocumentNode.SelectNodes("//a[@href]"))
                {
                    HtmlAttribute att = link.Attributes["href"];
                    if (att.Value.EndsWith("torrent"))
                    {
                        string torrent = att.Value;
                        newsTorrents.Add(torrent);
                        if (!lastTorrents.Contains(torrent))
                        {
                            changedTorrents.Add(torrent);
                        }
                    }
                }
            }

            // write files
            File.WriteAllText(lastFilePath, FileTextFromSet(newsTorrents));
            File.WriteAllText(changedFilePath, FileTextFromSet(changedTorrents));

            // notify
            if(changedTorrents.Count > 0)
            {
                Console.WriteLine("Notifiying by email");
                NotifyEmailNews(changedTorrents);
            }

            Environment.Exit(1);
        }

        private static string FileTextFromSet(HashSet<string> data)
        {
            StringBuilder text = new StringBuilder();
            text.AppendLine("#" + DateTime.Now.ToString("dd/MM/yyyy-HH:mm:ss"));
            foreach(string value in data)
            {
                text.AppendLine(value);
            }
            return text.ToString();
        }

        private static HashSet<string> LoadProviders(string providersPath)
        {
            HashSet<string> providers = new HashSet<string>();
            string[] lines = File.ReadAllLines(providersPath);
            foreach(string line in lines)
            {
                if (line != null && line.Length > 0 && !line.StartsWith("#"))
                {
                    providers.Add(line);
                }
            }
            return providers;
        }

        private static void NotifyEmailNews(HashSet<string> news)
        {
            MailMessage objMail = new MailMessage(new MailAddress("miguialberto@hotmail.com", "Miguel"), new MailAddress("0bb1bb946b7cf2b13f33af9705ac651a2df904290924b896+0@nmamail.net", "WebTorrent"));
            objMail.Subject = "Web Tracker";
            objMail.Body = "[" + news.Count + "] new torrents found";
            NetworkCredential objNC = new NetworkCredential("miguialberto@hotmail.com", "HOTMAILrsnky2");
            SmtpClient objsmtp = new SmtpClient("smtp.live.com", 587);
            objsmtp.EnableSsl = true;
            objsmtp.DeliveryMethod = SmtpDeliveryMethod.Network;
            objsmtp.UseDefaultCredentials = false;
            objsmtp.Credentials = objNC;
            objsmtp.Send(objMail);
        }
    }
}
