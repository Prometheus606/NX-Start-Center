using NXStartCenter.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace NXStartCenter
{
    public class AppInfo
    {
        public static string Version { get; } = AppMetadata.Version;
        public static string AppDate { get; } = $"{DateTime.Now}";
        public static string Author { get; } = AppMetadata.Author;
        public static string SupportMail { get; } = AppMetadata.SupportMail;
        public static string UpdateUrl { get; } = AppMetadata.UpdateUrl;
    }
}
