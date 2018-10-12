using System.ComponentModel;
using System.IO;
using System.Reflection;
using BepInEx;
using MessagePack;
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

        string savePath;
        ParamData data = new ParamData();
        CharacterParam charaParam;
        SceneParam sceneParam;

        DefaultParamEditor()
        {
            savePath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "DefaultParamEditorData.bin");

            if(File.Exists(savePath))
                data = MessagePackSerializer.Deserialize<ParamData>(File.ReadAllBytes(savePath));
        }

        void Save()
        {
            var bytes = MessagePackSerializer.Serialize(data);
            File.WriteAllBytes(savePath, bytes);
            data.PrintData();
        }

        void SaveCharaParam()
        {
            if(GUILayout.Button("Save", GUILayout.ExpandWidth(true)))
            {
                charaParam.Save();
                Save();
            }
        }

        void SaveSceneParam()
        {
            if(GUILayout.Button("Save", GUILayout.ExpandWidth(true)))
            {
                sceneParam.Save();
                Save();
            }
        }

        void Awake()
        {
            charaParam = new CharacterParam(data.charaParamData);
            sceneParam = new SceneParam(data.sceneParamData);
        }
    }
}
