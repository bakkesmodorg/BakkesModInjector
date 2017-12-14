using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace BakkesModInjector
{
    public enum UpdateResult
    {
        ServerOffline,
        UpToDate,
        UpdateAvailable
    }

    class Updater
    {
        private static readonly string UPDATE_URL = "http://149.210.150.107/updater/";
        private string _currentVersion;
        private JObject latestResult = null;
        public Updater(string currentVersion)
        {
            _currentVersion = currentVersion;
        }

        public UpdateResult CheckForUpdates()
        {
            try
            {
                ServicePointManager.DefaultConnectionLimit = 5;
                ServicePointManager.Expect100Continue = false;

                using (WebClient wc = new WebDownload(3000))
                {
                    //wc.DownloadFile(UPDATE_URL + _currentVersion + "/", "test.txt");
                    wc.Proxy = null;
                    wc.Headers.Add("user-agent", "BakkesMod Updater (2.0)");
                    string fullUrl = UPDATE_URL + _currentVersion + "/";
                    var json = wc.DownloadString(fullUrl);
                    
                    latestResult = JObject.Parse(json);
                    if((bool)latestResult["update_required"])
                    {
                        return UpdateResult.UpdateAvailable;
                    }
                    return UpdateResult.UpToDate;
                };
            }
            catch (Exception e) { }
            return UpdateResult.ServerOffline;
        }

        public String GetUpdateMessage()
        {
            return (String)((JObject)latestResult["update_info"])["message"];
        }

        public String GetUpdateURL()
        {
            return (String)((JObject)latestResult["update_info"])["download_url"];
        }

        public bool IsBlocked()
        {
            return (latestResult["blocked"] != null && (bool)latestResult["blocked"]);
        }

    }
}
