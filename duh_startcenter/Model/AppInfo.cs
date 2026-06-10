using NXStartCenter.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace NXStartCenter.Model
{
    public class AppInfo
    {
        private static readonly string CurrentDate = DateTime.Now.ToShortDateString();

        public static string Version { get; } = AppMetadata.Version;
        public static string AppDate { get; } = $"{CurrentDate}";
        public static string Author { get; } = AppMetadata.Author;
        public static string SupportMail { get; } = AppMetadata.SupportMail;
        public static string UpdateUrl { get; } = AppMetadata.UpdateUrl;
    }
}
