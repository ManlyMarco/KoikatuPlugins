using System.Linq;
using Harmony;
using Studio;
using static BepInEx.Logger;
using BepInEx.Logging;

namespace DefaultParamEditor
{
    public class CharacterParam
    {
        static ParamData.CharaData charaData;

        public CharacterParam(ParamData.CharaData data)
        {
            charaData = data;
            var harmony = HarmonyInstance.Create("keelhauled.defaultparameditor.characterparam.harmony");
            harmony.PatchAll(typeof(CharacterParam));
        }

        public void Save()
        {
            var selected = GuideObjectManager.Instance.selectObjectKey.Select(x => Studio.Studio.GetCtrlInfo(x) as OCIChar).Where(x => x != null).ToList();

            if(selected.Count > 0)
            {
                var status = selected[0].charFileStatus;

                charaData.clothesState = status.clothesState;
                charaData.shoesType = status.shoesType;
                charaData.hohoAkaRate = status.hohoAkaRate;
                charaData.nipStandRate = status.nipStandRate;
                charaData.tearsLv = status.tearsLv;

                charaData.eyesLookPtn = status.eyesLookPtn;
                charaData.neckLookPtn = status.neckLookPtn;
                charaData.eyebrowPtn = status.eyebrowPtn;
                charaData.eyesPtn = status.eyesPtn;
                charaData.eyesOpenMax = status.eyesOpenMax;
                charaData.eyesBlink = status.eyesBlink;
                charaData.mouthPtn = status.mouthPtn;

                charaData.saved = true;
                Log(LogLevel.Message, "Default character settings saved");
            }
            else
            {
                Log(LogLevel.Message, "Select character to save default settings");
            }
        }

        [HarmonyPostfix, HarmonyPatch(typeof(ChaFileStatus), nameof(ChaFileStatus.MemberInit))]
        public static void HarmonyPatch_ChaFileStatus_MemberInit(ChaFileStatus __instance)
        {
            if(charaData.saved)
            {
                __instance.clothesState = charaData.clothesState;
                __instance.shoesType = charaData.shoesType;
                __instance.hohoAkaRate = charaData.hohoAkaRate;
                __instance.nipStandRate = charaData.nipStandRate;
                __instance.tearsLv = charaData.tearsLv;

                __instance.eyesLookPtn = charaData.eyesLookPtn;
                __instance.neckLookPtn = charaData.neckLookPtn;
                __instance.eyebrowPtn = charaData.eyebrowPtn;
                __instance.eyesPtn = charaData.eyesPtn;
                __instance.eyesOpenMax = charaData.eyesOpenMax;
                __instance.eyesBlink = charaData.eyesBlink;
                __instance.mouthPtn = charaData.mouthPtn;
            }
        }
    }
}
