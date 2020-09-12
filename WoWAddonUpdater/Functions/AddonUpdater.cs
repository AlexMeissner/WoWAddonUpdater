using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Linq;
using System.IO.Compression;
using System.Collections.Generic;
using Newtonsoft.Json;
using WoWAddonUpdater.CurseForge;
using System.Collections.ObjectModel;
using WoWAddonUpdater.ViewModels;

namespace WoWAddonUpdater.Functions
{
    public class AddonUpdater
    {
        private readonly string AddonBaseDirectory = Path.Combine(Properties.Settings.Default.BaseDirectory, @"Interface\Addons");
        private readonly string BaseAddress = "https://addons-ecs.forgesvc.net";

        public ObservableCollection<AddonViewModel> GetViewModel()
        {
            ObservableCollection<AddonViewModel> addons = new ObservableCollection<AddonViewModel>
            {
                new AddonViewModel() { Name = "Big Wigs", Blacklisted = true, Icon = "https://upload.wikimedia.org/wikipedia/commons/thumb/e/eb/WoW_icon.svg/1200px-WoW_icon.svg.png" },
                new AddonViewModel() { Name = "ElvUI", Blacklisted = false, Icon = "https://upload.wikimedia.org/wikipedia/commons/thumb/e/eb/WoW_icon.svg/1200px-WoW_icon.svg.png" },
                new AddonViewModel() { Name = "Details", Blacklisted = false, Icon = "https://upload.wikimedia.org/wikipedia/commons/thumb/e/eb/WoW_icon.svg/1200px-WoW_icon.svg.png" },
                new AddonViewModel() { Name = "WeakAuras", Blacklisted = true, Icon = "https://upload.wikimedia.org/wikipedia/commons/thumb/e/eb/WoW_icon.svg/1200px-WoW_icon.svg.png" }
            };

            return addons;
        }

        public HashSet<CurseData> Refresh()
        {
            var addonDirectories = GetAddonDirectoryNames();
            var curseData = GetCurseForgeData();

            return new HashSet<CurseData>(curseData.Where(f => addonDirectories.Contains(f.Name)));
        }

        public void UpdateAddon(string downloadURL)
        {
            using (WebClient webClient = new WebClient())
            {
                try
                {
                    string temporaryZipPath = Path.Combine(AddonBaseDirectory, @"\temp.zip");
                    webClient.DownloadFile(downloadURL, temporaryZipPath);

                    using (ZipArchive archive = ZipFile.OpenRead(temporaryZipPath))
                    {
                        foreach (ZipArchiveEntry zipEntry in archive.Entries)
                        {
                            string zipEntryPath = Path.GetFullPath(Path.Combine(AddonBaseDirectory, zipEntry.FullName));

                            if (zipEntry.Name.Length == 0)
                            {
                                Directory.CreateDirectory(zipEntryPath);
                            }
                            else
                            {
                                zipEntry.ExtractToFile(zipEntryPath, true);
                            }
                        }
                    }

                    File.Delete(temporaryZipPath);
                }
                catch (Exception exception)
                {
                    Logger.Exception(exception);
                }
            }
        }

        private HashSet<string> GetAddonDirectoryNames()
        {
            HashSet<string> addonDirectories = new HashSet<string>();

            try
            {
                if (Directory.Exists(AddonBaseDirectory))
                {
                    foreach (string addonDirectory in Directory.GetDirectories(AddonBaseDirectory))
                    {
                        if (Directory.GetFiles(addonDirectory, "*.toc").Length > 0)
                        {
                            addonDirectories.Add(Path.GetFileName(addonDirectory));
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                Logger.Exception(exception);
            }

            return addonDirectories;
        }

        private HashSet<CurseData> GetCurseForgeData()
        {
            HashSet<CurseData> data = new HashSet<CurseData>();

            using (HttpClient restAPI = new HttpClient())
            {
                restAPI.BaseAddress = new Uri(BaseAddress);
                restAPI.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                const uint pageSize = 10000;
                uint startIndex = 0;

                while (true)
                {
                    string request = "api/v2/addon/search?gameId=1&pageSize=" + pageSize + "&index=" + startIndex;

                    HttpResponseMessage response = restAPI.GetAsync(request).Result;

                    if (!response.IsSuccessStatusCode)
                    {
                        break;
                    }

                    try
                    {
                        string json = response.Content.ReadAsStringAsync().Result;
                        List<AddonData> addons = JsonConvert.DeserializeObject<List<AddonData>>(json);

                        foreach (var addonData in addons)
                        {
                            data.Add(new CurseData()
                            {
                                ID = addonData.Id,
                                Name = GetAddonName(addonData)
                            });
                        }
                    }
                    catch (Exception exception)
                    {
                        Logger.Exception(exception);
                    }

                    startIndex += pageSize;
                }
            }

            return data;
        }

        private string GetAddonName(AddonData addonData)
        {
            try
            {
                for (int index = addonData.LatestFiles.Count - 1; index >= 0; --index)
                {
                    if (addonData.LatestFiles[index].ReleaseType == 1)
                    {
                        foreach (var module in addonData.LatestFiles[index].Modules)
                        {
                            if (module.Type == 3)
                            {
                                return module.Foldername;
                            }
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                Logger.Exception(exception);
            }

            return string.Empty;
        }
    }
}