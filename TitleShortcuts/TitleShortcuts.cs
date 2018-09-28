using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using BepInEx;
using static BepInEx.Logger;
using BepInEx.Logging;

namespace TitleShortcuts
{
    [BepInPlugin("keelhauled.titleshortcuts", "TitleShortcuts", "1.0.0")]
    [BepInProcess("Koikatu.exe")]
    class TitleShortcuts : BaseUnityPlugin
    {
        ConfigWrapper<bool> AutoStart { get; }
        SavedKeyboardShortcut StartFemaleEditor { get; }
        SavedKeyboardShortcut StartMaleEditor { get; }
        SavedKeyboardShortcut StartUploader { get; }
        SavedKeyboardShortcut StartDownloader { get; }
        SavedKeyboardShortcut StartFreeH { get; }
        SavedKeyboardShortcut StartLiveShow { get; }
        bool check = false;

        TitleShortcuts()
        {
            AutoStart = new ConfigWrapper<bool>("AutoStart", this, false);
            StartFemaleEditor = new SavedKeyboardShortcut("Start Female Editor", this, new KeyboardShortcut(KeyCode.F));
            StartMaleEditor = new SavedKeyboardShortcut("Start Male Editor", this, new KeyboardShortcut(KeyCode.M));
            StartUploader = new SavedKeyboardShortcut("Start Uploader", this, new KeyboardShortcut(KeyCode.U));
            StartDownloader = new SavedKeyboardShortcut("Start Downloader", this, new KeyboardShortcut(KeyCode.D));
            StartFreeH = new SavedKeyboardShortcut("Start FreeH", this, new KeyboardShortcut(KeyCode.H));
            StartLiveShow = new SavedKeyboardShortcut("Start Live Show", this, new KeyboardShortcut(KeyCode.L));
        }

        void Awake()
        {
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += (x, y) => StartInput(x);
        }

        void StartInput(Scene scene)
        {
            if(scene.name == "Title")
            {
                if(!check)
                {
                    check = true;
                    StartCoroutine(InputCheck()); 
                }
            }
            else
            {
                check = false;
            }
        }

        IEnumerator InputCheck()
        {
            while(check)
            {
                if(!Manager.Scene.Instance.IsNowLoadingFade)
                {
                    if(StartFemaleEditor.IsPressed())
                    {
                        OnCustomFemale();
                    }
                    else if(StartMaleEditor.IsPressed())
                    {
                        OnCustomMale();
                    }

                    else if(StartUploader.IsPressed())
                    {
                        OnUploader();
                    }
                    else if(StartDownloader.IsPressed())
                    {
                        OnDownloader();
                    }

                    else if(StartFreeH.IsPressed())
                    {
                        OnOtherFreeH();
                    }
                    else if(StartLiveShow.IsPressed())
                    {
                        OnOtherIdolLive();
                    }

                    else if(AutoStart.Value)
                    {
                        OnCustomFemale();
                    }
                }

                yield return null;
            }
        }

        void OnCustomMale()
        {
            Log(LogLevel.Message, StartMaleEditor.Key);
            Singleton<TitleScene>.Instance.OnCustomMale();
            check = false;
        }

        void OnCustomFemale()
        {
            Log(LogLevel.Message, StartFemaleEditor.Key);
            Singleton<TitleScene>.Instance.OnCustomFemale();
            check = false;
        }

        void OnUploader()
        {
            Log(LogLevel.Message, StartUploader.Key);
            Singleton<TitleScene>.Instance.OnUploader();
            check = false;
        }

        void OnDownloader()
        {
            Log(LogLevel.Message, StartDownloader.Key);
            Singleton<TitleScene>.Instance.OnDownloader();
            check = false;
        }

        void OnOtherFreeH()
        {
            Log(LogLevel.Message, StartFreeH.Key);
            Singleton<TitleScene>.Instance.OnOtherFreeH();
            check = false;
        }

        void OnOtherIdolLive()
        {
            Log(LogLevel.Message, StartLiveShow.Key);
            Singleton<TitleScene>.Instance.OnOtherIdolLive();
            check = false;
        }
    }
}
