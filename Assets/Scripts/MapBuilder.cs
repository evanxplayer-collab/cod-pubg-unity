using UnityEngine;

public class MapBuilder : MonoBehaviour
{
    public GameObject MapRoot;
    public Vector3[] EnemySpawnPoints;
    public Vector3 PlayerSpawn = new Vector3(0f, 0f, 28f);

    public void Build()
    {
        SetupLighting();
        SetupGround();
        MapRoot = MeshBundleLoader.InstantiateBundle("cs_dust2", true, true);
        MapRoot.name = "Dust2Map";
        MapRoot.transform.position = Vector3.zero;
        MapRoot.transform.rotation = Quaternion.identity;
        AddCover();
        EnemySpawnPoints = new Vector3[]
        {
            new Vector3(-18f, 0f, -22f), new Vector3(16f, 0f, -20f), new Vector3(-10f, 0f, 8f),
            new Vector3(12f, 0f, 10f), new Vector3(0f, 0f, -8f), new Vector3(-22f, 0f, 18f),
            new Vector3(22f, 0f, 16f), new Vector3(4f, 0f, -28f), new Vector3(-4f, 0f, 24f),
            new Vector3(18f, 0f, -4f)
        };
    }

    private void SetupLighting()
    {
        GameObject sunGo = new GameObject("Sun");
        sunGo.transform.rotation = Quaternion.Euler(48f, -32f, 0f);
        Light sun = sunGo.AddComponent<Light>();
        sun.type = LightType.Directional;
        sun.intensity = 1.25f;
        sun.color = new Color(1f, 0.96f, 0.88f);
        sun.shadows = LightShadows.Soft;
        sun.shadowResolution = 2048;
        RenderSettings.ambientLight = new Color(0.58f, 0.64f, 0.74f);
        RenderSettings.fog = true;
        RenderSettings.fogColor = new Color(0.72f, 0.78f, 0.86f);
        RenderSettings.fogMode = FogMode.ExponentialSquared;
        RenderSettings.fogDensity = 0.0045f;
    }

    private void SetupGround()
    {
        GameObject ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
        ground.name = "Ground";
        ground.transform.position = new Vector3(0f, -0.02f, 0f);
        ground.transform.localScale = new Vector3(24f, 1f, 24f);
        MeshRenderer mr = ground.GetComponent<MeshRenderer>();
        Material mat = new Material(MeshBundleLoader.GetBaseMaterial());
        mat.color = new Color(0.76f, 0.68f, 0.5f);
        mr.sharedMaterial = mat;
    }

    private void AddCover()
    {
        Vector3[] positions = new Vector3[]
        {
            new Vector3(-8f, 0.6f, -6f), new Vector3(9f, 0.6f, -12f), new Vector3(-14f, 0.6f, 6f),
            new Vector3(6f, 0.6f, 14f), new Vector3(0f, 0.6f, -18f), new Vector3(-4f, 0.6f, 18f)
        };
        foreach (Vector3 p in positions)
        {
            GameObject crate = GameObject.CreatePrimitive(PrimitiveType.Cube);
            crate.name = "CoverCrate";
            crate.transform.position = p;
            crate.transform.localScale = new Vector3(2.2f, 1.2f, 2.2f);
            crate.transform.rotation = Quaternion.Euler(0f, Random.Range(0f, 180f), 0f);
            MeshRenderer mr = crate.GetComponent<MeshRenderer>();
            Material mat = new Material(MeshBundleLoader.GetBaseMaterial());
            mat.color = new Color(0.45f, 0.38f, 0.28f);
            mr.sharedMaterial = mat;
        }
    }

    public Vector3 GetRandomEnemySpawn()
    {
        if (EnemySpawnPoints == null || EnemySpawnPoints.Length == 0) return new Vector3(0f, 0f, -20f);
        return EnemySpawnPoints[Random.Range(0, EnemySpawnPoints.Length)];
    }

    public Vector3 GetRandomLootPosition()
    {
        return new Vector3(Random.Range(-22f, 22f), 0f, Random.Range(-26f, 26f));
    }

    public Vector3 GetAirdropPosition()
    {
        return new Vector3(Random.Range(-12f, 12f), 0f, Random.Range(-14f, 14f));
    }
}
