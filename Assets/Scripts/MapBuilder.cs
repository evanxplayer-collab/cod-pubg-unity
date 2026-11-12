using UnityEngine;

public class MapBuilder : MonoBehaviour
{
    public GameObject MapRoot;
    public Vector3[] EnemySpawnPoints;
    public Vector3 PlayerSpawn = new Vector3(0f, 0f, 28f);

    public void Build()
    {
        SetupSkybox();
        SetupLighting();
        SetupGround();
        MapRoot = MeshBundleLoader.InstantiateBundle("cs_dust2", true, true);
        MapRoot.name = "Dust2Map";
        MapRoot.transform.position = Vector3.zero;
        MapRoot.transform.rotation = Quaternion.identity;
        AddCover();
        AddEnvironmentProps();
        AddDistantHills();
        EnemySpawnPoints = new Vector3[]
        {
            new Vector3(-18f, 0f, -22f), new Vector3(16f, 0f, -20f), new Vector3(-10f, 0f, 8f),
            new Vector3(12f, 0f, 10f), new Vector3(0f, 0f, -8f), new Vector3(-22f, 0f, 18f),
            new Vector3(22f, 0f, 16f), new Vector3(4f, 0f, -28f), new Vector3(-4f, 0f, 24f),
            new Vector3(18f, 0f, -4f)
        };
    }

    private void SetupSkybox()
    {
        Material sky = Resources.Load<Material>("SkyboxMat");
        if (sky == null)
        {
            Shader procedural = Shader.Find("Procedural");
            if (procedural != null)
            {
                sky = new Material(procedural);
                sky.SetFloat("_Exposure", 1.1f);
                sky.SetColor("_SkyTint", new Color(0.45f, 0.62f, 0.9f));
                sky.SetColor("_GroundColor", new Color(0.35f, 0.3f, 0.22f));
            }
        }
        if (sky != null)
        {
            RenderSettings.skybox = sky;
            RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Trilight;
            RenderSettings.ambientSkyColor = new Color(0.55f, 0.65f, 0.85f);
            RenderSettings.ambientEquatorColor = new Color(0.6f, 0.58f, 0.5f);
            RenderSettings.ambientGroundColor = new Color(0.4f, 0.36f, 0.28f);
        }
    }

    private void SetupLighting()
    {
        GameObject sunGo = new GameObject("Sun");
        // Warm late-afternoon sun for long, dramatic shadows.
        sunGo.transform.rotation = Quaternion.Euler(42f, -38f, 0f);
        Light sun = sunGo.AddComponent<Light>();
        sun.type = LightType.Directional;
        sun.intensity = 1.45f;
        sun.color = new Color(1f, 0.93f, 0.8f);
        sun.shadows = LightShadows.Soft;
        sun.shadowCustomResolution = 2048;
        sun.shadowBias = 0.06f;
        sun.shadowNormalBias = 0.4f;
        // Gentle cool fill from the opposite side so shadows are not pure black.
        GameObject fillGo = new GameObject("FillLight");
        fillGo.transform.rotation = Quaternion.Euler(30f, 140f, 0f);
        Light fill = fillGo.AddComponent<Light>();
        fill.type = LightType.Directional;
        fill.intensity = 0.28f;
        fill.color = new Color(0.65f, 0.75f, 0.95f);
        fill.shadows = LightShadows.None;
        RenderSettings.ambientLight = new Color(0.55f, 0.6f, 0.72f);
        RenderSettings.ambientIntensity = 1.05f;
        RenderSettings.fog = true;
        RenderSettings.fogColor = new Color(0.74f, 0.8f, 0.88f);
        RenderSettings.fogMode = FogMode.ExponentialSquared;
        RenderSettings.fogDensity = 0.0038f;
    }

    private void SetupGround()
    {
        GameObject ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
        ground.name = "Ground";
        ground.transform.position = new Vector3(0f, -0.02f, 0f);
        ground.transform.localScale = new Vector3(26f, 1f, 26f);
        MeshRenderer mr = ground.GetComponent<MeshRenderer>();
        Material mat = new Material(MeshBundleLoader.GetBaseMaterial());
        mat.color = new Color(0.72f, 0.63f, 0.45f);
        mat.SetFloat("_Metallic", 0f);
        mat.SetFloat("_Glossiness", 0.05f);
        mr.sharedMaterial = mat;

        // Dirt / grass variation patches so the ground is not one flat color.
        Color[] patchColors = new Color[]
        {
            new Color(0.6f, 0.52f, 0.36f), new Color(0.5f, 0.55f, 0.34f),
            new Color(0.66f, 0.58f, 0.42f), new Color(0.55f, 0.48f, 0.33f)
        };
        for (int i = 0; i < 26; i++)
        {
            GameObject patch = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            patch.name = "GroundPatch";
            float x = Random.Range(-30f, 30f);
            float z = Random.Range(-34f, 34f);
            patch.transform.position = new Vector3(x, 0.015f, z);
            float s = Random.Range(2.5f, 6f);
            patch.transform.localScale = new Vector3(s, 0.03f, s);
            patch.transform.rotation = Quaternion.identity;
            MeshRenderer pmr = patch.GetComponent<MeshRenderer>();
            Material pmat = new Material(MeshBundleLoader.GetBaseMaterial());
            pmat.color = patchColors[i % patchColors.Length];
            pmat.SetFloat("_Glossiness", 0.02f);
            pmr.sharedMaterial = pmat;
            pmr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            pmr.receiveShadows = true;
            Collider col = patch.GetComponent<Collider>();
            if (col != null) Destroy(col);
        }
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

    private Material MakePropMat(Color c, float glossiness)
    {
        Material mat = new Material(MeshBundleLoader.GetBaseMaterial());
        mat.color = c;
        mat.SetFloat("_Metallic", 0.05f);
        mat.SetFloat("_Glossiness", glossiness);
        return mat;
    }

    private GameObject MakeProp(PrimitiveType type, Vector3 pos, Vector3 scale, Quaternion rot, Color color, float glossiness, bool collider)
    {
        GameObject go = GameObject.CreatePrimitive(type);
        go.transform.position = pos;
        go.transform.localScale = scale;
        go.transform.rotation = rot;
        MeshRenderer mr = go.GetComponent<MeshRenderer>();
        mr.sharedMaterial = MakePropMat(color, glossiness);
        if (!collider)
        {
            Collider col = go.GetComponent<Collider>();
            if (col != null) Destroy(col);
        }
        return go;
    }

    private void AddEnvironmentProps()
    {
        Color trunkColor = new Color(0.32f, 0.22f, 0.13f);
        Color leafColor = new Color(0.16f, 0.38f, 0.16f);
        Color leafColor2 = new Color(0.22f, 0.45f, 0.2f);
        Color rockColor = new Color(0.42f, 0.42f, 0.44f);
        Color barrelRed = new Color(0.55f, 0.2f, 0.14f);
        Color barrelGreen = new Color(0.2f, 0.38f, 0.22f);
        Color sandColor = new Color(0.68f, 0.6f, 0.42f);
        Color concreteColor = new Color(0.55f, 0.55f, 0.57f);
        Color woodColor = new Color(0.5f, 0.38f, 0.22f);

        // Trees (trunk + cone foliage) scattered around the edges.
        Vector2[] treeSpots = new Vector2[]
        {
            new Vector2(-26f, -26f), new Vector2(26f, -24f), new Vector2(-28f, 4f), new Vector2(28f, 6f),
            new Vector2(-24f, 26f), new Vector2(24f, 28f), new Vector2(-14f, -30f), new Vector2(14f, -30f),
            new Vector2(-30f, -8f), new Vector2(30f, -10f), new Vector2(-8f, 30f), new Vector2(10f, 30f)
        };
        foreach (Vector2 spot in treeSpots)
        {
            float h = Random.Range(2.6f, 4.2f);
            MakeProp(PrimitiveType.Cylinder, new Vector3(spot.x, h * 0.25f, spot.y), new Vector3(0.35f, h * 0.5f, 0.35f), Quaternion.identity, trunkColor, 0.05f, false);
            Color leaf = Random.value > 0.5f ? leafColor : leafColor2;
            MakeProp(PrimitiveType.Sphere, new Vector3(spot.x, h * 0.8f, spot.y), new Vector3(1.9f, h * 0.85f, 1.9f), Quaternion.identity, leaf, 0.05f, false);
        }

        // Rocks.
        for (int i = 0; i < 12; i++)
        {
            float x = Random.Range(-28f, 28f);
            float z = Random.Range(-30f, 30f);
            if (Mathf.Abs(x) < 5f && Mathf.Abs(z) < 5f) continue; // keep center clear
            float s = Random.Range(0.5f, 1.4f);
            MakeProp(PrimitiveType.Sphere, new Vector3(x, s * 0.3f, z), new Vector3(s, s * 0.7f, s), Quaternion.Euler(0f, Random.Range(0f, 360f), 0f), rockColor, 0.1f, true);
        }

        // Barrels (cover).
        Vector2[] barrelSpots = new Vector2[]
        {
            new Vector2(-6f, -14f), new Vector2(7f, -4f), new Vector2(-12f, 12f), new Vector2(10f, 18f),
            new Vector2(2f, -24f), new Vector2(-18f, -2f), new Vector2(18f, 12f), new Vector2(-3f, 6f)
        };
        foreach (Vector2 spot in barrelSpots)
        {
            Color bc = Random.value > 0.5f ? barrelRed : barrelGreen;
            MakeProp(PrimitiveType.Cylinder, new Vector3(spot.x, 0.55f, spot.y), new Vector3(0.8f, 1.1f, 0.8f), Quaternion.identity, bc, 0.35f, true);
            MakeProp(PrimitiveType.Cylinder, new Vector3(spot.x + 0.9f, 0.55f, spot.y + 0.3f), new Vector3(0.8f, 1.1f, 0.8f), Quaternion.identity, bc, 0.35f, true);
        }

        // Sandbag walls (low cover).
        Vector2[] sandSpots = new Vector2[]
        {
            new Vector2(-4f, -10f), new Vector2(12f, -16f), new Vector2(-16f, 16f), new Vector2(4f, 20f), new Vector2(16f, 2f)
        };
        foreach (Vector2 spot in sandSpots)
        {
            MakeProp(PrimitiveType.Cube, new Vector3(spot.x, 0.5f, spot.y), new Vector3(3f, 1f, 0.8f), Quaternion.Euler(0f, Random.Range(0f, 180f), 0f), sandColor, 0.03f, true);
        }

        // Concrete barriers.
        Vector2[] barrierSpots = new Vector2[]
        {
            new Vector2(-10f, -20f), new Vector2(8f, 8f), new Vector2(-20f, 10f), new Vector2(20f, -14f), new Vector2(0f, 12f)
        };
        foreach (Vector2 spot in barrierSpots)
        {
            MakeProp(PrimitiveType.Cube, new Vector3(spot.x, 0.7f, spot.y), new Vector3(2.6f, 1.4f, 0.6f), Quaternion.Euler(0f, Random.Range(0f, 180f), 0f), concreteColor, 0.08f, true);
        }

        // Extra wooden crates for cover variety.
        for (int i = 0; i < 8; i++)
        {
            float x = Random.Range(-22f, 22f);
            float z = Random.Range(-26f, 26f);
            if (Mathf.Abs(x) < 4f && Mathf.Abs(z) < 4f) continue;
            float s = Random.Range(1f, 1.6f);
            MakeProp(PrimitiveType.Cube, new Vector3(x, s * 0.5f, z), new Vector3(s, s, s), Quaternion.Euler(0f, Random.Range(0f, 180f), 0f), woodColor, 0.06f, true);
        }
    }

    private void AddDistantHills()
    {
        // Low hills on the horizon so the skyline is not empty.
        Color hillColor = new Color(0.5f, 0.48f, 0.4f);
        for (int i = 0; i < 10; i++)
        {
            float angle = (360f / 10f) * i + Random.Range(-8f, 8f);
            float rad = angle * Mathf.Deg2Rad;
            float dist = Random.Range(85f, 120f);
            Vector3 pos = new Vector3(Mathf.Cos(rad) * dist, 0f, Mathf.Sin(rad) * dist);
            float h = Random.Range(10f, 22f);
            float w = Random.Range(30f, 55f);
            MakeProp(PrimitiveType.Sphere, new Vector3(pos.x, h * 0.25f, pos.z), new Vector3(w, h, w), Quaternion.identity, hillColor, 0.02f, false);
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
