using System;
using System.Collections;
using ChaCustom;
using Harmony;
using Manager;
using UnityEngine;

namespace CardOrganizer
{
    class CharaMaker : MonoBehaviour
    {
        CustomCharaFile customCharaFile;
        CustomFileListCtrl listCtrl;
        string folderName;

        void Start()
        {
            customCharaFile = FindObjectOfType<CustomCharaFile>();
            listCtrl = Traverse.Create(customCharaFile).Field("listCtrl").GetValue<CustomFileListCtrl>();
            folderName = BepInEx.Config.GetEntry("FolderName", "", CardOrganizer.configName);
            StartCoroutine(SetListFolderDelayed());
        }

        IEnumerator SetListFolderDelayed()
        {
            yield return null;
            if(folderName != "") SetListFolder(folderName);
        }

        void SetListFolder(string folderName)
        {
            int modeSex = Singleton<CustomBase>.Instance.modeSex;
            FolderAssist folderAssist = new FolderAssist();
            string folder = UserData.Path + (modeSex != 0 ? string.Format("CardOrganizer/chara/{0}/", folderName) : "chara/male/");
            folderAssist.CreateFolderInfoEx(folder, new string[]{ "*.png" }, true);
            listCtrl.ClearList();

            int num = 0;
            for(int i = 0; i < folderAssist.GetFileCount(); i++)
            {
                ChaFileControl chaFileControl = new ChaFileControl();
                if(!chaFileControl.LoadCharaFile(folderAssist.lstFile[i].FullPath, 255, false, true))
                {
                    int lastErrorCode = chaFileControl.GetLastErrorCode();
                }
                else if(chaFileControl.parameter.sex == modeSex)
                {
                    string club = "";
                    string personality = "";

                    if(modeSex != 0)
                    {
                        personality = Voice.Instance.voiceInfoDic.TryGetValue(chaFileControl.parameter.personality, out VoiceInfo.Param param) ? param.Personality : "不明";
                        club = Game.ClubInfos.TryGetValue(chaFileControl.parameter.clubActivities, out ClubInfo.Param param2) ? param2.Name : "不明";
                    }
                    else
                    {
                        listCtrl.DisableAddInfo();
                    }

                    listCtrl.AddList(num, chaFileControl.parameter.fullname, club, personality, folderAssist.lstFile[i].FullPath, folderAssist.lstFile[i].FileName, folderAssist.lstFile[i].time, false);
                    num++;
                }
            }

            listCtrl.Create(customCharaFile.OnChangeSelect);
        }

        void LoadCharaInMaker(string path)
        {
            //confirmpanel.gameObject.SetActive(false);
            //optionspanel.gameObject.SetActive(false);
            //if(autoClose) UISystem.gameObject.SetActive(false);

            ChaControl chaCtrl = CustomBase.Instance.chaCtrl;
            bool face = true, body = true, hair = true;
            bool parameter = true;
            bool coordinate = true;
            chaCtrl.chaFile.LoadFileLimited(path, chaCtrl.sex, face, body, hair, parameter, coordinate);
            chaCtrl.ChangeCoordinateType(true);
            chaCtrl.Reload(!coordinate, !face && !coordinate, !hair, !body);
            CustomBase.Instance.updateCustomUI = true;
            CustomHistory.Instance.Add5(chaCtrl, chaCtrl.Reload, !coordinate, !face && !coordinate, !hair, !body);
        }
    }
}
