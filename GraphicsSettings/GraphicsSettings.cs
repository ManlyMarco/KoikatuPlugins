using System;
using System.ComponentModel;
using UnityEngine;
using BepInEx;

namespace GraphicsSettings
{
    [BepInPlugin("keelhauled.graphicssettings", "Graphics Settings", "1.0.0")]
    public class GraphicsSettings : BaseUnityPlugin
    {
        // settings to add
        // rimlighting toggle
        // max fov adjustment

        const string CATEGORY_RESOLUTION = "Resolution settings";
        const string CATEGORY_RENDER = "Rendering settings";
        const string CATEGORY_SHADOW = "Shadow settings";
        const string CATEGORY_MISC = "Misc settings";

        [Category(CATEGORY_RESOLUTION)]
        [DisplayName("Fullscreen")]
        ConfigWrapper<bool> Fullscreen { get; }

        [Category(CATEGORY_RESOLUTION)]
        [DisplayName("!Resolution X")]
        ConfigWrapper<int> ResolutionX { get; }

        [Category(CATEGORY_RESOLUTION)]
        [DisplayName("!Resolution Y")]
        ConfigWrapper<int> ResolutionY { get; }

        [Browsable(true)]
        [Category(CATEGORY_RESOLUTION)]
        [DisplayName("!")]
        [CustomSettingDraw(nameof(ApplyResolutionDrawer))]
        string ApplyResolution { get; set; } = "";

        [Category(CATEGORY_RENDER)]
        [DisplayName("VSync level")]
        ConfigWrapper<VSyncType> VSyncCount { get; }

        [Category(CATEGORY_RENDER)]
        [DisplayName("Frame rate limiter")]
        [Description("VSync has to be disabled for this to take effect.")]
        ConfigWrapper<bool> LimitFrameRate { get; }

        [Category(CATEGORY_RENDER)]
        [DisplayName("Frame rate limit")]
        [AcceptableValueRange(20, 200, false)]
        ConfigWrapper<int> TargetFrameRate { get; }

        [Category(CATEGORY_RENDER)]
        [DisplayName("Anti-aliasing")]
        [AcceptableValueList(new object[]{0, 2, 4, 8})]
        ConfigWrapper<int> AntiAliasing { get; }

        [Category(CATEGORY_RENDER)]
        [DisplayName("Anisotropic filtering")]
        ConfigWrapper<AnisotropicFiltering> AnisotropicTextures { get; }

        [Category(CATEGORY_SHADOW)]
        [DisplayName("Shadow type")]
        ConfigWrapper<ShadowQuality> ShadowType { get; }

        [Category(CATEGORY_SHADOW)]
        [DisplayName("Shadow resolution")]
        ConfigWrapper<ShadowResolution> ShadowRes { get; }

        [Category(CATEGORY_SHADOW)]
        [DisplayName("Shadow projection")]
        ConfigWrapper<ShadowProjection> ShadowProject { get; }

        [Category(CATEGORY_SHADOW)]
        [DisplayName("Shadow cascades")]
        [AcceptableValueList(new object[]{0, 2, 4})]
        ConfigWrapper<int> ShadowCascades { get; }

        [Category(CATEGORY_SHADOW)]
        [DisplayName("Shadow distance")]
        [AcceptableValueRange(0f, 100f, false)]
        ConfigWrapper<float> ShadowDistance { get; }

        [Category(CATEGORY_SHADOW)]
        [DisplayName("Shadow near plane offset")]
        [AcceptableValueRange(0f, 4f, false)]
        ConfigWrapper<float> ShadowNearPlaneOffset { get; }

        // this value is not loaded on start yet
        [Category(CATEGORY_MISC)]
        [DisplayName("Camera near clip plane")]
        [AcceptableValueRange(0.01f, 0.06f, false)]
        ConfigWrapper<float> CameraNearClipPlane { get; }

        GraphicsSettings()
        {
            Fullscreen = new ConfigWrapper<bool>("Fullscreen", this, false);
            ResolutionX = new ConfigWrapper<int>("ResolutionX", this, 1920);
            ResolutionY = new ConfigWrapper<int>("ResolutionY", this, 1080);
            VSyncCount = new ConfigWrapper<VSyncType>("VSyncCount", this, VSyncType.Enabled);
            LimitFrameRate = new ConfigWrapper<bool>("EnableFramerateLimit", this, false);
            TargetFrameRate = new ConfigWrapper<int>("TargetFrameRate", this, 60);
            AntiAliasing = new ConfigWrapper<int>("AntiAliasing", this, 8);
            AnisotropicTextures = new ConfigWrapper<AnisotropicFiltering>("AnisotropicTextures", this, AnisotropicFiltering.ForceEnable);
            ShadowType = new ConfigWrapper<ShadowQuality>("ShadowType", this, ShadowQuality.All);
            ShadowRes = new ConfigWrapper<ShadowResolution>("ShadowRes", this, ShadowResolution.VeryHigh);
            ShadowProject = new ConfigWrapper<ShadowProjection>("ShadowProject", this, ShadowProjection.CloseFit);
            ShadowCascades = new ConfigWrapper<int>("ShadowCascades", this, 4);
            ShadowDistance = new ConfigWrapper<float>("ShadowDistance", this, 50f);
            ShadowNearPlaneOffset = new ConfigWrapper<float>("ShadowNearPlaneOffset", this, 2f);
            CameraNearClipPlane = new ConfigWrapper<float>("CameraNearClipPlane", this, 0.06f);
        }

        void ApplyResolutionDrawer()
        {
            if(GUILayout.Button("Apply resolution", GUILayout.ExpandWidth(true)))
            {
                if(Screen.width != ResolutionX.Value || Screen.height != ResolutionY.Value)
                    Screen.SetResolution(ResolutionX.Value, ResolutionY.Value, Screen.fullScreen);
            }
        }

        void Awake()
        {
            Fullscreen.SettingChanged += (sender, args) => Screen.SetResolution(Screen.width, Screen.height, Fullscreen.Value);

            QualitySettings.vSyncCount = (int)VSyncCount.Value;
            VSyncCount.SettingChanged += (sender, args) => QualitySettings.vSyncCount = (int)VSyncCount.Value;

            if(LimitFrameRate.Value) Application.targetFrameRate = TargetFrameRate.Value;
            LimitFrameRate.SettingChanged += (sender, args) => Application.targetFrameRate = LimitFrameRate.Value ? TargetFrameRate.Value : -1;
            TargetFrameRate.SettingChanged += (sender, args) => { if(LimitFrameRate.Value) Application.targetFrameRate = TargetFrameRate.Value; };

            QualitySettings.antiAliasing = AntiAliasing.Value;
            AntiAliasing.SettingChanged += (sender, args) => QualitySettings.antiAliasing = AntiAliasing.Value;

            QualitySettings.anisotropicFiltering = AnisotropicTextures.Value;
            AnisotropicTextures.SettingChanged += (sender, args) => QualitySettings.anisotropicFiltering = AnisotropicTextures.Value;

            QualitySettings.shadows = ShadowType.Value;
            ShadowType.SettingChanged += (sender, args) => QualitySettings.shadows = ShadowType.Value;

            QualitySettings.shadowResolution = ShadowRes.Value;
            ShadowRes.SettingChanged += (sender, args) => QualitySettings.shadowResolution = ShadowRes.Value;

            QualitySettings.shadowProjection = ShadowProject.Value;
            ShadowProject.SettingChanged += (sender, args) => QualitySettings.shadowProjection = ShadowProject.Value;

            QualitySettings.shadowCascades = ShadowCascades.Value;
            ShadowCascades.SettingChanged += (sender, args) => QualitySettings.shadowCascades = ShadowCascades.Value;
            
            QualitySettings.shadowDistance = ShadowDistance.Value;
            ShadowDistance.SettingChanged += (sender, args) => QualitySettings.shadowDistance = ShadowDistance.Value;

            QualitySettings.shadowNearPlaneOffset = ShadowNearPlaneOffset.Value;
            ShadowNearPlaneOffset.SettingChanged += (sender, args) => QualitySettings.shadowNearPlaneOffset = ShadowNearPlaneOffset.Value;

            CameraNearClipPlane.SettingChanged += (sender, args) => { if(Camera.main) Camera.main.nearClipPlane = CameraNearClipPlane.Value; };
        }

        enum VSyncType
        {
            Disabled,
            Enabled,
            Half
        }
    }
}
