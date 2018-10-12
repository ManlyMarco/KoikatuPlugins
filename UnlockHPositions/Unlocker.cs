using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using Harmony;
using BepInEx;

namespace UnlockHPositions
{
    [BepInPlugin("keelhauled.unlockhpositions", "UnlockHPositions", "1.1.0")]
    public class Unlocker : BaseUnityPlugin
    {
        [DisplayName("Unlock all positions")]
        [Description("Unlocks every possible position, including ones that are not supposed to be used in that spot.\n" +
                     "Scene restart required for changes to take effect.")]
        public static ConfigWrapper<bool> UnlockAll { get; set; }

        Unlocker()
        {
            UnlockAll = new ConfigWrapper<bool>("UnlockAll", this, false);
        }

        void Awake()
        {
            var harmony = HarmonyInstance.Create("keelhauled.unlockhpositions.harmony");
            harmony.PatchAll(typeof(Unlocker));
        }

        [HarmonyPrefix, HarmonyPatch(typeof(HSceneProc), "CreateListAnimationFileName")]
        public static bool HarmonyPatch_HSceneProc_CreateListAnimationFileName(HSceneProc __instance, ref bool _isAnimListCreate, ref int _list)
        {
            var traverse = Traverse.Create(__instance);

            if(_isAnimListCreate)
                traverse.Method("CreateAllAnimationList").GetValue();

            var lstAnimInfo = traverse.Field("lstAnimInfo").GetValue<List<HSceneProc.AnimationListInfo>[]>();
            var lstUseAnimInfo = traverse.Field("lstUseAnimInfo").GetValue<List<HSceneProc.AnimationListInfo>[]>();

            for(int i = 0; i < lstAnimInfo.Length; i++)
            {
                lstUseAnimInfo[i] = new List<HSceneProc.AnimationListInfo>();
                if(_list == -1 || i == _list)
                {
                    for(int j = 0; j < lstAnimInfo[i].Count; j++)
                    {
                        if(UnlockAll.Value || lstAnimInfo[i][j].lstCategory.Any(c => __instance.categorys.Contains(c.category)))
                        {
                            lstUseAnimInfo[i].Add(lstAnimInfo[i][j]);
                        }
                    }
                }
            }

            return false;
        }
    }
}
