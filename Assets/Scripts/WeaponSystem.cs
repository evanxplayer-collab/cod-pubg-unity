using System.Collections.Generic;
using UnityEngine;

public class WeaponData
{
    public string id;
    public string displayName;
    public string resourceName;
    public float damage;
    public float headshotMultiplier = 2f;
    public float fireRate;
    public int magazineSize;
    public int startReserve;
    public float reloadTime;
    public float range = 220f;
    public float spreadHip = 2.2f;
    public float spreadAds = 0.25f;
    public float adsFov = 55f;
    public float recoil = 0.35f;
    public bool automatic = true;
    public int pellets = 1;
    public float desiredLength = 0.62f;
    public Vector3 hipPos = new Vector3(0.22f, -0.18f, -0.5f);
    public Vector3 adsPos = new Vector3(0f, -0.145f, -0.42f);
}

public class WeaponSlot
{
    public WeaponData data;
    public GameObject wrapper;
    public GameObject root;
    public int ammoInMag;
    public int reserve;
    public bool reloading;
    public float reloadEndTime;
    public float nextFireTime;
}

public class WeaponSystem : MonoBehaviour
{
    public List<WeaponSlot> Slots = new List<WeaponSlot>();
    public int CurrentIndex;
    public float AdsAmount;
    public bool IsAds;
    public bool IsReloading => CurrentSlot != null && CurrentSlot.reloading;
    public WeaponSlot CurrentSlot => Slots.Count > 0 ? Slots[Mathf.Clamp(CurrentIndex, 0, Slots.Count - 1)] : null;

    private Camera _cam;
    private Transform _mount;
    private FPSPlayer _player;
    private float _triggerHeld;
    private float _recoilRecover;

    public void Initialize(Camera cam, Transform mount, FPSPlayer player)
    {
        _cam = cam;
        _mount = mount;
        _player = player;
        CreateWeapons();
        SelectWeapon(0, true);
    }

    private void CreateWeapons()
    {
        WeaponData[] defs = new WeaponData[]
        {
            new WeaponData{id="m416", displayName="M416", resourceName="gun_m416", damage=26f, fireRate=8.5f, magazineSize=30, startReserve=120, reloadTime=2.1f, spreadHip=2.0f, spreadAds=0.22f, adsFov=55f, recoil=0.32f, automatic=true, pellets=1, desiredLength=0.62f},
            new WeaponData{id="ak47", displayName="AK-47", resourceName="gun_ak47", damage=34f, fireRate=6.2f, magazineSize=30, startReserve=90, reloadTime=2.4f, spreadHip=2.6f, spreadAds=0.35f, adsFov=55f, recoil=0.5f, automatic=true, pellets=1, desiredLength=0.64f},
            new WeaponData{id="awm", displayName="AWM", resourceName="gun_awm", damage=105f, fireRate=0.9f, magazineSize=5, startReserve=25, reloadTime=3.2f, spreadHip=4.5f, spreadAds=0.03f, adsFov=22f, recoil=1.4f, automatic=false, pellets=1, desiredLength=0.88f, hipPos=new Vector3(0.24f,-0.2f,-0.55f), adsPos=new Vector3(0f,-0.13f,-0.45f)},
            new WeaponData{id="shotgun", displayName="Remington 870", resourceName="gun_shotgun", damage=13f, fireRate=1.4f, magazineSize=6, startReserve=30, reloadTime=2.8f, spreadHip=5.5f, spreadAds=3.2f, adsFov=60f, recoil=1.1f, automatic=false, pellets=8, desiredLength=0.6f},
            new WeaponData{id="glock", displayName="Glock-17", resourceName="gun_glock", damage=18f, fireRate=5f, magazineSize=17, startReserve=68, reloadTime=1.6f, spreadHip=1.8f, spreadAds=0.3f, adsFov=58f, recoil=0.22f, automatic=false, pellets=1, desiredLength=0.3f, hipPos=new Vector3(0.18f,-0.15f,-0.4f), adsPos=new Vector3(0f,-0.12f,-0.35f)}
        };

        foreach (WeaponData d in defs)
        {
            WeaponSlot slot = new WeaponSlot();
            slot.data = d;
            slot.ammoInMag = d.magazineSize;
            slot.reserve = d.startReserve;
            slot.wrapper = new GameObject("Weapon_" + d.id);
            slot.wrapper.transform.SetParent(_mount, false);
            slot.wrapper.transform.localPosition = d.hipPos;
            slot.wrapper.transform.localRotation = Quaternion.identity;
            GameObject root = MeshBundleLoader.InstantiateBundle(d.resourceName, false, false);
            root.name = "Model_" + d.id;
            Bounds b = ComputeBounds(root);
            float maxSize = Mathf.Max(b.size.x, Mathf.Max(b.size.y, b.size.z));
            float scale = maxSize > 0.001f ? d.desiredLength / maxSize : 1f;
            root.transform.SetParent(slot.wrapper.transform, false);
            root.transform.localScale = Vector3.one * scale;
            root.transform.localPosition = -b.center * scale;
            root.transform.localRotation = Quaternion.identity;
            slot.root = root;
            Slots.Add(slot);
        }
    }

    private Bounds ComputeBounds(GameObject root)
    {
        MeshFilter[] filters = root.GetComponentsInChildren<MeshFilter>();
        bool has = false;
        Bounds bounds = new Bounds(Vector3.zero, Vector3.one);
        foreach (MeshFilter mf in filters)
        {
            if (mf.sharedMesh == null) continue;
            Bounds mb = mf.sharedMesh.bounds;
            Vector3 c = mf.transform.TransformPoint(mb.center);
            Vector3 e = mb.extents;
            Vector3[] corners = new Vector3[]
            {
                c + new Vector3(-e.x,-e.y,-e.z), c + new Vector3(e.x,-e.y,-e.z), c + new Vector3(-e.x,e.y,-e.z), c + new Vector3(e.x,e.y,-e.z),
                c + new Vector3(-e.x,-e.y,e.z), c + new Vector3(e.x,-e.y,e.z), c + new Vector3(-e.x,e.y,e.z), c + new Vector3(e.x,e.y,e.z)
            };
            foreach (Vector3 p in corners)
            {
                if (!has) { bounds = new Bounds(p, Vector3.zero); has = true; }
                else bounds.Encapsulate(p);
            }
        }
        if (!has) bounds = new Bounds(Vector3.zero, Vector3.one);
        return bounds;
    }

    public void SelectWeapon(int index, bool instant = false)
    {
        if (index < 0 || index >= Slots.Count) return;
        CurrentIndex = index;
        for (int i = 0; i < Slots.Count; i++)
            Slots[i].wrapper.SetActive(i == index);
        if (GameManager.Instance != null && GameManager.Instance.HUD != null)
            GameManager.Instance.HUD.RefreshWeaponBar();
        RefreshHudAmmo();
    }

    public void NextWeapon()
    {
        SelectWeapon((CurrentIndex + 1) % Slots.Count);
    }

    public void SetAds(bool ads)
    {
        IsAds = ads;
    }

    public void SetTrigger(bool held)
    {
        _triggerHeld = held ? 1f : 0f;
    }

    public void StartReload()
    {
        WeaponSlot s = CurrentSlot;
        if (s == null || s.reloading) return;
        if (s.ammoInMag >= s.data.magazineSize || s.reserve <= 0) return;
        s.reloading = true;
        s.reloadEndTime = Time.time + s.data.reloadTime;
        if (AudioManager.Instance != null) AudioManager.Instance.Play("reload", 0.9f);
        RefreshHudAmmo();
    }

    public void AddReserveAmmo(int amount)
    {
        foreach (WeaponSlot s in Slots) s.reserve += amount;
        RefreshHudAmmo();
    }

    private void RefreshHudAmmo()
    {
        if (GameManager.Instance != null && GameManager.Instance.HUD != null)
            GameManager.Instance.HUD.SetAmmo(CurrentSlot);
    }

    private void Update()
    {
        if (CurrentSlot == null) return;
        WeaponSlot s = CurrentSlot;
        if (s.reloading && Time.time >= s.reloadEndTime)
        {
            int need = s.data.magazineSize - s.ammoInMag;
            int take = Mathf.Min(need, s.reserve);
            s.ammoInMag += take;
            s.reserve -= take;
            s.reloading = false;
            RefreshHudAmmo();
        }

        float targetAds = IsAds ? 1f : 0f;
        AdsAmount = Mathf.MoveTowards(AdsAmount, targetAds, Time.deltaTime * 7f);
        for (int i = 0; i < Slots.Count; i++)
        {
            WeaponSlot slot = Slots[i];
            Vector3 pos = Vector3.Lerp(slot.data.hipPos, slot.data.adsPos, AdsAmount);
            slot.wrapper.transform.localPosition = pos;
            slot.wrapper.transform.localRotation = Quaternion.identity;
        }

        if (_cam != null)
        {
            float targetFov = Mathf.Lerp(75f, s.data.adsFov, AdsAmount);
            _cam.fieldOfView = Mathf.Lerp(_cam.fieldOfView, targetFov, Time.deltaTime * 12f);
        }

        if (_triggerHeld > 0.5f && !s.reloading && Time.time >= s.nextFireTime)
        {
            if (s.ammoInMag <= 0)
            {
                s.nextFireTime = Time.time + 0.25f;
                if (AudioManager.Instance != null) AudioManager.Instance.Play("empty", 0.7f);
                StartReload();
            }
            else
            {
                Fire(s);
                if (!s.data.automatic) _triggerHeld = 0f;
            }
        }
    }

    private void Fire(WeaponSlot s)
    {
        s.ammoInMag--;
        s.nextFireTime = Time.time + 1f / Mathf.Max(0.1f, s.data.fireRate);
        if (AudioManager.Instance != null) AudioManager.Instance.PlayGun(s.data.id);
        if (_player != null) _player.AddRecoil(s.data.recoil);
        RefreshHudAmmo();

        Vector3 camPos = _cam.transform.position;
        Vector3 camForward = _cam.transform.forward;
        Vector3 muzzle = camPos + camForward * 0.55f + _cam.transform.right * 0.1f + _cam.transform.up * -0.07f;
        Effects.MuzzleFlash(muzzle, camForward);

        float spreadDeg = Mathf.Lerp(s.data.spreadHip, s.data.spreadAds, AdsAmount);
        float spreadRad = spreadDeg * Mathf.Deg2Rad;
        for (int p = 0; p < s.data.pellets; p++)
        {
            Vector3 dir = camForward;
            dir += _cam.transform.right * Random.Range(-spreadRad, spreadRad);
            dir += _cam.transform.up * Random.Range(-spreadRad, spreadRad);
            dir.Normalize();
            RaycastHit hit;
            Vector3 end = camPos + dir * s.data.range;
            bool hasHit = Physics.Raycast(camPos, dir, out hit, s.data.range);
            if (hasHit && hit.collider.GetComponentInParent<FPSPlayer>() != null) hasHit = false;
            if (hasHit) end = hit.point;
            Effects.Tracer(muzzle, end, new Color(1f, 0.85f, 0.35f));
            if (!hasHit) continue;
            EnemySoldier enemy = hit.collider.GetComponentInParent<EnemySoldier>();
            if (enemy != null)
            {
                float dmg = s.data.damage;
                if (hit.collider.name.Contains("Head")) dmg *= s.data.headshotMultiplier;
                enemy.TakeDamage(dmg, _player != null ? _player.transform.position : camPos);
                Effects.HitSparks(hit.point, true);
                if (GameManager.Instance != null && GameManager.Instance.HUD != null)
                    GameManager.Instance.HUD.ShowHitmarker(enemy.IsDead);
                if (AudioManager.Instance != null) AudioManager.Instance.Play(enemy.IsDead ? "kill" : "hit", 0.8f);
            }
            else
            {
                Effects.HitSparks(hit.point, false);
            }
        }
    }
}
