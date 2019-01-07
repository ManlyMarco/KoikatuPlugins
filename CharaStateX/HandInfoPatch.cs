using System.Collections.Generic;
using System.Linq;
using Harmony;
using Studio;

namespace CharaStateX
{
    static class HandInfoPatch
    {
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

        static List<OCIChar> GetSelectedCharacters()
        {
            return GuideObjectManager.Instance.selectObjectKey.Select(x => Studio.Studio.GetCtrlInfo(x) as OCIChar).Where(x => x != null).ToList();
        }

        static void Patch_ChangeLeftHandAnime(ref int _no)
        {
            foreach(var chara in GetSelectedCharacters())
                chara.ChangeHandAnime(0, _no);
        }

        static void Patch_ChangeRightHandAnime(ref int _no)
        {
            foreach(var chara in GetSelectedCharacters())
                chara.ChangeHandAnime(1, _no);
        }
    }
}
