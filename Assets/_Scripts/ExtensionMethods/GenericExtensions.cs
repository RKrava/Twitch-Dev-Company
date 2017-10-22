using Newtonsoft.Json;
using System;
using System.IO;

public static class GenericExtensions
{
    public static void Save<T>(this T item, string path)
    {
        string json = JsonConvert.SerializeObject(item, Formatting.Indented);
        File.WriteAllText(path, json);
    }

    public static T Load<T>(this T item, string path)
    {
        if (File.Exists(path) == false)
        {
            throw new Exception($"This file does not exist: {path}");
        }

        string json = File.ReadAllText(path);
        return JsonConvert.DeserializeObject<T>(json);
    }
}
