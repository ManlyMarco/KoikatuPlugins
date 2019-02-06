using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using ParadoxNotion.Serialization;
using Studio;
using UnityEngine;

namespace BlendShaper
{
    class BlendSets
    {
        const string resourceFile = "BlendSets.json";

        public static BlendSets LoadBlendSetData()
        {
            string resourceName = $"{nameof(BlendShaper)}.{resourceFile}";
            using(var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName))
            {
                using(var reader = new StreamReader(stream))
                {
                    string json = reader.ReadToEnd();
                    return JSONSerializer.Deserialize<BlendSets>(json);
                }
            }
        }

        public List<Category> Categories;

        public class Category
        {
            public string Name;
            public List<Set> Sets;
        }

        public class Set
        {
            public void ChangeValue(float value)
            {
                Enabled = true;
                Value = value;
            }

            public void CreateAction(OCIChar chara)
            {
                var skinnedMeshRenderers = chara.charInfo.animBody.GetComponentsInChildren<SkinnedMeshRenderer>(true).Where(x => x.sharedMesh && x.sharedMesh.blendShapeCount > 0);

                var firstShape = Shapes.First();
                Value = skinnedMeshRenderers.FirstOrDefault((x) => x.name == firstShape.Renderer).GetBlendShapeWeight(firstShape.Index);

                Action = () =>
                {
                    foreach(var renderer in skinnedMeshRenderers)
                    {
                        foreach(var shape in Shapes)
                        {
                            if(renderer.name == shape.Renderer)
                                renderer.SetBlendShapeWeight(shape.Index, Value);
                        }
                    }
                };
            }

            public bool Enabled = false;
            public float Value;
            public Action Action;

            public string Name;
            public List<Shape> Shapes;

            public class Shape
            {
                public string Renderer;
                public int Index;
            }
        }
    }
}
