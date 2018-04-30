using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using BepInEx;

namespace ObjectTreeDebugKK
{
    [BepInPlugin("ObjectTreeDebugKKPort", "ObjectTreeDebugKK", "1.0.0")]
    public class ObjectTreeDebug : BaseUnityPlugin
    {
        //static void Bootstrap()
        //{
        //    string name = "ObjectTreeDebugObject";
        //    var gameobject = GameObject.Find(name);
        //    if(gameobject != null) GameObject.DestroyImmediate(gameobject);
        //    new GameObject(name).AddComponent<ObjectTreeDebug>();

        //    //var comp = Chainloader.ManagerObject.GetComponent("ObjectTreeDebugKK.ObjectTreeDebug");
        //    //if(comp) GameObject.DestroyImmediate(comp);
        //    //Chainloader.ManagerObject.AddComponent<ObjectTreeDebug>();
        //}

        private Transform _target;
        private readonly HashSet<GameObject> _openedObjects = new HashSet<GameObject>();
        private Vector2 _scroll;
        private Vector2 _scroll2;
        private Vector2 _scroll3;
        private static readonly LinkedList<KeyValuePair<LogType, string>> _lastlogs = new LinkedList<KeyValuePair<LogType, string>>();
        private static bool _debug;
        private Rect _rect = new Rect(Screen.width / 4f, Screen.height / 4f, Screen.width / 2f, Screen.height / 2f);
        private int _randomId;
        KeyCode key = KeyCode.RightControl;

        void Awake()
        {
            try
            {
                string keyS = BepInEx.Config.GetEntry(this, "key");
                if(keyS == "")
                    BepInEx.Config.SetEntry(this, "key", key.ToString());
                else
                    key = (KeyCode)Enum.Parse(typeof(KeyCode), keyS);
            }
            catch(Exception ex)
            {
                BepInLogger.Log(ex.ToString());
            }

            Application.logMessageReceived += this.HandleLog;
            this._randomId = (int)(UnityEngine.Random.value * UInt32.MaxValue);
            for (int i = 0; i < 32; i++)
            {
                string n = LayerMask.LayerToName(i);
                //UnityEngine.Debug.Log("Layer " + i + " " + n);
            }
        }

        void Update()
        {
            if (Input.GetKeyDown(key))
                _debug = !_debug;
        }

        void OnDestroy()
        {
            Application.logMessageReceived -= this.HandleLog;
        }

        private void HandleLog(string condition, string stackTrace, LogType type)
        {
            _lastlogs.AddLast(new KeyValuePair<LogType, string>(type, type + " " + condition));
            if (_lastlogs.Count == 101)
                _lastlogs.RemoveFirst();
            this._scroll3.y += 999999;
        }

        private void DisplayObjectTree(GameObject go, int indent)
        {
            Color c = GUI.color;
            if (this._target == go.transform)
                GUI.color = Color.cyan;
            GUILayout.BeginHorizontal();
            GUILayout.Space(indent * 20f);
            if (go.transform.childCount != 0)
            {
                if (GUILayout.Toggle(this._openedObjects.Contains(go), "", GUILayout.ExpandWidth(false)))
                {
                    if (this._openedObjects.Contains(go) == false)
                        this._openedObjects.Add(go);
                }
                else
                {
                    if (this._openedObjects.Contains(go))
                        this._openedObjects.Remove(go);

                }
            }
            else
                GUILayout.Space(20f);
            if (GUILayout.Button(go.name, GUILayout.ExpandWidth(false)))
            {
                this._target = go.transform;
            }
            GUI.color = c;
            go.SetActive(GUILayout.Toggle(go.activeSelf, "", GUILayout.ExpandWidth(false)));
            GUILayout.EndHorizontal();
            if (this._openedObjects.Contains(go))
                for (int i = 0; i < go.transform.childCount; ++i)
                    this.DisplayObjectTree(go.transform.GetChild(i).gameObject, indent + 1);
        }

        void OnGUI()
        {
            if (_debug == false)
                return;
            this._rect = GUILayout.Window(this._randomId, this._rect, this.WindowFunc, "Debug Console");
        }

        private void WindowFunc(int id)
        {
            GUILayout.BeginHorizontal();
            this._scroll = GUILayout.BeginScrollView(this._scroll, GUI.skin.box, GUILayout.ExpandHeight(true), GUILayout.MinWidth(300));
            foreach (Transform t in Resources.FindObjectsOfTypeAll<Transform>())
                if (t.parent == null)
                    this.DisplayObjectTree(t.gameObject, 0);
            GUILayout.EndScrollView();
            GUILayout.BeginVertical();
            this._scroll2 = GUILayout.BeginScrollView(this._scroll2, GUI.skin.box);
            if (this._target != null)
            {
                Transform t = this._target.parent;
                string n = this._target.name;
                while (t != null)
                {
                    n = t.name + "/" + n;
                    t = t.parent;
                }
                GUILayout.BeginHorizontal();
                GUILayout.Label(n);
                if (GUILayout.Button("Copy to clipboard", GUILayout.ExpandWidth(false)))
                    GUIUtility.systemCopyBuffer = n;
                GUILayout.EndHorizontal();
                GUILayout.Label("Layer: " + LayerMask.LayerToName(this._target.gameObject.layer) + " " + this._target.gameObject.layer);
                foreach (Component c in this._target.GetComponents<Component>())
                {
                    if (c == null)
                        continue;
                    GUILayout.BeginHorizontal();
                    MonoBehaviour m = c as MonoBehaviour;
                    if (m != null)
                        m.enabled = GUILayout.Toggle(m.enabled, c.GetType().FullName, GUILayout.ExpandWidth(false));
                    else if (c is Animator)
                    {
                        Animator an = (Animator)c;
                        an.enabled = GUILayout.Toggle(an.enabled, c.GetType().FullName, GUILayout.ExpandWidth(false));
                    }
                    else
                        GUILayout.Label(c.GetType().FullName);

                    if (c is Image)
                    {
                        Image img = c as Image;
                        if (img.sprite != null && img.sprite.texture != null)
                        {
                            try
                            {
                                GUILayout.Label(img.sprite.name);
                                Color[] newImg = img.sprite.texture.GetPixels((int)img.sprite.textureRect.x, (int)img.sprite.textureRect.y, (int)img.sprite.textureRect.width, (int)img.sprite.textureRect.height);
                                Texture2D tex = new Texture2D((int)img.sprite.textureRect.width, (int)img.sprite.textureRect.height);
                                tex.SetPixels(newImg);
                                tex.Apply();
                                GUILayout.Label(tex);

                            }
                            catch (Exception)
                            {
                            }
                        }
                    }
                    else if (c is Slider)
                    {
                        Slider b = c as Slider;
                        for (int i = 0; i < b.onValueChanged.GetPersistentEventCount(); ++i)
                            GUILayout.Label(b.onValueChanged.GetPersistentTarget(i).GetType().FullName + "." + b.onValueChanged.GetPersistentMethodName(i));
                    }
                    else if (c is Text)
                    {
                        Text text = c as Text;
                        GUILayout.Label(text.text + " " + text.font + " " + text.fontStyle + " " + text.fontSize + " " + text.alignment + " " + text.alignByGeometry + " " + text.resizeTextForBestFit + " " + text.color);

                    }
                    else if (c is RawImage)
                        GUILayout.Label(((RawImage)c).mainTexture);
                    else if (c is Renderer)
                        GUILayout.Label(((Renderer)c).material != null ? ((Renderer)c).material.shader.name : "");
                    else if (c is Button)
                    {
                        Button b = c as Button;
                        for (int i = 0; i < b.onClick.GetPersistentEventCount(); ++i)
                            GUILayout.Label(b.onClick.GetPersistentTarget(i).GetType().FullName + "." + b.onClick.GetPersistentMethodName(i));
                        IList calls = b.onClick.GetPrivateExplicit<UnityEventBase>("m_Calls").GetPrivate("m_RuntimeCalls") as IList;
                        for (int i = 0; i < calls.Count; ++i)
                        {
                            UnityAction unityAction = ((UnityAction)calls[i].GetPrivate("Delegate"));
                            GUILayout.Label(unityAction.Target.GetType().FullName + "." + unityAction.Method.Name);
                        }
                    }
                    else if (c is Toggle)
                    {
                        Toggle b = c as Toggle;
                        for (int i = 0; i < b.onValueChanged.GetPersistentEventCount(); ++i)
                            GUILayout.Label(b.onValueChanged.GetPersistentTarget(i).GetType().FullName + "." + b.onValueChanged.GetPersistentMethodName(i));
                        IList calls = b.onValueChanged.GetPrivateExplicit<UnityEventBase>("m_Calls").GetPrivate("m_RuntimeCalls") as IList;
                        for (int i = 0; i < calls.Count; ++i)
                        {
                            UnityAction<bool> unityAction = ((UnityAction<bool>)calls[i].GetPrivate("Delegate"));
                            GUILayout.Label(unityAction.Target.GetType().FullName + "." + unityAction.Method.Name);
                        }
                    }
                    else if (c is RectTransform)
                    {
                        RectTransform rt = c as RectTransform;
                        GUILayout.Label("anchorMin " + rt.anchorMin);
                        GUILayout.Label("anchorMax " + rt.anchorMax);
                        GUILayout.Label("offsetMin " + rt.offsetMin);
                        GUILayout.Label("offsetMax " + rt.offsetMax);
                        GUILayout.Label("rect " + rt.rect);
                    }
                    GUILayout.EndHorizontal();
                }
            }
            GUILayout.EndScrollView();
            this._scroll3 = GUILayout.BeginScrollView(this._scroll3, GUI.skin.box, GUILayout.Height(Screen.height / 4f));
            foreach (KeyValuePair<LogType, string> lastlog in _lastlogs)
            {
                Color c = GUI.color;
                switch (lastlog.Key)
                {
                    case LogType.Error:
                    case LogType.Exception:
                        GUI.color = Color.red;
                        break;
                    case LogType.Warning:
                        GUI.color = Color.yellow;
                        break;
                }
                GUILayout.BeginHorizontal();
                GUILayout.Label(lastlog.Value);
                GUI.color = c;
                if (GUILayout.Button("Copy to clipboard", GUILayout.ExpandWidth(false)))
                    GUIUtility.systemCopyBuffer = lastlog.Value;
                GUILayout.EndHorizontal();
            }
            GUILayout.EndScrollView();
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Clear AssetBundle Cache"))
            {
                foreach (KeyValuePair<string, AssetBundleManager.BundlePack> pair in AssetBundleManager.ManifestBundlePack)
                {
                    foreach (KeyValuePair<string, LoadedAssetBundle> bundle in new Dictionary<string, LoadedAssetBundle>(pair.Value.LoadedAssetBundles))
                    {
                        AssetBundleManager.UnloadAssetBundle(bundle.Key, true, pair.Key);
                    }
                }
            }
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Clear logs", GUILayout.ExpandWidth(false)))
                _lastlogs.Clear();
            if (GUILayout.Button("Open log file", GUILayout.ExpandWidth(false)))
                System.Diagnostics.Process.Start(System.IO.Path.Combine(Application.dataPath, "output_log.txt"));
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
            GUI.DragWindow();
        }
    }
}

