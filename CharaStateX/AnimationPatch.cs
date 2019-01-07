using System;
using System.Collections.Generic;
using System.Linq;
using Harmony;
using Studio;

namespace CharaStateX
{
    static class AnimationPatch
    {
        [HarmonyPostfix, HarmonyPatch(typeof(PauseRegistrationList), "OnClickLoad")]
        public static void PoseLoadPatch(PauseRegistrationList __instance)
        {
            var traverse = Traverse.Create(__instance);
            var listPath = traverse.Field("listPath").GetValue<List<string>>();
            var select = traverse.Field("select").GetValue<int>();

            foreach(var chara in GetSelectedCharacters())
            {
                if(chara != __instance.ociChar)
                    PauseCtrl.Load(chara, listPath[select]);
            }
        }

        [HarmonyPostfix, HarmonyPatch(typeof(MPCharCtrl), "LoadAnime")]
        public static void LoadAnimePatch(MPCharCtrl __instance, ref AnimeGroupList.SEX _sex, ref int _group, ref int _category, ref int _no)
        {
            // use _sex to choose the right H anim to ease things further

            foreach(var chara in GetSelectedCharacters())
            {
                if(chara != __instance.ociChar)
                    chara.LoadAnime(_group, _category, _no, 0f);
            }
        }

        static List<OCIChar> GetSelectedCharacters()
        {
            return GuideObjectManager.Instance.selectObjectKey.Select(x => Studio.Studio.GetCtrlInfo(x) as OCIChar).Where(x => x != null).ToList();
        }
    }
}
