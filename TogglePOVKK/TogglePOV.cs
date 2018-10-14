using BepInEx;
using UnityEngine.SceneManagement;

namespace TogglePOVKK
{
    [BepInPlugin("togglepovkk", "TogglePOVKK", "1.0.0")]
    public class TogglePOV : BaseUnityPlugin
    {
        void Awake()
        {
            SceneLoaded();
            SceneManager.sceneLoaded += SceneLoaded;
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
            var comp = gameObject.GetComponent<BaseMono>();

            if(FindObjectOfType<HScene>())
            {
                if(!comp) gameObject.AddComponent<HSceneMono>();
            }
            else if(comp)
            {
                Destroy(comp);
                Destroy(gameObject.GetComponent<DragManager>());
            }
        }
    }
}
