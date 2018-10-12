using System.ComponentModel;
using System.IO;
using System.Reflection;
using BepInEx;
using BepInEx.Logging;
using MessagePack;
using UnityEngine;
using Logger = BepInEx.Logger;

namespace DefaultParamEditor
{
    [BepInProcess("CharaStudio")]
    [BepInPlugin("keelhauled.defaultparameditor", "DefaultParamEditor", "1.0.1")]
    internal class DefaultParamEditor : BaseUnityPlugin
    {
        private const string ResetValue = "Reset";

        [Browsable(true)]
        [DisplayName("Save character parameters")]
        [CustomSettingDraw(nameof(SaveCharaParam))]
        [DefaultValue(ResetValue)]
        protected string CharaParamSetting
        {
            get => null;
            set
            {
                if (value == ResetValue)
                {
                    Logger.Log(LogLevel.Debug, "Resetting charaParam");
                    charaParam.Reset();
                    Save();
                }
            }
        }

        [Browsable(true)]
        [DisplayName("Save scene parameters")]
        [CustomSettingDraw(nameof(SaveSceneParam))]
        [DefaultValue(ResetValue)]
        protected string SceneParamSetting
        {
            get => null;
            set
            {
                if (value == ResetValue)
                {
                    Logger.Log(LogLevel.Debug, "Resetting sceneParam");
                    sceneParam.Reset();
                    Save();
                }
            }
        }

        private readonly string savePath;
        private ParamData data = new ParamData();
        private CharacterParam charaParam;
        private SceneParam sceneParam;

        public DefaultParamEditor()
        {
            savePath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "DefaultParamEditorData.bin");
        }

        private void Save()
        {
            var bytes = MessagePackSerializer.Serialize(data);
            File.WriteAllBytes(savePath, bytes);
            //data.PrintData();
        }

        private void SaveCharaParam()
        {
            if (GUILayout.Button("Save", GUILayout.ExpandWidth(true)))
            {
                charaParam.Save();
                Save();
            }
        }

        private void SaveSceneParam()
        {
            if (GUILayout.Button("Save", GUILayout.ExpandWidth(true)))
            {
                sceneParam.Save();
                Save();
            }
        }

        protected void Awake()
        {
            if (File.Exists(savePath))
                data = MessagePackSerializer.Deserialize<ParamData>(File.ReadAllBytes(savePath));

            charaParam = new CharacterParam(data.charaParamData);
            sceneParam = new SceneParam(data.sceneParamData);
        }
    }
}
