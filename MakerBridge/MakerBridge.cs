using BepInEx;
using Harmony;
using UnityEngine;
using MakerBridge.Remoting;

namespace MakerBridge
{
    [BepInPlugin("keelhauled.makerbridge", "MakerBridge", "1.0.0")]
    public class MakerBridge : BaseUnityPlugin
    {
        public static string ServerName = "MakerBridge";
        public static int ServerPort = 9236;

        public static SavedKeyboardShortcut SendChara { get; set; }

        static GameObject container;

        MakerBridge()
        {
            SendChara = new SavedKeyboardShortcut("SendChara", this, new KeyboardShortcut(KeyCode.C));
        }

        void Awake()
        {
            RPCServer.Start(ServerName, ServerPort);

            container = new GameObject("MakerBridge");
            container.transform.SetParent(gameObject.transform);

            var harmony = HarmonyInstance.Create("keelhauled.makerbridge.harmony");
            harmony.PatchAll(typeof(MakerBridge));
        }

        void OnDestroy()
        {
            RPCServer.Stop();
        }

        [HarmonyPrefix, HarmonyPatch(typeof(CustomScene), "Start")]
        public static void CustomSceneInit()
        {
            var comp = container.GetComponent<MakerSender>();
            if(!comp) container.AddComponent<MakerSender>();
        }

        [HarmonyPrefix, HarmonyPatch(typeof(CustomScene), "OnDestroy")]
        public static void CustomSceneStop()
        {
            var comp = container.GetComponent<MakerSender>();
            if(comp) Destroy(comp);
        }

        [HarmonyPrefix, HarmonyPatch(typeof(StudioScene), "Start")]
        public static void StudioSceneInit()
        {
            var comp = container.GetComponent<StudioReceive>();
            if(!comp) container.AddComponent<StudioReceive>();
        }
    }
}
