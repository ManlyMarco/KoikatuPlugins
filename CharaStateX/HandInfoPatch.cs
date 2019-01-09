using System.Linq;
using Harmony;
using Studio;

namespace CharaStateX
{
    static class HandInfoPatch
    {
        static string charaParamName = "ociChar";

        public static void Patch(HarmonyInstance harmony)
        {
            var type = typeof(MPCharCtrl).GetNestedType("HandInfo", AccessTools.all);

            {
                var target = AccessTools.Method(type, "ChangeLeftHandAnime");
                var patch = AccessTools.Method(typeof(HandInfoPatch), nameof(Patch_ChangeLeftHandAnime));
                harmony.Patch(target, null, new HarmonyMethod(patch));
            }

            {
                var target = AccessTools.Method(type, "ChangeRightHandAnime");
                var patch = AccessTools.Method(typeof(HandInfoPatch), nameof(Patch_ChangeRightHandAnime));
                harmony.Patch(target, null, new HarmonyMethod(patch));
            }
        }

        static void Patch_ChangeLeftHandAnime(object __instance, ref int _no)
        {
            var ociChar = Traverse.Create(__instance).Property(charaParamName).GetValue<OCIChar>();
            foreach(var chara in CharaStateX.GetSelectedCharacters().Where((x) => x != ociChar))
                chara.ChangeHandAnime(0, _no);
        }

        static void Patch_ChangeRightHandAnime(object __instance, ref int _no)
        {
            var ociChar = Traverse.Create(__instance).Property(charaParamName).GetValue<OCIChar>();
            foreach(var chara in CharaStateX.GetSelectedCharacters().Where((x) => x != ociChar))
                chara.ChangeHandAnime(1, _no);
        }
    }
}
