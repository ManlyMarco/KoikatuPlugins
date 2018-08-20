using System;
using BepInEx;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace FixCompilation
{
    [BepInPlugin("keelhauled.fixcompilation", "FixCompilation", "1.0.0")]
    public class FixCompilation : BaseUnityPlugin
    {
        void Awake()
        {
            SceneManager.sceneLoaded += (scene, mode) => Init();
            CustomHairComponentFix.Patch();
        }

        static void Init()
        {
            if(FindObjectOfType<StudioScene>())
            {
                //QualitySettings.shadowProjection = ShadowProjection.CloseFit;
                //QualitySettings.shadowResolution = ShadowResolution.VeryHigh;
                GameObject.Find("StudioScene/Camera/Main Camera/CameraTarget")?.SetActive(false);
            }
            else if(SceneManager.GetActiveScene().name == "CustomScene")
            {
                //QualitySettings.shadowProjection = ShadowProjection.CloseFit;
                //QualitySettings.shadowResolution = ShadowResolution.VeryHigh;
                GameObject.Find("CustomScene/CamBase/Camera/CameraTarget")?.SetActive(false);
            }
            else if(SceneManager.GetActiveScene().name == "H")
            {
                //QualitySettings.shadowProjection = ShadowProjection.CloseFit;
                //QualitySettings.shadowResolution = ShadowResolution.VeryHigh;
                GameObject.Find("HScene/CameraBase/Camera/CameraTarget")?.SetActive(false); ;
            }
        }

        public static void Bootstrap()
        {
            Init();
        }
    }
}
