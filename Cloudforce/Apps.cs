using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cloudforce
{
    internal class Apps
    {
    }


    public class App
    {
        public string Appname { get; set; }
        public string AppLink { get; set; }
        public string DownloadServer { get; set; }
        public List<DownloadMain> DownloadMain { get; set; }
        public string Admin { get; set; }
        public string Category { get; set; }
        public string Version { get; set; }
        public string AppBanner { get; set; }
        public string LaunchLocation { get; set; }
    }

    public class DownloadMain
    {
        [JsonProperty("Setup Exe")]
        public string SetupExe { get; set; }

        [JsonProperty("Setup MSI")]
        public string SetupMSI { get; set; }

        [JsonProperty("Portable Setup")]
        public string PortableSetup { get; set; }

        [JsonProperty("Portable Zip")]
        public string PortableZip { get; set; }

        [JsonProperty("Portable Exe")]
        public string PortableExe { get; set; }
    }

    public class Root
    {
        public List<App> Apps { get; set; }
    }



}
