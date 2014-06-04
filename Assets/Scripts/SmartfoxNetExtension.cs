using UnityEngine;
using System.Collections;
using Sfs2X.Entities;

public static class SmartfoxNetExtension
{
    public static object GetObject(this IDictionary dictionary, string key)
    {
        return dictionary.GetValue<object>(key);
    }

    public static T GetValue<T>(this IDictionary dictionary, string key)
    {
        if (dictionary == null)
            return default(T);

        if (!dictionary.Contains(key))
            return default(T);

        object obj = dictionary[key];

        if (obj is T)
            return (T)obj;

        return default(T);
    }


    public static Room GetRoom(this IDictionary dictionary){ return dictionary.GetValue<Room>("room"); }
    public static User GetUser(this IDictionary dictionary) { return dictionary.GetValue<User>("user"); }
    public static User GetSender(this IDictionary dictionary) { return dictionary.GetValue<User>("sender"); }
    public static string GetMessage(this IDictionary dictionary) { return dictionary.GetValue<string>("message"); }
}
