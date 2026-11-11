using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    public Vector2 JoystickVector;
    private GameManager _gm;
    private TMP_FontAsset _fontAsset;
    private Canvas _canvas;
    private GameObject _menuPanel;
    private GameObject _settingsPanel;
    private GameObject _creditsPanel;
    private GameObject _hudPanel;
    private GameObject _pausePanel;
    private GameObject _endPanel;
    private TextMeshProUGUI _healthText;
    private Image _healthFill;
    private Image _armorFill;
    private TextMeshProUGUI _ammoText;
    private TextMeshProUGUI _scoreText;
    private TextMeshProUGUI _aliveText;
    private TextMeshProUGUI _killfeedText;
    private TextMeshProUGUI _hitmarker;
    private TextMeshProUGUI _centerMessage;
    private Image _damageFlash;
    private TextMeshProUGUI _inventoryText;
    private TextMeshProUGUI _languageButtonText;
    private TextMeshProUGUI _sensitivityText;
    private TextMeshProUGUI _endTitle;
    private TextMeshProUGUI _endStats;
    private Button[] _weaponButtons = new Button[5];
    private readonly List<(TextMeshProUGUI tmp, string key)> _localized = new List<(TextMeshProUGUI, string)>();
    private float _hitmarkerUntil;
    private float _damageFlashUntil;
    private float _centerMsgUntil;
    private readonly List<string> _killfeed = new List<string>();

    public void Initialize(GameManager gm)
    {
        _gm = gm;
        SetupFont();
        _canvas = new GameObject("Canvas").AddComponent<Canvas>();
        _canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        _canvas.sortingOrder = 10;
        CanvasScaler scaler = _canvas.gameObject.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920f, 1080f);
        scaler.matchWidthOrHeight = 0.5f;
        _canvas.gameObject.AddComponent<GraphicRaycaster>();
        BuildMenu();
        BuildSettings();
        BuildCredits();
        BuildHud();
        BuildPause();
        BuildEnd();
        RefreshAllTexts();
        ShowMenu();
    }

    private void SetupFont()
    {
        Font f = Resources.Load<Font>("Fonts/Vazirmatn-Bold");
        if (f != null)
        {
            _fontAsset = TMP_FontAsset.CreateFontAsset(f, 90, 9, TMPro.GlyphRenderMode.SMOOTH, 2048, 2048, TMPro.DynamicAtlasGenerationMode.Dynamic, true);
            if (TMP_Settings.defaultFontAsset != null)
                _fontAsset.fallbackFontAssetTable = new List<TMP_FontAsset> { TMP_Settings.defaultFontAsset };
        }
        else
        {
            _fontAsset = TMP_Settings.defaultFontAsset;
        }
    }

    private RectTransform MakeRect(GameObject parent, string name, Vector2 anchorMin, Vector2 anchorMax, Vector2 pos, Vector2 size)
    {
        GameObject go = new GameObject(name);
        go.transform.SetParent(parent.transform, false);
        RectTransform rt = go.AddComponent<RectTransform>();
        rt.anchorMin = anchorMin;
        rt.anchorMax = anchorMax;
        rt.anchoredPosition = pos;
        rt.sizeDelta = size;
        return rt;
    }

    private Image MakeImage(GameObject parent, string name, Color color, Vector2 anchorMin, Vector2 anchorMax, Vector2 pos, Vector2 size)
    {
        RectTransform rt = MakeRect(parent, name, anchorMin, anchorMax, pos, size);
        Image img = rt.gameObject.AddComponent<Image>();
        img.color = color;
        return img;
    }

    private TextMeshProUGUI MakeText(GameObject parent, string name, string text, int fontSize, TextAlignmentOptions alignment, Color color, Vector2 anchorMin, Vector2 anchorMax, Vector2 pos, Vector2 size)
    {
        RectTransform rt = MakeRect(parent, name, anchorMin, anchorMax, pos, size);
        TextMeshProUGUI tmp = rt.gameObject.AddComponent<TextMeshProUGUI>();
        tmp.font = _fontAsset;
        tmp.fontSize = fontSize;
        tmp.alignment = alignment;
        tmp.color = color;
        tmp.text = text;
        tmp.enableWordWrapping = true;
        tmp.overflowMode = TextOverflowModes.Overflow;
        tmp.raycastTarget = false;
        return tmp;
    }

    private TextMeshProUGUI MakeLocalizedText(GameObject parent, string name, string key, int fontSize, TextAlignmentOptions alignment, Color color, Vector2 anchorMin, Vector2 anchorMax, Vector2 pos, Vector2 size)
    {
        TextMeshProUGUI tmp = MakeText(parent, name, Localization50.Get(_gm.LanguageCode, key), fontSize, alignment, color, anchorMin, anchorMax, pos, size);
        _localized.Add((tmp, key));
        return tmp;
    }

    private Button MakeButton(GameObject parent, string name, string key, Vector2 anchorMin, Vector2 anchorMax, Vector2 pos, Vector2 size, System.Action onClick)
    {
        Image img = MakeImage(parent, name, new Color(0.12f, 0.16f, 0.22f, 0.85f), anchorMin, anchorMax, pos, size);
        Button btn = img.gameObject.AddComponent<Button>();
        btn.targetGraphic = img;
        ColorBlock colors = btn.colors;
        colors.normalColor = Color.white;
        colors.highlightedColor = new Color(0.85f, 0.9f, 1f);
        colors.pressedColor = new Color(0.6f, 0.75f, 1f);
        btn.colors = colors;
        TextMeshProUGUI label = MakeLocalizedText(img.gameObject, "Label", key, 30, TextAlignmentOptions.Center, Color.white, new Vector2(0f, 0f), new Vector2(1f, 1f), Vector2.zero, Vector2.zero);
        label.rectTransform.anchorMin = new Vector2(0f, 0f);
        label.rectTransform.anchorMax = new Vector2(1f, 1f);
        label.rectTransform.anchoredPosition = Vector2.zero;
        label.rectTransform.sizeDelta = Vector2.zero;
        if (onClick != null) btn.onClick.AddListener(() => onClick());
        return btn;
    }

    private Button MakeHoldButton(GameObject parent, string name, string key, Vector2 anchorMin, Vector2 anchorMax, Vector2 pos, Vector2 size, System.Action onDown, System.Action onUp)
    {
        Button btn = MakeButton(parent, name, key, anchorMin, anchorMax, pos, size, null);
        EventTrigger trigger = btn.gameObject.AddComponent<EventTrigger>();
        EventTrigger.Entry down = new EventTrigger.Entry();
        down.eventID = EventTriggerType.PointerDown;
        down.callback.AddListener((data) => { if (onDown != null) onDown(); });
        EventTrigger.Entry up = new EventTrigger.Entry();
        up.eventID = EventTriggerType.PointerUp;
        up.callback.AddListener((data) => { if (onUp != null) onUp(); });
        trigger.triggers = new List<EventTrigger.Entry> { down, up };
        return btn;
    }

    private void BuildMenu()
    {
        _menuPanel = new GameObject("MenuPanel");
        _menuPanel.transform.SetParent(_canvas.transform, false);
        MakeImage(_menuPanel, "Bg", new Color(0.05f, 0.07f, 0.1f, 1f), new Vector2(0f, 0f), new Vector2(1f, 1f), Vector2.zero, Vector2.zero);
        MakeText(_menuPanel, "Title", "COD & PUBG Battlegrounds", 64, TextAlignmentOptions.Center, new Color(1f, 0.85f, 0.3f), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -120f), new Vector2(1200f, 110f));
        MakeLocalizedText(_menuPanel, "Subtitle", "Play", 34, TextAlignmentOptions.Center, Color.white, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -185f), new Vector2(800f, 60f));
        MakeButton(_menuPanel, "BtnBR", "BattleRoyale", new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0f, 170f), new Vector2(520f, 90f), () => _gm.StartGame(GameMode.BattleRoyale));
        MakeButton(_menuPanel, "BtnTDM", "Multiplayer", new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0f, 60f), new Vector2(520f, 90f), () => _gm.StartGame(GameMode.Multiplayer));
        MakeButton(_menuPanel, "BtnSettings", "Settings", new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0f, -50f), new Vector2(520f, 90f), () => ShowSettings());
        MakeButton(_menuPanel, "BtnCredits", "Credits", new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0f, -160f), new Vector2(520f, 90f), () => ShowCredits());
        MakeText(_menuPanel, "Creator", "پرهام اذانگو | Parham Azangoo", 28, TextAlignmentOptions.Center, new Color(0.8f, 0.85f, 0.95f), new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0f, 60f), new Vector2(900f, 50f));
    }

    private void BuildSettings()
    {
        _settingsPanel = new GameObject("SettingsPanel");
        _settingsPanel.transform.SetParent(_canvas.transform, false);
        MakeImage(_settingsPanel, "Bg", new Color(0.06f, 0.08f, 0.11f, 0.97f), new Vector2(0f, 0f), new Vector2(1f, 1f), Vector2.zero, Vector2.zero);
        MakeLocalizedText(_settingsPanel, "Title", "Settings", 54, TextAlignmentOptions.Center, Color.white, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -90f), new Vector2(700f, 80f));
        MakeLocalizedText(_settingsPanel, "LangLabel", "Language", 32, TextAlignmentOptions.Center, Color.white, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(-260f, 150f), new Vector2(300f, 60f));
        Button langBtn = MakeButton(_settingsPanel, "LangBtn", "Language", new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(180f, 150f), new Vector2(420f, 80f), CycleLanguage);
        _languageButtonText = langBtn.GetComponentInChildren<TextMeshProUGUI>();
        MakeLocalizedText(_settingsPanel, "SensLabel", "Sensitivity", 32, TextAlignmentOptions.Center, Color.white, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(-260f, 40f), new Vector2(300f, 60f));
        Button sensMinus = MakeButton(_settingsPanel, "SensMinus", "Back", new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(40f, 40f), new Vector2(110f, 80f), () => ChangeSensitivity(-0.2f));
        sensMinus.GetComponentInChildren<TextMeshProUGUI>().text = "-";
        _localized.RemoveAt(_localized.Count - 1);
        _sensitivityText = MakeText(_settingsPanel, "SensValue", "2.2", 32, TextAlignmentOptions.Center, Color.white, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(180f, 40f), new Vector2(140f, 80f));
        Button sensPlus = MakeButton(_settingsPanel, "SensPlus", "Start", new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(320f, 40f), new Vector2(110f, 80f), () => ChangeSensitivity(0.2f));
        sensPlus.GetComponentInChildren<TextMeshProUGUI>().text = "+";
        _localized.RemoveAt(_localized.Count - 1);
        MakeLocalizedText(_settingsPanel, "QualLabel", "Quality", 32, TextAlignmentOptions.Center, Color.white, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(-260f, -70f), new Vector2(300f, 60f));
        Button qLow = MakeButton(_settingsPanel, "QualLow", "Quality", new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(40f, -70f), new Vector2(150f, 80f), () => SetQuality(0));
        qLow.GetComponentInChildren<TextMeshProUGUI>().text = "Low";
        _localized.RemoveAt(_localized.Count - 1);
        Button qMed = MakeButton(_settingsPanel, "QualMed", "Quality", new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(210f, -70f), new Vector2(150f, 80f), () => SetQuality(1));
        qMed.GetComponentInChildren<TextMeshProUGUI>().text = "Medium";
        _localized.RemoveAt(_localized.Count - 1);
        Button qHigh = MakeButton(_settingsPanel, "QualHigh", "Quality", new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(380f, -70f), new Vector2(150f, 80f), () => SetQuality(2));
        qHigh.GetComponentInChildren<TextMeshProUGUI>().text = "High";
        _localized.RemoveAt(_localized.Count - 1);
        MakeButton(_settingsPanel, "BackBtn", "Back", new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0f, -200f), new Vector2(420f, 90f), () => ShowMenu());
    }

    private void BuildCredits()
    {
        _creditsPanel = new GameObject("CreditsPanel");
        _creditsPanel.transform.SetParent(_canvas.transform, false);
        MakeImage(_creditsPanel, "Bg", new Color(0.06f, 0.08f, 0.11f, 0.97f), new Vector2(0f, 0f), new Vector2(1f, 1f), Vector2.zero, Vector2.zero);
        MakeLocalizedText(_creditsPanel, "Title", "Credits", 54, TextAlignmentOptions.Center, Color.white, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -90f), new Vector2(700f, 80f));
        MakeText(_creditsPanel, "CreatorName", "پرهام اذانگو", 56, TextAlignmentOptions.Center, new Color(1f, 0.85f, 0.3f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0f, 80f), new Vector2(900f, 90f));
        MakeText(_creditsPanel, "CreatorEn", "Parham Azangoo", 40, TextAlignmentOptions.Center, Color.white, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0f, 10f), new Vector2(900f, 70f));
        MakeText(_creditsPanel, "Thanks", "COD & PUBG Battlegrounds | Unity", 28, TextAlignmentOptions.Center, new Color(0.8f, 0.85f, 0.95f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0f, -60f), new Vector2(900f, 50f));
        MakeButton(_creditsPanel, "BackBtn", "Back", new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0f, -180f), new Vector2(420f, 90f), () => ShowMenu());
    }

    private void BuildHud()
    {
        _hudPanel = new GameObject("HudPanel");
        _hudPanel.transform.SetParent(_canvas.transform, false);
        RectTransform lookRt = MakeRect(_hudPanel, "LookArea", new Vector2(0f, 0f), new Vector2(1f, 1f), Vector2.zero, Vector2.zero);
        Image lookImg = lookRt.gameObject.AddComponent<Image>();
        lookImg.color = new Color(0f, 0f, 0f, 0f);
        lookRt.gameObject.AddComponent<TouchLookArea>();

        _damageFlash = MakeImage(_hudPanel, "DamageFlash", new Color(1f, 0f, 0f, 0f), new Vector2(0f, 0f), new Vector2(1f, 1f), Vector2.zero, Vector2.zero);
        _damageFlash.raycastTarget = false;

        MakeImage(_hudPanel, "HealthBg", new Color(0f, 0f, 0f, 0.45f), new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(230f, -55f), new Vector2(420f, 34f));
        _healthFill = MakeImage(_hudPanel, "HealthFill", new Color(0.25f, 0.9f, 0.35f), new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(230f, -55f), new Vector2(410f, 24f));
        _armorFill = MakeImage(_hudPanel, "ArmorFill", new Color(0.3f, 0.6f, 1f), new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(230f, -88f), new Vector2(410f, 14f));
        _healthText = MakeText(_hudPanel, "HealthText", "100", 28, TextAlignmentOptions.Center, Color.white, new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(230f, -55f), new Vector2(420f, 40f));

        _scoreText = MakeText(_hudPanel, "ScoreText", "0", 34, TextAlignmentOptions.Center, Color.white, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -45f), new Vector2(500f, 50f));
        _aliveText = MakeText(_hudPanel, "AliveText", "Alive: 10", 30, TextAlignmentOptions.Center, Color.white, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -95f), new Vector2(500f, 45f));
        _killfeedText = MakeText(_hudPanel, "Killfeed", "", 24, TextAlignmentOptions.TopRight, new Color(1f, 0.9f, 0.6f), new Vector2(1f, 1f), new Vector2(1f, 1f), new Vector2(-260f, -70f), new Vector2(500f, 140f));

        MakeText(_hudPanel, "Crosshair", "+", 42, TextAlignmentOptions.Center, new Color(1f, 1f, 1f, 0.9f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(60f, 60f));
        _hitmarker = MakeText(_hudPanel, "Hitmarker", "✕", 54, TextAlignmentOptions.Center, new Color(1f, 0.25f, 0.2f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(90f, 90f));
        _hitmarker.gameObject.SetActive(false);
        _centerMessage = MakeText(_hudPanel, "CenterMessage", "", 36, TextAlignmentOptions.Center, new Color(1f, 0.9f, 0.4f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0f, 150f), new Vector2(1000f, 60f));
        _centerMessage.gameObject.SetActive(false);

        GameObject joyBase = new GameObject("JoystickBase");
        joyBase.transform.SetParent(_hudPanel.transform, false);
        RectTransform joyRt = joyBase.AddComponent<RectTransform>();
        joyRt.anchorMin = new Vector2(0f, 0f);
        joyRt.anchorMax = new Vector2(0f, 0f);
        joyRt.anchoredPosition = new Vector2(170f, 170f);
        joyRt.sizeDelta = new Vector2(240f, 240f);
        Image joyImg = joyBase.AddComponent<Image>();
        joyImg.color = new Color(1f, 1f, 1f, 0.18f);
        Joystick joystick = joyBase.AddComponent<Joystick>();
        joystick.OnValueChanged = (v) => { JoystickVector = v; };
        GameObject handle = new GameObject("JoystickHandle");
        handle.transform.SetParent(joyBase.transform, false);
        RectTransform handleRt = handle.AddComponent<RectTransform>();
        handleRt.anchorMin = new Vector2(0.5f, 0.5f);
        handleRt.anchorMax = new Vector2(0.5f, 0.5f);
        handleRt.anchoredPosition = Vector2.zero;
        handleRt.sizeDelta = new Vector2(100f, 100f);
        Image handleImg = handle.AddComponent<Image>();
        handleImg.color = new Color(1f, 1f, 1f, 0.45f);
        joystick.HandleRect = handleRt;

        MakeHoldButton(_hudPanel, "BtnFire", "Fire", new Vector2(1f, 0f), new Vector2(1f, 0f), new Vector2(-150f, 170f), new Vector2(170f, 170f), () => _gm.Player.SetFireHeld(true), () => _gm.Player.SetFireHeld(false));
        MakeHoldButton(_hudPanel, "BtnAds", "ADS", new Vector2(1f, 0f), new Vector2(1f, 0f), new Vector2(-330f, 120f), new Vector2(130f, 130f), () => _gm.Player.SetAdsHeld(true), () => _gm.Player.SetAdsHeld(false));
        MakeButton(_hudPanel, "BtnReload", "Reload", new Vector2(1f, 0f), new Vector2(1f, 0f), new Vector2(-150f, 330f), new Vector2(130f, 100f), () => _gm.Player.PressReload());
        MakeButton(_hudPanel, "BtnJump", "Jump", new Vector2(1f, 0f), new Vector2(1f, 0f), new Vector2(-330f, 280f), new Vector2(130f, 100f), () => _gm.Player.PressJump());
        MakeButton(_hudPanel, "BtnGrenade", "Grenade", new Vector2(1f, 0f), new Vector2(1f, 0f), new Vector2(-480f, 120f), new Vector2(130f, 100f), () => _gm.Player.PressGrenade());
        MakeButton(_hudPanel, "BtnMedkit", "Medkit", new Vector2(1f, 0f), new Vector2(1f, 0f), new Vector2(-480f, 240f), new Vector2(130f, 100f), () => _gm.Player.PressMedkit());
        MakeButton(_hudPanel, "BtnWeapon", "Weapon", new Vector2(1f, 0f), new Vector2(1f, 0f), new Vector2(-150f, 460f), new Vector2(150f, 90f), () => _gm.Player.PressWeaponNext());
        MakeButton(_hudPanel, "BtnPause", "Pause", new Vector2(1f, 1f), new Vector2(1f, 1f), new Vector2(-90f, -50f), new Vector2(140f, 80f), () => _gm.PauseGame(true));

        _ammoText = MakeText(_hudPanel, "AmmoText", "30 / 120", 40, TextAlignmentOptions.Center, Color.white, new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0f, 70f), new Vector2(420f, 60f));
        _inventoryText = MakeText(_hudPanel, "InventoryText", "", 24, TextAlignmentOptions.Center, Color.white, new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0f, 120f), new Vector2(600f, 40f));
        string[] names = new string[] { "M416", "AK-47", "AWM", "870", "Glock" };
        for (int i = 0; i < 5; i++)
        {
            int idx = i;
            Button b = MakeButton(_hudPanel, "WeaponBtn" + i, "Weapon", new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(-240f + i * 120f, 170f), new Vector2(110f, 60f), () => _gm.Player.Weapons.SelectWeapon(idx));
            _localized.RemoveAt(_localized.Count - 1);
            b.GetComponentInChildren<TextMeshProUGUI>().text = names[i];
            _weaponButtons[i] = b;
        }
    }

    private void BuildPause()
    {
        _pausePanel = new GameObject("PausePanel");
        _pausePanel.transform.SetParent(_canvas.transform, false);
        MakeImage(_pausePanel, "Bg", new Color(0f, 0f, 0f, 0.55f), new Vector2(0f, 0f), new Vector2(1f, 1f), Vector2.zero, Vector2.zero);
        MakeLocalizedText(_pausePanel, "Title", "Pause", 54, TextAlignmentOptions.Center, Color.white, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0f, 220f), new Vector2(600f, 80f));
        MakeButton(_pausePanel, "BtnResume", "Resume", new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0f, 110f), new Vector2(420f, 90f), () => _gm.PauseGame(false));
        MakeButton(_pausePanel, "BtnRestart", "Restart", new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0f, 0f), new Vector2(420f, 90f), () => _gm.RestartGame());
        MakeButton(_pausePanel, "BtnMenu", "Back", new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0f, -110f), new Vector2(420f, 90f), () => _gm.BackToMenu());
    }

    private void BuildEnd()
    {
        _endPanel = new GameObject("EndPanel");
        _endPanel.transform.SetParent(_canvas.transform, false);
        MakeImage(_endPanel, "Bg", new Color(0.04f, 0.05f, 0.08f, 0.92f), new Vector2(0f, 0f), new Vector2(1f, 1f), Vector2.zero, Vector2.zero);
        _endTitle = MakeLocalizedText(_endPanel, "Title", "Winner", 64, TextAlignmentOptions.Center, new Color(1f, 0.85f, 0.3f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0f, 180f), new Vector2(1000f, 100f));
        _endStats = MakeText(_endPanel, "Stats", "", 32, TextAlignmentOptions.Center, Color.white, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0f, 60f), new Vector2(900f, 120f));
        MakeButton(_endPanel, "BtnAgain", "Restart", new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0f, -80f), new Vector2(420f, 90f), () => _gm.RestartGame());
        MakeButton(_endPanel, "BtnMenu", "Back", new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0f, -190f), new Vector2(420f, 90f), () => _gm.BackToMenu());
    }

    private void CycleLanguage()
    {
        int idx = Localization50.IndexOf(_gm.LanguageCode);
        idx = (idx + 1) % Localization50.Codes.Length;
        _gm.LanguageCode = Localization50.Codes[idx];
        RefreshAllTexts();
    }

    private void ChangeSensitivity(float delta)
    {
        if (_gm.Player != null)
        {
            _gm.Player.Sensitivity = Mathf.Clamp(_gm.Player.Sensitivity + delta, 0.5f, 5f);
            _sensitivityText.text = _gm.Player.Sensitivity.ToString("0.0");
        }
    }

    private void SetQuality(int level)
    {
        QualitySettings.SetQualityLevel(Mathf.Clamp(level, 0, 2), true);
        if (level == 0) { QualitySettings.pixelLightCount = 0; QualitySettings.shadows = ShadowQuality.Disable; Application.targetFrameRate = 30; }
        else if (level == 1) { QualitySettings.pixelLightCount = 1; QualitySettings.shadows = ShadowQuality.HardOnly; Application.targetFrameRate = 60; }
        else { QualitySettings.pixelLightCount = 2; QualitySettings.shadows = ShadowQuality.All; Application.targetFrameRate = 60; }
    }

    public void RefreshAllTexts()
    {
        foreach (var item in _localized) item.tmp.text = Localization50.Get(_gm.LanguageCode, item.key);
        if (_languageButtonText != null)
        {
            int idx = Localization50.IndexOf(_gm.LanguageCode);
            _languageButtonText.text = Localization50.Names[idx];
        }
        if (_sensitivityText != null && _gm.Player != null) _sensitivityText.text = _gm.Player.Sensitivity.ToString("0.0");
        RefreshWeaponBar();
        RefreshInventory();
    }

    public void ShowMenu()
    {
        _menuPanel.SetActive(true);
        _settingsPanel.SetActive(false);
        _creditsPanel.SetActive(false);
        _hudPanel.SetActive(false);
        _pausePanel.SetActive(false);
        _endPanel.SetActive(false);
    }

    public void ShowSettings()
    {
        _menuPanel.SetActive(false);
        _settingsPanel.SetActive(true);
        _creditsPanel.SetActive(false);
        _hudPanel.SetActive(false);
        _pausePanel.SetActive(false);
        _endPanel.SetActive(false);
    }

    public void ShowCredits()
    {
        _menuPanel.SetActive(false);
        _settingsPanel.SetActive(false);
        _creditsPanel.SetActive(true);
        _hudPanel.SetActive(false);
        _pausePanel.SetActive(false);
        _endPanel.SetActive(false);
    }

    public void ShowHud()
    {
        _menuPanel.SetActive(false);
        _settingsPanel.SetActive(false);
        _creditsPanel.SetActive(false);
        _hudPanel.SetActive(true);
        _pausePanel.SetActive(false);
        _endPanel.SetActive(false);
    }

    public void ShowPause(bool show)
    {
        _pausePanel.SetActive(show);
    }

    public void ShowEnd(bool won, int kills, int score, int alive)
    {
        _hudPanel.SetActive(false);
        _pausePanel.SetActive(false);
        _endPanel.SetActive(true);
        _endTitle.text = Localization50.Get(_gm.LanguageCode, won ? "Winner" : "Defeat");
        _endStats.text = Localization50.Get(_gm.LanguageCode, "Kill") + ": " + kills + "  |  " + Localization50.Get(_gm.LanguageCode, "Score") + ": " + score + "  |  " + Localization50.Get(_gm.LanguageCode, "Alive") + ": " + alive;
    }

    public void SetHealth(float health, float maxHealth, float armor)
    {
        float f = Mathf.Clamp01(health / Mathf.Max(1f, maxHealth));
        if (_healthFill != null)
        {
            _healthFill.rectTransform.sizeDelta = new Vector2(410f * f, 24f);
            _healthFill.color = f > 0.5f ? new Color(0.25f, 0.9f, 0.35f) : (f > 0.25f ? new Color(1f, 0.75f, 0.2f) : new Color(1f, 0.25f, 0.2f));
        }
        if (_armorFill != null) _armorFill.rectTransform.sizeDelta = new Vector2(410f * Mathf.Clamp01(armor / 100f), 14f);
        if (_healthText != null) _healthText.text = Mathf.CeilToInt(health).ToString();
    }

    public void SetAmmo(WeaponSlot slot)
    {
        if (_ammoText == null || slot == null) return;
        _ammoText.text = slot.reloading ? "..." : (slot.ammoInMag + " / " + slot.reserve);
    }

    public void RefreshWeaponBar()
    {
        if (_gm.Player == null || _gm.Player.Weapons == null) return;
        for (int i = 0; i < _weaponButtons.Length; i++)
        {
            Button b = _weaponButtons[i];
            if (b == null) continue;
            Image img = b.GetComponent<Image>();
            img.color = i == _gm.Player.Weapons.CurrentIndex ? new Color(0.9f, 0.65f, 0.2f, 0.95f) : new Color(0.12f, 0.16f, 0.22f, 0.85f);
        }
        SetAmmo(_gm.Player.Weapons.CurrentSlot);
    }

    public void RefreshInventory()
    {
        if (_inventoryText == null || _gm.Player == null) return;
        _inventoryText.text = Localization50.Get(_gm.LanguageCode, "Grenade") + ": " + _gm.Player.GrenadeCount + "   |   " + Localization50.Get(_gm.LanguageCode, "Medkit") + ": " + _gm.Player.MedkitCount;
    }

    public void SetScore(int score, int target)
    {
        if (_scoreText != null) _scoreText.text = Localization50.Get(_gm.LanguageCode, "Score") + ": " + score + " / " + target;
    }

    public void SetAlive(int alive)
    {
        if (_aliveText != null) _aliveText.text = Localization50.Get(_gm.LanguageCode, "Alive") + ": " + alive;
    }

    public void AddKillfeed(string message)
    {
        _killfeed.Insert(0, message);
        while (_killfeed.Count > 4) _killfeed.RemoveAt(_killfeed.Count - 1);
        if (_killfeedText != null) _killfeedText.text = string.Join("\n", _killfeed.ToArray());
    }

    public void ShowHitmarker(bool kill)
    {
        if (_hitmarker == null) return;
        _hitmarker.gameObject.SetActive(true);
        _hitmarker.color = kill ? new Color(1f, 0.3f, 0.2f) : Color.white;
        _hitmarkerUntil = Time.time + 0.16f;
    }

    public void ShowDamageFlash()
    {
        _damageFlashUntil = Time.time + 0.25f;
    }

    public void ShowCenterMessage(string message)
    {
        if (_centerMessage == null) return;
        _centerMessage.text = message;
        _centerMessage.gameObject.SetActive(true);
        _centerMsgUntil = Time.time + 1.6f;
    }

    private void Update()
    {
        if (_hitmarker != null && _hitmarker.gameObject.activeSelf && Time.time > _hitmarkerUntil) _hitmarker.gameObject.SetActive(false);
        if (_centerMessage != null && _centerMessage.gameObject.activeSelf && Time.time > _centerMsgUntil) _centerMessage.gameObject.SetActive(false);
        if (_damageFlash != null)
        {
            float a = Time.time < _damageFlashUntil ? 0.32f : 0f;
            Color c = _damageFlash.color;
            c.a = Mathf.Lerp(c.a, a, Time.deltaTime * 10f);
            _damageFlash.color = c;
        }
    }
}

public class TouchLookArea : MonoBehaviour, IDragHandler, IPointerDownHandler
{
    public void OnPointerDown(PointerEventData eventData) { }
    public void OnDrag(PointerEventData eventData)
    {
        if (GameManager.Instance != null && GameManager.Instance.Player != null)
            GameManager.Instance.Player.AddLookDelta(eventData.delta);
    }
}

public class Joystick : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler
{
    public RectTransform HandleRect;
    public System.Action<Vector2> OnValueChanged;
    private Vector2 _value;
    private float _radius = 110f;

    private void Start()
    {
        RectTransform rt = GetComponent<RectTransform>();
        _radius = rt.rect.width * 0.42f;
    }

    public void OnPointerDown(PointerEventData eventData) { HandleDrag(eventData); }
    public void OnDrag(PointerEventData eventData) { HandleDrag(eventData); }
    public void OnPointerUp(PointerEventData eventData)
    {
        _value = Vector2.zero;
        if (HandleRect != null) HandleRect.anchoredPosition = Vector2.zero;
        if (OnValueChanged != null) OnValueChanged(_value);
    }

    private void HandleDrag(PointerEventData eventData)
    {
        RectTransform rt = GetComponent<RectTransform>();
        Vector3 world = new Vector3(eventData.position.x, eventData.position.y, 0f);
        Vector2 local = rt.InverseTransformPoint(world);
        if (local.magnitude > _radius) local = local.normalized * _radius;
        if (HandleRect != null) HandleRect.anchoredPosition = local;
        _value = local / Mathf.Max(1f, _radius);
        if (OnValueChanged != null) OnValueChanged(_value);
    }
}
