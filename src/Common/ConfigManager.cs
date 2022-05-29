using Newtonsoft.Json;
using System.IO;
using System;
using System.Collections.Generic;
using UnityEngine;
using HarmonyLib;
using System.Reflection;
using Newtonsoft.Json.Linq;
using System.Linq;
using Newtonsoft.Json.Serialization;
using Common;

namespace Config
{
    public class TranslationResolver : DefaultContractResolver // todo: always allow English language: https://stackoverflow.com/questions/33155458/json-deserialize-from-legacy-property-names
    {
        protected override string ResolvePropertyName(string propertyName)
        {
            if (Strings.TryGet($"{Helpers.ModName}.PROPERTY.{propertyName}", out StringEntry result) && result != "")
            {
                Helpers.PrintDebug($"resolving {Helpers.ModName}.PROPERTY.{propertyName} as {result}");
                return result;
            }

            Helpers.PrintDebug($"resolving {Helpers.ModName}.PROPERTY.{propertyName} as {propertyName}");
            return propertyName;
        }

        protected override string ResolveDictionaryKey(string dictionaryKey)
        {
            return dictionaryKey;
        }
    }

    public class JsonManager
    {
        public JsonSerializer Serializer = JsonSerializer.CreateDefault(new JsonSerializerSettings
        {
            Formatting = Formatting.Indented,
            NullValueHandling = NullValueHandling.Ignore,
            ObjectCreationHandling = ObjectCreationHandling.Replace,
            //Converters = new List<JsonConverter>() { new ApiErrorConverter() },
            ContractResolver = new TranslationResolver(),
        });

        public T Deserialize<T>(string path)
        {
            T result;

            using (StreamReader streamReader = new StreamReader(path))
            {
                using (JsonTextReader jsonReader = new JsonTextReader(streamReader))
                {
                    result = this.Serializer.Deserialize<T>(jsonReader);

                    jsonReader.Close();
                }

                streamReader.Close();
            }

            return result;
        }
        public void Serialize<T>(T value, string path)
        {
            using (StreamWriter streamReader = new StreamWriter(path))
            {
                using (JsonTextWriter jsonReader = new JsonTextWriter(streamReader))
                {
                    this.Serializer.Serialize(jsonReader, value);

                    jsonReader.Close();
                }

                streamReader.Close();
            }
        }
    }

    public class JsonFileManager
    {
        private readonly JsonManager _jsonManager;


        public JsonManager GetJsonManager()
        {
            return _jsonManager;
        }

        public JsonFileManager(JsonManager jsonManager)
        {
            this._jsonManager = jsonManager;
        }


        public bool TryLoadConfiguration<T>(string path, out T state)
        {
            try
            {
                state = _jsonManager.Deserialize<T>(path);
                return true;
            }
            catch (Exception ex)
            {
                Debug.LogWarning(ex);
                Debug.LogWarning("Can't load configuration!");
                PostBootDialog.ErrorList.Add("Error in config file: " + ex.Message);

                state = (T)Activator.CreateInstance(typeof(T));

                return false;
            }
        }

        public bool TrySaveConfiguration<T>(string path, T state)
        {
            try
            {
                _jsonManager.Serialize<T>(state, path);
                return true;
            }
            catch (Exception ex)
            {
                const string Message = "Can't save configuration!";

                Debug.LogWarning(ex);
                Debug.LogWarning(Message);

                return false;
            }
        }
    }

    public class Manager<T> where T : class, new()
    {
        public readonly string StateFilePath;

        public readonly JsonFileManager JsonLoader;

        private T _state;

        public Func<T, bool> updateCallback = null;

        public System.Action<T> loadedCallback = null;

        public T State
        {
            get
            {
                if (_state != null)
                {
                    return _state;
                }
                Debug.Log("Loading: " + this.StateFilePath);

                if (!File.Exists(this.StateFilePath))
                {
                    Debug.Log(this.StateFilePath + " not found. Creating a default config file...");
                    EnsureDirectoryExists(new FileInfo(this.StateFilePath).Directory.FullName);

                    JsonLoader.TrySaveConfiguration(this.StateFilePath, (T)Activator.CreateInstance(typeof(T)));
                }
                JsonLoader.TryLoadConfiguration(this.StateFilePath, out _state);

                if (loadedCallback != null) loadedCallback(_state);

                return _state;
            }

            private set
            {
                _state = value;
            }
        }


        public bool TryReloadConfiguratorState()
        {
            T state;
            if (JsonLoader.TryLoadConfiguration(this.StateFilePath, out state))
            {
                State = state;
                return true;
            }

            return false;
        }

        public bool TrySaveConfigurationState()
        {
            if (_state != null)
                return JsonLoader.TrySaveConfiguration(this.StateFilePath, _state);

            return false;
        }

        public bool TrySaveConfigurationState(T state)
        {
            _state = state;
            if (_state != null)
                return JsonLoader.TrySaveConfiguration(this.StateFilePath, _state);

            return false;
        }

        /// <summary>
        /// if not isAbsolute then path is the mods name
        /// </summary>
        public Manager(string path, bool isAbsolute, Func<T, bool> updateCallback = null, System.Action<T> loadedCallback = null)
        {
            this.updateCallback = updateCallback;
            this.loadedCallback = loadedCallback;

            bool errorFlag = false;
            string resultPath = null;

            if (isAbsolute)
            {
                resultPath = path;
                EnsureDirectoryExists(new FileInfo(resultPath).Directory.FullName);
                errorFlag = !Directory.Exists(new FileInfo(resultPath).Directory.FullName);
            }

            if (!isAbsolute || errorFlag)
            {
                resultPath = GetFallBackPath(path);
                EnsureDirectoryExists(new FileInfo(path).Directory.FullName);
            }

            this.StateFilePath = resultPath;
            this.JsonLoader = new JsonFileManager(new JsonManager());

            UpdateVersion();
        }

        public void UpdateVersion()
        {
            try
            {
                object newObj = Activator.CreateInstance(typeof(T));
                int newVersion = (int)typeof(T).GetProperty("version").GetValue(newObj, null);
                int savedVersion = (int)typeof(T).GetProperty("version").GetValue(State, null);

                if (savedVersion != 0 && newVersion != 0)
                {
                    if (savedVersion != newVersion)
                    {
                        Debug.Log("Updating version...");
                        bool shouldSave = true;
                        if (updateCallback != null) shouldSave = updateCallback(State);
                        State.GetType().GetProperty("version").SetValue(State, newVersion, null);
                        if (shouldSave) this.TrySaveConfigurationState();
                    }
                }
            }
            catch (Exception e)
            {
                if (this.StateFilePath != null)
                    Debug.Log("Config.Manager could not check version of: " + Path.GetFileName(this.StateFilePath) + "\n" + e.Message);
                return;
            }
        }

        /// <summary>
        /// name: file name without extension
        /// returns file-path to save config, located in root mod folder; NOT TESTED
        /// </summary>
        private static string GetKleiDocs(string name)
        {
            //return System.getProperty("user.home") + Path.DirectorySeparatorChar + "Documents" + Path.DirectorySeparatorChar
            return Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + Path.DirectorySeparatorChar
            + "Klei" + Path.DirectorySeparatorChar
            + "OxygenNotIncluded" + Path.DirectorySeparatorChar
            + "mods" + Path.DirectorySeparatorChar
            + name + ".json";
        }

        public static void EnsureDirectoryExists(string path)
        {
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);
        }

        public static string GetFallBackPath(string path)
        {
            string name = Path.GetFileNameWithoutExtension(Path.GetFileName(path));
            return "Mods" + Path.DirectorySeparatorChar + name + Path.DirectorySeparatorChar + "Config" + Path.DirectorySeparatorChar + name + ".json";
        }
    }

    public class PathHelper
    {
        //https://stackoverflow.com/questions/52797/how-do-i-get-the-path-of-the-assembly-the-code-is-in
        public static string AssemblyDirectoryOld
        {
            get
            {
                string codeBase = Assembly.GetExecutingAssembly().CodeBase;
                UriBuilder uri = new UriBuilder(codeBase);
                string path = Uri.UnescapeDataString(uri.Path);
                return Path.GetDirectoryName(path);
            }
        }

        public static string AssemblyDirectory
        {
            get => Directory.GetParent(Assembly.GetExecutingAssembly().Location)?.FullName;//GetCallingAssembly
        }

        public static string ModsDirectory
        {
            get
            {
                return Path.Combine(Util.RootFolder(), "mods");
                //return System.IO.Directory.GetParent(AssemblyDirectory).Parent.FullName;
            }
        }

        /// <summary>
        /// returns absolute file-path
        /// </summary>
        public static string CreatePath(string name)
        {
            return Path.Combine(ModsDirectory, name + ".json");
        }
    }

}