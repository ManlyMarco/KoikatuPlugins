using UnityEngine;
using ChaCustom;
using MakerBridge.Remoting;

namespace MakerBridge
{
    class MakerHandler : MonoBehaviour
    {
        void Start()
        {
            RPCClient.Init(MakerBridge.ServerName, MakerBridge.ServerPort, 1, 0, (msg) => UnityMainThreadDispatcher.instance.Enqueue(() => LoadChara(msg.path, true, true, true, true, true)));
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
                if(CustomBase.Instance)
                {
                    SaveCharacter(MakerBridge.TempFilePath);
                    RPCClient.SendMessage(new MsgObject{ path = MakerBridge.TempFilePath });
                }
            }
        }

        void LoadChara(string path, bool loadFace, bool loadBody, bool loadHair, bool parameter, bool loadCoord)
        {
            var chaCtrl = CustomBase.Instance.chaCtrl;
            chaCtrl.chaFile.LoadFileLimited(path, chaCtrl.sex, loadFace, loadBody, loadHair, parameter, loadCoord);
            chaCtrl.ChangeCoordinateType(true);
            chaCtrl.Reload(!loadCoord, !loadFace && !loadCoord, !loadHair, !loadBody);
            CustomBase.Instance.updateCustomUI = true;
            CustomHistory.Instance.Add5(chaCtrl, chaCtrl.Reload, !loadCoord, !loadFace && !loadCoord, !loadHair, !loadBody);
        }

        public void SaveCharacter(string path)
        {
            var customBase = CustomBase.Instance;

            var empty = new Texture2D(1, 1, TextureFormat.ARGB32, false);
            empty.SetPixel(0, 0, Color.black);
            empty.Apply();

            var charFile = customBase.chaCtrl.chaFile;
            charFile.pngData = empty.EncodeToPNG();
            charFile.facePngData = empty.EncodeToPNG();

            customBase.chaCtrl.chaFile.SaveCharaFile(path, byte.MaxValue, false);
        }
    }
}
