using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Random = UnityEngine.Random;

public static class ExtensionMethodsString
{
    /// <summary>
    ///     Replaces variable names encapsulted by a tag identifier. The variable names and values are given through a
    ///     dictionary.
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
        foreach (KeyValuePair<string, string> item in referenceVars)
        {
            string varName = string.Format("{0}{1}{0}", tagIdentifier, item.Key);

            result = result.Replace(varName, item.Value);
        }

        return result;
    }

    public static string[] SplitByTags(string target)
    {
        return target.Split('<', '>');
    }

    public static string[] SplitParameters(this string s, bool removeAllSpaces = true)
    {
        if (removeAllSpaces)
        {
            return s.Replace(" ", "").Split(',');
        }

        return s.Split(',');
    }

    public static string RemoveStartAndEndSpaces(this string s)
    {
        s = s.RemoveStartSpaces();
        s = s.RemoveEndSpaces();

        return s;
    }

    public static string RemoveStartSpaces(this string s)
    {
        while (s.StartsWith(" "))
        {
            s = s.Remove(0, 1);
        }

        return s;
    }

    public static string RemoveEndSpaces(this string s)
    {
        while (s.EndsWith(" "))
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

        foreach (string t in characters)
        {
            if (result.Contains(t))
            {
                result = result.Replace(t, "");
            }
        }

        return result;
    }
}

public static class ExtensionMethodsDictionary
{
    public static void AddOrOverwrite<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key, TValue value)
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
    ///     Copies values of a dictionary and separates them into two lists given.
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="dict"></param>
    /// <param name="keylist"></param>
    /// <param name="valueList"></param>
    public static void CopyValuesToLists<TKey, TValue>(this Dictionary<TKey, TValue> dict, out List<TKey> keylist,
        out List<TValue> valueList)
    {
        keylist = new List<TKey>();
        valueList = new List<TValue>();
        foreach (KeyValuePair<TKey, TValue> kvp in dict)
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

        return new SerializedDictionary<Tkey, Tvalue>(keys, values);
    }

    [Serializable]
    public struct SerializedDictionary<Tkey, Tvalue>
    {
        public List<Tkey> keys;
        public List<Tvalue> values;

        public SerializedDictionary(List<Tkey> keys, List<Tvalue> values)
        {
            this.keys = keys;
            this.values = values;
        }

        public SerializedDictionary<Tkey, Tvalue> Empty()
        {
            keys = new List<Tkey>();
            values = new List<Tvalue>();

            return this;
        }
    }

    public static Dictionary<Tkey, Tvalue> Deserialize<Tkey, Tvalue>(this SerializedDictionary<Tkey, Tvalue> dictionary)
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
        byte[] data = texture.EncodeToPNG();
        File.WriteAllBytes(savePath, data);
    }

    public static void SaveAsJPG(this Texture2D texture, string savePath)
    {
        byte[] data = texture.EncodeToJPG();
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

        Color[] bData = aBottom.GetPixels();
        Color[] tData = aTop.GetPixels();
        int count = bData.Length;
        Color[] rData = new Color[count];

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

        Texture2D res = new Texture2D(aTop.width, aTop.height);
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

public static class ExtensionMethods_Keycode
{
    public static bool KeyPressed_Down(this KeyCode[] keys)
    {
        bool result = false;
        for (int i = 0; i < keys.Length; i++)
        {
            if (Input.GetKeyDown(keys[i]))
            {
                result = true;
                break;
            }
        }

        return result;
    }
}

public static class ExtensionMethods_bool
{
    public static bool Toggle(this bool state)
    {
        if (state)
        {
            state = false;
        }
        else
        {
            state = true;
        }

        return state;
    }
}

public static partial class ExtensionMethods_Rigidbody
{
    public static Vector3 PredictNextPosition(this Rigidbody self)
    {
        return self.position + self.velocity * Time.deltaTime;
    }
}

public static partial class ExtensionMethods_List
{
    public static T ChooseRandom<T>(this List<T> self)
    {
        return self[Random.Range(0, self.Count)];
    }

    public static T SafeGet<T>(this List<T> self, int index)
    {
        if(self.Count > index)
        {
            return self[index];
        }
        else
        {
            return default(T);
        }
    }
    
    public static T GetMax<T>(this List<T> self, Func<T, int> filterMethod, bool randomizeTie = false)
    {
        List<T> ties = new List<T>();
        
        int currentMax = 0;
        foreach (T item in self)
        {
            int value = filterMethod(item);
            if (value > currentMax)
            {
                currentMax = value;
                ties.Clear();
                ties.Add(item);
            }
            else if (value == currentMax)
            {
                ties.Add(item);
            }
        }
        
        T max = randomizeTie ? ties.ChooseRandom() : ties[0];
        return max;
    }
    
    public static int DifferenceAmount<T>(this List<T> list, List<T> compareList) where T : class
    {
        int maxCount = Mathf.Max(list.Count, compareList.Count);
        int minCount = Mathf.Min(list.Count, compareList.Count);
        int differences = 0;
        for (int i = 0; i < maxCount; i++)
        {
            if (i >= minCount)
            {
                differences++;
            }
            else
            {
                if (list[i] != compareList[i])
                {
                    differences++;
                }
            }
        }

        return differences;
    }
}

public static partial class ExtensionMethods_LayerMask
{
    public static bool CheckLayer(this LayerMask self, int layer)
    {
        return self == (self | (1 << layer));
    }
}

public static class ExtensionMethods_Vector3
{
    public static string ToCoordinatesAsString(this Vector3 self)
    {
        return $"x: {self.x}; y: {self.y}; z: {self.z};";
    }

    public static Vector3 Flatten(this Vector3 self, float xWeight, float yWeight, float zWeight)
    {
        return new Vector3(self.x * xWeight, self.y * yWeight, self.z * zWeight);
    }
}

public static class ExtensionMethods_Numbers
{
    /// <summary>
    /// Turns a number into its percentage equivalent in the range given.
    /// </summary>
    /// <param name="self">The number in the range</param>
    /// <param name="min">The minimum number in the range</param>
    /// <param name="max">The maximum number in the range</param>
    /// <returns></returns>
    public static float ToPercentageOfRange(this float self, float min, float max)
    {
        if (self < min || self > max)
        {
            throw new Exception($"The number was not within the specified range! Range: {min} to {max}. Number: {self}");
        }

        float range = max - min;
        float current = self - min;

        return (current * 100) / range;
    }
    
    /// <summary>
    /// Turns a number into its percentage equivalent in the range given.
    /// </summary>
    /// <param name="self">The number in the range</param>
    /// <param name="min">The minimum number in the range</param>
    /// <param name="max">The maximum number in the range</param>
    /// <returns></returns>
    public static float ToPercentageOfRange(this int self, int min, int max)
    {
        return ToPercentageOfRange((float)self, min, max);
    }
    
    
}

public enum Direction
{
    Left, Right, Back, Front, Up, Down
}
public static class ExtensionMethods_Transform
{
    public static Direction GetDirectionTo(this Transform self, Transform obj) {
        
        Vector3 dir = obj.position - self.position;

        float angle = Vector3.Angle(new Vector3(dir.x,0,dir.z), new Vector3(self.forward.x,0,self.forward.z));

        if (angle <= 45)
        {
            //Front
            return Direction.Front;
        }
        
        if (angle <= 135)
        {
            //One of the sides

            Vector3 perp = Vector3.Cross(self.forward, dir);
            float dir2 = Vector3.Dot(perp, self.up);
		
            if (dir2 > 0f) 
            {
                //Right
                return Direction.Right;
            } 
            
            if (dir2 < 0f) 
            {
                //Left
                return Direction.Left;
            }

            throw new Exception("Could not find direction");
        }
        else
        {
            //Back
            return Direction.Back;
        }
    }

    public static Vector3 GetDirectionFrom(this Transform self, Direction dir)
    {
        switch (dir)
        {
            case Direction.Back:
                return -self.forward;
            
            case Direction.Down:
                return -self.up;
            
            case Direction.Front:
                return self.forward;
            
            case Direction.Left:
                return -self.right;
            
            case Direction.Right:
                return self.right;
            
            case Direction.Up:
                return self.up;
        }

        throw new Exception("Couldn't find direction");
    }
}

public static class ExtensionMethods_Collider
{
    public static Vector3 GetRandomPositionInBounds<T>(this T collider) where T : Collider
    {
        Bounds _bounds = collider.bounds;
        return new Vector3(Random.Range(_bounds.min.x, _bounds.max.x), Random.Range(_bounds.min.y, _bounds.max.y),
            Random.Range(_bounds.min.z, _bounds.max.z));
    }
}