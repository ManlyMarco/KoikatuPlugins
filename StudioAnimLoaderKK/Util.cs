using System;
using System.Linq;
using UnityEngine;

namespace StudioAnimLoader
{
    internal static class Util
    {
        //internal static string[] ParseKeyNames(string section, string key, string defaultValue)
        //{
        //	string text = ModPrefs.GetString(section, key, defaultValue, true).ToLower();
        //	if (text == null || text == "")
        //	{
        //		return null;
        //	}
        //	foreach (int num in Enumerable.Range(0, Util.replacePairs.GetLength(0)))
        //	{
        //		text = text.Replace(Util.replacePairs[num, 0], Util.replacePairs[num, 1]);
        //	}
        //	string[] array = text.Split(Util.separators, StringSplitOptions.RemoveEmptyEntries);
        //	if (array == null)
        //	{
        //		return null;
        //	}
        //	foreach (int num2 in Enumerable.Range(0, array.Length))
        //	{
        //		array[num2] = array[num2].Trim();
        //	}
        //	return array;
        //}

        internal static int[] ParseJsonArray(string json)
        {
            int[] array;
            try
            {
                array = JsonUtility.FromJson<Util.UnityJsonWrapper>("{\"Value\":" + json + "}").Value;
            }
            catch(Exception)
            {
                array = null;
            }
            if(array != null && array.Length == 0)
            {
                array = null;
            }
            return array;
        }

        internal static bool InputChecker(string[] keyNames, bool isKeyDown)
        {
            if(keyNames == null)
            {
                return false;
            }
            bool flag = true;
            try
            {
                for(int i = 0; i < keyNames.Length; i++)
                {
                    string text = keyNames[i];
                    if(leftRightKeys.Contains(text))
                    {
                        if(isKeyDown && i + 1 == keyNames.Length)
                        {
                            flag &= (Input.GetKeyDown("left " + text) | Input.GetKeyDown("right " + text));
                        }
                        else
                        {
                            flag &= (Input.GetKey("left " + text) | Input.GetKey("right " + text));
                        }
                    }
                    else if(isKeyDown && i + 1 == keyNames.Length)
                    {
                        flag &= Input.GetKeyDown(text);
                    }
                    else
                    {
                        flag &= Input.GetKey(text);
                    }
                }
            }
            catch(Exception)
            {
                return false;
            }
            return flag;
        }

        static Util()
        {
            string[,] array = new string[1, 2];
            array[0, 0] = "control";
            array[0, 1] = "ctrl";
            replacePairs = array;
            leftRightKeys = new string[]
            {
                "ctrl",
                "shift",
                "alt"
            };
        }

        private static string[] separators = new string[] { "+" };
        private static string[,] replacePairs;
        private static string[] leftRightKeys;

        private class UnityJsonWrapper
        {
            public int[] Value;
        }
    }
}
