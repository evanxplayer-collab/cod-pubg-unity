using UnityEngine;

public class EnemySoldier : MonoBehaviour
{
    public float Health = 100f;
    public float MaxHealth = 100f;
    public bool IsDead;
    public float MoveSpeed = 3.2f;
    public float AttackRange = 26f;
    public float Damage = 8f;
    public float FireInterval = 0.9f;
    public bool RespawnInTdm = true;

    private MeshFilter _meshFilter;
    private MeshRenderer _renderer;
    private Mesh _mesh;
    private Vector3[] _verts;
    private float _nextFireTime;
    private float _deathTime;
    private float _respawnTime;
    private float _flashTime;
    private Color _baseColor = Color.white;
    private Vector3 _spawnPos;
    private bool _initialized;

    public void Initialize(Vector3 spawnPos, bool respawnInTdm)
    {
        _spawnPos = spawnPos;
        RespawnInTdm = respawnInTdm;
        transform.position = spawnPos;
        transform.rotation = Quaternion.identity;
        MeshBundleLoader.LoadSoldierAnims();
        GameObject visual = MeshBundleLoader.InstantiateBundle("enemy_soldier", false, true);
        visual.name = "SoldierVisual";
        visual.transform.SetParent(transform, false);
        visual.transform.localPosition = Vector3.zero;
        visual.transform.localRotation = Quaternion.identity;
        _meshFilter = visual.GetComponentInChildren<MeshFilter>();
        _renderer = visual.GetComponentInChildren<MeshRenderer>();
        if (_meshFilter != null)
        {
            _mesh = _meshFilter.sharedMesh;
            _verts = new Vector3[_mesh.vertexCount];
            _mesh.bounds = new Bounds(Vector3.zero, new Vector3(3f, 3f, 3f));
        }
        if (_renderer != null) _baseColor = _renderer.material.color;

        CapsuleCollider body = gameObject.AddComponent<CapsuleCollider>();
        body.name = "Body";
        body.center = new Vector3(0f, 0.9f, 0f);
        body.radius = 0.34f;
        body.height = 1.7f;
        SphereCollider head = gameObject.AddComponent<SphereCollider>();
        head.name = "Head";
        head.center = new Vector3(0f, 1.58f, 0f);
        head.radius = 0.22f;
        _initialized = true;
    }

    public void TakeDamage(float amount, Vector3 fromPosition)
    {
        if (IsDead || !_initialized) return;
        Health -= amount;
        _flashTime = 0.12f;
        if (_renderer != null) _renderer.material.color = new Color(1f, 0.25f, 0.25f);
        if (Health <= 0f)
        {
            IsDead = true;
            _deathTime = Time.time;
            _respawnTime = Time.time + (RespawnInTdm ? 3f : 2f);
            if (GameManager.Instance != null) GameManager.Instance.OnEnemyKilled(this);
        }
    }

    private void Update()
    {
        if (!_initialized) return;
        if (_flashTime > 0f)
        {
            _flashTime -= Time.deltaTime;
            if (_flashTime <= 0f && _renderer != null) _renderer.material.color = _baseColor;
        }

        if (IsDead)
        {
            AnimateDeath();
            if (Time.time >= _respawnTime)
            {
                if (RespawnInTdm) Respawn();
                else Destroy(gameObject);
            }
            return;
        }

        FPSPlayer player = GameManager.Instance != null ? GameManager.Instance.Player : null;
        if (player == null) { AnimateRun(0.2f); return; }
        Vector3 toPlayer = player.transform.position - transform.position;
        toPlayer.y = 0f;
        float dist = toPlayer.magnitude;
        Vector3 dir = dist > 0.001f ? toPlayer / dist : Vector3.forward;
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), Time.deltaTime * 6f);

        bool attacking = false;
        if (dist > AttackRange * 0.75f)
        {
            transform.position += dir * MoveSpeed * Time.deltaTime;
            transform.position.y = 0f;
            AnimateRun(1f);
        }
        else
        {
            attacking = true;
            AnimateShoot();
            if (Time.time >= _nextFireTime)
            {
                _nextFireTime = Time.time + FireInterval + Random.Range(-0.15f, 0.25f);
                Vector3 from = transform.position + Vector3.up * 1.45f;
                Vector3 target = player.transform.position + Vector3.up * 1.35f;
                Vector3 shotDir = (target - from).normalized;
                RaycastHit hit;
                if (Physics.Raycast(from, shotDir, out hit, AttackRange))
                {
                    if (hit.collider.GetComponentInParent<EnemySoldier>() == this) { }
                    else if (hit.collider.GetComponentInParent<FPSPlayer>() != null)
                    {
                        player.TakeDamage(Damage * Random.Range(0.7f, 1.3f));
                        Effects.HitSparks(hit.point, true);
                    }
                }
                Effects.Tracer(from + dir * 0.4f, from + shotDir * 18f, new Color(1f, 0.6f, 0.25f));
                if (AudioManager.Instance != null) AudioManager.Instance.Play("gun", 0.25f, 0.8f);
            }
        }
        if (!attacking && dist <= AttackRange * 0.75f) AnimateShoot();
    }

    private void Respawn()
    {
        IsDead = false;
        Health = MaxHealth;
        transform.position = _spawnPos + new Vector3(Random.Range(-4f, 4f), 0f, Random.Range(-4f, 4f));
        transform.position.y = 0f;
        if (_renderer != null) _renderer.material.color = _baseColor;
        if (_meshFilter != null && MeshBundleLoader.SoldierAnims != null && MeshBundleLoader.SoldierAnims.shootFrames.Length > 0)
        {
            _meshFilter.sharedMesh.vertices = MeshBundleLoader.SoldierAnims.shootFrames[0];
        }
    }

    private void ApplyFrame(Vector3[] a, Vector3[] b, float f)
    {
        if (_meshFilter == null || a == null || b == null) return;
        if (_verts == null || _verts.Length != a.Length) _verts = new Vector3[a.Length];
        for (int i = 0; i < a.Length; i++) _verts[i] = Vector3.Lerp(a[i], b[i], f);
        _meshFilter.sharedMesh.vertices = _verts;
    }

    private void AnimateRun(float speedFactor)
    {
        if (MeshBundleLoader.SoldierAnims == null || MeshBundleLoader.SoldierAnims.runFrames.Length == 0) return;
        Vector3[][] frames = MeshBundleLoader.SoldierAnims.runFrames;
        float t = Time.time * 8f * Mathf.Max(0.1f, speedFactor);
        int i0 = Mathf.FloorToInt(t) % frames.Length;
        int i1 = (i0 + 1) % frames.Length;
        float f = t - Mathf.Floor(t);
        ApplyFrame(frames[i0], frames[i1], f);
    }

    private void AnimateShoot()
    {
        if (MeshBundleLoader.SoldierAnims == null || MeshBundleLoader.SoldierAnims.shootFrames.Length == 0) return;
        Vector3[][] frames = MeshBundleLoader.SoldierAnims.shootFrames;
        float t = Time.time * 4f;
        int i0 = Mathf.FloorToInt(t) % frames.Length;
        int i1 = (i0 + 1) % frames.Length;
        float f = t - Mathf.Floor(t);
        ApplyFrame(frames[i0], frames[i1], f);
    }

    private void AnimateDeath()
    {
        if (MeshBundleLoader.SoldierAnims == null || MeshBundleLoader.SoldierAnims.deathFrames.Length == 0) return;
        Vector3[][] frames = MeshBundleLoader.SoldierAnims.deathFrames;
        float t = Mathf.Clamp01((Time.time - _deathTime) / 1.1f) * (frames.Length - 1);
        int i0 = Mathf.Clamp(Mathf.FloorToInt(t), 0, frames.Length - 1);
        int i1 = Mathf.Clamp(i0 + 1, 0, frames.Length - 1);
        float f = t - Mathf.Floor(t);
        ApplyFrame(frames[i0], frames[i1], f);
    }
}
