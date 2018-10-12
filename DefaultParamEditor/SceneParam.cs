using Harmony;
using Studio;
using static BepInEx.Logger;
using BepInEx.Logging;

namespace DefaultParamEditor
{
    public class SceneParam
    {
        static ParamData.SceneData sceneData;

        public SceneParam(ParamData.SceneData data)
        {
            sceneData = data;
            var harmony = HarmonyInstance.Create("keelhauled.defaultparameditor.sceneparam.harmony");
            harmony.PatchAll(typeof(SceneParam));
        }

        public void Save()
        {
            var sceneInfo = Studio.Studio.Instance.sceneInfo;
            var systemButtonCtrl = Studio.Studio.Instance.systemButtonCtrl;

            if(sceneInfo != null && systemButtonCtrl != null)
            {
                sceneData.aceNo = sceneInfo.aceNo;
                sceneData.aceBlend = sceneInfo.aceBlend;
                sceneData.enableAOE = Traverse.Create(systemButtonCtrl).Field("amplifyOcculusionEffectInfo").Property("aoe").Property("enabled").GetValue<bool>();
                sceneData.aoeColor = sceneInfo.aoeColor;
                sceneData.aoeRadius = sceneInfo.aoeRadius;
                sceneData.enableBloom = sceneInfo.enableBloom;
                sceneData.bloomIntensity = sceneInfo.bloomIntensity;
                sceneData.bloomThreshold = sceneInfo.bloomThreshold;
                sceneData.bloomBlur = sceneInfo.bloomBlur;
                sceneData.enableDepth = sceneInfo.enableDepth;
                sceneData.depthFocalSize = sceneInfo.depthFocalSize;
                sceneData.depthAperture = sceneInfo.depthAperture;
                sceneData.enableVignette = sceneInfo.enableVignette;
                sceneData.enableFog = sceneInfo.enableFog;
                sceneData.fogColor = sceneInfo.fogColor;
                sceneData.fogHeight = sceneInfo.fogHeight;
                sceneData.fogStartDistance = sceneInfo.fogStartDistance;
                sceneData.enableSunShafts = sceneInfo.enableSunShafts;
                sceneData.sunThresholdColor = sceneInfo.sunThresholdColor;
                sceneData.sunColor = sceneInfo.sunColor;
                sceneData.enableShadow = Traverse.Create(systemButtonCtrl).Field("selfShadowInfo").Field("toggleEnable").Property("isOn").GetValue<bool>();
                sceneData.rampG = sceneInfo.rampG;
                sceneData.ambientShadowG = sceneInfo.ambientShadowG;
                sceneData.lineWidthG = sceneInfo.lineWidthG;
                sceneData.lineColorG = sceneInfo.lineColorG;
                sceneData.ambientShadow = sceneInfo.ambientShadow;

                sceneData.saved = true;
                Log(LogLevel.Message, "Default scene settings saved");
            }
        }

        [HarmonyPostfix, HarmonyPatch(typeof(SceneInfo), nameof(SceneInfo.Init))]
        public static void HarmonyPatch_SceneInfo_Init(SceneInfo __instance)
        {
            if(sceneData.saved)
            {
                __instance.aceNo = sceneData.aceNo;
                __instance.aceBlend = sceneData.aceBlend;
                __instance.enableAOE = sceneData.enableAOE;
                __instance.aoeColor = sceneData.aoeColor;
                __instance.aoeRadius = sceneData.aoeRadius;
                __instance.enableBloom = sceneData.enableBloom;
                __instance.bloomIntensity = sceneData.bloomIntensity;
                __instance.bloomThreshold = sceneData.bloomThreshold;
                __instance.bloomBlur = sceneData.bloomBlur;
                __instance.enableDepth = sceneData.enableDepth;
                __instance.depthFocalSize = sceneData.depthFocalSize;
                __instance.depthAperture = sceneData.depthAperture;
                __instance.enableVignette = sceneData.enableVignette;
                __instance.enableFog = sceneData.enableFog;
                __instance.fogColor = sceneData.fogColor;
                __instance.fogHeight = sceneData.fogHeight;
                __instance.fogStartDistance = sceneData.fogStartDistance;
                __instance.enableSunShafts = sceneData.enableSunShafts;
                __instance.sunThresholdColor = sceneData.sunThresholdColor;
                __instance.sunColor = sceneData.sunColor;
                __instance.enableShadow = sceneData.enableShadow;
                __instance.rampG = sceneData.rampG;
                __instance.ambientShadowG = sceneData.ambientShadowG;
                __instance.lineWidthG = sceneData.lineWidthG;
                __instance.lineColorG = sceneData.lineColorG;
                __instance.ambientShadow = sceneData.ambientShadow;
            }
        }
    }
}
