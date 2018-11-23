using UnityEngine;
using ChaCustom;
using MakerBridge.Remoting;

namespace MakerBridge
{
    class MakerSender : MonoBehaviour
    {
        void Start()
        {
            RPCClient.Init(MakerBridge.ServerName, MakerBridge.ServerPort, 1, 0, (message) => UnityMainThreadDispatcher.instance.Enqueue(() => LoadChara(message)));
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

        void LoadChara(MsgObject message)
        {
            var chaCtrl = CustomBase.Instance.chaCtrl;

            chaCtrl.chaFile.custom.face = message.face;
            chaCtrl.chaFile.custom.body = message.body;
            chaCtrl.chaFile.custom.hair = message.hair;
            chaCtrl.chaFile.parameter.Copy(message.param);
            chaCtrl.chaFile.SetCoordinateBytes(message.coord, ChaFileDefine.ChaFileCoordinateVersion);

            chaCtrl.ChangeCoordinateType(true);
            chaCtrl.Reload();
            CustomBase.Instance.updateCustomUI = true;
            CustomHistory.Instance.Add5(chaCtrl, chaCtrl.Reload, false, false, false, false);
        }

        MsgObject GetData()
        {
            if(CustomBase.Instance)
            {
                var chaFile = CustomBase.Instance.chaCtrl.chaFile;

                return new MsgObject
                {
                    face = chaFile.custom.face,
                    body = chaFile.custom.body,
                    hair = chaFile.custom.hair,
                    param = chaFile.parameter,
                    coord = ChaFile.GetCoordinateBytes(chaFile.coordinate)
                };
            }

            return null;
        }
    }
}
