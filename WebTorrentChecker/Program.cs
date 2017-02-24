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
            TorrentChecker checker = new TorrentChecker();
            checker.CheckNewTorrents();
            Environment.Exit(1);
        }
    }
}
