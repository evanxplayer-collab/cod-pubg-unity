using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameMode
{
    BattleRoyale,
    Multiplayer
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public string LanguageCode = "fa";
    public GameMode Mode = GameMode.BattleRoyale;
    public bool IsPlaying;
    public FPSPlayer Player;
    public HUD HUD;
    public MapBuilder Map;
    public BlueZone Zone;
    public Camera MenuCamera;

    private readonly List<EnemySoldier> _enemies = new List<EnemySoldier>();
    private int _score;
    private int _kills;
    private int _aliveCount;
    private int _targetScore = 20;
    private Coroutine _airdropRoutine;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        LanguageCode = PlayerPrefs.GetString("Language", "fa");

        // World first (map, zone). Each step is guarded so a single failure
        // can never prevent the menu from appearing.
        try
        {
            GameObject audioGo = new GameObject("AudioManager");
            audioGo.AddComponent<AudioManager>();
        }
        catch (System.Exception e) { Debug.LogError("[GM] AudioManager: " + e.Message); }

        try
        {
            GameObject mapGo = new GameObject("MapBuilder");
            Map = mapGo.AddComponent<MapBuilder>();
            Map.Build();
        }
        catch (System.Exception e) { Debug.LogError("[GM] MapBuilder: " + e.Message); }

        try
        {
            GameObject zoneGo = new GameObject("BlueZone");
            Zone = zoneGo.AddComponent<BlueZone>();
            Zone.Initialize();
        }
        catch (System.Exception e) { Debug.LogError("[GM] BlueZone: " + e.Message); }

        // Menu camera: renders the battlefield behind the menu so the player
        // always sees the world (and the menu has a live background).
        try
        {
            GameObject camGo = new GameObject("MenuCamera");
            camGo.transform.position = new Vector3(0f, 9f, 34f);
            camGo.transform.rotation = Quaternion.Euler(18f, 180f, 0f);
            MenuCamera = camGo.AddComponent<Camera>();
            MenuCamera.fieldOfView = 60f;
            MenuCamera.clearFlags = CameraClearFlags.Skybox;
            MenuCamera.depth = -10f;
        }
        catch (System.Exception e) { Debug.LogError("[GM] MenuCamera: " + e.Message); }

        // Player (created but hidden until a game starts).
        try
        {
            GameObject playerGo = new GameObject("Player");
            Player = playerGo.AddComponent<FPSPlayer>();
            Player.Initialize(Map != null ? Map.PlayerSpawn : new Vector3(0f, 0f, 28f));
            playerGo.SetActive(false);
        }
        catch (System.Exception e) { Debug.LogError("[GM] Player: " + e.Message); }

        // HUD / menu last and guaranteed.
        try
        {
            GameObject hudGo = new GameObject("HUD");
            HUD = hudGo.AddComponent<HUD>();
            HUD.Initialize(this);
        }
        catch (System.Exception e) { Debug.LogError("[GM] HUD: " + e.Message); }

        try { SpawnInitialLoot(); } catch (System.Exception e) { Debug.LogError("[GM] Loot: " + e.Message); }

        if (HUD != null) HUD.ShowMenu();
        IsPlaying = false;
    }

    public void StartGame(GameMode mode)
    {
        Mode = mode;
        IsPlaying = true;
        Time.timeScale = 1f;
        ClearEnemies();
        _score = 0;
        _kills = 0;
        if (mode == GameMode.Multiplayer)
        {
            _targetScore = 20;
            _aliveCount = 9;
            Zone.Enabled = false;
            Zone.gameObject.SetActive(false);
            SpawnEnemies(8, true);
        }
        else
        {
            _targetScore = 9;
            _aliveCount = 10;
            Zone.CurrentRadius = 85f;
            Zone.Activate();
            SpawnEnemies(9, false);
            if (_airdropRoutine != null) StopCoroutine(_airdropRoutine);
            _airdropRoutine = StartCoroutine(AirdropRoutine());
        }
        if (MenuCamera != null) MenuCamera.enabled = false;
        Player.gameObject.SetActive(true);
        Player.Respawn();
        HUD.ShowHud();
        HUD.SetHealth(Player.Health, Player.MaxHealth, Player.Armor);
        HUD.RefreshWeaponBar();
        HUD.RefreshInventory();
        HUD.SetScore(_score, _targetScore);
        HUD.SetAlive(_aliveCount);
        HUD.ShowCenterMessage(Localization50.Get(LanguageCode, mode == GameMode.BattleRoyale ? "BattleRoyale" : "Multiplayer"));
    }

    public void PauseGame(bool paused)
    {
        if (!IsPlaying) return;
        Time.timeScale = paused ? 0f : 1f;
        HUD.ShowPause(paused);
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        StartGame(Mode);
    }

    public void BackToMenu()
    {
        IsPlaying = false;
        Time.timeScale = 1f;
        ClearEnemies();
        if (Player != null) Player.gameObject.SetActive(false);
        if (Zone != null) { Zone.Enabled = false; Zone.gameObject.SetActive(false); }
        if (MenuCamera != null) MenuCamera.enabled = true;
        HUD.ShowMenu();
    }

    public void OnEnemyKilled(EnemySoldier enemy)
    {
        _kills++;
        _score++;
        if (Mode == GameMode.BattleRoyale)
        {
            _aliveCount = Mathf.Max(1, _aliveCount - 1);
        }
        HUD.SetScore(_score, _targetScore);
        HUD.SetAlive(_aliveCount);
        HUD.AddKillfeed(Localization50.Get(LanguageCode, "Kill") + " #" + _kills);
        if (Mode == GameMode.Multiplayer && _score >= _targetScore) WinGame();
        else if (Mode == GameMode.BattleRoyale && _aliveCount <= 1 && !Player.IsDead) WinGame();
    }

    public void OnPlayerDeath()
    {
        if (Mode == GameMode.Multiplayer)
        {
            HUD.ShowCenterMessage(Localization50.Get(LanguageCode, "Death"));
            StartCoroutine(RespawnPlayerRoutine());
        }
        else
        {
            LoseGame();
        }
    }

    private IEnumerator RespawnPlayerRoutine()
    {
        yield return new WaitForSeconds(2f);
        if (IsPlaying && Player != null) Player.Respawn();
    }

    private void WinGame()
    {
        if (!IsPlaying) return;
        IsPlaying = false;
        if (AudioManager.Instance != null) AudioManager.Instance.Play("victory", 1f);
        HUD.ShowEnd(true, _kills, _score, _aliveCount);
    }

    private void LoseGame()
    {
        if (!IsPlaying) return;
        IsPlaying = false;
        if (AudioManager.Instance != null) AudioManager.Instance.Play("defeat", 1f);
        HUD.ShowEnd(false, _kills, _score, _aliveCount);
    }

    private void SpawnEnemies(int count, bool respawn)
    {
        for (int i = 0; i < count; i++)
        {
            GameObject go = new GameObject("Enemy_" + i);
            EnemySoldier enemy = go.AddComponent<EnemySoldier>();
            Vector3 spawn = Map.GetRandomEnemySpawn();
            enemy.Initialize(spawn, respawn);
            _enemies.Add(enemy);
        }
    }

    private void ClearEnemies()
    {
        foreach (EnemySoldier e in _enemies) if (e != null) Destroy(e.gameObject);
        _enemies.Clear();
    }

    private IEnumerator AirdropRoutine()
    {
        yield return new WaitForSeconds(15f);
        if (!IsPlaying || Mode != GameMode.BattleRoyale) yield break;
        Vector3 pos = Map.GetAirdropPosition();
        GameObject go = MeshBundleLoader.InstantiateBundle("pubg_airdrop", false, true);
        go.name = "Airdrop";
        Airdrop drop = go.AddComponent<Airdrop>();
        drop.StartDrop(pos);
        HUD.ShowCenterMessage(Localization50.Get(LanguageCode, "Airdrop"));
    }

    private void SpawnInitialLoot()
    {
        SpawnLoot(LootType.Medkit, new Vector3(-6f, 0f, 10f), 1);
        SpawnLoot(LootType.Medkit, new Vector3(8f, 0f, -6f), 1);
        SpawnLoot(LootType.Ammo, new Vector3(0f, 0f, 14f), 60);
        SpawnLoot(LootType.Armor, new Vector3(-12f, 0f, -10f), 1);
        SpawnLoot(LootType.Ammo, new Vector3(14f, 0f, 8f), 60);
    }

    public void SpawnLoot(LootType type, Vector3 position, int amount)
    {
        GameObject go;
        if (type == LootType.Medkit)
        {
            go = MeshBundleLoader.InstantiateBundle("medkit", false, false);
            go.transform.localScale = Vector3.one * 0.7f;
        }
        else if (type == LootType.AwmCrate)
        {
            go = MeshBundleLoader.InstantiateBundle("gun_awm", false, false);
            go.transform.localScale = Vector3.one * 0.55f;
        }
        else
        {
            go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            go.name = type == LootType.Armor ? "ArmorPickup" : "AmmoPickup";
            go.transform.localScale = new Vector3(0.7f, 0.45f, 0.7f);
            MeshRenderer mr = go.GetComponent<MeshRenderer>();
            Material mat = new Material(MeshBundleLoader.GetBaseMaterial());
            mat.color = type == LootType.Armor ? new Color(0.25f, 0.5f, 1f) : new Color(1f, 0.75f, 0.2f);
            mr.sharedMaterial = mat;
            Collider col = go.GetComponent<Collider>();
            if (col != null) Destroy(col);
        }
        go.name = "Loot_" + type;
        go.transform.position = new Vector3(position.x, 0.35f, position.z);
        LootPickup pickup = go.AddComponent<LootPickup>();
        pickup.Initialize(type, amount);
    }
}

public static class Bootstrap
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Init()
    {
        if (GameManager.Instance != null) return;
        GameObject go = new GameObject("GameManager");
        go.AddComponent<GameManager>();
    }
}
