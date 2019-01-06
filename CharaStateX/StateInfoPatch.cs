using System;
using System.Collections.Generic;
using System.Linq;
using Harmony;
using Studio;

namespace CharaStateX
{
    static class StateInfoPatch
    {
        static HarmonyInstance harmony;
        static Type stateInfoType;

        public static void Patch(HarmonyInstance harmonyInstance)
        {
            harmony = harmonyInstance;
            stateInfoType = typeof(MPCharCtrl).GetNestedType("StateInfo", AccessTools.all);
            PatchStateInfoMethod("OnClickCosType", nameof(Patch_OnClickCosType));
            PatchStateInfoMethod("OnClickShoesType", nameof(Patch_OnClickShoesType));
            PatchStateInfoMethod("OnClickCosState", nameof(Patch_OnClickCosState));
            PatchStateInfoMethod("OnClickClothingDetails", nameof(Patch_OnClickClothingDetails));
            PatchStateInfoMethod("OnClickAccessories", nameof(Patch_OnClickAccessories));
            PatchStateInfoMethod("OnClickLiquid", nameof(Patch_OnClickLiquid));
            PatchStateInfoMethod("OnClickTears", nameof(Patch_OnClickTears));
            PatchStateInfoMethod("OnValueChangedCheek", nameof(Patch_OnValueChangedCheek));
            PatchStateInfoMethod("OnValueChangedNipple", nameof(Patch_OnValueChangedNipple));
            PatchStateInfoMethod("OnValueChangedSon", nameof(Patch_OnValueChangedSon));
            PatchStateInfoMethod("OnValueChangedSonLength", nameof(Patch_OnValueChangedSonLength));
        }

        static void PatchStateInfoMethod(string targetName, string patchName)
        {
            var target = AccessTools.Method(stateInfoType, targetName);
            var patch = AccessTools.Method(typeof(StateInfoPatch), patchName);
            harmony.Patch(target, new HarmonyMethod(patch), null);
        }

        static List<OCIChar> GetSelectedCharacters()
        {
            return GuideObjectManager.Instance.selectObjectKey.Select(x => Studio.Studio.GetCtrlInfo(x) as OCIChar).Where(x => x != null).ToList();
        }

        static void Patch_OnClickCosType(ref int _value)
        {
            foreach(var chara in GetSelectedCharacters())
                chara.SetCoordinateInfo((ChaFileDefine.CoordinateType)_value, false);
        }

        static void Patch_OnClickShoesType(ref int _value)
        {
            foreach(var chara in GetSelectedCharacters())
                chara.SetShoesType(_value);
        }

        static void Patch_OnClickCosState(ref int _value)
        {
            foreach(var chara in GetSelectedCharacters())
                chara.SetClothesStateAll(_value);
        }

        static void Patch_OnClickClothingDetails(ref int _id, ref byte _state)
        {
            foreach(var chara in GetSelectedCharacters())
                chara.SetClothesState(_id, _state);
        }

        static void Patch_OnClickAccessories(ref int _id, ref bool _flag)
        {
            foreach(var chara in GetSelectedCharacters())
                chara.ShowAccessory(_id, _flag);
        }

        static void Patch_OnClickLiquid(ref ChaFileDefine.SiruParts _parts, ref byte _state)
        {
            foreach(var chara in GetSelectedCharacters())
                chara.SetSiruFlags(_parts, _state);
        }

        static void Patch_OnClickTears(ref byte _state)
        {
            foreach(var chara in GetSelectedCharacters())
                chara.SetTearsLv(_state);
        }

        static void Patch_OnValueChangedCheek(ref float _value)
        {
            foreach(var chara in GetSelectedCharacters())
                chara.SetHohoAkaRate(_value);
        }

        static void Patch_OnValueChangedNipple(ref float _value)
        {
            foreach(var chara in GetSelectedCharacters())
                chara.SetNipStand(_value);
        }

        static void Patch_OnValueChangedSon(ref bool _value)
        {
            foreach(var chara in GetSelectedCharacters())
                chara.SetVisibleSon(_value);
        }

        static void Patch_OnValueChangedSonLength(ref float _value)
        {
            foreach(var chara in GetSelectedCharacters())
                chara.SetSonLength(_value);
        }
    }
}
