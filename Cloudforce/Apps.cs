using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cloudforce
{
    public class App
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Banner { get; set; }
        public string Version { get; set; }
        public List<DownloadLink> DownloadLinks { get; set; }
        public bool NeedAdmin { get; set; }
        public string Category { get; set; }
    }

    public class DownloadLink
    {
        public string Name { get; set; }
        public List<string> Links { get; set; }
        public string Exelocation { get; set; }
    }

    public class Root
    {
        public List<App> Apps { get; set; }
    }




}
