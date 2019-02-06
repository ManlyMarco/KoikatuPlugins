using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BepInEx;
using BepInEx.Logging;
using static BepInEx.Logger;
using UILib;
using UnityEngine;
using UnityEngine.UI;
using Studio;
//using Harmony;

namespace BlendShaper
{
    [BepInProcess("CharaStudio")]
    [BepInPlugin("keelhauled.blendshaper", "BlendShaper", Version)]
    class BlendShaper : BaseUnityPlugin
    {
        public const string Version = "1.0.0";
        //HarmonyInstance harmony;

        float UIScale = 1.0f;
        float elementSize = 20f;

        Canvas UISystem;
        ScrollRect rendererList;
        ScrollRect shapeList;
        static List<Action> changes = new List<Action>();

        void Awake()
        {
            //harmony = HarmonyInstance.Create("keelhauled.blendshaper.harmony");
            //harmony.PatchAll(GetType());

            UIUtility.Init(nameof(BlendShaper));

            var panel = CreateUIBase(600f, 400f);
            rendererList = CreateList(panel.transform, 0f, 0f, 0.3f, 1f);
            shapeList = CreateList(panel.transform, 0.3f, 0f, 1f, 1f);

            if(Studio.Studio.Instance.treeNodeCtrl.selectNode)
                UpdateUI(Studio.Studio.Instance.treeNodeCtrl.selectNode);

            Studio.Studio.Instance.treeNodeCtrl.onSelect += UpdateUI;
            Studio.Studio.Instance.treeNodeCtrl.onDelete += ClearUI;
        }

        void OnDestroy()
        {
            Destroy(UISystem);
            Studio.Studio.Instance.treeNodeCtrl.onSelect -= UpdateUI;
            Studio.Studio.Instance.treeNodeCtrl.onDelete -= ClearUI;
            //harmony.UnpatchAll(GetType());
        }

        void LateUpdate()
        {
            if(changes.Count > 0)
            {
                foreach(var change in changes)
                    change();
            }
        }

        void UpdateUI(TreeNodeObject node)
        {
            ClearUI(Studio.Studio.Instance.treeNodeCtrl.selectNode);

            if(Studio.Studio.Instance.dicInfo.TryGetValue(node, out var objectCtrlInfo))
            {
                if(objectCtrlInfo is OCIChar chara)
                {
                    var skinnedMeshRenderers = chara.charInfo.animBody.GetComponentsInChildren<SkinnedMeshRenderer>(true).Where(x => x.sharedMesh.blendShapeCount > 0);
                    foreach(var renderer in skinnedMeshRenderers)
                    {
                        var button = UIUtility.CreateButton("Button", rendererList.content.transform, renderer.name);
                        button.gameObject.AddComponent<LayoutElement>().preferredHeight = elementSize;
                        button.onClick.AddListener(() => UpdateBlend(renderer));
                    }
                }
            }
        }

        void UpdateBlend(SkinnedMeshRenderer renderer)
        {
            foreach(var button in shapeList.content.GetComponentsInChildren<Image>())
            {
                button.gameObject.SetActive(false);
                Destroy(button);
            }

            for(int i = 0; i < renderer.sharedMesh.blendShapeCount; i++)
            {
                var index = i;

                var panel = UIUtility.CreatePanel("ShapePanel", shapeList.content.transform);
                panel.gameObject.AddComponent<LayoutElement>().preferredHeight = elementSize;
                panel.color = new Color(1f, 1f, 1f, 1f);

                var shapeName = renderer.sharedMesh.GetBlendShapeName(index);
                var text = UIUtility.CreateText("ShapeName", panel.transform, shapeName);
                text.transform.SetRect(0f, 0f, 0.3f, 1f);
                text.color = Color.black;

                var slider = UIUtility.CreateSlider("ShapeSlider", panel.transform);
                slider.transform.SetRect(0.3f, 0f, 1f, 1f);
                slider.maxValue = 100f; slider.minValue = 0f;
                slider.value = renderer.GetBlendShapeWeight(index);
                slider.onValueChanged.AddListener((value) => changes.Add(() => renderer.SetBlendShapeWeight(index, value)));
            }
        }

        void ClearUI(TreeNodeObject node)
        {
            foreach(var button in rendererList.content.GetComponentsInChildren<Button>())
            {
                button.gameObject.SetActive(false);
                Destroy(button);
            }

            foreach(var button in shapeList.content.GetComponentsInChildren<Image>())
            {
                button.gameObject.SetActive(false);
                Destroy(button);
            }
        }

        Image CreateUIBase(float width, float height)
        {
            var margin = 4f;
            var title = 25f;

            UISystem = UIUtility.CreateNewUISystem("BlendShaperUI");
            UISystem.GetComponent<CanvasScaler>().referenceResolution = new Vector2(1920f / UIScale, 1080f / UIScale);
            UISystem.transform.SetParent(gameObject.transform);

            var bg = UIUtility.CreatePanel("Background", UISystem.transform);
            bg.transform.SetRect(0.5f, 0.5f, 0.5f, 0.5f, -width / 2f, -height / 2f, width / 2f, height / 2f);
            bg.color = new Color(bg.color.r, bg.color.g, bg.color.b, 1f);

            var drag = UIUtility.CreatePanel("Drag", bg.transform);
            drag.transform.SetRect(0f, 1f, 1f, 1f, margin, -title, -margin, -margin);
            UIUtility.MakeObjectDraggable(drag.rectTransform, bg.rectTransform);
            drag.color = Color.gray;

            var text = UIUtility.CreateText("Text", drag.transform, "BlendShaper");
            text.transform.SetRect(0f, 0f, 1f, 1f);
            text.alignment = TextAnchor.MiddleCenter;
            text.resizeTextMinSize = 10;
            text.resizeTextMaxSize = 16;

            var panel = UIUtility.CreatePanel("Panel", bg.transform);
            panel.transform.SetRect(0f, 0f, 1f, 1f, margin, margin, -margin, -title);
            panel.color = new Color(0f, 0f, 0f, 0f);

            return panel;
        }

        ScrollRect CreateList(Transform parent, float anchorLeft, float anchorBottom, float anchorRight, float anchorTop)
        {
            float scrollOffsetX = -15f;

            var scroll = UIUtility.CreateScrollView("List", parent);
            scroll.transform.SetRect(anchorLeft, anchorBottom, anchorRight, anchorTop);
            scroll.gameObject.AddComponent<Mask>();
            scroll.content.gameObject.AddComponent<VerticalLayoutGroup>();
            scroll.content.gameObject.AddComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            scroll.verticalScrollbar.GetComponent<RectTransform>().offsetMin = new Vector2(scrollOffsetX, 0f);
            scroll.viewport.offsetMax = new Vector2(scrollOffsetX, 0f);
            scroll.movementType = ScrollRect.MovementType.Clamped;
            scroll.scrollSensitivity = 20f;
            return scroll;
        }
    }
}
