using System.Collections.Generic;
using System.Linq;
using Harmony;
using Studio;

namespace CharaStateX
{
    static class NeckLookPatch
    {
        static string charaParamName = "ociChar";

        public static void Patch(HarmonyInstance harmony)
        {
            {
                var type = typeof(MPCharCtrl).GetNestedType("LookAtInfo", AccessTools.all);
                var target = AccessTools.Method(type, "OnClick");
                var patch = AccessTools.Method(typeof(NeckLookPatch), nameof(Patch_LookAtInfo_OnClick));
                harmony.Patch(target, null, new HarmonyMethod(patch));
            }

            {
                var type = typeof(MPCharCtrl).GetNestedType("NeckInfo", AccessTools.all);
                var target = AccessTools.Method(type, "OnClick");
                var patch = AccessTools.Method(typeof(NeckLookPatch), nameof(Patch_NeckInfo_OnClick));
                harmony.Patch(target, null, new HarmonyMethod(patch));
            }
        }

        static List<OCIChar> GetSelectedCharacters()
        {
            return GuideObjectManager.Instance.selectObjectKey.Select(x => Studio.Studio.GetCtrlInfo(x) as OCIChar).Where(x => x != null).ToList();
        }

        static void Patch_LookAtInfo_OnClick(object __instance, ref int _no)
        {
            var ociChar = Traverse.Create(__instance).Property(charaParamName).GetValue<OCIChar>();
            foreach(var chara in GetSelectedCharacters().Where((x) => x != ociChar))
                chara.ChangeLookEyesPtn(_no, false);
        }

        static void Patch_NeckInfo_OnClick(object __instance, ref int _idx)
        {
            var ociChar = Traverse.Create(__instance).Property(charaParamName).GetValue<OCIChar>();
            var patterns = Traverse.Create(__instance).Field("patterns").GetValue<int[]>();

            foreach(var chara in GetSelectedCharacters().Where((x) => x != ociChar))
                chara.ChangeLookNeckPtn(patterns[_idx]);
        }
    }
}
