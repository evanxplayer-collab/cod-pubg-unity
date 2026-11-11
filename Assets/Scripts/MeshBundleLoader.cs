using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Rendering;

public class SoldierAnimClips
{
    public int vertCount;
    public Vector3[][] runFrames;
    public Vector3[][] shootFrames;
    public Vector3[][] deathFrames;
}

public static class MeshBundleLoader
{
    private static Material _baseStandardMat;
    private static Dictionary<string, GameObject> _prefabCache = new Dictionary<string, GameObject>();
    public static SoldierAnimClips SoldierAnims;

    public static Material GetBaseMaterial()
    {
        if (_baseStandardMat == null)
        {
            GameObject temp = GameObject.CreatePrimitive(PrimitiveType.Cube);
            _baseStandardMat = new Material(temp.GetComponent<MeshRenderer>().sharedMaterial);
            UnityEngine.Object.Destroy(temp);
        }
        return _baseStandardMat;
    }

    public static void LoadSoldierAnims()
    {
        if (SoldierAnims != null) return;
        TextAsset ta = Resources.Load<TextAsset>("enemy_soldier_anims");
        if (ta == null) return;
        byte[] data = ta.bytes;
        using (MemoryStream ms = new MemoryStream(data))
        using (BinaryReader br = new BinaryReader(ms))
        {
            SoldierAnimClips clips = new SoldierAnimClips();
            clips.vertCount = br.ReadInt32();
            int nRun = br.ReadInt32();
            int nShoot = br.ReadInt32();
            int nDeath = br.ReadInt32();
            clips.runFrames = new Vector3[nRun][];
            for (int i = 0; i < nRun; i++) clips.runFrames[i] = ReadVec3Array(br, clips.vertCount);
            clips.shootFrames = new Vector3[nShoot][];
            for (int i = 0; i < nShoot; i++) clips.shootFrames[i] = ReadVec3Array(br, clips.vertCount);
            clips.deathFrames = new Vector3[nDeath][];
            for (int i = 0; i < nDeath; i++) clips.deathFrames[i] = ReadVec3Array(br, clips.vertCount);
            SoldierAnims = clips;
        }
    }

    private static Vector3[] ReadVec3Array(BinaryReader br, int count)
    {
        byte[] raw = br.ReadBytes(count * 12);
        float[] floats = new float[count * 3];
        Buffer.BlockCopy(raw, 0, floats, 0, raw.Length);
        Vector3[] arr = new Vector3[count];
        for (int i = 0; i < count; i++)
        {
            int b = i * 3;
            arr[i] = new Vector3(floats[b], floats[b + 1], floats[b + 2]);
        }
        return arr;
    }

    private static Vector2[] ReadVec2Array(BinaryReader br, int count)
    {
        byte[] raw = br.ReadBytes(count * 8);
        float[] floats = new float[count * 2];
        Buffer.BlockCopy(raw, 0, floats, 0, raw.Length);
        Vector2[] arr = new Vector2[count];
        for (int i = 0; i < count; i++)
        {
            int b = i * 2;
            arr[i] = new Vector2(floats[b], floats[b + 1]);
        }
        return arr;
    }

    private static int[] ReadIntArray(BinaryReader br, int count)
    {
        byte[] raw = br.ReadBytes(count * 4);
        int[] arr = new int[count];
        Buffer.BlockCopy(raw, 0, arr, 0, raw.Length);
        return arr;
    }

    public static GameObject InstantiateBundle(string resourceName, bool addMeshColliders, bool castShadows = true)
    {
        TextAsset ta = Resources.Load<TextAsset>(resourceName);
        if (ta == null)
        {
            GameObject fallback = GameObject.CreatePrimitive(PrimitiveType.Cube);
            fallback.name = resourceName + "_Missing";
            return fallback;
        }

        GameObject root = new GameObject(resourceName);
        byte[] bytes = ta.bytes;
        using (MemoryStream ms = new MemoryStream(bytes))
        using (BinaryReader br = new BinaryReader(ms))
        {
            int subCount = br.ReadInt32();
            for (int s = 0; s < subCount; s++)
            {
                float r = br.ReadSingle();
                float g = br.ReadSingle();
                float b = br.ReadSingle();
                float a = br.ReadSingle();
                float metallic = br.ReadSingle();
                float roughness = br.ReadSingle();

                int texLen = br.ReadInt32();
                Texture2D tex = null;
                if (texLen > 0)
                {
                    byte[] texBytes = br.ReadBytes(texLen);
                    tex = new Texture2D(2, 2, TextureFormat.RGBA32, true);
                    ImageConversion.LoadImage(tex, texBytes);
                    tex.wrapMode = TextureWrapMode.Repeat;
                    tex.filterMode = FilterMode.Bilinear;
                }

                int vcnt = br.ReadInt32();
                Vector3[] verts = ReadVec3Array(br, vcnt);
                Vector3[] norms = ReadVec3Array(br, vcnt);
                Vector2[] uvs = ReadVec2Array(br, vcnt);

                int icnt = br.ReadInt32();
                int[] indices = ReadIntArray(br, icnt);

                Mesh m = new Mesh();
                if (vcnt > 65000) m.indexFormat = IndexFormat.UInt32;
                m.vertices = verts;
                m.normals = norms;
                m.uv = uvs;
                m.triangles = indices;
                m.RecalculateBounds();

                GameObject child = new GameObject("Submesh_" + s);
                child.transform.SetParent(root.transform, false);

                MeshFilter mf = child.AddComponent<MeshFilter>();
                mf.sharedMesh = m;

                MeshRenderer mr = child.AddComponent<MeshRenderer>();
                Material mat = new Material(GetBaseMaterial());
                mat.color = new Color(r, g, b, a);
                if (tex != null)
                {
                    mat.mainTexture = tex;
                    mat.color = Color.white;
                }
                if (mat.HasProperty("_Metallic")) mat.SetFloat("_Metallic", Mathf.Clamp01(metallic));
                if (mat.HasProperty("_Glossiness")) mat.SetFloat("_Glossiness", Mathf.Clamp01(1f - roughness));
                mr.sharedMaterial = mat;
                mr.shadowCastingMode = castShadows ? ShadowCastingMode.On : ShadowCastingMode.Off;
                mr.receiveShadows = true;

                if (addMeshColliders && icnt >= 3)
                {
                    MeshCollider mc = child.AddComponent<MeshCollider>();
                    mc.sharedMesh = m;
                }
            }
        }
        return root;
    }
}
