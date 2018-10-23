using System;
using System.ComponentModel;
using UnityEngine;
using BepInEx;

namespace GraphicsSettings
{
    [BepInPlugin("keelhauled.graphicssettings", "Graphics Settings", "1.0.0")]
    public class GraphicsSettings : BaseUnityPlugin
    {
        private const string ResetValue = "Reset";

        [DisplayName("VSync level")]
        ConfigWrapper<VSyncType> VSyncCount { get; }

        [DisplayName("Enable frame rate limiter")]
        ConfigWrapper<bool> LimitFrameRate { get; }

        [DisplayName("Target frame rate")]
        [Description("VSync has to be disabled for this to take effect.\n-1 indicates that the frame rate is unlocked.")]
        [AcceptableValueRange(20, 200, false)]
        ConfigWrapper<int> TargetFrameRate { get; }

        [DisplayName("Antialiasing")]
        [AcceptableValueList(new object[]{0, 2, 4, 8})]
        ConfigWrapper<int> AntiAliasing { get; }

        GraphicsSettings()
        {
            VSyncCount = new ConfigWrapper<VSyncType>("VSyncCount", this, VSyncType.Enabled);
            LimitFrameRate = new ConfigWrapper<bool>("EnableFramerateLimit", this, false);
            TargetFrameRate = new ConfigWrapper<int>("TargetFrameRate", this, 60);
            AntiAliasing = new ConfigWrapper<int>("AntiAliasing", this, 8);
        }

        void Awake()
        {
            QualitySettings.vSyncCount = (int)VSyncCount.Value;
            VSyncCount.SettingChanged += (sender, args) => QualitySettings.vSyncCount = (int)VSyncCount.Value;

            Application.targetFrameRate = LimitFrameRate.Value ? TargetFrameRate.Value : -1;
            LimitFrameRate.SettingChanged += (sender, args) => Application.targetFrameRate = LimitFrameRate.Value ? TargetFrameRate.Value : -1;

            if(LimitFrameRate.Value) Application.targetFrameRate = TargetFrameRate.Value;
            TargetFrameRate.SettingChanged += (sender, args) => { if(LimitFrameRate.Value) Application.targetFrameRate = TargetFrameRate.Value; };

            QualitySettings.antiAliasing = AntiAliasing.Value;
            AntiAliasing.SettingChanged += (sender, args) => QualitySettings.antiAliasing = AntiAliasing.Value;
        }

        enum VSyncType
        {
            Disabled,
            Enabled,
            Half
        }
    }
}
