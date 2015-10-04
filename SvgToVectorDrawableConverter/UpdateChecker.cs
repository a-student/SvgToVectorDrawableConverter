using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Win32;
using Newtonsoft.Json.Linq;
using SvgToVectorDrawableConverter.Utils;

namespace SvgToVectorDrawableConverter
{
    class UpdateChecker : IDisposable
    {
        private readonly RegistryKey _registryKey;

        public UpdateChecker()
        {
            _registryKey = Registry.CurrentUser.CreateSubKey(@"Software\SvgToVectorDrawableConverter");
        }

        public void Dispose()
        {
            _registryKey.Dispose();
        }

        private DateTime? GetDateTime(string name)
        {
            var binary = _registryKey.GetValue(name) as string;
            return binary == null ? null : (DateTime?)DateTime.FromBinary(long.Parse(binary));
        }

        private void SetDateTime(string name, DateTime? value)
        {
            if (!value.HasValue)
            {
                _registryKey.DeleteValue(name, false);
                return;
            }
            _registryKey.SetValue(name, value.Value.ToBinary().ToString());
        }

        private readonly DateTime _currentVersion = App.GetBuildDate();

        private DateTime? LatestVersion
        {
            get { return GetDateTime("LatestAppVersion"); }
            set { SetDateTime("LatestAppVersion", value); }
        }

        private DateTime? LastCheck
        {
            get { return GetDateTime("LastCheckForUpdate"); }
            set { SetDateTime("LastCheckForUpdate", value); }
        }

        private bool CheckLocal()
        {
            var latestVersion = LatestVersion;
            if (latestVersion != null && latestVersion - _currentVersion > TimeSpan.FromDays(3))
            {
                Console.WriteLine("New converter version is available! Please, update from https://github.com/a-student/SvgToVectorDrawableConverter/releases");
                return true;
            }
            return false;
        }

        public async Task CheckForUpdateAsync()
        {
            Console.WriteLine();
            if (CheckLocal())
            {
                return;
            }
            var lastCheck = LastCheck;
            if (lastCheck != null && DateTime.UtcNow - lastCheck < TimeSpan.FromDays(1))
            {
                return;
            }
            Console.Write("Checking for the latest converter version (specify --no-update-check to disable)... ");
            var latestVersion = await CheckServerAsync();
            if (latestVersion == null)
            {
                Console.WriteLine("Error :(");
                return;
            }
            LatestVersion = latestVersion;
            LastCheck = DateTime.UtcNow;
            if (!CheckLocal())
            {
                Console.WriteLine("Your converter is quite fresh.");
            }
        }

        private static async Task<DateTime?> CheckServerAsync()
        {
            try
            {
                var client = new HttpClient();
                client.DefaultRequestHeaders.Add("User-Agent", "SvgToVectorDrawableConverter Update Checker 1.0");
                using (client)
                {
                    var response = await client.GetStringAsync("https://api.github.com/repos/a-student/SvgToVectorDrawableConverter/releases/latest");
                    var jObject = JObject.Parse(response);
                    return (DateTime)jObject["published_at"];
                }
            }
            catch
            {
                return null;
            }
        }
    }
}
