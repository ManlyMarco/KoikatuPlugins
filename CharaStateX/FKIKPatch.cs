using System.Linq;
using Harmony;
using Studio;

namespace CharaStateX
{
    static class FKIKPatch
    {
        static string charaParamName = "ociChar";

        public static void Patch(HarmonyInstance harmony)
        {
            {
                var type = typeof(MPCharCtrl).GetNestedType("FKInfo", AccessTools.all);
                var target = AccessTools.Method(type, "OnChangeValueFunction");
                var patch = AccessTools.Method(typeof(FKIKPatch), nameof(Patch_FKInfo_OnChangeValueFunction));
                harmony.Patch(target, null, new HarmonyMethod(patch));
            }

            {
                var type = typeof(MPCharCtrl).GetNestedType("IKInfo", AccessTools.all);
                var target = AccessTools.Method(type, "OnChangeValueFunction");
                var patch = AccessTools.Method(typeof(FKIKPatch), nameof(Patch_IKInfo_OnChangeValueFunction));
                harmony.Patch(target, null, new HarmonyMethod(patch));
            }
        }

        static void Patch_FKInfo_OnChangeValueFunction(object __instance, ref bool _value)
        {
            var ociChar = Traverse.Create(__instance).Property(charaParamName).GetValue<OCIChar>();
            foreach(var chara in CharaStateX.GetSelectedCharacters().Where((x) => x != ociChar))
                chara.ActiveKinematicMode(OICharInfo.KinematicMode.FK, _value, false);
        }

        static void Patch_IKInfo_OnChangeValueFunction(object __instance, ref bool _value)
        {
            var ociChar = Traverse.Create(__instance).Property(charaParamName).GetValue<OCIChar>();
            foreach(var chara in CharaStateX.GetSelectedCharacters().Where((x) => x != ociChar))
                chara.ActiveKinematicMode(OICharInfo.KinematicMode.IK, _value, false);
        }
    }
}
