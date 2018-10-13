using System;
using BepInEx;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TogglePOVKK
{
    [BepInPlugin("togglepovkk", "TogglePOVKK", "1.0.0")]
    public class TogglePOV : BaseUnityPlugin
    {
        void Awake()
        {
            if(Application.productName == "CharaStudio")
            {

            }
            else
            {
                SceneLoaded();
                SceneManager.sceneLoaded += SceneLoaded;
            }
        }

        void OnDestroy()
        {
            SceneManager.sceneLoaded -= SceneLoaded;
        }

        void SceneLoaded(Scene scene, LoadSceneMode mode)
        {
            SceneLoaded();
        }

        void SceneLoaded()
        {
            var hsceneObject = gameObject.GetComponent<HSceneMono>();

            if(FindObjectOfType<HScene>())
            {
                if(!hsceneObject)
                {
                    gameObject.AddComponent<HSceneMono>();
                }
            }
            else if(hsceneObject)
            {
                Destroy(hsceneObject);
                Destroy(gameObject.GetComponent<DragManager>());
            }
        }
    }
}
