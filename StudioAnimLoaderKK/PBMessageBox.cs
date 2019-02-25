using UnityEngine;

namespace StudioAnimLoader
{
    internal class PBMessageBox : MonoBehaviour
    {
        public void Init(float width, float height, float screenWidth, float screenHeight, string message, float time)
        {
            rect = new Rect((screenWidth - width) / 2f, topOffset, width, height);
            this.message = message;
            this.time = time;
            initialized = true;
        }

        private void OnGUI()
        {
            if(initialized)
            {
                GUIStyle guistyle = new GUIStyle(GUI.skin.textArea);
                guistyle.alignment = TextAnchor.MiddleCenter;
                GUI.Label(rect, message, guistyle);
            }
        }

        private void Start()
        {
            Destroy(gameObject, time);
        }

        private float topOffset = 100f;
        private Rect rect;
        private string message;
        private float time;
        private bool initialized;
    }
}
