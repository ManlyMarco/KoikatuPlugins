using System;
using System.Linq;
using Harmony;
using Studio;

namespace CharaStateX
{
    static class StateInfoPatch
    {
        static HarmonyInstance harmony;
        static Type stateInfoType;
        static string charaParamName = "ociChar";

        public static void Patch(HarmonyInstance harmonyInstance)
        {
            harmony = harmonyInstance;
            stateInfoType = typeof(MPCharCtrl).GetNestedType("StateInfo", AccessTools.all);
            PatchStateInfoMethod("OnClickCosType");
            PatchStateInfoMethod("OnClickShoesType");
            PatchStateInfoMethod("OnClickCosState");
            PatchStateInfoMethod("OnClickClothingDetails");
            PatchStateInfoMethod("OnClickAccessories");
            PatchStateInfoMethod("OnClickLiquid");
            PatchStateInfoMethod("OnClickTears");
            PatchStateInfoMethod("OnValueChangedCheek");
            PatchStateInfoMethod("OnValueChangedNipple");
            PatchStateInfoMethod("OnValueChangedSon");
            PatchStateInfoMethod("OnValueChangedSonLength");
        }

        static void PatchStateInfoMethod(string targetName)
        {
            var target = AccessTools.Method(stateInfoType, targetName);
            var patch = AccessTools.Method(typeof(StateInfoPatch), $"Patch_{targetName}");
            harmony.Patch(target, null, new HarmonyMethod(patch));
        }

        static void Patch_OnClickCosType(object __instance, ref int _value)
        {
            var ociChar = Traverse.Create(__instance).Property(charaParamName).GetValue<OCIChar>();
            foreach(var chara in CharaStateX.GetSelectedCharacters().Where((x) => x != ociChar))
                chara.SetCoordinateInfo((ChaFileDefine.CoordinateType)_value, false);
        }

        static void Patch_OnClickShoesType(object __instance, ref int _value)
        {
            var ociChar = Traverse.Create(__instance).Property(charaParamName).GetValue<OCIChar>();
            foreach(var chara in CharaStateX.GetSelectedCharacters().Where((x) => x != ociChar))
                chara.SetShoesType(_value);
        }

        static void Patch_OnClickCosState(object __instance, ref int _value)
        {
            var ociChar = Traverse.Create(__instance).Property(charaParamName).GetValue<OCIChar>();
            foreach(var chara in CharaStateX.GetSelectedCharacters().Where((x) => x != ociChar))
                chara.SetClothesStateAll(_value);
        }

        static void Patch_OnClickClothingDetails(object __instance, ref int _id, ref byte _state)
        {
            var ociChar = Traverse.Create(__instance).Property(charaParamName).GetValue<OCIChar>();
            foreach(var chara in CharaStateX.GetSelectedCharacters().Where((x) => x != ociChar))
                chara.SetClothesState(_id, _state);
        }

        static void Patch_OnClickAccessories(object __instance, ref int _id, ref bool _flag)
        {
            var ociChar = Traverse.Create(__instance).Property(charaParamName).GetValue<OCIChar>();
            foreach(var chara in CharaStateX.GetSelectedCharacters().Where((x) => x != ociChar))
                chara.ShowAccessory(_id, _flag);
        }

        static void Patch_OnClickLiquid(object __instance, ref ChaFileDefine.SiruParts _parts, ref byte _state)
        {
            var ociChar = Traverse.Create(__instance).Property(charaParamName).GetValue<OCIChar>();
            foreach(var chara in CharaStateX.GetSelectedCharacters().Where((x) => x != ociChar))
                chara.SetSiruFlags(_parts, _state);
        }

        static void Patch_OnClickTears(object __instance, ref byte _state)
        {
            var ociChar = Traverse.Create(__instance).Property(charaParamName).GetValue<OCIChar>();
            foreach(var chara in CharaStateX.GetSelectedCharacters().Where((x) => x != ociChar))
                chara.SetTearsLv(_state);
        }

        static void Patch_OnValueChangedCheek(object __instance, ref float _value)
        {
            var ociChar = Traverse.Create(__instance).Property(charaParamName).GetValue<OCIChar>();
            foreach(var chara in CharaStateX.GetSelectedCharacters().Where((x) => x != ociChar))
                chara.SetHohoAkaRate(_value);
        }

        static void Patch_OnValueChangedNipple(object __instance, ref float _value)
        {
            var ociChar = Traverse.Create(__instance).Property(charaParamName).GetValue<OCIChar>();
            foreach(var chara in CharaStateX.GetSelectedCharacters().Where((x) => x != ociChar))
                chara.SetNipStand(_value);
        }

        static void Patch_OnValueChangedSon(object __instance, ref bool _value)
        {
            var ociChar = Traverse.Create(__instance).Property(charaParamName).GetValue<OCIChar>();
            foreach(var chara in CharaStateX.GetSelectedCharacters().Where((x) => x != ociChar))
                chara.SetVisibleSon(_value);
        }

        static void Patch_OnValueChangedSonLength(object __instance, ref float _value)
        {
            var ociChar = Traverse.Create(__instance).Property(charaParamName).GetValue<OCIChar>();
            foreach(var chara in CharaStateX.GetSelectedCharacters().Where((x) => x != ociChar))
                chara.SetSonLength(_value);
        }
    }
}
