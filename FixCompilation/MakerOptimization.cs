using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BepInEx;
using Harmony;
using ChaCustom;
using Illusion.Extensions;
using UnityEngine;
using UnityEngine.UI;

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
                typeof(MakerOptimization).GetMethod(nameof(HarmonyPatch_CustomSelectInfoComponent_Disvisible), FullBindingFlags),
                FixCompilation.DisableNewIndicator);

            SetupSetting(harmony,
                typeof(CustomNewAnime).GetMethod("Update", FullBindingFlags),
                typeof(MakerOptimization).GetMethod(nameof(HarmonyPatch_CustomNewAnime_Update), FullBindingFlags),
                FixCompilation.DisableNewAnimation);

            if (FixCompilation.DisableIKCalc.Value)
            {
                var replace = typeof(CustomBase).GetMethod("UpdateIKCalc", FullBindingFlags);
                var prefix = typeof(MakerOptimization).GetMethod(nameof(HarmonyPatch_CustomBase_UpdateIKCalc), FullBindingFlags);
                harmony.Patch(replace, new HarmonyMethod(prefix), null);
            }

            {
                var replace = typeof(CustomScene).GetMethod("Start", FullBindingFlags);
                var prefix = typeof(MakerOptimization).GetMethod(nameof(MakerStartHook), FullBindingFlags);
                harmony.Patch(replace, null, new HarmonyMethod(prefix));
            }
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

        public static void MakerStartHook(CustomScene __instance)
        {
            __instance.StartCoroutine(OnMakerLoaded());
        }

        private static IEnumerator OnMakerLoaded()
        {
            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();

            // Put logic to run after maker is loaded here

            if (FixCompilation.DisableHiddenTabs.Value)
            {
                /* Tried and not working:
                 * treeGroup.interactable and treeItem.SetActive Doesn't do anything
                 * Splitting tabs into separate canvases makes fps worse but stable
                 * Disabling treeGroup.GetComponentsInChildren<UI_RaycastCtrl>() doesn't do much
                 * SetActive on top tab groups gives best fps but takes long to switch
                 * Changing tab parent out of canvas is same as setactive, canvas needs to recalculate everything
                 */

                var kkKiyaseExists = GameObject.Find("KK_Kiyase") != null;
                var treeTop = GameObject.Find("CvsMenuTree");

                foreach (Transform mainTab in treeTop.transform)
                {
                    var topMenuToggle = CanvasObjectLinks.TryGetValue(mainTab.name, out var topTabName)
                        ? GameObject.Find(topTabName)?.GetComponent<Toggle>()
                        : null;

                    var updateTabCallbacks = new List<Action>();
                    foreach (Transform subTab in mainTab)
                    {
                        var toggle = subTab.GetComponent<Toggle>();
                        if (toggle == null) continue;

                        var innerContent = subTab.Cast<Transform>().FirstOrDefault(x =>
                        {
                            // Needed for KK_Kiyase to not crash, it uses slides under this tab
                            if (kkKiyaseExists && x.GetComponent<CvsBreast>() != null) return false;

                            // Tab pages have raycast controllers on them, buttons have only image
                            return x.GetComponent<UI_RaycastCtrl>() != null;
                        })?.gameObject;
                        if (innerContent == null) continue;

                        void SetTabActive(bool val)
                        {
                            innerContent.SetActive(val && (topMenuToggle == null || topMenuToggle.isOn));
                        }

                        toggle.onValueChanged.AddListener(SetTabActive);
                        updateTabCallbacks.Add(() => SetTabActive(toggle.isOn));
                    }

                    topMenuToggle?.onValueChanged.AddListener(val =>
                    {
                        foreach (var callback in updateTabCallbacks)
                            callback();
                    });

                    foreach (var callback in updateTabCallbacks)
                        callback();
                }
            }
        }

        /// <summary>
        /// Because Illusion can't make consistent names. There's probably a better way.
        /// </summary>
        private static readonly Dictionary<string, string> CanvasObjectLinks = new Dictionary<string, string>
        {
            {"00_FaceTop"      , "tglFace"       },
            {"01_BodyTop"      , "tglBody"       },
            {"02_HairTop"      , "tglHair"       },
            {"03_ClothesTop"   , "tglCoordinate" },
            {"04_AccessoryTop" , "tglAccessories"},
            {"05_ParameterTop" , "tglParameter"  },
            {"06_SystemTop"    , "tglSystem"     },
        };
    }
}
