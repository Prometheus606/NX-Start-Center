using System;
using System.Collections.Generic;
using System.Text;

namespace NXStartCenter
{
    public class GitHubRelease
    {
        public string tag_name { get; set; } = "";
        public string name { get; set; } = "";
        public string body { get; set; } = "";
        public GitHubAsset[] assets { get; set; } = [];
    }

    public class GitHubAsset
    {
        public string browser_download_url { get; set; } = "";
    }
}
