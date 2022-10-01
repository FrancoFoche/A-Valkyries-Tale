using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class ExtensionMethods_string
{

    /// <summary>
    /// Replaces variable names encapsulted by a tag identifier. The variable names and values are given through a dictionary.
    /// </summary>
    /// <param name="s">String to be modified</param>
    /// <param name="referenceVars">Dictionary holding the variable names and values to replace</param>
    /// <param name="tagIdentifier">char or string encapsulating variable names to identify the</param>
    public static string Inject(this string s, Dictionary<string, string> referenceVars, string tagIdentifier = "@")
    {
        bool contains = s.Contains(tagIdentifier);
        if (!contains)
        {
            return s;
        }

        string result = s;
        foreach (var item in referenceVars)
        {
            string varName = string.Format("{0}{1}{0}", tagIdentifier, item.Key);

            result = result.Replace(varName, item.Value);
        }

        return result;
    }
    public static string[] SplitByTags(string target)
    {
        return target.Split(new char[2] { '<', '>' });
    }

    public static string[] SplitParameters(this string s, bool removeAllSpaces = true)
    {
        if (removeAllSpaces)
        {
            return s.Replace(" ", "").Split(',');
        }
        else
        {
            return s.Split(',');
        }
    }

    public static string RemoveStartAndEndSpaces(this string s)
    {
        s = s.RemoveStartSpaces();
        s = s.RemoveEndSpaces();

        return s;
    }

    public static string RemoveStartSpaces(this string s)
    {
        while(s.StartsWith(" "))
        {
            s = s.Remove(0, 1);
        }

        return s;
    }

    public static string RemoveEndSpaces(this string s)
    {
        while(s.EndsWith(" "))
        {
            s = s.Remove(s.Length - 1);
        }

        return s;
    }

    public static string Colorize(this string str, Color color)
    {
        string newString = "<color=#" + ColorUtility.ToHtmlStringRGBA(color) + ">" + str + "</color>";
        return newString;
    }

    public static string Remove(this string str, params string[] characters)
    {
        string result = str;

        for (int i = 0; i < characters.Length; i++)
        {
            if (result.Contains(characters[i]))
            {
                result = result.Replace(characters[i], "");
            }
        }

        return result;
    }

}

public static class ExtensionMethods_Dictionary
{
    public static void AddOrOverwrite<TKey,TValue>(this Dictionary<TKey, TValue> dictionary, TKey key, TValue value)
    {
        if (dictionary.ContainsKey(key))
        {
            dictionary[key] = value;
        }
        else
        {
            dictionary.Add(key, value);
        }
    }
    public static string AddOrRename<TValue>(this Dictionary<string, TValue> dictionary, string key, TValue value)
    {
        if (dictionary.ContainsKey(key))
        {
            string oldKey = key;
            int i = 1;
            while (dictionary.ContainsKey(key))
            {
                key = oldKey + i;
                i++;
            }
        }

        dictionary.Add(key, value);

        return key;
    }

    #region Serialization
    /// <summary>
    /// Copies values of a dictionary and separates them into two lists given.
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="dict"></param>
    /// <param name="keylist"></param>
    /// <param name="valueList"></param>
    public static void CopyValuesToLists<TKey, TValue>(this Dictionary<TKey, TValue> dict, out List<TKey> keylist, out List<TValue> valueList)
    {
        keylist = new List<TKey>();
        valueList = new List<TValue>();
        foreach (var kvp in dict)
        {
            keylist.Add(kvp.Key);
            valueList.Add(kvp.Value);
        }
    }
    public static SerializedDictionary<Tkey, Tvalue> Serialize<Tkey, Tvalue>(this Dictionary<Tkey, Tvalue> dictionary)
    {
        List<Tkey> keys;
        List<Tvalue> values;
        dictionary.CopyValuesToLists(out keys, out values);

        return new SerializedDictionary<Tkey,Tvalue>(keys, values);
    }

    [System.Serializable]
    public struct SerializedDictionary<Tkey, Tvalue>
    {
        public List<Tkey> keys;
        public List<Tvalue> values;

        public SerializedDictionary<Tkey, Tvalue> Empty()
        {
            keys = new List<Tkey>();
            values = new List<Tvalue>();

            return this;
        }
        public SerializedDictionary(List<Tkey> keys, List<Tvalue> values)
        {
            this.keys = keys;
            this.values = values;
        }
    }
    public static Dictionary<Tkey,Tvalue> Deserialize<Tkey, Tvalue>(this SerializedDictionary<Tkey, Tvalue> dictionary)
    {
        Dictionary<Tkey, Tvalue> deserialized = new Dictionary<Tkey, Tvalue>();
        for (int i = 0; i < dictionary.keys.Count; i++)
        {
            deserialized.Add(dictionary.keys[i], dictionary.values[i]);
        }

        return deserialized;
    }
    #endregion
}

public static class ExtensionMethods_Texture2D
{
    public static void SaveAsPNG(this Texture2D texture, string savePath)
    {
        var data = texture.EncodeToPNG();
        File.WriteAllBytes(savePath, data);
    }

    public static void SaveAsJPG(this Texture2D texture, string savePath)
    {
        var data = texture.EncodeToJPG();
        File.WriteAllBytes(savePath, data);
    }

    public static Texture2D ChangeFormat(this Texture2D old, TextureFormat newFormat)
    {
        Texture2D newTexture = new Texture2D(2, 2, newFormat, false);
        Color[] pixels = old.GetPixels();
        newTexture.SetPixels(pixels);
        newTexture.Apply();

        return newTexture;
    }

    public static Texture2D AlphaBlend(this Texture2D aBottom, Texture2D aTop)
    {
        aTop.Resize(aBottom.width, aBottom.height);

        var bData = aBottom.GetPixels();
        var tData = aTop.GetPixels();
        int count = bData.Length;
        var rData = new Color[count];

        for (int i = 0; i < count; i++)
        {
            Color B = bData[i];
            Color T = tData[i];
            float srcF = T.a;
            float destF = 1f - T.a;
            float alpha = srcF + destF * B.a;
            Color R = (T * srcF + B * B.a * destF) / alpha;
            R.a = alpha;
            rData[i] = R;
        }
        var res = new Texture2D(aTop.width, aTop.height);
        res.SetPixels(rData);
        res.Apply();
        return res;
    }

    public static Texture2D Kam_Resize(this Texture2D texture2D, int width, int height)
    {
        RenderTexture rt = new RenderTexture(width, height, 24);
        RenderTexture.active = rt;
        Graphics.Blit(texture2D, rt);

        Texture2D result = new Texture2D(width, height);
        result.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        result.Apply();
        return result;
    }
}

public static class ExtensionMethods_Color
{
    public static Color SetAlpha(this Color c, float alpha)
    {
        return new Color(c.r, c.g, c.b, alpha);
    }
}
