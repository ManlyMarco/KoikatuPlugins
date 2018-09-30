using System.Reflection;
using BepInEx;
using Harmony;
using ChaCustom;
using Illusion.Extensions;

namespace FixCompilation
{
    public static class MakerOptimization
    {
        private const BindingFlags FullBindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static;

        public static void Patch()
        {
            var harmony = HarmonyInstance.Create("keelhauled.fixcompilation.makeroptimization.harmony");
            
            SetupSetting(harmony, 
                typeof(CustomSelectInfoComponent).GetMethod("Disvisible", FullBindingFlags), 
                typeof(MakerOptimization).GetMethod("HarmonyPatch_CustomSelectInfoComponent_Disvisible", FullBindingFlags), 
                FixCompilation.DisableNewIndicator);

            SetupSetting(harmony, 
                typeof(CustomNewAnime).GetMethod("Update", FullBindingFlags),
                typeof(MakerOptimization).GetMethod("HarmonyPatch_CustomNewAnime_Update", FullBindingFlags),
                FixCompilation.DisableNewAnimation);

            SetupSetting(harmony, 
                typeof(CustomBase).GetMethod("UpdateIKCalc", FullBindingFlags),
                typeof(MakerOptimization).GetMethod("HarmonyPatch_CustomBase_UpdateIKCalc", FullBindingFlags),
                FixCompilation.DisableIKCalc);
        }

        private static void SetupSetting(HarmonyInstance harmony, MethodInfo targetMethod, MethodInfo patchMethod, ConfigWrapper<bool> targetSetting)
        {
            if (targetSetting.Value)
                harmony.Patch(targetMethod, new HarmonyMethod(patchMethod), null);

            targetSetting.SettingChanged += (sender, args) =>
            {
                if (targetSetting.Value)
                    harmony.Patch(targetMethod, new HarmonyMethod(patchMethod), null);
                else
                    harmony.RemovePatch(targetMethod, patchMethod);
            };
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

        // Disable heavy method with little use
        static bool HarmonyPatch_CustomBase_UpdateIKCalc()
        {
            return false;
        }
    }
}
