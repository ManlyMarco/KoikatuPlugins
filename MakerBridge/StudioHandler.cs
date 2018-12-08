using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static BepInEx.Logger;
using BepInEx.Logging;
using Studio;
using Harmony;
using MakerBridge.Remoting;
using System.IO;

namespace MakerBridge
{
    class StudioHandler : MonoBehaviour
    {
        void Start()
        {
            RPCClient.Init(MakerBridge.ServerName, MakerBridge.ServerPort, 0, 1, (msg) => UnityMainThreadDispatcher.instance.Enqueue(() => LoadCharas(msg)));
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
                SaveChara(MakerBridge.TempFilePath);
            }
        }

        void SaveChara(string path)
        {
            var characters = GetSelectedCharacters();
            if(characters.Count > 0)
            {
                var empty = new Texture2D(1, 1, TextureFormat.ARGB32, false);
                empty.SetPixel(0, 0, Color.black);
                empty.Apply();

                var charFile = characters[0].charInfo.chaFile;
                charFile.pngData = empty.EncodeToPNG();
                charFile.facePngData = empty.EncodeToPNG();

                using(var fileStream = new FileStream(path, FileMode.Create, FileAccess.Write))
                {
                    charFile.SaveCharaFile(fileStream, true);
                }

                RPCClient.SendMessage(new MsgObject{ path = MakerBridge.TempFilePath });
            }
            else
            {
                Log(LogLevel.Message, "Select a character to send to maker");
            }
        }

        void LoadCharas(MsgObject message)
        {
            var characters = GetSelectedCharacters();
            if(characters.Count > 0)
            {
                Log(LogLevel.Message, "Character received");
                
                foreach(var chara in characters)
                {
                    chara.ChangeChara(message.path);
                }

                UpdateStateInfo();
            }
            else
            {
                Log(LogLevel.Message, "Select a character before replacing it");
            }
        }

        List<OCIChar> GetSelectedCharacters()
        {
            return GuideObjectManager.Instance.selectObjectKey.Select(x => Studio.Studio.GetCtrlInfo(x) as OCIChar).Where(x => x != null).ToList();
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
