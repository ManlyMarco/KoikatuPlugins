using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static BepInEx.Logger;
using BepInEx.Logging;
using UnityEngine.Events;
using Studio;
using Harmony;
using MakerBridge.Remoting;

namespace MakerBridge
{
    class StudioReceive : MonoBehaviour
    {
        void Start()
        {
            RPCClient.Init(MakerBridge.ServerName, MakerBridge.ServerPort, 0, 1, (message) => UnityMainThreadDispatcher.instance.Enqueue(() => LoadCharas(message)));
            RPCClient.Listen();
        }

        void OnDestroy()
        {
            RPCClient.StopServer();
        }

        void Update()
        {
            if(MakerBridge.SendChara.IsDown())
            {
                RPCClient.SendMessage(GetData());
            }
        }

        MsgObject GetData()
        {
            var characters = GetSelectedCharacters();
            if(characters.Count > 0)
            {
                var chaFile = characters[0].charInfo.chaFile;

                return new MsgObject
                {
                    face = chaFile.custom.face,
                    body = chaFile.custom.body,
                    hair = chaFile.custom.hair,
                    param = chaFile.parameter,
                    coord = ChaFile.GetCoordinateBytes(chaFile.coordinate)
                };
            }
            else
            {
                Log(LogLevel.Message, "Select a character to send to maker");
            }

            return null;
        }

        void LoadCharas(MsgObject message)
        {
            var characters = GetSelectedCharacters();
            if(characters.Count > 0)
            {
                Log(LogLevel.Message, "Character received");

                DelayAction(() =>
                {
                    foreach(var chara in characters)
                    {
                        LoadChara(chara, message);
                    }

                    UpdateStateInfo();
                });
            }
            else
            {
                Log(LogLevel.Message, "Select a character before replacing it");
            }
        }

        void LoadChara(OCIChar chara, MsgObject message)
        {
            foreach(var boneInfo in (from v in chara.listBones where v.boneGroup == OIBoneInfo.BoneGroup.Hair select v).ToList())
            {
                Singleton<GuideObjectManager>.Instance.Delete(boneInfo.guideObject, true);
            }

            chara.listBones = (from v in chara.listBones where v.boneGroup != OIBoneInfo.BoneGroup.Hair select v).ToList();

            int[] array = (from b in chara.oiCharInfo.bones where b.Value.@group == OIBoneInfo.BoneGroup.Hair select b.Key).ToArray();
            for(int j = 0; j < array.Length; j++)
            {
                chara.oiCharInfo.bones.Remove(array[j]);
            }

            chara.hairDynamic = null;
            chara.skirtDynamic = null;
            
            chara.charInfo.chaFile.custom.face = message.face;
            chara.charInfo.chaFile.custom.body = message.body;
            chara.charInfo.chaFile.custom.hair = message.hair;
            chara.charInfo.chaFile.parameter = message.param;
            chara.charInfo.chaFile.SetCoordinateBytes(message.coord, ChaFileDefine.ChaFileCoordinateVersion);

            chara.charInfo.ChangeCoordinateType((ChaFileDefine.CoordinateType)chara.charFileStatus.coordinateType, true);
            chara.charInfo.Reload(false, false, false, false);
            chara.treeNodeObject.textName = chara.charInfo.chaFile.parameter.fullname;
            AddObjectAssist.InitHairBone(chara, Singleton<Info>.Instance.dicBoneInfo);
            chara.hairDynamic = AddObjectFemale.GetHairDynamic(chara.charInfo.objHair);
            chara.skirtDynamic = AddObjectFemale.GetSkirtDynamic(chara.charInfo.objClothes);
            chara.InitFK(null);

            foreach(var __AnonType in FKCtrl.parts.Select((OIBoneInfo.BoneGroup p, int i) => new { p, i }))
            {
                chara.ActiveFK(__AnonType.p, chara.oiCharInfo.activeFK[__AnonType.i], chara.oiCharInfo.activeFK[__AnonType.i]);
            }

            chara.ActiveKinematicMode(OICharInfo.KinematicMode.FK, chara.oiCharInfo.enableFK, true);
            chara.UpdateFKColor(new OIBoneInfo.BoneGroup[]{ OIBoneInfo.BoneGroup.Hair });
            chara.ChangeEyesOpen(chara.charFileStatus.eyesOpenMax);
            chara.ChangeBlink(chara.charFileStatus.eyesBlink);
            chara.ChangeMouthOpen(chara.oiCharInfo.mouthOpen);
        }

        List<OCIChar> GetSelectedCharacters()
        {
            return GuideObjectManager.Instance.selectObjectKey.Select(x => Studio.Studio.GetCtrlInfo(x) as OCIChar).Where(x => x != null).ToList();
        }

        public void DelayAction(UnityAction action, int wait = 1)
        {
            if(wait < 0) wait = 0;
            StartCoroutine(Coroutine());

            IEnumerator Coroutine()
            {
                for(int i = 0; i < wait; i++) yield return null;
                yield return new WaitForEndOfFrame();

                action();
            }
        }

        void UpdateStateInfo()
        {
            var mpCharCtrl = FindObjectOfType<MPCharCtrl>();
            if(mpCharCtrl)
            {
                int select = Traverse.Create(mpCharCtrl).Field("select").GetValue<int>();
                if(select == 0) mpCharCtrl.OnClickRoot(0);
            }
        }
    }
}
