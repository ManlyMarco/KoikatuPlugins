using System;
using System.Linq;
using Harmony;
using Studio;

namespace CharaStateX
{
    static class EtcInfoPatch
    {
        static HarmonyInstance harmony;
        static Type etcInfoType;
        static string charaParamName = "ociChar";

        public static void Patch(HarmonyInstance harmonyInstance)
        {
            harmony = harmonyInstance;
            etcInfoType = typeof(MPCharCtrl).GetNestedType("EtcInfo", AccessTools.all);
            PatchEtcInfoMethod("ChangeEyebrowsPtn");
            PatchEtcInfoMethod("OnValueChangedForegroundEyebrow");
            PatchEtcInfoMethod("ChangeEyesPtn");
            PatchEtcInfoMethod("OnValueChangedEyesOpen");
            PatchEtcInfoMethod("OnValueChangedEyesBlink");
            PatchEtcInfoMethod("OnValueChangedForegroundEyes");
            PatchEtcInfoMethod("ChangeMouthPtn");
            PatchEtcInfoMethod("OnValueChangedMouthOpen");
            PatchEtcInfoMethod("OnValueChangedLipSync");
        }

        static void PatchEtcInfoMethod(string targetName)
        {
            var target = AccessTools.Method(etcInfoType, targetName);
            var patch = AccessTools.Method(typeof(EtcInfoPatch), $"Patch_{targetName}");
            harmony.Patch(target, null, new HarmonyMethod(patch));
        }

        static void Patch_ChangeEyebrowsPtn(object __instance, ref int _no)
        {
            var ociChar = Traverse.Create(__instance).Property(charaParamName).GetValue<OCIChar>();
            foreach(var chara in CharaStateX.GetSelectedCharacters().Where((x) => x != ociChar))
                chara.charInfo.ChangeEyebrowPtn(_no, true);
        }

        static void Patch_OnValueChangedForegroundEyebrow(object __instance, ref int _value)
        {
            var ociChar = Traverse.Create(__instance).Property(charaParamName).GetValue<OCIChar>();
            foreach(var chara in CharaStateX.GetSelectedCharacters().Where((x) => x != ociChar))
                chara.foregroundEyebrow = (byte)_value;
        }

        static void Patch_ChangeEyesPtn(object __instance, ref int _no)
        {
            var ociChar = Traverse.Create(__instance).Property(charaParamName).GetValue<OCIChar>();
            foreach(var chara in CharaStateX.GetSelectedCharacters().Where((x) => x != ociChar))
                chara.charInfo.ChangeEyesPtn(_no, true);
        }

        static void Patch_OnValueChangedEyesOpen(object __instance, ref float _value)
        {
            var ociChar = Traverse.Create(__instance).Property(charaParamName).GetValue<OCIChar>();
            foreach(var chara in CharaStateX.GetSelectedCharacters().Where((x) => x != ociChar))
                chara.ChangeEyesOpen(_value);
        }

        static void Patch_OnValueChangedEyesBlink(object __instance, ref bool _value)
        {
            var ociChar = Traverse.Create(__instance).Property(charaParamName).GetValue<OCIChar>();
            foreach(var chara in CharaStateX.GetSelectedCharacters().Where((x) => x != ociChar))
                chara.ChangeBlink(_value);
        }

        static void Patch_OnValueChangedForegroundEyes(object __instance, ref int _value)
        {
            var ociChar = Traverse.Create(__instance).Property(charaParamName).GetValue<OCIChar>();
            foreach(var chara in CharaStateX.GetSelectedCharacters().Where((x) => x != ociChar))
                chara.foregroundEyes = (byte)_value;
        }

        static void Patch_ChangeMouthPtn(object __instance, ref int _no)
        {
            var ociChar = Traverse.Create(__instance).Property(charaParamName).GetValue<OCIChar>();
            foreach(var chara in CharaStateX.GetSelectedCharacters().Where((x) => x != ociChar))
                chara.charInfo.ChangeMouthPtn(_no, true);
        }

        static void Patch_OnValueChangedMouthOpen(object __instance, ref float _value)
        {
            var ociChar = Traverse.Create(__instance).Property(charaParamName).GetValue<OCIChar>();
            foreach(var chara in CharaStateX.GetSelectedCharacters().Where((x) => x != ociChar))
                chara.ChangeMouthOpen(_value);
        }

        static void Patch_OnValueChangedLipSync(object __instance, ref bool _value)
        {
            var ociChar = Traverse.Create(__instance).Property(charaParamName).GetValue<OCIChar>();
            foreach(var chara in CharaStateX.GetSelectedCharacters().Where((x) => x != ociChar))
                chara.ChangeLipSync(_value);
        }
    }
}
