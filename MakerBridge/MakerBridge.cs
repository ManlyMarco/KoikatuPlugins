using System.IO;
using System.ComponentModel;
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
        public static string TempFilePath;
        static GameObject container;

        [DisplayName("Send character")]
        public static SavedKeyboardShortcut SendChara { get; set; }

        MakerBridge()
        {
            SendChara = new SavedKeyboardShortcut("SendChara", this, new KeyboardShortcut(KeyCode.C));
        }

        void Awake()
        {
            TempFilePath = Path.Combine(Paths.PluginPath, "makerbridgecard.png");

            RPCServer.Start(ServerName, ServerPort);

            container = new GameObject("MakerBridge");
            container.transform.SetParent(gameObject.transform);
            container.AddComponent<UnityMainThreadDispatcher>();

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
            var comp = container.GetComponent<MakerHandler>();
            if(!comp) container.AddComponent<MakerHandler>();
        }

        [HarmonyPrefix, HarmonyPatch(typeof(CustomScene), "OnDestroy")]
        public static void CustomSceneStop()
        {
            var comp = container.GetComponent<MakerHandler>();
            if(comp) Destroy(comp);
        }

        [HarmonyPrefix, HarmonyPatch(typeof(StudioScene), "Start")]
        public static void StudioSceneInit()
        {
            var comp = container.GetComponent<StudioHandler>();
            if(!comp) container.AddComponent<StudioHandler>();
        }
    }
}
