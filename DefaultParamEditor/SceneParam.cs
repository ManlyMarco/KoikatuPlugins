using System;
using System.IO;
using System.Reflection;
using Harmony;
using MessagePack;
using Studio;
using UnityEngine;

namespace DefaultParamEditor
{
    public class SceneParam
    {
        static string savePath;
        static bool propertiesInitialized = false;
        static SceneParamData data = new SceneParamData();

        public SceneParam()
        {
            savePath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "DefaultParamEditor_SceneParamData.bin");

            if(File.Exists(savePath))
            {
                data = MessagePackSerializer.Deserialize<SceneParamData>(File.ReadAllBytes(savePath));
                data.PrintProperties();
                propertiesInitialized = true;
            }

            var harmony = HarmonyInstance.Create("keelhauled.defaultparameditor.sceneparam.harmony");
            harmony.PatchAll(typeof(SceneParam));
        }

        public void Save()
        {
            var sceneInfo = Studio.Studio.Instance.sceneInfo;

            if(sceneInfo != null)
            {
                foreach(var field in AccessTools.GetDeclaredFields(typeof(SceneParamData)))
                {
                    var target = AccessTools.Field(sceneInfo.GetType(), field.Name);
                    var value = target.GetValue(sceneInfo);
                    field.SetValue(data, value);
                }

                var bytes = MessagePackSerializer.Serialize(data);
                File.WriteAllBytes(savePath, bytes);
                propertiesInitialized = true;
            }
        }

        [HarmonyPostfix, HarmonyPatch(typeof(SceneInfo), nameof(SceneInfo.Init))]
        public static void HarmonyPatch_SceneInfo_Init(SceneInfo __instance)
        {
            if(propertiesInitialized)
            {
                foreach(var prop in AccessTools.GetDeclaredFields(typeof(SceneParamData)))
                {
                    var target = AccessTools.Field(__instance.GetType(), prop.Name);
                    target.SetValue(__instance, prop.GetValue(data));
                }
            }
        }

        [MessagePackObject(true)]
        public class SceneParamData
        {
            public void PrintProperties()
            {
                Console.WriteLine(new string('=', 40));
                foreach(var prop in AccessTools.GetDeclaredFields(typeof(SceneParamData)))
                {
                    var target = AccessTools.Field(typeof(SceneParamData), prop.Name);
                    var value = target.GetValue(this);
                    Console.WriteLine($"{prop.Name} = {value}");
                }
                Console.WriteLine(new string('=', 40));
            }

            public int aceNo;
            public float aceBlend;
            public bool enableAOE;
            public Color aoeColor;
            public float aoeRadius;
            public bool enableBloom;
            public float bloomIntensity;
            public float bloomThreshold;
            public float bloomBlur;
            public bool enableDepth;
            public float depthFocalSize;
            public float depthAperture;
            public bool enableVignette;
            public bool enableFog;
            public Color fogColor;
            public float fogHeight;
            public float fogStartDistance;
            public bool enableSunShafts;
            public Color sunThresholdColor;
            public Color sunColor;
            public bool enableShadow;
            //public bool faceNormal;
            //public bool faceShadow;
            public float lineColorG;
            public Color ambientShadow;
            public float lineWidthG;
            public int rampG;
            public float ambientShadowG;
        }
    }
}
