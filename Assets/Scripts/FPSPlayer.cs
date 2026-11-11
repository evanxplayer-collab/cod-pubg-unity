using System.Collections;
using UnityEngine;

public class FPSPlayer : MonoBehaviour
{
    public float Health = 100f;
    public float MaxHealth = 100f;
    public float Armor;
    public bool IsDead;
    public float Sensitivity = 2.2f;
    public int GrenadeCount = 2;
    public int MedkitCount = 3;

    public CharacterController Controller;
    public Camera PlayerCamera;
    public WeaponSystem Weapons;

    private float _yaw;
    private float _pitch;
    private float _verticalVelocity;
    private Vector2 _lookAccum;
    private bool _fireHeld;
    private bool _adsHeld;
    private bool _jumpQueued;
    private bool _slideQueued;
    private float _slideEndTime;
    private float _healEndTime;
    private float _healRate;
    private bool _healing;
    private Vector3 _spawnPos;

    public void Initialize(Vector3 spawnPos)
    {
        _spawnPos = spawnPos;
        transform.position = spawnPos;
        transform.rotation = Quaternion.identity;
        Controller = gameObject.AddComponent<CharacterController>();
        Controller.height = 1.8f;
        Controller.radius = 0.35f;
        Controller.center = new Vector3(0f, 0.9f, 0f);
        Controller.skinWidth = 0.05f;
        Controller.stepOffset = 0.35f;

        GameObject camGo = new GameObject("PlayerCamera");
        camGo.transform.SetParent(transform, false);
        camGo.transform.localPosition = new Vector3(0f, 1.62f, 0f);
        camGo.transform.localRotation = Quaternion.identity;
        PlayerCamera = camGo.AddComponent<Camera>();
        PlayerCamera.fieldOfView = 75f;
        PlayerCamera.nearClipPlane = 0.08f;
        PlayerCamera.farClipPlane = 600f;
        PlayerCamera.clearFlags = CameraClearFlags.Skybox;
        PlayerCamera.backgroundColor = new Color(0.55f, 0.68f, 0.85f);

        GameObject mountGo = new GameObject("WeaponMount");
        mountGo.transform.SetParent(PlayerCamera.transform, false);
        mountGo.transform.localPosition = Vector3.zero;
        mountGo.transform.localRotation = Quaternion.identity;

        Weapons = gameObject.AddComponent<WeaponSystem>();
        Weapons.Initialize(PlayerCamera, mountGo.transform, this);
    }

    public void AddLookDelta(Vector2 delta)
    {
        _lookAccum += delta;
    }

    public void SetFireHeld(bool held)
    {
        _fireHeld = held;
        if (Weapons != null) Weapons.SetTrigger(held);
    }

    public void SetAdsHeld(bool held)
    {
        _adsHeld = held;
        if (Weapons != null) Weapons.SetAds(held);
    }

    public void PressJump() { _jumpQueued = true; }
    public void PressSlide() { _slideQueued = true; }
    public void PressReload() { if (Weapons != null) Weapons.StartReload(); }
    public void PressWeaponNext() { if (Weapons != null) Weapons.NextWeapon(); }

    public void PressGrenade()
    {
        if (IsDead || GrenadeCount <= 0) return;
        GrenadeCount--;
        if (GameManager.Instance != null && GameManager.Instance.HUD != null) GameManager.Instance.HUD.RefreshInventory();
        Vector3 origin = PlayerCamera.transform.position + PlayerCamera.transform.forward * 0.4f;
        GameObject grenade = MeshBundleLoader.InstantiateBundle("frag_grenade", false, false);
        grenade.name = "ThrownGrenade";
        grenade.transform.position = origin;
        grenade.transform.rotation = PlayerCamera.transform.rotation;
        GrenadeProjectile proj = grenade.AddComponent<GrenadeProjectile>();
        proj.Launch(PlayerCamera.transform.forward * 15f + Vector3.up * 4.5f);
        if (AudioManager.Instance != null) AudioManager.Instance.Play("jump", 0.6f, 0.7f);
    }

    public void PressMedkit()
    {
        if (IsDead || _healing || MedkitCount <= 0 || Health >= MaxHealth) return;
        MedkitCount--;
        _healing = true;
        _healEndTime = Time.time + 2f;
        _healRate = 22f;
        if (AudioManager.Instance != null) AudioManager.Instance.Play("heal", 0.8f);
        if (GameManager.Instance != null && GameManager.Instance.HUD != null) GameManager.Instance.HUD.RefreshInventory();
    }

    public void AddRecoil(float amount)
    {
        _pitch += amount * 0.32f;
        _yaw += Random.Range(-amount, amount) * 0.12f;
        _pitch = Mathf.Clamp(_pitch, -85f, 85f);
    }

    public void TakeDamage(float damage)
    {
        if (IsDead) return;
        float remaining = damage;
        if (Armor > 0f)
        {
            float absorbed = Mathf.Min(Armor, damage * 0.6f);
            Armor -= absorbed;
            remaining = damage - absorbed;
        }
        Health -= remaining;
        if (GameManager.Instance != null && GameManager.Instance.HUD != null)
        {
            GameManager.Instance.HUD.ShowDamageFlash();
            GameManager.Instance.HUD.SetHealth(Health, MaxHealth, Armor);
        }
        if (Health <= 0f)
        {
            Health = 0f;
            IsDead = true;
            if (GameManager.Instance != null) GameManager.Instance.OnPlayerDeath();
        }
    }

    public void Heal(float amount)
    {
        Health = Mathf.Min(MaxHealth, Health + amount);
        if (GameManager.Instance != null && GameManager.Instance.HUD != null)
            GameManager.Instance.HUD.SetHealth(Health, MaxHealth, Armor);
    }

    public void SetArmor(float amount)
    {
        Armor = Mathf.Clamp(amount, 0f, 100f);
        if (GameManager.Instance != null && GameManager.Instance.HUD != null)
            GameManager.Instance.HUD.SetHealth(Health, MaxHealth, Armor);
    }

    public void Respawn()
    {
        IsDead = false;
        Health = MaxHealth;
        Armor = 0f;
        _healing = false;
        transform.position = _spawnPos + new Vector3(Random.Range(-3f, 3f), 0f, Random.Range(-3f, 3f));
        _yaw = 0f;
        _pitch = 0f;
        _verticalVelocity = 0f;
        if (GameManager.Instance != null && GameManager.Instance.HUD != null)
            GameManager.Instance.HUD.SetHealth(Health, MaxHealth, Armor);
    }

    private void Update()
    {
        if (IsDead) return;
        ApplyLook();
        HandleHealing();
        HandleMovement();
        HandleKeyboard();
    }

    private void ApplyLook()
    {
        if (_lookAccum.sqrMagnitude > 0.0001f)
        {
            _yaw += _lookAccum.x * Sensitivity;
            _pitch -= _lookAccum.y * Sensitivity;
            _pitch = Mathf.Clamp(_pitch, -85f, 85f);
            _lookAccum = Vector2.zero;
        }
        transform.rotation = Quaternion.Euler(0f, _yaw, 0f);
        if (PlayerCamera != null)
        {
            float slideDip = Time.time < _slideEndTime ? -0.35f : 0f;
            PlayerCamera.transform.localRotation = Quaternion.Euler(_pitch, 0f, 0f);
            PlayerCamera.transform.localPosition = new Vector3(0f, 1.62f + slideDip, 0f);
        }
    }

    private void HandleHealing()
    {
        if (!_healing) return;
        Heal(_healRate * Time.deltaTime);
        if (Time.time >= _healEndTime || Health >= MaxHealth)
        {
            _healing = false;
            Effects.HealSparkle(transform.position);
        }
    }

    private void HandleMovement()
    {
        Vector2 input = Vector2.zero;
        if (GameManager.Instance != null && GameManager.Instance.HUD != null)
            input = GameManager.Instance.HUD.JoystickVector;
        if (input.sqrMagnitude < 0.01f)
        {
            input.x = Input.GetAxisRaw("Horizontal");
            input.y = Input.GetAxisRaw("Vertical");
        }
        input = Vector2.ClampMagnitude(input, 1f);

        bool sprinting = input.y > 0.9f && !_adsHeld && Time.time >= _slideEndTime;
        bool sliding = Time.time < _slideEndTime;
        float speed = sliding ? 9f : (sprinting ? 7.4f : 5.2f);
        if (_adsHeld) speed *= 0.55f;

        Vector3 forward = transform.forward;
        Vector3 right = transform.right;
        Vector3 move = (right * input.x + forward * input.y);
        if (move.sqrMagnitude > 1f) move.Normalize();
        move *= speed;

        if (Controller.isGrounded)
        {
            _verticalVelocity = -1f;
            if (_jumpQueued)
            {
                _verticalVelocity = 5.6f;
                if (AudioManager.Instance != null) AudioManager.Instance.Play("jump", 0.5f);
            }
        }
        else
        {
            _verticalVelocity -= 18f * Time.deltaTime;
        }
        _jumpQueued = false;

        if (_slideQueued)
        {
            _slideQueued = false;
            if (sprinting || input.magnitude > 0.5f)
            {
                _slideEndTime = Time.time + 0.85f;
                _verticalVelocity = Mathf.Min(_verticalVelocity, 0f);
            }
        }

        Vector3 velocity = move + Vector3.up * _verticalVelocity;
        Controller.Move(velocity * Time.deltaTime);
    }

    private void HandleKeyboard()
    {
        if (Input.GetKeyDown(KeyCode.Space)) PressJump();
        if (Input.GetKeyDown(KeyCode.C) || Input.GetKeyDown(KeyCode.LeftControl)) PressSlide();
        if (Input.GetKeyDown(KeyCode.R)) PressReload();
        if (Input.GetKeyDown(KeyCode.G)) PressGrenade();
        if (Input.GetKeyDown(KeyCode.H)) PressMedkit();
        if (Input.GetKeyDown(KeyCode.Tab)) PressWeaponNext();
        for (int i = 0; i < 5; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i) && Weapons != null) Weapons.SelectWeapon(i);
        }
        bool ads = Input.GetMouseButton(1) || Input.GetKey(KeyCode.Mouse1);
        if (Weapons != null && Weapons.IsAds != ads) SetAdsHeld(ads);
        bool fire = Input.GetMouseButton(0);
        if (Weapons != null && _fireHeld != fire && !Application.isMobilePlatform) SetFireHeld(fire);
    }
}
