using NXStartCenter.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace NXStartCenter
{
    public class AppInfo
    {
        public string Version { get; } = AppMetadata.Version;
        public string AppDate { get; } = $"{DateTime.Now}";
        public string Author { get; } = AppMetadata.Author;
        public string SupportMail { get; } = AppMetadata.SupportMail;
        public string UpdateUrl { get; } = AppMetadata.UpdateUrl;
    }
}
