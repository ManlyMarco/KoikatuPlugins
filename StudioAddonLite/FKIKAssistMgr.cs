using System;
using System.Collections.Generic;
using System.Reflection;
using Studio;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;

namespace StudioAddonLite
{
    public class FKIKAssistMgr : BaseMgr<FKIKAssistMgr>
    {
        //private Dictionary<KEY, string> DEFAULT_KEYS = new Dictionary<KEY, string>
        //{
        //    { KEY.FORCE_FK_ON, "Ctrl+K" }
        //};

        //private Dictionary<KEY, KeyUtil> keyUtils = new Dictionary<KEY, KeyUtil>();

        private static HashSet<string> FKIKBodyBone_Female = new HashSet<string>
        {
            "cf_J_Toes01_L",
            "cf_J_Toes01_R"
        };

        private static HashSet<string> FKIKBodyBone_Male = new HashSet<string>
        {
            "cm_J_Toes01_L",
            "cm_J_Toes01_R"
        };

        private static FieldInfo f_listBones = typeof(FKCtrl).GetField("listBones", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.GetField);
        private static Type t_TargetInfo = typeof(FKCtrl).GetNestedType("TargetInfo", BindingFlags.Public | BindingFlags.NonPublic);
        private static FieldInfo f_enable = t_TargetInfo.GetField("enable", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.GetField | BindingFlags.SetField);
        private static FieldInfo f_gameObject = t_TargetInfo.GetField("gameObject", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.GetField);

        public override void Init()
        {
            InstallFKMenu();
        }

        //private void InitKey()
        //{
        //    keyUtils = new Dictionary<KEY, KeyUtil>();
        //    foreach(KEY key in DEFAULT_KEYS.Keys)
        //    {
        //        keyUtils[key] = KeyUtil.Parse(DEFAULT_KEYS[key]);
        //    }
        //}

        public void InstallFKMenu()
        {
            Studio.Studio instance = Studio.Studio.Instance;
            if(instance != null)
            {
                Transform transform = instance.gameObject.transform.Find("Canvas Main Menu/02_Manipulate/00_Chara/02_Kinematic/Viewport/Content");
                if(transform.Find("HSNA_ForceFKActive") == null)
                {
                    Transform transform2 = transform.Find("Pose");
                    GameObject gameObject = GameObject.Instantiate<GameObject>(transform2.gameObject);
                    gameObject.name = "HSNA_ForceFKActive";
                    var component = gameObject.transform.Find("TextMeshPro").gameObject.GetComponent<TextMeshProUGUI>();
                    component.text = "FK&IK";
                    component.SetLayoutDirty();
                    gameObject.transform.SetParent(transform2.transform.parent);
                    Button component2 = gameObject.GetComponent<Button>();
                    component2.onClick = new Button.ButtonClickedEvent();
                    component2.onClick.AddListener(new UnityAction(OnForceFKIKButtonClick));
                    gameObject.transform.localPosition = transform2.transform.localPosition - new Vector3(0f, 30f, 0f);
                    gameObject.transform.localScale = Vector3.one;
                    Console.WriteLine("FK button installed.");
                }
            }
        }

        private void OnForceFKIKButtonClick()
        {
            DoActivateFKIKForce();
        }

        private void DoActivateFKIKForce()
        {
            foreach(ObjectCtrlInfo objectCtrlInfo in Singleton<Studio.Studio>.Instance.treeNodeCtrl.selectObjectCtrl)
            {
                if(objectCtrlInfo is OCIChar)
                {
                    ActivateFKIK((OCIChar)objectCtrlInfo);
                }
            }
        }

        public static bool IsFKIKActivated(OCIChar ociChar)
        {
            return ociChar.oiCharInfo.enableFK && ociChar.oiCharInfo.enableIK;
        }

        public static void ActivateFKIK(OCIChar ociChar)
        {
            HashSet<string> fkikBodyBone;
            if(ociChar.charInfo.sex == 0)
            {
                fkikBodyBone = FKIKBodyBone_Male;
            }
            else
            {
                fkikBodyBone = FKIKBodyBone_Female;
            }
            ociChar.ActiveKinematicMode((OICharInfo.KinematicMode)2, true, true);
            ociChar.fkCtrl.enabled = true;
            ociChar.oiCharInfo.enableFK = true;
            //Func<OCIChar.BoneInfo, bool> thing = null;
            for(int i = 0; i < ociChar.oiCharInfo.activeFK.Length; i++)
            {
                if(ociChar.oiCharInfo.activeFK[i])
                {
                    var boneGroup = FKCtrl.parts[i];
                    //if(boneGroup == (OIBoneInfo.BoneGroup)1)
                    //{
                    //    IEnumerable<OCIChar.BoneInfo> listBones = ociChar.listBones;
                    //    Func<OCIChar.BoneInfo, bool> predicate;
                    //    if((predicate = thing) == null)
                    //    {
                    //        predicate = (thing = ((OCIChar.BoneInfo b) => fkikBodyBone.Contains(b.guideObject.transformTarget.name)));
                    //    }
                    //    foreach(OCIChar.BoneInfo boneInfo in listBones.Where(predicate))
                    //    {
                    //        boneInfo.active = true;
                    //    }

                    //    IEnumerator enumerator2 = (f_listBones.GetValue(ociChar.fkCtrl) as IEnumerable).GetEnumerator();
                    //    while(enumerator2.MoveNext())
                    //    {
                    //        object obj = enumerator2.Current;
                    //        GameObject gameObject = FKIKAssistMgr.f_gameObject.GetValue(obj) as GameObject;
                    //        if(gameObject != null && fkikBodyBone.Contains(gameObject.name))
                    //        {
                    //            FKIKAssistMgr.f_enable.SetValue(obj, true);
                    //        }
                    //    }
                    //    goto IL_168;
                    //}
                    ociChar.fkCtrl.SetEnable(boneGroup, true);
                    ociChar.ActiveFK(boneGroup, true, true);
                }
                //IL_168:;
            }
        }

        //private bool GetKeyDown(KEY key)
        //{
        //    return keyUtils.ContainsKey(key) && keyUtils[key].TestKeyDown();
        //}

        //public enum KEY
        //{
        //    FORCE_FK_ON
        //}
    }
}
