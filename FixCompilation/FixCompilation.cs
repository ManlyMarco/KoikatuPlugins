using System;
using System.ComponentModel;
using BepInEx;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace FixCompilation
{
    [BepInPlugin("keelhauled.fixcompilation", "FixCompilation", "1.0.0")]
    public class FixCompilation : BaseUnityPlugin
    {
        [DisplayName("Disable animation for new items")]
        [Description("Changes take effect after game restart.")]
        public static ConfigWrapper<bool> DisableNewAnimation { get; set; }

        [DisplayName("Disable indicator for new items")]
        [Description("Changes take effect after game restart.")]
        public static ConfigWrapper<bool> DisableNewIndicator { get; set; }

        [DisplayName("Disable maker IK")]
        [Description("Disabling IK increases performance and reduces stuttering but the lack of it is especially noticeable on characters with wide hips.\nChanges take effect after game restart.")]
        public static ConfigWrapper<bool> DisableIKCalc { get; set; }

        [DisplayName("Disable cameratarget")]
        [Description("Changes take effect after scene change.")]
        public static ConfigWrapper<bool> DisableCameraTarget { get; set; }

        [DisplayName("Disable character name")]
        [Description("Changes take effect after scene change.")]
        public static ConfigWrapper<bool> DisableCharaName { get; set; }

        FixCompilation()
        {
            DisableNewAnimation = new ConfigWrapper<bool>("DisableNewAnimation", this, true);
            DisableNewIndicator = new ConfigWrapper<bool>("DisableNewIndicator", this, true);
            DisableIKCalc = new ConfigWrapper<bool>("DisableIKCalc", this, true);
            DisableCameraTarget = new ConfigWrapper<bool>("DisableCameraTarget", this, true);
            DisableCharaName = new ConfigWrapper<bool>("DisableCharaName", this, true);
        }

        void Awake()
        {
            SceneManager.sceneLoaded += (scene, mode) => SceneLoaded();
            CustomHairComponentFix.Patch();
            MakerOptimization.Patch();
        }

        static void SceneLoaded()
        {
            //QualitySettings.shadowProjection = ShadowProjection.CloseFit;
            //QualitySettings.shadowResolution = ShadowResolution.VeryHigh;

            if(FindObjectOfType<StudioScene>())
            {
                if(DisableCameraTarget.Value)
                    GameObject.Find("StudioScene/Camera/Main Camera/CameraTarget")?.SetActive(false);
            }
            else if(SceneManager.GetActiveScene().name == "CustomScene")
            {
                if(DisableCameraTarget.Value)
                    GameObject.Find("CustomScene/CamBase/Camera/CameraTarget")?.SetActive(false);

                if(DisableCharaName.Value)
                    GameObject.Find("CustomScene/CustomRoot/FrontUIGroup/CustomUIGroup/CvsCharaName")?.SetActive(false);
            }
            else if(SceneManager.GetActiveScene().name == "H")
            {
                if(DisableCameraTarget.Value)
                    GameObject.Find("HScene/CameraBase/Camera/CameraTarget")?.SetActive(false);
            }
        }
    }
}
