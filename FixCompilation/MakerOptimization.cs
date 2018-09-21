using System;
using System.Reflection;
using Harmony;
using ChaCustom;
using Illusion.Extensions;

namespace FixCompilation
{
    public static class MakerOptimization
    {
        public static void Patch()
        {
            var harmony = HarmonyInstance.Create("keelhauled.fixcompilation.makeroptimization.harmony");
            var bindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static;

            if(FixCompilation.DisableNewAnimation.Value)
            {
                var replace = typeof(CustomNewAnime).GetMethod("Update", bindingFlags);
                var prefix = typeof(MakerOptimization).GetMethod("HarmonyPatch_CustomNewAnime_Update", bindingFlags);
                harmony.Patch(replace, new HarmonyMethod(prefix), null); 
            }

            if(FixCompilation.DisableNewIndicator.Value)
            {
                var replace = typeof(CustomSelectInfoComponent).GetMethod("Disvisible", bindingFlags);
                var prefix = typeof(MakerOptimization).GetMethod("HarmonyPatch_CustomSelectInfoComponent_Disvisible", bindingFlags);
                harmony.Patch(replace, new HarmonyMethod(prefix), null);
            }

            if(FixCompilation.DisableIKCalc.Value)
            {
                var replace = typeof(CustomBase).GetMethod("UpdateIKCalc", bindingFlags);
                var prefix = typeof(MakerOptimization).GetMethod("HarmonyPatch_CustomBase_UpdateIKCalc", bindingFlags);
                harmony.Patch(replace, new HarmonyMethod(prefix), null);
            }
        }

        // Stop animation on new items
        static bool HarmonyPatch_CustomNewAnime_Update()
        {
            return false;
        }

        // Disable indicator for new items
        static void HarmonyPatch_CustomSelectInfoComponent_Disvisible(CustomSelectInfoComponent __instance)
        {
            __instance.objNew.SetActiveIfDifferent(false);
        }

        // Disable heavy method I know nothing about
        static bool HarmonyPatch_CustomBase_UpdateIKCalc()
        {
            return false;
        }
    }
}
