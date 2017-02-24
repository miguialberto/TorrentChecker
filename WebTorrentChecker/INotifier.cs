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
    interface INotifier
    {
        void NotifyCheckOk(NotificationResult result);
        void NotifyCheckError();
    }

    public class NotificationResult
    {
        public HashSet<string> providedTorrents { get; set; }
        public HashSet<string> newTorrents { get; set; }
    }

    public class MailNotifier : INotifier
    {
        NetworkCredential credentials { get; set; }
        SmtpClient client { get; set; }
        MailAddress from { get; set; }
        MailAddress to { get; set; }

        public MailNotifier(NetworkCredential credentials_, SmtpClient client_, MailAddress from_, MailAddress to_)
        {
            credentials = credentials_;
            client = client_;
            from = from_;
            to = to_;
        }

        public void NotifyCheckError()
        {
            throw new NotImplementedException();
        }

        public void NotifyCheckOk(NotificationResult result)
        {
            MailMessage mail = new MailMessage(from, to);
            mail.Subject = "Web Tracker";
            mail.Body = "[" + result.newTorrents.Count + "] new torrents found";
            client.EnableSsl = true;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.UseDefaultCredentials = false;
            client.Credentials = credentials;
            client.Send(mail);
        }
    }

    public class FileNotifier : INotifier
    {
        string providedFile { get; set; }
        string newsFile { get; set; }

        public FileNotifier(string providedFile_, string newsFile_)
        {
            providedFile = providedFile_;
            newsFile = newsFile_;
        }

        public void NotifyCheckError()
        {
            // Nothing
        }

        public void NotifyCheckOk(NotificationResult result)
        {
            WriteTorrentsToFile(providedFile, result.providedTorrents);
            WriteTorrentsToFile(newsFile, result.newTorrents);
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
            catch (Exception ex)
            {
                Console.WriteLine("Error writing file: " + ex.Message);
            }
        }
    }
}
