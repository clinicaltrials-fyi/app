using System.Text.Json;
using System.Text.Json.Serialization;
using ClinicalTrials.Core;

namespace ClinicalTrials.Apps
{
    public class DeviceProfileUtility : IDataUtility<QueryInfo>
    {
        private const string extension = ".userdata.json";

        public Task<string> GetData(string key)
        {
            throw new NotImplementedException();
        }

        public async Task<List<QueryInfo>> GetAllItems()
        {
            var queryInfos = new List<QueryInfo>();

            var keys = await GetAllKeys();
            foreach (var key in keys)
            {
                var queryInfo = await Load(key);
                queryInfos.Add(queryInfo);
            }

            return queryInfos;
        }

        public async Task<List<string>> GetAllKeys()
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(FileSystem.Current.AppDataDirectory);
            var profileNames = new List<string>();
            await Task.Run(() =>
            {
                var files = directoryInfo.GetFiles("*" + extension);
                foreach (var file in files)
                {
                    var keyNameEnd = file.Name.IndexOf(extension);
                    if (keyNameEnd != -1)
                    {
                        var key = file.Name.Substring(0, keyNameEnd);
                        profileNames.Add(key);
                    }
                }
            });

            return profileNames.OrderBy(p => p).ToList();
        }

        public void DeleteItem(string key)
        {
            var profileFile = GetFilename(key);
            File.Delete(profileFile);
        }

        public void RenameItem(string oldName, string newName)
        {
            var oldFile = GetFilename(oldName);
            var newFile = GetFilename(newName);
            if (!File.Exists(newName))
            {
                if (File.Exists(oldFile))
                {
                    File.Move(oldFile, newFile);
                }
                else
                {
                    throw new InvalidDataException($"old file '{oldFile}' is missing.");
                }
            }

            throw new InvalidOperationException($"new file '{newFile}' already exists.");
        }

        public async Task<string> FindProfileName(string baseName)
        {
            var keys = await GetAllKeys();
            int i = 1;
            string newKeyName = baseName + i.ToString();

            while (i < 10000)
            {
                if (!keys.Contains(newKeyName))
                {
                    return newKeyName;
                }
                else
                {
                    newKeyName = baseName + i.ToString();
                }

                i++;
            }

            throw new InvalidOperationException("cannot have more than 10,000 names starting with " + baseName);
        }

        public async Task<QueryInfo> Load(string key)
        {
            ArgumentNullException.ThrowIfNull(key);

            string targetFile = GetFilename(key);
            var json = await File.ReadAllTextAsync(targetFile);

            if (json != null)
            {
                var options = new JsonSerializerOptions()
                {
                    Converters =
                            {
                                new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
                            }
                };

                var loadedData = QueryInfo.LoadFromJson(json, options);
                return loadedData;
            }

            throw new InvalidDataException($"key '{key}' has invalid data or is missing.");
        }

        public async Task Save(string? key, QueryInfo? data)
        {
            var options = new JsonSerializerOptions()
            {
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault,
                IgnoreReadOnlyProperties = true,
                WriteIndented = true,
                Converters =
                {
                    new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
                }
            };

            if (data is not null)
            {
                var familyDataJson = JsonSerializer.Serialize(data, options);

                await Save(key, familyDataJson);
            }
        }

        public async Task Save(string? key, string json)
        {
            if (key is not null)
            {
                string targetFile = GetFilename(key);
                await File.WriteAllTextAsync(targetFile, json);
            }
        }

        public string GetFilename(string key)
        {
            return Path.Combine(FileSystem.Current.AppDataDirectory, key + extension);
        }


        public Task ClearAll()
        {
            throw new NotImplementedException();
        }
    }
}
