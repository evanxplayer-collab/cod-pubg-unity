using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

public enum LootType
{
    Medkit,
    Armor,
    Ammo,
    AwmCrate
}

public class LootPickup : MonoBehaviour
{
    public LootType Type;
    public int Amount = 1;
    private float _baseY;
    private float _t;

    public void Initialize(LootType type, int amount)
    {
        Type = type;
        Amount = amount;
        _baseY = transform.position.y;
        SphereCollider col = gameObject.AddComponent<SphereCollider>();
        col.isTrigger = true;
        col.radius = 1.3f;
        col.center = new Vector3(0f, 0.6f, 0f);
    }

    private void Update()
    {
        _t += Time.deltaTime;
        transform.Rotate(0f, 45f * Time.deltaTime, 0f);
        transform.position = new Vector3(transform.position.x, _baseY + Mathf.Sin(_t * 2f) * 0.08f, transform.position.z);
        FPSPlayer player = GameManager.Instance != null ? GameManager.Instance.Player : null;
        if (player != null && !player.IsDead && player.gameObject.activeInHierarchy)
        {
            Vector3 d = player.transform.position - transform.position;
            d.y = 0f;
            if (d.magnitude < 1.6f) Collect(player);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        FPSPlayer player = other.GetComponentInParent<FPSPlayer>();
        if (player != null) Collect(player);
    }

    private void Collect(FPSPlayer player)
    {
        if (player == null || player.IsDead) return;
        bool consumed = false;
        if (Type == LootType.Medkit)
        {
            player.MedkitCount += Amount;
            consumed = true;
        }
        else if (Type == LootType.Armor)
        {
            player.SetArmor(100f);
            consumed = true;
        }
        else if (Type == LootType.Ammo)
        {
            if (player.Weapons != null) player.Weapons.AddReserveAmmo(Amount);
            consumed = true;
        }
        else if (Type == LootType.AwmCrate)
        {
            if (player.Weapons != null)
            {
                player.Weapons.AddReserveAmmo(60);
                player.Weapons.SelectWeapon(2);
            }
            player.SetArmor(100f);
            player.MedkitCount += 2;
            consumed = true;
        }
        if (consumed)
        {
            if (AudioManager.Instance != null) AudioManager.Instance.Play("pickup", 0.9f);
            Effects.HealSparkle(transform.position);
            if (GameManager.Instance != null && GameManager.Instance.HUD != null)
            {
                GameManager.Instance.HUD.RefreshInventory();
                GameManager.Instance.HUD.ShowCenterMessage(Localization50.Get(GameManager.Instance.LanguageCode, Type == LootType.AwmCrate ? "Airdrop" : "Medkit"));
            }
            Destroy(gameObject);
        }
    }
}

public class Airdrop : MonoBehaviour
{
    private Vector3 _target;
    private float _fallSpeed = 7f;
    private bool _landed;
    private float _smokeEndTime;

    public void StartDrop(Vector3 target)
    {
        _target = target;
        transform.position = new Vector3(target.x, 85f, target.z);
        transform.rotation = Quaternion.identity;
        if (AudioManager.Instance != null) AudioManager.Instance.Play("whistle", 0.7f);
    }

    private void Update()
    {
        if (!_landed)
        {
            transform.position = Vector3.MoveTowards(transform.position, new Vector3(_target.x, 0.6f, _target.z), _fallSpeed * Time.deltaTime);
            transform.Rotate(0f, 25f * Time.deltaTime, 0f);
            if (transform.position.y <= 0.65f)
            {
                _landed = true;
                transform.position = new Vector3(_target.x, 0.6f, _target.z);
                _smokeEndTime = Time.time + 9f;
                SpawnLoot();
            }
        }
        else if (Time.time < _smokeEndTime)
        {
            Effects.Smoke(transform.position + Vector3.up * 0.4f, new Color(1f, 0.08f, 0.08f), 1.2f);
        }
    }

    private void SpawnLoot()
    {
        GameManager gm = GameManager.Instance;
        if (gm == null) return;
        gm.SpawnLoot(LootType.AwmCrate, transform.position + new Vector3(1.2f, 0f, 0f), 1);
        gm.SpawnLoot(LootType.Armor, transform.position + new Vector3(-1.2f, 0f, 0.6f), 1);
        gm.SpawnLoot(LootType.Medkit, transform.position + new Vector3(0f, 0f, 1.4f), 2);
        gm.SpawnLoot(LootType.Ammo, transform.position + new Vector3(0.6f, 0f, -1.2f), 90);
    }
}

public class BlueZone : MonoBehaviour
{
    public bool Enabled;
    public float CurrentRadius = 85f;
    public float TargetRadius = 16f;
    public float DamagePerSecond = 5f;
    private float _shrinkStartTime;
    private float _shrinkDuration = 90f;
    private float _startRadius;
    private MeshRenderer _renderer;

    public void Initialize()
    {
        GameObject cylinder = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        cylinder.name = "BlueZoneWall";
        cylinder.transform.SetParent(transform, false);
        cylinder.transform.localPosition = new Vector3(0f, 18f, 0f);
        cylinder.transform.localScale = new Vector3(CurrentRadius * 2f, 36f, CurrentRadius * 2f);
        _renderer = cylinder.GetComponent<MeshRenderer>();
        Material mat = new Material(MeshBundleLoader.GetBaseMaterial());
        mat.color = new Color(0.15f, 0.45f, 1f, 0.28f);
        mat.SetFloat("_Mode", 3f);
        mat.SetInt("_SrcBlend", (int)BlendMode.SrcAlpha);
        mat.SetInt("_DstBlend", (int)BlendMode.OneMinusSrcAlpha);
        mat.SetInt("_ZWrite", 0);
        mat.DisableKeyword("_ALPHATEST_ON");
        mat.EnableKeyword("_ALPHABLEND_ON");
        mat.DisableKeyword("_ALPHATEST_ON");
        mat.renderQueue = (int)RenderQueue.Transparent;
        _renderer.sharedMaterial = mat;
        _renderer.shadowCastingMode = ShadowCastingMode.Off;
        _renderer.receiveShadows = false;
        Collider col = cylinder.GetComponent<Collider>();
        if (col != null) Destroy(col);
        _startRadius = CurrentRadius;
        _shrinkStartTime = Time.time + 20f;
        gameObject.SetActive(false);
    }

    public void Activate()
    {
        Enabled = true;
        gameObject.SetActive(true);
        _startRadius = CurrentRadius;
        _shrinkStartTime = Time.time + 20f;
    }

    public bool IsOutside(Vector3 pos)
    {
        Vector2 p = new Vector2(pos.x, pos.z);
        return p.magnitude > CurrentRadius;
    }

    private void Update()
    {
        if (!Enabled) return;
        if (Time.time > _shrinkStartTime)
        {
            float f = Mathf.Clamp01((Time.time - _shrinkStartTime) / _shrinkDuration);
            CurrentRadius = Mathf.Lerp(_startRadius, TargetRadius, f);
            Transform wall = transform.GetChild(0);
            if (wall != null) wall.localScale = new Vector3(CurrentRadius * 2f, 36f, CurrentRadius * 2f);
        }
        FPSPlayer player = GameManager.Instance != null ? GameManager.Instance.Player : null;
        if (player != null && !player.IsDead && IsOutside(player.transform.position))
        {
            player.TakeDamage(DamagePerSecond * Time.deltaTime);
            if (GameManager.Instance != null && GameManager.Instance.HUD != null)
                GameManager.Instance.HUD.ShowCenterMessage(Localization50.Get(GameManager.Instance.LanguageCode, "BlueZone"));
        }
    }
}
