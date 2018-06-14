using BepInEx;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace BetterSceneLoader
{
    [BepInProcess("CharaStudio")]
    [BepInPlugin("com.keelhauled.bettersceneloader", "BetterSceneLoader", "1.1.1")]
    class BetterSceneLoaderInit : BaseUnityPlugin
    {
        void OnLevelWasLoaded(int level)
        {
            if(SceneManager.GetActiveScene().name == "Studio") gameObject.AddComponent<BetterSceneLoader>();
        }
    }
}
