using UnityEngine;

public class GrenadeProjectile : MonoBehaviour
{
    private Vector3 _velocity;
    private float _explodeTime;
    private bool _exploded;

    public void Launch(Vector3 velocity)
    {
        _velocity = velocity;
        _explodeTime = Time.time + 2.2f;
    }

    private void Update()
    {
        if (_exploded) return;
        _velocity.y -= 14f * Time.deltaTime;
        transform.position += _velocity * Time.deltaTime;
        transform.Rotate(240f * Time.deltaTime, 180f * Time.deltaTime, 0f);
        if (transform.position.y <= 0.12f)
        {
            transform.position = new Vector3(transform.position.x, 0.12f, transform.position.z);
            _velocity.y *= -0.35f;
            _velocity.x *= 0.7f;
            _velocity.z *= 0.7f;
        }
        if (Time.time >= _explodeTime) Explode();
    }

    private void Explode()
    {
        _exploded = true;
        Vector3 pos = transform.position;
        Effects.Explosion(pos);
        if (AudioManager.Instance != null) AudioManager.Instance.Play("explosion", 1f);
        float radius = 6.5f;
        EnemySoldier[] enemies = FindObjectsOfType<EnemySoldier>();
        foreach (EnemySoldier e in enemies)
        {
            if (e == null || e.IsDead) continue;
            float d = Vector3.Distance(pos, e.transform.position);
            if (d <= radius)
            {
                float dmg = 110f * (1f - d / radius);
                e.TakeDamage(dmg, pos);
            }
        }
        FPSPlayer player = GameManager.Instance != null ? GameManager.Instance.Player : null;
        if (player != null && !player.IsDead)
        {
            float d = Vector3.Distance(pos, player.transform.position);
            if (d <= radius)
            {
                float dmg = 70f * (1f - d / radius);
                player.TakeDamage(dmg);
            }
        }
        Destroy(gameObject);
    }
}
