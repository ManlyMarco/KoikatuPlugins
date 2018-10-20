using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using ParadoxNotion.Serialization;

namespace LockOnPluginKK
{
    public class TargetData
    {
        public static TargetData data;
        const string dataFileName = "LockOnPluginData.json";

        public static void LoadData()
        {
            string dataPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), dataFileName);

            if(File.Exists(dataPath))
            {
                try
                {
                    var json = File.ReadAllText(dataPath);
                    data = JSONSerializer.Deserialize<TargetData>(json);
                }
                catch(Exception)
                {
                    Console.WriteLine("Failed to deserialize target data. Loading backup.");
                    LoadResourceData();
                }
            }
            else
            {
                Console.WriteLine("Loading default target data.");
                LoadResourceData();
            }
        }

        static void LoadResourceData()
        {
            string resourceName = $"{nameof(LockOnPluginKK)}.{dataFileName}";
            using(var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName))
            {
                using(var reader = new StreamReader(stream))
                {
                    string json = reader.ReadToEnd();
                    data = JSONSerializer.Deserialize<TargetData>(json);
                }
            }
        }

        public List<string> quickTargets;
        public List<CustomTarget> customTargets;
        public List<CenterWeigth> centerWeigths;

        public class CustomTarget
        {
            public string target;
            public string point1;
            public string point2;
            public float midpoint;
        }

        public class CenterWeigth
        {
            public string bone;
            public float weigth;
        }
    }
}
