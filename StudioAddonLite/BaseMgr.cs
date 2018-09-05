using System;
using UnityEngine;

namespace StudioAddonLite
{
    public class BaseMgr<T> : MonoBehaviour where T : MonoBehaviour
    {
        public static T Instance { get; protected set; }

        public static T Install(GameObject container)
        {
            if(BaseMgr<T>.Instance == null)
            {
                BaseMgr<T>.Instance = container.AddComponent<T>();
                BaseMgr<T>.Instance.Invoke("Init", 0f);
            }
            return BaseMgr<T>.Instance;
        }

        public virtual void Init()
        {
        }
    }
}
