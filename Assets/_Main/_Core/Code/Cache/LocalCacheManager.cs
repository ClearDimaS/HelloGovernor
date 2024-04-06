using System;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

public class LocalCacheManager 
{
    public string UserID
    {
        get
        {
            if (useEmptyID)
                return "";

            InitID();

            return userID.UserId;
        }
        set
        {
            useEmptyID = true;

            InitID();

            var newValue = value.Replace(" ", "");
            newValue = newValue.Replace($"+", "");
            userID.UserId = newValue;
            Save<UserID>(userID, true);

            useEmptyID = false;
        }
    }

    public string SavePath => Application.persistentDataPath + $"/{Application.productName}";

    private UserID userID;
    private bool useEmptyID;

    private const string jsonFileExtension = "json";

    // single file

    public T Load<T>()
    {
        string filePath = GetFilePath<T>();

        if (File.Exists(filePath) == true)
        {
            try
            {
                var jObject = JObject.Parse(File.ReadAllText(filePath));
                T objectData = jObject.ToObject<T>();
                return objectData;
            }
            catch (Exception ex)
            {
                return default;
            }
        }
        return default;
    }

    public bool Save<T>(T data, bool allowOverwrite)
    {
        string filePath = GetFilePath<T>();
        return SaveAtPath(data, filePath, allowOverwrite);
    }

    public bool FileExists<T>()
    {
        string filePath = GetFilePath<T>();
        return File.Exists(filePath);
    }

    // multi file

    public T LoadFromTypedFolder<T>(string filename)
    {
        string filePath = GetFileInFolderPath<T>(filename);

        if (File.Exists(filePath) == true)
        {
            try
            {
                var jObject = JObject.Parse(File.ReadAllText(filePath));
                T objectData = jObject.ToObject<T>();
                return objectData;
            }
            catch
            {
                return default;
            }
        }
        return default;
    }

    public string[] ListTypedFolder<T>()
    {
        string folderPath = GetTypedFolderPath<T>();
        string[] result;
        if (Directory.Exists(folderPath) == false)
        {
            result = new string[] { };
        }
        else
        {
            result = Directory.GetFiles(folderPath);
        }
        for (int i = 0; i < result.Length; i++)
        {
            result[i] = Path.GetFileNameWithoutExtension(result[i]);
        }
        return result;
    }

    public bool FileExistsInTypedFolder<T>(string fileName)
    {
        foreach (string fileInFolder in ListTypedFolder<T>())
        {
            if (fileInFolder == fileName)
            {
                return true;
            }
        }
        return false;
    }

    public bool SaveInTypedFolder<T>(T data, string filename, bool allowOverwrite)
    {
        return SaveInTypedFolderT<T>(data, filename, allowOverwrite);   
    }

    public bool SaveInTypedFolderT<T>(object data, string filename, bool allowOverwrite)
    {
        string filePath = GetFileInFolderPath<T>(filename);

        return SaveAtPathT<T>(data, filePath, allowOverwrite);
    }

    public void Clear<T>()
    {
        var directoryPath = GetFolderPath();
        if (!Directory.Exists(directoryPath))
            return;
        string filePath = GetFilePath<T>();
            
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }
    }

    public void Clear(string className)
    {
        string filePath = GetFilePath(className);
        if (File.Exists(filePath) == true)
        {
            File.Delete(filePath);
        }
    }

    public void DeleteInFolderPath<T>(string filename)
    {
        string folderPath = GetTypedFolderPath<T>();
        if (!Directory.Exists(folderPath))
            return;
        Directory.Delete(GetTypedFolderFilePath<T>(filename));
    }

    public string LoadRaw(string dir)
    {
        var path = dir;
        if (File.Exists(path))
        {
            return File.ReadAllText(path);
        }
        return String.Empty;
    }

    public void ClearRaw(string dir)
    {
        var path = dir;
            
        if (File.Exists(path))
        {
            File.Delete(path);
        }
    }

    public string SaveRaw(string dir, string text)
    {
        File.WriteAllText(dir, text);
        return dir;
    }

    // private helpers

    private bool SaveAtPath<T>(T data, string filePath, bool allowOverwrite)
    {
        return SaveAtPathT<T>(data, filePath, allowOverwrite);  
    }

    private bool SaveAtPathT<T>(object data, string filePath, bool allowOverwrite)
    {
        if (File.Exists(filePath) == true && allowOverwrite == true)
        {
            File.Delete(filePath);
        }

        if (File.Exists(filePath) == false)
        {
            string textData = JsonConvert.SerializeObject(data, Formatting.Indented,
                new JsonSerializerSettings
                {
                    PreserveReferencesHandling = PreserveReferencesHandling.All
                });
            string folderPath = GetTypedFolderPath<T>();
            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            File.WriteAllText(filePath, textData);
        }
        else
        {
            Debug.LogError($"could not save data: {typeof(T).Name}");
            return false;
        }

        return true;
    }

    private string GetFileInFolderPath<T>(string filename)
    {
        string folderPath = GetTypedFolderPath<T>();
        if(!Directory.Exists(folderPath))
            Directory.CreateDirectory(folderPath);
        return GetTypedFolderFilePath<T>(filename);
    }

    private string GetTypedFolderPath<T>()
    {
        return Path.Combine(SavePath, UserID, typeof(T).FullName);
    }

    private string GetTypedFolderFilePath<T>(string fileName)
    {
        return Path.Combine(SavePath, UserID, typeof(T).FullName, fileName + "." + jsonFileExtension);
    }

    private string GetFolderPath() 
    {
        return Path.Combine(SavePath, UserID);
    }

    private string GetFilePath<T>()
    {
        return GetFilePath(typeof(T).FullName);
    }

    private string GetFilePath(string fileName)
    {
        return Path.Combine(SavePath, UserID, fileName + "." + jsonFileExtension);
    }

    private void InitID()
    {
        if (userID == null)
        {
            useEmptyID = true;

            userID = new UserID();
            if (FileExists<UserID>())
            {
                userID = Load<UserID>();
            }
            else
            {

                Save<UserID>(userID, true);
            }

            useEmptyID = false;
        }
    }

    public void ClearAll()
    {
        var path = Path.Combine(SavePath, userID == null ? "" : userID.UserId);
            
        if (Directory.Exists(path))
            Directory.Delete(path, true);
    }
}