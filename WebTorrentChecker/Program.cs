using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebTorrentChecker
{
    class Program
    {
        static void Main(string[] args)
        {
            // check args
            if (args.Length != 2)
            {
                Console.WriteLine("Expected more arguments");
                Environment.Exit(0);
            }

            string webUrlPara = args[0];
            string aliasPara = args[1];
            string lastFileName = aliasPara + ".last";
            string changedFileName = aliasPara + ".news";
            string lastFilePath = Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar + lastFileName;
            string changedFilePath = Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar + changedFileName;
            HashSet<string> lastTorrents = new HashSet<string>();
            StringBuilder newTorrents = new StringBuilder();
            newTorrents.AppendLine("#" + DateTime.Now.ToString("yyyyMMdd_HHmmss"));
            StringBuilder changedTorrents = new StringBuilder();
            changedTorrents.AppendLine("#" + DateTime.Now.ToString("yyyyMMdd_HHmmss"));

            // load last torrents
            if (File.Exists(lastFilePath))
            {
                string[] lastFileLines = File.ReadAllLines(lastFilePath);
                foreach(string lastFileLine in lastFileLines)
                {
                    if (!lastFileLine.StartsWith("#"))
                    {
                        lastTorrents.Add(lastFileLine);
                    }
                }
            }


            // read torrents from web
            HtmlWeb web = new HtmlWeb();
            HtmlDocument doc = web.Load(webUrlPara);
            List<string> hrefTags = new List<string>();
            foreach (HtmlNode link in doc.DocumentNode.SelectNodes("//a[@href]"))
            {
                HtmlAttribute att = link.Attributes["href"];
                if (att.Value.EndsWith("torrent"))
                {
                    string torrent = att.Value;
                    newTorrents.AppendLine(torrent);
                    if (!lastTorrents.Contains(torrent))
                    {
                        Console.WriteLine("Added new changed torrent: " + torrent);
                        changedTorrents.AppendLine(torrent);
                    }
                }
            }

            // write files
            File.WriteAllText(lastFilePath, newTorrents.ToString());
            File.WriteAllText(changedFilePath, changedTorrents.ToString());
            Environment.Exit(1);
        }
    }
}
