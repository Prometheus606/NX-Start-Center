using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;

namespace NXStartCenter
{
    public static class UpdateService
    {
        private static readonly HttpClient _httpClient = new HttpClient();

        static UpdateService()
        {
            _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Startcenter-Updater");
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

                var result = MessageBox.Show(
                    owner,
                    $"Eine neue Version ist verfügbar!\n\n" +
                    $"Aktuelle Version: {currentVersion}\n" +
                    $"Neue Version: {release.tag_name}\n\n" +
                    $"{release.body}\n\n" +
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

        private static async Task<GitHubRelease?> CheckForUpdateAsync(
            string repoApiUrl,
            string currentVersion)
        {
            string apiUrl = $"{repoApiUrl}/releases/latest";

            var response = await _httpClient.GetAsync(apiUrl);

            if (!response.IsSuccessStatusCode)
                return null;

            string json = await response.Content.ReadAsStringAsync();

            var release = JsonSerializer.Deserialize<GitHubRelease>(json);

            if (release == null)
                return null;

            string latest = release.tag_name.ToLower().Replace("v", "");
            string current = currentVersion.ToLower().Replace("v", "");

            if (Version.TryParse(latest, out var latestVersion) &&
                Version.TryParse(current, out var currentVer))
            {
                return latestVersion > currentVer
                    ? release
                    : null;
            }

            return null;
        }

        private static async Task<string> DownloadUpdateAsync(
            GitHubRelease release,
            IProgress<double> progress)
        {
            var asset = release.assets.FirstOrDefault();

            if (asset == null)
                throw new Exception("Keine Download-Datei gefunden.");

            string downloadPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                "Downloads",
                $"startcenter-installer-{release.tag_name}.exe"
            );

            using var response = await _httpClient.GetAsync(
                asset.browser_download_url,
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
                await fileStream.WriteAsync(buffer.AsMemory(0, read));

                totalRead += read;

                if (totalBytes.HasValue)
                {
                    double percent =
                        (double)totalRead /
                        totalBytes.Value * 100;

                    progress.Report(percent);
                }
            }

            progress.Report(100);

            return downloadPath;
        }
    }
}