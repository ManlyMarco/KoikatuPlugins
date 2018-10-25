using System.Collections;
using UnityEngine;
using Harmony;
using Studio;

namespace FixCompilation
{
    public static class YamadamodFix
    {
        public static void Patch()
        {
            var harmony = HarmonyInstance.Create("keelhauled.fixcompilation.yamadamodfix.harmony");
            harmony.PatchAll(typeof(YamadamodFix));
        }

        [HarmonyPrefix, HarmonyPatch(typeof(ChaCustomHairComponent), "Update")]
        public static bool HarmonyPatch_ChaCustomHairComponent_Update(ChaCustomHairComponent __instance)
        {
            if(__instance.trfLength != null)
            {
                for(int i = 0; i < __instance.trfLength.Length; i++)
                {
                    var transform = __instance.trfLength[i];
                    if(transform) // Check trfLength[i] before using it
                    {
                        float y = __instance.baseLength[i] + Mathf.Lerp(0f, __instance.addLength, __instance.lengthRate);
                        transform.localPosition = new Vector3(transform.localPosition.x, y, transform.localPosition.z);
                    }
                }
            }

            return false;
        }

        //[HarmonyPrefix, HarmonyPatch(typeof(GuideObject), "LateUpdate")]
        //public static bool HarmonyPatch_GuideObject_LateUpdate(GuideObject __instance)
        //{
        //    if(__instance.transformTarget) return true;

        //    var traverse = Traverse.Create(__instance);
        //    var roots = traverse.Field("roots").GetValue<GameObject[]>();

        //    if(__instance.parent && __instance.nonconnect)
        //    {
        //        traverse.Method("CalcPosition").GetValue();
        //        traverse.Method("CalcRotation").GetValue();
        //    }

        //    var mode = __instance.mode;
        //    if(mode != GuideObject.Mode.Local)
        //    {
        //        if(mode == GuideObject.Mode.World)
        //            roots[0].transform.eulerAngles = Vector3.zero;
        //    }
        //    else
        //    {
        //        roots[0].transform.eulerAngles = ((!__instance.parent) ? Vector3.zero : __instance.parent.eulerAngles);
        //    }

        //    return false;
        //}

        //[HarmonyPrefix, HarmonyPatch(typeof(FKCtrl), "LateUpdate")]
        //public static bool HarmonyPatch_FKCtrl_LateUpdate(FKCtrl __instance)
        //{
        //    var traverse = Traverse.Create(__instance);

        //    int count = traverse.Property("count").GetValue<int>();
        //    for(int i = 0; i < count; i++)
        //    {
        //        var listBones = traverse.Field("listBones").GetValue<IList>();
        //        if(listBones[i] != null)
        //        {
        //            var bone = Traverse.Create(listBones[i]);
        //            if(bone.Property("gameObject").GetValue<GameObject>())
        //                bone.Method("Update").GetValue();
        //        }
        //    }

        //    return false;
        //}
    }
}
