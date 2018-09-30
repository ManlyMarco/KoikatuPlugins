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
        [DisplayName("Disable \"NEW\" indicator animation")]
        [Description("Major performance improvement in some cases. Changes take effect after game restart.")]
        public static ConfigWrapper<bool> DisableNewAnimation { get; set; }

        [DisplayName("Disable \"NEW\" indicator for new items completely")]
        [Description("Good performance improvement in some cases. Changes take effect after game restart.")]
        public static ConfigWrapper<bool> DisableNewIndicator { get; set; }

        [DisplayName("Disable maker IK")]
        [Description("Disabling IK increases performance and reduces stuttering but the lack of it is especially noticeable on characters with wide hips.\nChanges take effect after game restart.")]
        public static ConfigWrapper<bool> DisableIKCalc { get; set; }

        [DisplayName("Disable camera target (white focus ring)")]
        [Description("Changes take effect after scene change.")]
        public static ConfigWrapper<bool> DisableCameraTarget { get; set; }

        [DisplayName("Disable character name box in maker")]
        [Description("Less clutter on the screen. Changes take effect after scene change.")]
        public static ConfigWrapper<bool> DisableCharaName { get; set; }

        FixCompilation()
        {
            DisableNewAnimation = new ConfigWrapper<bool>("DisableNewAnimation", this, true);
            DisableNewIndicator = new ConfigWrapper<bool>("DisableNewIndicator", this, true);
            DisableIKCalc = new ConfigWrapper<bool>("DisableIKCalc", this, true);
            DisableCameraTarget = new ConfigWrapper<bool>("DisableCameraTarget", this, false);
            DisableCharaName = new ConfigWrapper<bool>("DisableCharaName", this, true);
        }

        void Awake()
        {
            SceneManager.sceneLoaded += SceneLoaded;
            CustomHairComponentFix.Patch();
            MakerOptimization.Patch();
        }

        void SceneLoaded(Scene scene, LoadSceneMode mode)
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
