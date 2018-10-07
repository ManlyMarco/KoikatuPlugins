using System;
using System.Linq;
using System.IO;
using System.Reflection;
using Harmony;
using Studio;
using MessagePack;

namespace DefaultParamEditor
{
    public class CharacterParam
    {
        static string savePath;
        static bool propertiesInitialized = false;
        static CharacterParamData data = new CharacterParamData();

        public CharacterParam()
        {
            savePath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "DefaultParamEditor_CharacterParamData.bin");

            if(File.Exists(savePath))
            {
                data = MessagePackSerializer.Deserialize<CharacterParamData>(File.ReadAllBytes(savePath));
                data.PrintProperties();
                propertiesInitialized = true;
            }

            var harmony = HarmonyInstance.Create("keelhauled.defaultparameditor.characterparam.harmony");
            harmony.PatchAll(typeof(CharacterParam));
        }

        public void Save()
        {
            var selected = GuideObjectManager.Instance.selectObjectKey.Select(x => Studio.Studio.GetCtrlInfo(x) as OCIChar).Where(x => x != null).ToList();

            if(selected.Count > 0)
            {
                foreach(var prop in AccessTools.GetDeclaredProperties(typeof(CharacterParamData)))
                {
                    var target = AccessTools.Property(selected[0].charFileStatus.GetType(), prop.Name);
                    var value = target.GetValue(selected[0].charFileStatus, null);
                    prop.SetValue(data, value, null);
                }

                var bytes = MessagePackSerializer.Serialize(data);
                File.WriteAllBytes(savePath, bytes);
                propertiesInitialized = true;
            }
        }

        [HarmonyPostfix, HarmonyPatch(typeof(ChaFileStatus), nameof(ChaFileStatus.MemberInit))]
        public static void HarmonyPatch_ChaFileStatus_MemberInit(ChaFileStatus __instance)
        {
            if(propertiesInitialized)
            {
                foreach(var prop in AccessTools.GetDeclaredProperties(typeof(CharacterParamData)))
                {
                    var target = AccessTools.Property(__instance.GetType(), prop.Name);
                    target.SetValue(__instance, prop.GetValue(data, null), null);
                }
            }
        }

        [MessagePackObject(true)]
        public class CharacterParamData
        {
            public void PrintProperties()
            {
                Console.WriteLine(new string('=', 40));
                foreach(var prop in AccessTools.GetDeclaredProperties(typeof(CharacterParamData)))
                {
                    var target = AccessTools.Property(typeof(CharacterParamData), prop.Name);
                    var value = target.GetValue(this, null);
                    Console.WriteLine($"{prop.Name} = {value}");
                }
                Console.WriteLine(new string('=', 40));
            }

            public int coordinateType { get; set; }
            public int backCoordinateType { get; set; }
            public byte[] clothesState { get; set; }
            public byte shoesType { get; set; }
            public bool[] showAccessory { get; set; }
            public int eyebrowPtn { get; set; }
            public float eyebrowOpenMax { get; set; }
            public int eyesPtn { get; set; }
            public float eyesOpenMax { get; set; }
            public bool eyesBlink { get; set; }
            public bool eyesYure { get; set; }
            public int mouthPtn { get; set; }
            public float mouthOpenMax { get; set; }
            public bool mouthFixed { get; set; }
            public bool mouthAdjustWidth { get; set; }
            public byte tongueState { get; set; }
            public int eyesLookPtn { get; set; }
            public int neckLookPtn { get; set; }
            public float nipStandRate { get; set; }
            public byte tearsLv { get; set; }
            //public byte[] siruLv { get; set; }
        }
    }
}
