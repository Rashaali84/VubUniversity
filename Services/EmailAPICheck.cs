using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using VubUniversity.Models;

namespace VubUniversity.Services
{
  
        public class EmailAPICheck : IEmailCheckRisky
        {
            private readonly IConfiguration Configuration;
            public EmailAPICheck(IConfiguration config)
            {
                Configuration = config;
            }

            public bool IsUntrusted(string emailAddress)
            {
            var emailRepApiKey = "7w06xw104v6aenrpco4ks8bq77qdvooeonzma2nsukvio5im";
                HttpWebRequest repEmailRequest = (HttpWebRequest)WebRequest.Create($"https://emailrep.io/{emailAddress}");
                repEmailRequest.Headers.Add("Cookie", $"{emailRepApiKey}");
                repEmailRequest.Headers.Add("User-Agent", "MyAppName");
                HttpWebResponse repEmailResponse = (HttpWebResponse)repEmailRequest.GetResponse();

                Stream newStream = repEmailResponse.GetResponseStream();
                var repEmail = new StreamReader(newStream).ReadToEnd();
                var reputation = JsonSerializer.Deserialize<Reputation>(repEmail);

                if (reputation.suspicious || reputation.details.blacklisted || reputation.details.spam || reputation.details.malicious_activity || reputation.details.malicious_activity_recent)
                    return true;

                return false;
            }
        }
        public interface IEmailCheckRisky
    {
            bool IsUntrusted(string input);
        }

    
}
