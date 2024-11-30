// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace VirtualCamProxy
{
    public class JsonStorage<T> where T : class, new()
    {
        public string FileName { get; set; }
        public bool AutoCreate { get; set; } = false;

        public T Storage { get; set; } = new T();

        public JsonStorage()
        {
            FileName = string.Empty;
        }

        public JsonStorage(string file, bool autoCreateFile = false)
        {
            FileName = file;
            AutoCreate = autoCreateFile;

            if (!Load())
                throw new FileNotFoundException($"Can not load file {file}");
        }

        public bool Load()
        {
            if (string.IsNullOrEmpty(FileName))
                return false;

            if (File.Exists(FileName))
            {
                var json = JsonConvert.DeserializeObject<T>(File.ReadAllText(FileName), new JsonSerializerSettings()
                {
                    TypeNameHandling = TypeNameHandling.Auto
                });

                if (json != null)
                    Storage = json;
            }
            else if (AutoCreate)
            {
                Save();
            }
            else
                return false;

            return true;
        }

        public bool Save()
        {
            return Save(FileName);
        }

        public bool Save(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
                return false;

            File.WriteAllText(fileName,
                JsonConvert.SerializeObject(Storage, Formatting.Indented, new JsonSerializerSettings()
                {
                    TypeNameHandling = TypeNameHandling.Auto,
                    Converters = new List<JsonConverter>
                    {
                            new Newtonsoft.Json.Converters.StringEnumConverter()
                    }
                }));

            return true;
        }
    }
}
