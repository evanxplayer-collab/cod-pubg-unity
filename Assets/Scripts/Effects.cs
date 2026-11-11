using UnityEngine;

public static class Effects
{
    private static Material MakeMat(Color c)
    {
        Material m = new Material(MeshBundleLoader.GetBaseMaterial());
        m.color = c;
        m.SetFloat("_Metallic", 0f);
        m.SetFloat("_Glossiness", 0f);
        return m;
    }

    public static void MuzzleFlash(Vector3 pos, Vector3 forward)
    {
        GameObject lightGo = new GameObject("MuzzleLight");
        lightGo.transform.position = pos + forward * 0.25f;
        Light l = lightGo.AddComponent<Light>();
        l.type = LightType.Point;
        l.color = new Color(1f, 0.72f, 0.25f);
        l.intensity = 2.4f;
        l.range = 7f;
        Object.Destroy(lightGo, 0.06f);

        GameObject flash = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        flash.name = "MuzzleFlash";
        flash.transform.position = pos + forward * 0.18f;
        flash.transform.localScale = new Vector3(0.16f, 0.16f, 0.28f);
        flash.transform.LookAt(pos + forward * 2f);
        MeshRenderer mr = flash.GetComponent<MeshRenderer>();
        mr.sharedMaterial = MakeMat(new Color(1f, 0.8f, 0.25f));
        mr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        mr.receiveShadows = false;
        Object.Destroy(flash, 0.05f);
    }

    public static void Tracer(Vector3 start, Vector3 end, Color color)
    {
        GameObject go = new GameObject("Tracer");
        LineRenderer lr = go.AddComponent<LineRenderer>();
        lr.positionCount = 2;
        lr.SetPosition(0, start);
        lr.SetPosition(1, end);
        lr.startWidth = 0.035f;
        lr.endWidth = 0.012f;
        lr.useWorldSpace = true;
        lr.material = MakeMat(color);
        lr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        lr.receiveShadows = false;
        Object.Destroy(go, 0.07f);
    }

    private static void Burst(Vector3 pos, Color color, int count, float speed, float size, float lifetime, float gravity)
    {
        GameObject go = new GameObject("Burst");
        go.transform.position = pos;
        ParticleSystem ps = go.AddComponent<ParticleSystem>();
        ParticleSystem.MainModule main = ps.main;
        main.duration = lifetime;
        main.loop = false;
        main.startLifetime = lifetime;
        main.startSpeed = speed;
        main.startSize = size;
        main.gravityModifier = gravity;
        main.maxParticles = count;
        main.simulationSpace = ParticleSystemSimulationSpace.World;
        ParticleSystem.EmissionModule emission = ps.emission;
        emission.rateOverTime = 0f;
        emission.SetBursts(new ParticleSystem.Burst[] { new ParticleSystem.Burst(0f, (short)count) });
        ParticleSystem.ShapeModule shape = ps.shape;
        shape.shapeType = ParticleSystemShapeType.Sphere;
        shape.radius = 0.06f;
        ParticleSystemRenderer renderer = ps.GetComponent<ParticleSystemRenderer>();
        renderer.material = MakeMat(color);
        renderer.renderMode = ParticleSystemRenderMode.Billboard;
        renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        renderer.receiveShadows = false;
        ps.Play();
        Object.Destroy(go, lifetime + 0.4f);
    }

    public static void HitSparks(Vector3 pos, bool blood)
    {
        Burst(pos, blood ? new Color(0.75f, 0.05f, 0.05f) : new Color(1f, 0.75f, 0.25f), blood ? 14 : 10, 2.2f, 0.06f, 0.35f, 1.2f);
    }

    public static void Explosion(Vector3 pos)
    {
        Burst(pos, new Color(1f, 0.45f, 0.12f), 70, 7f, 0.16f, 0.8f, 0.6f);
        Burst(pos, new Color(0.25f, 0.25f, 0.25f), 35, 3.5f, 0.35f, 1.4f, -0.15f);
        GameObject lightGo = new GameObject("ExplosionLight");
        lightGo.transform.position = pos + Vector3.up * 0.6f;
        Light l = lightGo.AddComponent<Light>();
        l.type = LightType.Point;
        l.color = new Color(1f, 0.5f, 0.15f);
        l.intensity = 4f;
        l.range = 16f;
        Object.Destroy(lightGo, 0.35f);
        GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        sphere.name = "ExplosionSphere";
        sphere.transform.position = pos;
        sphere.transform.localScale = Vector3.one * 0.6f;
        MeshRenderer mr = sphere.GetComponent<MeshRenderer>();
        mr.sharedMaterial = MakeMat(new Color(1f, 0.55f, 0.15f));
        mr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        mr.receiveShadows = false;
        sphere.AddComponent<ExpandingSphere>();
        Object.Destroy(sphere, 0.45f);
    }

    public static void Smoke(Vector3 pos, Color color, float duration)
    {
        Burst(pos, color, 45, 0.7f, 0.55f, duration, -0.25f);
    }

    public static void HealSparkle(Vector3 pos)
    {
        Burst(pos + Vector3.up * 0.8f, new Color(0.35f, 1f, 0.45f), 24, 1.2f, 0.08f, 0.7f, -0.4f);
    }
}

public class ExpandingSphere : MonoBehaviour
{
    private float _t;
    private void Update()
    {
        _t += Time.deltaTime;
        float s = 0.6f + _t * 11f;
        transform.localScale = Vector3.one * s;
        MeshRenderer mr = GetComponent<MeshRenderer>();
        if (mr != null)
        {
            Color c = mr.sharedMaterial.color;
            c.a = Mathf.Clamp01(1f - _t * 2.4f);
            mr.sharedMaterial.color = c;
        }
    }
}
