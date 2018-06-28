using System;
using System.IO;
using System.Linq;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;
using Studio;
using UILib;

// change sex of studio character accordingly when loading

namespace CardOrganizer
{
    class StudioLoaderUI : MonoBehaviour
    {
        static string mainPath = Environment.CurrentDirectory + "/UserData/CardOrganizer/";
        static string scenePath = mainPath + "scene/";
        static string charaPath = mainPath + "chara/";
        static string coordPath = mainPath + "coordinate/";

        float buttonSize = 10f;
        float marginSize = 5f;
        float headerSize = 20f;
        float UIScale = 1.0f;
        float scrollOffsetX = -15f;
        float windowMargin = 130f;

        Color dragColor = new Color(0.4f, 0.4f, 0.4f, 1f);
        Color backgroundColor = new Color(1f, 1f, 1f, 1f);
        Color outlineColor = new Color(0f, 0f, 0f, 1f);

        Canvas UISystem;
        Image mainPanel;
        Dropdown category;
        Dropdown cardType;
        ScrollRect imagelist;
        Image optionspanel;
        Image confirmpanel;
        Button yesbutton;
        Button nobutton;
        Button loadbutton;
        Button importbutton;
        Button savebutton;
        Text loadText;
        Text importText;

        float scrollSensitivity;
        int sceneColumnCount;
        int charaColumnCount;
        bool sceneAutoClose;
        bool charaAutoClose;
        bool smallWindow;

        Dictionary<string, Image> listCache = new Dictionary<string, Image>();
        Button currentButton;
        string currentPath;
        string currentTypePath = scenePath;
        LoaderType currentType = LoaderType.Scene;

        Dictionary<LoaderType, int> cardTypes = new Dictionary<LoaderType, int>()
        {
            { LoaderType.Scene, 0 },
            { LoaderType.Character, 0 },
            { LoaderType.Coordinate, 0 },
        };

        void Awake()
        {
            UIUtility.Init();
            CreateUI();
            LoadSettings();
            StartCoroutine(StartingScene());
        }

        IEnumerator StartingScene()
        {
            for(int i = 0; i < 10; i++) yield return null;
            var files = Directory.GetFiles(scenePath, "defaultscene.png", SearchOption.TopDirectoryOnly).ToList();
            if(files.Count > 0) LoadCard(files[0]);
        }

        void Start()
        {
            FixButtons();
        }

        void OnDestroy()
        {
            DestroyImmediate(UISystem.gameObject);
        }

        bool LoadSettings()
        {
            smallWindow = bool.Parse(BepInEx.Config.GetEntry("SmallWindow", "true", CardOrganizer.configName));
            scrollSensitivity = float.Parse(BepInEx.Config.GetEntry("ScrollSensitivity", "3", CardOrganizer.configName));
            sceneColumnCount = int.Parse(BepInEx.Config.GetEntry("SceneColumns", "3", CardOrganizer.configName));
            charaColumnCount = int.Parse(BepInEx.Config.GetEntry("CharaColumns", "5", CardOrganizer.configName));
            sceneAutoClose = bool.Parse(BepInEx.Config.GetEntry("SceneAutoClose", "false", CardOrganizer.configName));
            charaAutoClose = bool.Parse(BepInEx.Config.GetEntry("CharaAutoClose", "false", CardOrganizer.configName));

            //listCache = new Dictionary<string, Image>(); // hack to ensure every list has the right amount of columns

            UpdateWindow();
            return true;
        }

        void UpdateWindow()
        {
            foreach(var scene in listCache)
            {
                var gridlayout = scene.Value.gameObject.GetComponent<AutoGridLayout>();
                if(gridlayout != null)
                {
                    gridlayout.m_Column = sceneColumnCount;
                    gridlayout.CalculateLayoutInputHorizontal();
                }
            }

            if(imagelist != null)
            {
                imagelist.scrollSensitivity = Mathf.Lerp(30f, 300f, scrollSensitivity / 10f);
            }

            if(mainPanel)
            {
                if(smallWindow)
                    mainPanel.transform.SetRect(0.5f, 0f, 1f, 1f, windowMargin, windowMargin, -windowMargin, -windowMargin);
                else
                    mainPanel.transform.SetRect(0f, 0f, 1f, 1f, windowMargin, windowMargin, -windowMargin, -windowMargin);
            }
        }

        void CreateUI()
        {
            UISystem = UIUtility.CreateNewUISystem("CardOrganizerCanvas");
            UISystem.GetComponent<CanvasScaler>().referenceResolution = new Vector2(1920f / UIScale, 1080f / UIScale);
            UISystem.gameObject.SetActive(false);
            UISystem.gameObject.transform.SetParent(transform);

            mainPanel = UIUtility.CreatePanel("Panel", UISystem.transform);
            mainPanel.color = backgroundColor;
            UIUtility.AddOutlineToObject(mainPanel.transform, outlineColor);

            var drag = UIUtility.CreatePanel("Draggable", mainPanel.transform);
            drag.transform.SetRect(0f, 1f, 1f, 1f, 0f, -headerSize);
            drag.color = dragColor;
            UIUtility.MakeObjectDraggable(drag.rectTransform, mainPanel.rectTransform);

            var close = UIUtility.CreateButton("CloseButton", drag.transform, "");
            close.transform.SetRect(1f, 0f, 1f, 1f, -buttonSize * 2f);
            close.onClick.AddListener(() => UISystem.gameObject.SetActive(false));
            Utils.AddCloseSymbol(close);

            category = UIUtility.CreateDropdown("Dropdown", drag.transform, "Categories");
            category.transform.SetRect(0f, 0f, 0f, 1f, 80f, 0f, 180f);
            category.captionText.transform.SetRect(0f, 0f, 1f, 1f, 0f, 2f, -15f, -2f);
            category.captionText.alignment = TextAnchor.MiddleCenter;
            category.options = GetCategories();
            category.onValueChanged.AddListener((x) =>
            {
                cardTypes[currentType] = x;
                imagelist.content.GetComponentInChildren<Image>().gameObject.SetActive(false);
                imagelist.content.anchoredPosition = new Vector2(0f, 0f);
                PopulateGrid();
            });

            var refresh = UIUtility.CreateButton("RefreshButton", drag.transform, "Refresh");
            refresh.transform.SetRect(0f, 0f, 0f, 1f, 180f, 0f, 260f);
            refresh.onClick.AddListener(() => ReloadImages());

            savebutton = UIUtility.CreateButton("SaveButton", drag.transform, "Save");
            savebutton.transform.SetRect(0f, 0f, 0f, 1f, 260f, 0f, 340f);
            savebutton.onClick.AddListener(() => SaveCard());

            var folder = UIUtility.CreateButton("FolderButton", drag.transform, "Folder");
            folder.transform.SetRect(0f, 0f, 0f, 1f, 340f, 0f, 420f);
            folder.onClick.AddListener(() => Process.Start(currentTypePath));

            cardType = UIUtility.CreateDropdown("CardType", drag.transform, "CardType");
            cardType.transform.SetRect(0f, 0f, 0f, 1f, 0f, 0f, 80f);
            cardType.captionText.transform.SetRect(0f, 0f, 1f, 1f, 0f, 2f, -15f, -2f);
            cardType.captionText.alignment = TextAnchor.MiddleCenter;
            cardType.onValueChanged.AddListener((x) => ChangeListType(x));
            cardType.options = cardTypes.Keys.Select((x) => new Dropdown.OptionData(x.ToString().Substring(0, 5))).ToList();

            var loadingPanel = UIUtility.CreatePanel("LoadingIconPanel", drag.transform);
            loadingPanel.transform.SetRect(0f, 0f, 0f, 1f, 420f, 0f, 420f + headerSize);
            loadingPanel.color = new Color(0f, 0f, 0f, 0f);
            var loadingIcon = UIUtility.CreatePanel("LoadingIcon", loadingPanel.transform);
            loadingIcon.transform.SetRect(0.1f, 0.1f, 0.9f, 0.9f);
            var texture = Utils.LoadTexture(Properties.Resources.loadicon);
            loadingIcon.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
            LoadingIcon.Init(loadingIcon, -5f);

            imagelist = UIUtility.CreateScrollView("Imagelist", mainPanel.transform);
            imagelist.transform.SetRect(0f, 0f, 1f, 1f, marginSize, marginSize, -marginSize, -headerSize - marginSize / 2f);
            imagelist.gameObject.AddComponent<Mask>();
            imagelist.content.gameObject.AddComponent<VerticalLayoutGroup>();
            imagelist.content.gameObject.AddComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            imagelist.verticalScrollbar.GetComponent<RectTransform>().offsetMin = new Vector2(scrollOffsetX, 0f);
            imagelist.viewport.offsetMax = new Vector2(scrollOffsetX, 0f);
            imagelist.movementType = ScrollRect.MovementType.Clamped;

            optionspanel = UIUtility.CreatePanel("ButtonPanel", imagelist.transform);
            optionspanel.gameObject.SetActive(false);

            confirmpanel = UIUtility.CreatePanel("ConfirmPanel", imagelist.transform);
            confirmpanel.gameObject.SetActive(false);

            yesbutton = UIUtility.CreateButton("YesButton", confirmpanel.transform, "Y");
            yesbutton.transform.SetRect(0f, 0f, 0.5f, 1f);
            yesbutton.onClick.AddListener(() => DeleteCard(currentPath));

            nobutton = UIUtility.CreateButton("NoButton", confirmpanel.transform, "N");
            nobutton.transform.SetRect(0.5f, 0f, 1f, 1f);
            nobutton.onClick.AddListener(() => confirmpanel.gameObject.SetActive(false));

            loadbutton = UIUtility.CreateButton("LoadButton", optionspanel.transform, "Load");
            loadbutton.transform.SetRect(0f, 0f, 0.3f, 1f);
            loadbutton.onClick.AddListener(() => LoadCard(currentPath));
            loadText = loadbutton.GetComponentInChildren<Text>();

            importbutton = UIUtility.CreateButton("ImportButton", optionspanel.transform, "Import");
            importbutton.transform.SetRect(0.35f, 0f, 0.65f, 1f);
            importbutton.onClick.AddListener(() => ImportCard(currentPath));
            importText = importbutton.GetComponentInChildren<Text>();

            var deletebutton = UIUtility.CreateButton("DeleteButton", optionspanel.transform, "Delete");
            deletebutton.transform.SetRect(0.7f, 0f, 1f, 1f);
            deletebutton.onClick.AddListener(() => confirmpanel.gameObject.SetActive(true));

            PopulateGrid();
        }

        List<Dropdown.OptionData> GetCategories()
        {
            if(!File.Exists(currentTypePath)) Directory.CreateDirectory(currentTypePath);
            var folders = Directory.GetDirectories(currentTypePath);

            if(folders.Length == 0)
            {
                Directory.CreateDirectory(currentTypePath + "Category1");
                Directory.CreateDirectory(currentTypePath + "Category2");
                folders = Directory.GetDirectories(currentTypePath);
            }

            string orderPath = currentTypePath + "order.txt";
            string[] order;
            if(File.Exists(orderPath))
            {
                order = File.ReadAllLines(orderPath);
            }
            else
            {
                order = new string[0];
                File.Create(orderPath);
            }

            var sorted = folders.Select(x => Path.GetFileName(x)).OrderBy(x => order.Contains(x) ? Array.IndexOf(order, x) : order.Length);
            return sorted.Select(x => new Dropdown.OptionData(x)).ToList();
        }

        void ChangeListType(int index)
        {
            confirmpanel.gameObject.SetActive(false);
            optionspanel.gameObject.SetActive(false);

            switch(index)
            {
                case 0:
                {
                    currentType = LoaderType.Scene;
                    currentTypePath = scenePath;
                    loadText.text = "Load";
                    importText.text = "Import";
                    savebutton.interactable = true;
                    importbutton.interactable = true;
                    break;
                }

                case 1:
                {
                    currentType = LoaderType.Character;
                    currentTypePath = charaPath;
                    loadText.text = "Load";
                    importText.text = "Replace";
                    savebutton.interactable = false;
                    importbutton.interactable = true;
                    break;
                }

                case 2:
                {
                    currentType = LoaderType.Coordinate;
                    currentTypePath = coordPath;
                    loadText.text = "Load";
                    importText.text = "";
                    savebutton.interactable = false;
                    importbutton.interactable = false;
                    break;
                }
            }

            category.options = GetCategories();
            category.value = cardTypes[currentType];
            imagelist.content.GetComponentInChildren<Image>().gameObject.SetActive(false);
            imagelist.content.anchoredPosition = new Vector2(0f, 0f);
            PopulateGrid();
        }

        void LoadCard(string path)
        {
            confirmpanel.gameObject.SetActive(false);
            optionspanel.gameObject.SetActive(false);

            switch(currentType)
            {
                case LoaderType.Scene:
                {
                    StartCoroutine(Studio.Studio.Instance.LoadSceneCoroutine(path));
                    if(sceneAutoClose) UISystem.gameObject.SetActive(false);
                    break;
                }

                case LoaderType.Character:
                { 
                    var ocicharFemale = AddObjectFemale.Add(path);
                    if(Studio.Studio.optionSystem.autoSelect && ocicharFemale != null)
                        Studio.Studio.Instance.treeNodeCtrl.SelectSingle(ocicharFemale.treeNodeObject);
                    if(charaAutoClose) UISystem.gameObject.SetActive(false);
                    break;
                }

                case LoaderType.Coordinate:
                {
                    var list = (from v in GuideObjectManager.Instance.selectObjectKey
                                select Studio.Studio.GetCtrlInfo(v) as OCIChar into v
                                where v != null
                                //where v.oiCharInfo.sex == 1
                                select v).ToList();

                    if(list.Count > 0)
                    {
                        confirmpanel.gameObject.SetActive(false);
                        optionspanel.gameObject.SetActive(false);
                        list.ForEach(x => x.LoadClothesFile(path));
                        if(charaAutoClose) UISystem.gameObject.SetActive(false);
                    }

                    break;
                }
            }
        }

        void SaveCard()
        {
            switch(currentType)
            {
                case LoaderType.Scene:
                {
                    Studio.Studio.Instance.dicObjectCtrl.Values.ToList().ForEach(x => x.OnSavePreprocessing());
                    Studio.Studio.Instance.sceneInfo.cameraSaveData = Studio.Studio.Instance.cameraCtrl.Export();
                    string path = GetCategoryFolder() + DateTime.Now.ToString("yyyy_MMdd_HHmm_ss_fff") + ".png";
                    Studio.Studio.Instance.sceneInfo.Save(path);
                    var button = CreateSceneButton(imagelist.content.GetComponentInChildren<Image>().transform, PngAssist.LoadTexture(path), path);
                    button.transform.SetAsFirstSibling();
                    break;
                }

                case LoaderType.Character:
                {
                    break;
                }

                case LoaderType.Coordinate:
                {
                    break;
                }
            }
        }

        void ImportCard(string path)
        {
            confirmpanel.gameObject.SetActive(false);
            optionspanel.gameObject.SetActive(false);

            switch(currentType)
            {
                case LoaderType.Scene:
                {
                    Studio.Studio.Instance.ImportScene(path);
                    break;
                }

                case LoaderType.Character:
                {
                    var list = (from v in GuideObjectManager.Instance.selectObjectKey
                                       select Studio.Studio.GetCtrlInfo(v) as OCIChar into v
                                       where v != null
                                       //where v.oiCharInfo.sex == 1
                                       select v).ToList();

                    if(list.Count > 0)
                    {
                        confirmpanel.gameObject.SetActive(false);
                        optionspanel.gameObject.SetActive(false);
                        list.ForEach(x => x.ChangeChara(path));
                    }

                    break;
                }

                case LoaderType.Coordinate:
                {
                    break;
                }
            }
        }

        void DeleteCard(string path)
        {
            File.Delete(path);
            currentButton.gameObject.SetActive(false);
            confirmpanel.gameObject.SetActive(false);
            optionspanel.gameObject.SetActive(false);
        }

        void ReloadImages()
        {
            optionspanel.transform.SetParent(imagelist.transform);
            confirmpanel.transform.SetParent(imagelist.transform);
            optionspanel.gameObject.SetActive(false);
            confirmpanel.gameObject.SetActive(false);

            Destroy(imagelist.content.GetComponentInChildren<Image>().gameObject);
            imagelist.content.anchoredPosition = new Vector2(0f, 0f);
            PopulateGrid(true);
        }

        void PopulateGrid(bool forceUpdate = false)
        {
            string categoryText = currentType.ToString() + category.captionText.text;
            if(forceUpdate) listCache.Remove(categoryText);

            if(listCache.TryGetValue(categoryText, out Image list))
            {
                list.gameObject.SetActive(true);
            }
            else
            {
                var scenefiles = (from s in Directory.GetFiles(GetCategoryFolder(), "*.png") select new KeyValuePair<DateTime, string>(File.GetLastWriteTime(s), s)).ToList();
                scenefiles.Sort((KeyValuePair<DateTime, string> a, KeyValuePair<DateTime, string> b) => b.Key.CompareTo(a.Key));

                var container = UIUtility.CreatePanel("GridContainer", imagelist.content.transform);
                container.transform.SetRect(0f, 0f, 1f, 1f);
                container.gameObject.AddComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;

                var gridlayout = container.gameObject.AddComponent<AutoGridLayout>();
                gridlayout.spacing = new Vector2(marginSize, marginSize);
                gridlayout.m_IsColumn = true;
                gridlayout.m_Column = currentType == LoaderType.Scene ? sceneColumnCount : charaColumnCount;
                gridlayout.aspectRatio = currentType == LoaderType.Scene ? 16f / 9f : 3f / 4f;

                StartCoroutine(LoadButtonsAsync(container.transform, scenefiles));
                listCache.Add(categoryText, container);
            }

            IEnumerator LoadButtonsAsync(Transform parent, List<KeyValuePair<DateTime, string>> scenefiles)
            {
                foreach(var scene in scenefiles)
                {
                    LoadingIcon.loadingState[categoryText] = true;

                    using(WWW www = new WWW("file:///" + scene.Value))
                    {
                        yield return www;
                        if(!string.IsNullOrEmpty(www.error)) throw new Exception(www.error);
                        CreateSceneButton(parent, PngAssist.ChangeTextureFromByte(www.bytes), scene.Value);
                    }
                }

                LoadingIcon.loadingState[categoryText] = false;
            }
        }

        Button CreateSceneButton(Transform parent, Texture2D texture, string path)
        {
            var button = UIUtility.CreateButton("ImageButton", parent, "");
            button.onClick.AddListener(() =>
            {
                currentButton = button;
                currentPath = path;

                if(optionspanel.transform.parent != button.transform)
                {
                    optionspanel.transform.SetParent(button.transform);
                    optionspanel.transform.SetRect(0f, 0f, 1f, 0.15f);
                    optionspanel.gameObject.SetActive(true);

                    confirmpanel.transform.SetParent(button.transform);
                    confirmpanel.transform.SetRect(0.4f, 0.4f, 0.6f, 0.6f);
                }
                else
                {
                    optionspanel.gameObject.SetActive(!optionspanel.gameObject.activeSelf);
                }

                confirmpanel.gameObject.SetActive(false);
            });

            var sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
            button.gameObject.GetComponent<Image>().sprite = sprite;

            return button;
        }

        string GetCategoryFolder()
        {
            if(category?.captionText?.text != null)
                return currentTypePath + category.captionText.text + "/";

            return currentTypePath;
        }

        void FixButtons()
        {
            var loadbuttonScene = GameObject.Find("StudioScene/Canvas Main Menu/04_System/Viewport/Content/Load");
            if(loadbuttonScene)
            {
                var button = loadbuttonScene.GetComponent<Button>();
                if(button)
                {
                    button.onClick = new Button.ButtonClickedEvent();
                    button.onClick.AddListener(() => ChangeUI(LoaderType.Scene));
                }
            }

            var savebuttonScene = GameObject.Find("StudioScene/Canvas Main Menu/04_System/Viewport/Content/Save");
            if(savebuttonScene)
            {
                var button = savebuttonScene.GetComponent<Button>();
                if(button)
                {
                    button.onClick = new Button.ButtonClickedEvent();
                    //button.onClick.AddListener(() => SaveCard());
                    button.interactable = false;
                }
            }

            var loadbuttonFemale = GameObject.Find("StudioScene/Canvas Main Menu/01_Add/Scroll View Add Group/Viewport/Content/Chara Female");
            if(loadbuttonFemale)
            {
                var button = loadbuttonFemale.GetComponent<Button>();
                if(button)
                {
                    button.onClick = new Button.ButtonClickedEvent();
                    button.onClick.AddListener(() => ChangeUI(LoaderType.Character));
                }
            }

            var loadbuttonMale = GameObject.Find("StudioScene/Canvas Main Menu/01_Add/Scroll View Add Group/Viewport/Content/Chara Male");
            if(loadbuttonMale)
            {
                var button = loadbuttonMale.GetComponent<Button>();
                if(button)
                {
                    button.onClick = new Button.ButtonClickedEvent();
                    button.interactable = false;
                }
            }

            var costumeButton = GameObject.Find("StudioScene/Canvas Main Menu/02_Manipulate/00_Chara/00_Root/Viewport/Content/Cos");
            if(costumeButton)
            {
                var button = costumeButton.GetComponent<Button>();
                if(button)
                {
                    button.onClick = new Button.ButtonClickedEvent();
                    button.onClick.AddListener(() => ChangeUI(LoaderType.Coordinate));
                }
            }

            void ChangeUI(LoaderType loaderType)
            {
                int type = (int)loaderType;

                if(UISystem.gameObject.activeSelf && cardType.value != type)
                {
                    cardType.value = type;
                    ChangeListType(type);
                }
                else if(UISystem.gameObject.activeSelf)
                {
                    UISystem.gameObject.SetActive(false);
                }
                else
                {
                    UISystem.gameObject.SetActive(true);
                    ChangeListType(type);
                }
            }
        }

        enum LoaderType
        {
            Scene,
            Character,
            Coordinate
        }
    }
}
