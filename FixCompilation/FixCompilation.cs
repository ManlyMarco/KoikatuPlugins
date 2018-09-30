using System.ComponentModel;
using BepInEx;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace FixCompilation
{
    [BepInPlugin("keelhauled.fixcompilation", "FixCompilation", "1.0.1")]
    public class FixCompilation : BaseUnityPlugin
    {
        [DisplayName("Disable \"NEW\" indicator animation")]
        [Description("Major performance improvement in maker if there are many new items. Almost no visual effect.")]
        public static ConfigWrapper<bool> DisableNewAnimation { get; set; }

        [DisplayName("Disable \"NEW\" indicator for new items")]
        [Description("Good performance improvement in maker if there are many new items. Changes take effect after maker restart.")]
        public static ConfigWrapper<bool> DisableNewIndicator { get; set; }

        [DisplayName("Disable maker IK")]
        [Description("Improves performance and reduces stuttering at the cost of not recalculating positions of some body parts.\n" +
                     "Most noticeable on characters with wide hips where the hands are not moving with the hip line.")]
        public static ConfigWrapper<bool> DisableIKCalc { get; set; }

        [DisplayName("Disable camera target (white focus ring)")]
        [Description("Warning: This setting overrides any game setting that enables the ring.")]
        public static ConfigWrapper<bool> DisableCameraTarget { get; set; }

        [DisplayName("Disable character name box in maker")]
        [Description("Hides the name box in the bottom right part of the maker, giving you a clearer look at the character.")]
        public static ConfigWrapper<bool> DisableCharaName { get; set; }

        public FixCompilation()
        {
            DisableNewAnimation = new ConfigWrapper<bool>("DisableNewAnimation", this, true);
            DisableNewIndicator = new ConfigWrapper<bool>("DisableNewIndicator", this, true);
            DisableIKCalc = new ConfigWrapper<bool>("DisableIKCalc", this, true);
            DisableCameraTarget = new ConfigWrapper<bool>("DisableCameraTarget", this, false);
            DisableCharaName = new ConfigWrapper<bool>("DisableCharaName", this, true);
        }

        protected void Awake()
        {
            SceneManager.sceneLoaded += SceneLoaded;
            DisableCameraTarget.SettingChanged += (sender, args) => ApplyPatches();
            DisableCharaName.SettingChanged += (sender, args) => ApplyPatches();

            CustomHairComponentFix.Patch();

            MakerOptimization.Patch();
        }

        private static void SceneLoaded(Scene scene, LoadSceneMode mode)
        {
            //QualitySettings.shadowProjection = ShadowProjection.CloseFit;
            //QualitySettings.shadowResolution = ShadowResolution.VeryHigh;

            ApplyPatches();
        }

        private static void ApplyPatches()
        {
            if (FindObjectOfType<StudioScene>())
            {
                GameObject.Find("StudioScene/Camera/Main Camera/CameraTarget")?.SetActive(!DisableCameraTarget.Value);
            }
            else if (SceneManager.GetActiveScene().name == "CustomScene")
            {
                GameObject.Find("CustomScene/CamBase/Camera/CameraTarget")?.SetActive(!DisableCameraTarget.Value);

                GameObject.Find("CustomScene/CustomRoot/FrontUIGroup/CustomUIGroup/CvsCharaName")?.SetActive(!DisableCharaName.Value);
            }
            else if (SceneManager.GetActiveScene().name == "H")
            {
                GameObject.Find("HScene/CameraBase/Camera/CameraTarget")?.SetActive(!DisableCameraTarget.Value);
            }
        }
    }
}
