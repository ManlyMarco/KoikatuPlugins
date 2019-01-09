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

            foreach(var chara in GetSelectedCharacters().Where((x) => x != __instance.ociChar))
                PauseCtrl.Load(chara, listPath[select]);
        }

        static float animeOptionParam1;
        static float animeOptionParam2;

        [HarmonyPrefix, HarmonyPatch(typeof(MPCharCtrl), "LoadAnime")]
        public static void LoadAnimePrefix(MPCharCtrl __instance, ref int _group, ref int _category, ref int _no)
        {
            //Console.WriteLine($"Group: {_group} | Category: {_category} | No: {_no}");

            foreach(var chara in GetSelectedCharacters().Where((x) => x != __instance.ociChar))
            {
                float param1 = chara.animeOptionParam1;
                float param2 = chara.animeOptionParam2;
                chara.LoadAnime(MatchCategory(chara.sex, _group), _category, _no, 0f);
                chara.animeOptionParam1 = param1;
                chara.animeOptionParam2 = param2;
            }
            
            _group = MatchCategory(__instance.ociChar.sex, _group);
            animeOptionParam1 = __instance.ociChar.animeOptionParam1;
            animeOptionParam2 = __instance.ociChar.animeOptionParam2;
        }

        [HarmonyPostfix, HarmonyPatch(typeof(MPCharCtrl), "LoadAnime")]
        public static void LoadAnimePostfix(MPCharCtrl __instance)
        {
            __instance.ociChar.animeOptionParam1 = animeOptionParam1;
            __instance.ociChar.animeOptionParam2 = animeOptionParam2;
        }

        static int MatchCategory(int sex, int group)
        {
            if(sex == 1)
            {
                switch(group)
                {
                    case 3: return 2;
                    case 5: return 4;
                }
            }
            else if(sex == 0)
            {
                switch(group)
                {
                    case 2: return 3;
                    case 4: return 5;
                }
            }

            return group;
        }

        static List<OCIChar> GetSelectedCharacters()
        {
            return GuideObjectManager.Instance.selectObjectKey.Select(x => Studio.Studio.GetCtrlInfo(x) as OCIChar).Where(x => x != null).ToList();
        }
    }
}
