using System;
using Harmony;
using MessagePack;
using UnityEngine;

namespace DefaultParamEditor
{
    [MessagePackObject(true)]
    public class ParamData
    {
        public void PrintData()
        {
            Console.WriteLine(new string('=', 40));

            if(charaParamData.saved)
            {
                Console.WriteLine(nameof(CharaData));
                foreach(var field in AccessTools.GetDeclaredFields(typeof(CharaData)))
                {
                    var target = AccessTools.Field(typeof(CharaData), field.Name);
                    var value = target.GetValue(charaParamData);
                    Console.WriteLine($"{field.Name} = {value}");
                }
            }
            else
            {
                Console.WriteLine($"{nameof(CharaData)} not saved");
            }

            Console.WriteLine(new string('=', 40));

            if(sceneParamData.saved)
            {
                Console.WriteLine(nameof(SceneData));
                foreach(var field in AccessTools.GetDeclaredFields(typeof(SceneData)))
                {
                    var target = AccessTools.Field(typeof(SceneData), field.Name);
                    var value = target.GetValue(sceneParamData);
                    Console.WriteLine($"{field.Name} = {value}");
                }
            }
            else
            {
                Console.WriteLine($"{nameof(SceneData)} not saved");
            }

            Console.WriteLine(new string('=', 40));
        }

        public CharaData charaParamData = new CharaData();
        public SceneData sceneParamData = new SceneData();

        [MessagePackObject(true)]
        public class CharaData
        {
            public bool saved = false;

            public byte[] clothesState;
            public byte shoesType;
            public float hohoAkaRate;
            public float nipStandRate;
            public byte tearsLv;

            public int eyesLookPtn;
            public int neckLookPtn;
            public int eyebrowPtn;
            public int eyesPtn;
            public float eyesOpenMax;
            public bool eyesBlink;
            public int mouthPtn;

            // parameters to add
            // default animation
            // siru
            // mouth open
            // eyebrow and eye overlaying
            // donger options
        }

        [MessagePackObject(true)]
        public class SceneData
        {
            public bool saved = false;

            public int aceNo;
            public float aceBlend;
            public bool enableAOE;
            public Color aoeColor;
            public float aoeRadius;
            public bool enableBloom;
            public float bloomIntensity;
            public float bloomThreshold;
            public float bloomBlur;
            public bool enableDepth;
            public float depthFocalSize;
            public float depthAperture;
            public bool enableVignette;
            public bool enableFog;
            public Color fogColor;
            public float fogHeight;
            public float fogStartDistance;
            public bool enableSunShafts;
            public Color sunThresholdColor;
            public Color sunColor;
            public bool enableShadow;
            public float lineColorG;
            public Color ambientShadow;
            public float lineWidthG;
            public int rampG;
            public float ambientShadowG;

            // parameters to add
            // all character lighting options
            // default fov
        }
    }
}
