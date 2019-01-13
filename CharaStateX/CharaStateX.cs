using BepInEx;
using Harmony;

namespace CharaStateX
{
    [BepInPlugin("keelhauled.charastatex", "CharaStateX", "1.0.1")]
    class CharaStateX : BaseUnityPlugin
    {
        void Awake()
        {
            var harmony = HarmonyInstance.Create("keelhauled.charastatex.harmony");
            StateInfoPatch.Patch(harmony);
            NeckLookPatch.Patch(harmony);
            EtcInfoPatch.Patch(harmony);
            HandInfoPatch.Patch(harmony);
            harmony.PatchAll(typeof(AnimationPatch));
            JointInfoPatch.Patch(harmony);
            FKIKPatch.Patch(harmony);
        }
    }
}
