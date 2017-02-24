using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebTorrentChecker
{
    class WebProvider : IProvider
    {
        public string url;

        public WebProvider(string _url)
        {
            url = _url;
        }

        public List<string> GetTorrents()
        {
            List<string> torrents = new List<string>();
            try
            {
                HtmlWeb web = new HtmlWeb();
                HtmlDocument doc = web.Load(url);
                List<string> hrefTags = new List<string>();
                foreach (HtmlNode link in doc.DocumentNode.SelectNodes("//a[@href]"))
                {
                    HtmlAttribute att = link.Attributes["href"];
                    if (att.Value.EndsWith("torrent"))
                    {
                        torrents.Add(att.Value);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error getting torrents from [" + url + "]: " + ex.Message);
            }
            return torrents;
        }
    }
}
