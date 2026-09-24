using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using NXStartCenter.Model;
using NXStartCenter.View;

namespace NXStartCenter
{
    public static class UpdateService
    {
        private static readonly HttpClient _httpClient = new HttpClient();

        static UpdateService()
        {
            _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Startcenter-Updater");

            _httpClient.DefaultRequestHeaders.Add(
                "PRIVATE-TOKEN",
                Environment.GetEnvironmentVariable("GITLAB_TOKEN")
            );
        }

        public static async Task CheckUpdateOnStartup(Window owner)
        {
            string currentVersion = AppInfo.Version;

            string repoUrl = AppInfo.UpdateUrl;

            try
            {
                var release = await CheckForUpdateAsync(repoUrl, currentVersion);

                if (release == null)
                    return;

                release.Description = release.Description.Replace("<br>", "\n");

                var result = MessageBox.Show(
                    owner,
                    $"Eine neue Version ist verfügbar!\n\n" +
                    $"Aktuelle Version: {currentVersion}\n" +
                    $"Neue Version: {release.TagName}\n\n" +
                    $"{release.Description}\n\n" +
                    $"Jetzt herunterladen?",
                    "Update verfügbar",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Information
                );

                if (result != MessageBoxResult.Yes)
                    return;

                var progressWindow = new DownloadProgressWindow
                {
                    Owner = owner
                };

                progressWindow.Show();

                var progress = new Progress<double>(value =>
                {
                    progressWindow.SetProgress(value);
                });

                string path = await DownloadUpdateAsync(release, progress);

                progressWindow.Close();

                MessageBox.Show(
                    owner,
                    $"Das Update wurde erfolgreich heruntergeladen:\n\n{path}\n\n" +
                    "Bitte öffne die Datei manuell, wenn du das Update installieren möchtest.",
                    "Update heruntergeladen",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information
                );
            }
            catch (HttpRequestException) {}
            catch (Exception ex)
            {
                MessageBox.Show(
                    owner,
                    $"Fehler beim Update:\n{ex.Message}",
                    "Update Fehler",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
            }
        }

        private static async Task<GitLabRelease?> CheckForUpdateAsync(
            string repoApiUrl,
            string currentVersion)
        {
            string apiUrl = $"{repoApiUrl}/releases/permalink/latest";

            var response = await _httpClient.GetAsync(apiUrl);

            if (!response.IsSuccessStatusCode)
                return null;

            string json = await response.Content.ReadAsStringAsync();

            var release = JsonSerializer.Deserialize<GitLabRelease>(
                json,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

            if (release == null)
                return null;

            string latest = NormalizeVersion(release.TagName);
            string current = NormalizeVersion(currentVersion);

            if (Version.TryParse(latest, out var latestVersion) &&
                Version.TryParse(current, out var currentVersionParsed))
            {
                return latestVersion > currentVersionParsed
                    ? release
                    : null;
            }

            return null;
        }

        private static string NormalizeVersion(string version)
        {
            if (string.IsNullOrWhiteSpace(version))
                return string.Empty;

            version = version.Trim();

            if (version.StartsWith("v", StringComparison.OrdinalIgnoreCase))
                version = version.Substring(1);

            return version;
        }

        private static async Task<string> DownloadUpdateAsync(
            GitLabRelease release,
            IProgress<double> progress)
        {
            var asset = release.Assets?.Links?
                .FirstOrDefault(x =>
                    x.Name.EndsWith(".exe", StringComparison.OrdinalIgnoreCase));

            if (asset == null)
                throw new Exception("Keine EXE-Datei im Release gefunden.");

            string fileName = asset.Name;

            string downloadPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                "Downloads",
                fileName
            );

            string downloadUrl =
                !string.IsNullOrWhiteSpace(asset.DirectAssetUrl)
                    ? asset.DirectAssetUrl
                    : asset.Url;

            using var response = await _httpClient.GetAsync(
                downloadUrl,
                HttpCompletionOption.ResponseHeadersRead
            );

            response.EnsureSuccessStatusCode();

            long? totalBytes = response.Content.Headers.ContentLength;

            await using var stream = await response.Content.ReadAsStreamAsync();

            await using var fileStream = new FileStream(
                downloadPath,
                FileMode.Create,
                FileAccess.Write,
                FileShare.None
            );

            byte[] buffer = new byte[8192];

            long totalRead = 0;

            int read;

            while ((read = await stream.ReadAsync(buffer)) > 0)
            {
                await fileStream.WriteAsync(
                    buffer.AsMemory(0, read));

                totalRead += read;

                if (totalBytes.HasValue &&
                    totalBytes.Value > 0)
                {
                    double percent =
                        (double)totalRead /
                        totalBytes.Value *
                        100;

                    progress.Report(percent);
                }
            }

            progress.Report(100);

            return downloadPath;
        }
    }
}