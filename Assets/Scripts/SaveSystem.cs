using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public static class SaveSystem
{
    public static void Save<T>(T data, string file)
    {
        if (typeof(T) == typeof(Inventory))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            string path = Application.persistentDataPath + file;
            FileStream stream = new FileStream(path, FileMode.Create);
            formatter.Serialize(stream, data);
            stream.Close();
        }
        else
        {
            return;
        }
    }
    public static T Load<T>(string file)
    {
        string path = Application.persistentDataPath + file;
        if (File.Exists(path))
        {
            if (typeof(T) == typeof(Inventory))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                FileStream stream = new FileStream(path, FileMode.Open);
                T data = (T)formatter.Deserialize(stream);
                stream.Close();
                return data;
            }
            else
            {
                return default(T);
            }
        }
        else
        {
            return default(T);
        }
    }
}