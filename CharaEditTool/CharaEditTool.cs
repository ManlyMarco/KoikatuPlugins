using System;
using System.ComponentModel;
using BepInEx;
using UnityEngine;
using Harmony;
using ChaCustom;
using static BepInEx.Logger;
using BepInEx.Logging;
using UnityEngine.SceneManagement;
using System.Collections;

namespace CharaEditTool
{
    [BepInPlugin("keelhauled.charaedittool", "CharaEditTool", "1.0.0")]
    class CharaEditTool : BaseUnityPlugin
    {
        Color? savedColor;
        bool inScene = false;

        [Description("Press hotkey to save the selected color.\n" +
                     "Hold hotkey to apply the saved color to every accessory color slot that has the same color as the selected color.")]
        [DisplayName("Save/apply color")]
        SavedKeyboardShortcut ColorCopyKey { get; }
        Hotkey colorCopyHotkey;

        CharaEditTool()
        {
            ColorCopyKey = new SavedKeyboardShortcut("ColorCopyKey", this, new KeyboardShortcut(KeyCode.V));
        }

        void Awake()
        {
            SceneLoaded();
            SceneManager.sceneLoaded += SceneLoaded;
            colorCopyHotkey = new Hotkey(ColorCopyKey, 0.4f);
        }

        void OnDestroy() // for ScriptEngine
        {
            SceneManager.sceneLoaded -= SceneLoaded;
        }

        void SceneLoaded(Scene scene, LoadSceneMode mode)
        {
            SceneLoaded();
        }

        void SceneLoaded()
        {
            var customScene = FindObjectOfType<CustomScene>();

            if(customScene)
            {
                if(!inScene)
                {
                    inScene = true;
                    StartCoroutine(InputCheck());
                }
            }
            else
            {
                inScene = false;
            }
        }

        IEnumerator InputCheck()
        {
            while(inScene)
            {
                colorCopyHotkey.KeyHoldAction(SetColor);
                colorCopyHotkey.KeyUpAction(SaveColor);
                yield return null;
            }
        }

        void SaveColor()
        {
            savedColor = GetSelectedColor();
            if(savedColor != null)
                Log(LogLevel.Message, $"Color saved: {savedColor.Value}");
            else
                Log(LogLevel.Message, "One of the colors in an accessory has to be selected to save.");
        }

        void SetColor()
        {
            if(savedColor != null)
            {
                var chara = CustomBase.Instance.chaCtrl;
                var accMenu = FindObjectOfType<CustomAcsChangeSlot>();
                var cvsAccessory = Traverse.Create(accMenu).Field("cvsAccessory").GetValue<CvsAccessory[]>();
                var selectedColor = GetSelectedColor();

                foreach(var acc in cvsAccessory)
                {
                    var color0 = chara.nowCoordinate.accessory.parts[(int)acc.slotNo].color[0];
                    if(color0 == selectedColor) acc.UpdateAcsColor01(savedColor.Value);

                    var color1 = chara.nowCoordinate.accessory.parts[(int)acc.slotNo].color[1];
                    if(color1 == selectedColor) acc.UpdateAcsColor02(savedColor.Value);

                    var color2 = chara.nowCoordinate.accessory.parts[(int)acc.slotNo].color[2];
                    if(color2 == selectedColor) acc.UpdateAcsColor03(savedColor.Value);
                }

                Log(LogLevel.Message, "Color applied to accessories.");
            }
            else
            {
                Log(LogLevel.Message, "Save a color before applying it to the accessories.");
            }
        }

        Color? GetSelectedColor()
        {
            try
            {
                var chara = CustomBase.Instance.chaCtrl;
                var accMenu = FindObjectOfType<CustomAcsChangeSlot>();
                var cvsAccessory = Traverse.Create(accMenu).Field("cvsAccessory").GetValue<CvsAccessory[]>();
                int selectIndex = accMenu.GetSelectIndex();
                var selected = cvsAccessory[selectIndex];
                var cvsColor = Traverse.Create(selected).Field("cvsColor").GetValue<CvsColor>();

                if(cvsColor.connectColorKind != CvsColor.ConnectColorKind.None)
                {
                    string kind = cvsColor.connectColorKind.ToString();
                    int selectedColorIndex = int.Parse(kind.Substring(kind.Length - 1)) - 1;
                    return chara.nowCoordinate.accessory.parts[(int)selected.slotNo].color[selectedColorIndex];
                }
            }
            catch(Exception)
            {
                
            }

            return null;
        }
    }
}
