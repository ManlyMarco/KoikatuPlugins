using System.ComponentModel;
using BepInEx;
using UnityEngine;

namespace DefaultParamEditor
{
    [BepInProcess("CharaStudio")]
    [BepInPlugin("keelhauled.defaultparameditor", "DefaultParamEditor", "1.0.0")]
    class DefaultParamEditor : BaseUnityPlugin
    {
        [Browsable(true)]
        [DisplayName("Save character parameters")]
        [CustomSettingDraw(nameof(SaveCharaParam))]
        string SaveButton1 { get; set; } = "";

        [Browsable(true)]
        [DisplayName("Save scene parameters")]
        [CustomSettingDraw(nameof(SaveSceneParam))]
        string SaveButton2 { get; set; } = "";

        CharacterParam charaParam;
        SceneParam sceneParam;

        void SaveCharaParam()
        {
            if(GUILayout.Button("Save", GUILayout.ExpandWidth(true)))
                charaParam.Save();
        }

        void SaveSceneParam()
        {
            if(GUILayout.Button("Save", GUILayout.ExpandWidth(true)))
                sceneParam.Save();
        }

        void Awake()
        {
            charaParam = new CharacterParam();
            sceneParam = new SceneParam();
        }
    }
}
