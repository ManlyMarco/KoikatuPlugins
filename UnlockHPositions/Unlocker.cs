using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Harmony;
using BepInEx;
using Manager;

namespace UnlockHPositions
{
    [BepInPlugin("keelhauled.unlockhpositions", "H Position Unlocker", "1.0.0")]
    public class Unlocker : BaseUnityPlugin
    {
        void Awake()
        {
            var harmony = HarmonyInstance.Create("keelhauled.unlockhpositions.harmony");
            harmony.PatchAll(typeof(HarmonyPatches)); ;
        }
    }

    public static class HarmonyPatches
    {
        [HarmonyPrefix, HarmonyPatch(typeof(HSceneProc), "CreateListAnimationFileName")]
        public static bool HarmonyPatch_HSceneProc_CreateListAnimationFileName(HSceneProc __instance, ref bool _isAnimListCreate, ref int _list)
        {
            var traverse = Traverse.Create(__instance);

            if(_isAnimListCreate)
                traverse.Method("CreateAllAnimationList").GetValue();

            //SaveData saveData = Singleton<Game>.Instance.saveData;
            //Dictionary<int, HashSet<int>> clubContents = saveData.clubContents;

            //clubContents.TryGetValue(1, out HashSet<int> hashSet);
            //if(hashSet == null)
            //    hashSet = new HashSet<int>();

            //var playHList = Singleton<Game>.Instance.glSaveData.playHList;
            //bool flag = __instance.categorys.Any(c => MathfEx.IsRange(1010, c, 1099, true) || MathfEx.IsRange(1100, c, 1199, true));

            var lstAnimInfo = traverse.Field("lstAnimInfo").GetValue<List<HSceneProc.AnimationListInfo>[]>();
            var lstUseAnimInfo = traverse.Field("lstUseAnimInfo").GetValue<List<HSceneProc.AnimationListInfo>[]>();

            for(int i = 0; i < lstAnimInfo.Length; i++)
            {
                lstUseAnimInfo[i] = new List<HSceneProc.AnimationListInfo>();
                if(_list == -1 || i == _list)
                {
                    //if(!playHList.TryGetValue(i, out HashSet<int> hashSet2))
                    //    hashSet2 = new HashSet<int>();

                    //if(!__instance.flags.isFreeH || hashSet2.Count != 0 || flag)
                    //{
                        for(int j = 0; j < lstAnimInfo[i].Count; j++)
                        {
                            if(lstAnimInfo[i][j].lstCategory.Any(c => __instance.categorys.Contains(c.category)))
                            {
                                //if(!__instance.flags.isFreeH)
                                //{
                                //    if(lstAnimInfo[i][j].isRelease && !hashSet.Contains(i * 1000 + lstAnimInfo[i][j].id))
                                //        continue;

                                //    if(lstAnimInfo[i][j].isExperience != 2 && lstAnimInfo[i][j].isExperience > (int)__instance.flags.experience)
                                //        continue;
                                //}
                                //else
                                //{
                                //    if(lstAnimInfo[i][j].stateRestriction > (int)__instance.flags.lstHeroine[0].HExperience)
                                //        continue;

                                //    if(!hashSet2.Contains(lstAnimInfo[i][j].id) && !flag)
                                //        continue;
                                //}

                                lstUseAnimInfo[i].Add(lstAnimInfo[i][j]);
                            }
                        }
                    //}
                }
            }

            return false;
        }
    }
}
