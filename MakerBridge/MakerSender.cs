using UnityEngine;
using ChaCustom;
using MakerBridge.Remoting;

namespace MakerBridge
{
    class MakerSender : MonoBehaviour
    {
        void Start()
        {
            RPCClient_Send.Start(MakerBridge.ServerName, MakerBridge.ServerPort);
        }

        void Update()
        {
            if(MakerBridge.SendChara.IsDown())
            {
                RPCClient_Send.SendMessage(GetData());
            }
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
