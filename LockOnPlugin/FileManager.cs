using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace LockOnPluginKK
{
    internal static class FileManager
    {
        private static string settingsPath = Path.Combine(Environment.CurrentDirectory, @"BepInEx\TargetSettings");
        private static string quickFemaleTargetNamesPath = Path.Combine(settingsPath, "quicktargetsfemale.txt");
        private static string quickMaleTargetNamesPath = Path.Combine(settingsPath, "quicktargetsmale.txt");
        private static string normalTargetNamesPath = Path.Combine(settingsPath, "normaltargets.txt");
        private static string customTargetNamesPath = Path.Combine(settingsPath, "customtargets.txt");
        private static string centerTargetWeightsPath = Path.Combine(settingsPath, "centertargetweights.txt");

        public static bool TargetSettingsExist()
        {
            if(File.Exists(quickFemaleTargetNamesPath) && File.Exists(quickMaleTargetNamesPath) &&
            File.Exists(normalTargetNamesPath) && File.Exists(customTargetNamesPath) &&
            File.Exists(centerTargetWeightsPath))
            {
                return true;
            }
            else
            {
                Console.WriteLine("Target settings are missing");
                return false;
            }
        }

        public static List<string> GetQuickFemaleTargetNames() => GetTargetNames(quickFemaleTargetNamesPath);
        public static List<string> GetQuickMaleTargetNames() => GetTargetNames(quickMaleTargetNamesPath);
        public static List<string> GetNormalTargetNames() => GetTargetNames(normalTargetNamesPath);
        public static List<List<string>> GetCustomTargetNames() => GetTargetData(customTargetNamesPath);
        public static List<List<string>> GetCenterTargetWeights() => GetTargetData(centerTargetWeightsPath);

        private static List<List<string>> GetTargetData(string filePath)
        {
            List<List<string>> list = new List<List<string>>();
            foreach(string item in GetTargetNames(filePath))
            {
                list.Add(item.Split('|').ToList());
            }
            return list;
        }

        private static List<string> GetTargetNames(string filePath)
        {
            List<string> list = new List<string>();
            foreach(string item in File.ReadAllLines(filePath))
            {
                string line = StringUntil(item, "//");
                line = line.Replace(" ", "");
                if(line != "")
                {
                    list.Add(line);
                }
            }

            return list;
        }

        private static string StringUntil(string text, string stopAt)
        {
            if(text != null && stopAt != null)
            {
                int charLocation = text.IndexOf(stopAt, StringComparison.Ordinal);

                if(charLocation > 0)
                {
                    return text.Substring(0, charLocation);
                }
                else if(charLocation == 0)
                {
                    return "";
                }
            }

            return text;
        }
    }
}
