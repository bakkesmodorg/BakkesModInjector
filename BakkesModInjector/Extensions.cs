using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace BakkesModInjector
{
    public class WebDownload : WebClient
    {
        /// <summary>
        /// Time in milliseconds
        /// </summary>
        public int Timeout { get; set; }

        public WebDownload() : this(60000) { }

        public WebDownload(int timeout)
        {
            this.Timeout = timeout;
        }

        protected override WebRequest GetWebRequest(Uri address)
        {
            WebRequest.DefaultWebProxy = null;
            this.Proxy = null;
            var request = base.GetWebRequest(address);
            if (request != null)
            {
                request.Timeout = this.Timeout;
                request.Proxy = null;
            }
            return request;
        }
    }
    public static class ZipArchiveExtensions
    {
        public static Boolean BakkesModUpdated = false;
        public static void ExtractToDirectory(this ZipArchive archive, string destinationDirectoryName, bool overwrite)
        {
            BakkesModUpdated = false;
            if (!overwrite)
            {
                archive.ExtractToDirectory(destinationDirectoryName);
                return;
            }
            foreach (ZipArchiveEntry file in archive.Entries)
            {
                string completeFileName = Path.Combine(destinationDirectoryName, file.FullName);
                if (file.Name == "")
                {// Assuming Empty for Directory
                    Directory.CreateDirectory(Path.GetDirectoryName(completeFileName));
                    continue;
                }
                //skip cfgs
                if (completeFileName.ToLower().EndsWith(".cfg") || completeFileName.ToLower().EndsWith(".json"))
                {
                    if (File.Exists(completeFileName))
                        continue;
                }
                else if(completeFileName.ToLower().EndsWith("bakkesmod.exe"))
                {
                    BakkesModUpdated = true;
                }
                file.ExtractToFile(completeFileName, true);
            }
        }
    }
}
