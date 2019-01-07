using System;
using System.Collections.Generic;
using System.Linq;
using Harmony;
using Studio;

namespace CharaStateX
{
    static class EtcInfoPatch
    {
        static HarmonyInstance harmony;
        static Type etcInfoType;

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

        static void Patch_ChangeEyebrowsPtn(ref int _no)
        {
            foreach(var chara in GetSelectedCharacters())
                chara.charInfo.ChangeEyebrowPtn(_no, true);
        }

        static void Patch_OnValueChangedForegroundEyebrow(ref int _value)
        {
            foreach(var chara in GetSelectedCharacters())
                chara.foregroundEyebrow = (byte)_value;
        }

        static void Patch_ChangeEyesPtn(ref int _no)
        {
            foreach(var chara in GetSelectedCharacters())
                chara.charInfo.ChangeEyesPtn(_no, true);
        }

        static void Patch_OnValueChangedEyesOpen(ref float _value)
        {
            foreach(var chara in GetSelectedCharacters())
                chara.ChangeEyesOpen(_value);
        }

        static void Patch_OnValueChangedEyesBlink(ref bool _value)
        {
            foreach(var chara in GetSelectedCharacters())
                chara.ChangeBlink(_value);
        }

        static void Patch_OnValueChangedForegroundEyes(ref int _value)
        {
            foreach(var chara in GetSelectedCharacters())
                chara.foregroundEyes = (byte)_value;
        }

        static void Patch_ChangeMouthPtn(ref int _no)
        {
            foreach(var chara in GetSelectedCharacters())
                chara.charInfo.ChangeMouthPtn(_no, true);
        }

        static void Patch_OnValueChangedMouthOpen(ref float _value)
        {
            foreach(var chara in GetSelectedCharacters())
                chara.ChangeMouthOpen(_value);
        }

        static void Patch_OnValueChangedLipSync(ref bool _value)
        {
            foreach(var chara in GetSelectedCharacters())
                chara.ChangeLipSync(_value);
        }

        static List<OCIChar> GetSelectedCharacters()
        {
            return GuideObjectManager.Instance.selectObjectKey.Select(x => Studio.Studio.GetCtrlInfo(x) as OCIChar).Where(x => x != null).ToList();
        }
    }
}
