using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BepInEx;
using UnityEngine.SceneManagement;
using UnityEngine;

namespace TogglePOVKK
{
    [BepInPlugin("somenicedudewhomadetogglepov.togglepov", "TogglePOVKK", "1.0.0")]
    public class TogglePOVInit : BaseUnityPlugin
    {
        const string OBJECTNAME = "TogglePOVKKObject";

        void OnLevelWasLoaded(int level)
        {
            StartMod();
        }

        static void StartMod()
        {
            //Console.WriteLine(SceneManager.GetActiveScene().name);
            //switch(SceneManager.GetActiveScene().name)
            //{
            //    case "StaffAdultRoom":
            //    {
                    new GameObject(OBJECTNAME).AddComponent<KKMono>();
                    //break;
            //    }
            //}
        }

        static void Bootstrap()
        {
            var gameobject = GameObject.Find(OBJECTNAME);
            if(gameobject != null) GameObject.DestroyImmediate(gameobject);
            StartMod();
        }
    }
}
