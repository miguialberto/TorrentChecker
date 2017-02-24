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
    class TorrentChecker
    {
        public List<IProvider> providers { get; set; }
        public List<INotifier> notifiers { get; set; }

        public TorrentChecker()
        {
            providers = new List<IProvider>();
            notifiers = new List<INotifier>();
        }

        public void CheckNewTorrents()
        {
            LoadProviders();
            LoadNotifiers();
            HashSet<string> lastTorrents = ReadLastTorrents();
            HashSet<string> providedTorrents = ReadTorrentsFromProviders();
            HashSet<string> newTorrents = new HashSet<string>();
           
            foreach(string torrent in providedTorrents)
            {
                if (!lastTorrents.Contains(torrent))
                {
                    newTorrents.Add(torrent);
                }
            }

            // Notify 
            NotificationResult result = new NotificationResult();
            result.providedTorrents = providedTorrents;
            result.newTorrents = newTorrents;
            foreach(INotifier notifier in notifiers)
            {
                try
                {
                    notifier.NotifyCheckOk(result);
                }catch(Exception ex)
                {
                    Console.WriteLine("Error notifying: " + ex.Message);
                }
                
            }
        }


        private void LoadProviders()
        {
            string[] lines = File.ReadAllLines(Configurator.getInstance().GetPropertyValue(Configurator.PATH_PROVIDERS));
            foreach (string line in lines)
            {
                if (line != null && line.Length > 0 && !line.StartsWith("#"))
                {
                    providers.Add(new WebProvider(line));
                }
            }
        }

        private void LoadNotifiers()
        {
            MailAddress from = new MailAddress("miguialberto@hotmail.com", "Miguel");
            MailAddress to = new MailAddress("0bb1bb946b7cf2b13f33af9705ac651a2df904290924b896+0@nmamail.net", "WebTorrent");
            NetworkCredential creadentials = new NetworkCredential("miguialberto@hotmail.com", "HOTMAILrsnky2");
            SmtpClient client = new SmtpClient("smtp.live.com", 587);

            MailNotifier mn = new MailNotifier(creadentials, client, from, to);
            notifiers.Add(mn);

            MailAddress fromGmail = new MailAddress("mmp.pgp@gmail.com", "Miguel");
            MailAddress toGmail = new MailAddress("0bb1bb946b7cf2b13f33af9705ac651a2df904290924b896+0@nmamail.net", "WebTorrent");
            NetworkCredential creadentialsGmail = new NetworkCredential("mmp.pgp@gmail.com", "1qayxsw2");
            SmtpClient clientGmail = new SmtpClient("smtp.google.com", 587);

            MailNotifier mnGmail = new MailNotifier(creadentialsGmail, clientGmail, fromGmail, toGmail);
            notifiers.Add(mnGmail);

            FileNotifier fn = new FileNotifier(Configurator.getInstance().GetPropertyValue(Configurator.PATH_LASTS), 
                Configurator.getInstance().GetPropertyValue(Configurator.PATH_NEWS));
            notifiers.Add(fn);
        }

        private HashSet<string> ReadLastTorrents()
        {
            HashSet<string> lastTorrents = new HashSet<string>();
            string lastFilePath = Configurator.getInstance().GetPropertyValue(Configurator.PATH_LASTS);
            if (File.Exists(lastFilePath))
            {
                string[] lastFileLines = File.ReadAllLines(lastFilePath);
                foreach (string lastFileLine in lastFileLines)
                {
                    if (lastFileLine != null && lastFileLine.Length > 0 && !lastFileLine.StartsWith("#"))
                    {
                        lastTorrents.Add(lastFileLine);
                    }
                }
            }
            return lastTorrents;
        }

        private HashSet<string> ReadTorrentsFromProviders()
        {
            HashSet<string> providedTorrents = new HashSet<string>();
            foreach (IProvider provider in providers)
            {
                providedTorrents.UnionWith(provider.GetTorrents());
            }
            return providedTorrents;
        }

        private void WriteTorrentsToFile(string filePath, HashSet<string> torrents)
        {
            try
            {
                StringBuilder text = new StringBuilder();
                text.AppendLine("#" + DateTime.Now.ToString("dd/MM/yyyy-HH:mm:ss"));
                foreach (string value in torrents)
                {
                    text.AppendLine(value);
                }
                File.WriteAllText(filePath, text.ToString());
            }
            catch(Exception ex)
            {
                Console.WriteLine("Error writing file: " + ex.Message);
            }
        }
    }
}
