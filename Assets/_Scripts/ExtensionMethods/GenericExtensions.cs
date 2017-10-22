using Newtonsoft.Json;
using System;
using System.IO;

public static class GenericExtensions
{
    /// <summary>
    /// This extends EVERY type. Allowing it to be saved
    /// </summary>
    /// <typeparam name="T">Type being extended</typeparam>
    /// <param name="item">Object of the specific type</param>
    /// <param name="path">Path where you would like it to be saved</param>
    public static void Save<T>(this T item, string path)
    {
        string json = JsonConvert.SerializeObject(item, Formatting.Indented);
        File.WriteAllText(path, json);
    }

    /// <summary>
    /// Take a path to a JSON text file and load it into a type
    /// </summary>
    /// <typeparam name="T">Type to deserialize in to</typeparam>
    /// <param name="item">Object being extended</param>
    /// <param name="path">Path to the JSON</param>
    /// <returns>Object from the deserialized JSON</returns>
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
