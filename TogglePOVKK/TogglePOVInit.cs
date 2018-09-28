using System;
using BepInEx;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TogglePOVKK
{
    [BepInPlugin("togglepov", "TogglePOVKK", "1.0.0")]
    public class TogglePOVInit : BaseUnityPlugin
    {
        void Awake()
        {
            SceneManager.sceneLoaded += SceneLoaded;
        }

        void OnDestroy() // for ScriptEngine
        {
            SceneManager.sceneLoaded -= SceneLoaded;
        }

        void SceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if(!gameObject.GetComponent<KKMono>())
            {
                gameObject.AddComponent<KKMono>();
            }
        }
    }
}
