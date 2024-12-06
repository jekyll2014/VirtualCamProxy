using Newtonsoft.Json;

namespace VirtualCamProxy.Settings;

public class FileCameraSettings
{
    public string FolderName { get; set; } = string.Empty;
    public List<string> FileNames { get; set; } = new List<string>();
    public FileContainer Container { get; set; } = FileContainer.Folder;
    [JsonIgnore]
    public List<string> Files
    {
        get
        {
            if (Container == FileContainer.FileList)
                return FileNames;

            if (string.IsNullOrEmpty(FolderName))
                return new List<string>();

            return Directory.GetFiles(FolderName).ToList();
        }
    }
}