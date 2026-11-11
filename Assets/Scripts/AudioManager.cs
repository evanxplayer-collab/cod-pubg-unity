using System;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    private readonly Dictionary<string, AudioClip> _clips = new Dictionary<string, AudioClip>();
    private AudioSource _source;
    private const int SampleRate = 22050;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        _source = gameObject.AddComponent<AudioSource>();
        _source.playOnAwake = false;
        _source.loop = false;
        _source.spatialBlend = 0f;
        BuildClips();
    }

    private AudioClip MakeClip(string name, int samples, Func<int, float> fn)
    {
        AudioClip clip = AudioClip.Create(name, samples, 1, SampleRate, false);
        float[] data = new float[samples];
        for (int i = 0; i < samples; i++) data[i] = Mathf.Clamp(fn(i), -1f, 1f);
        clip.SetData(data, 0);
        return clip;
    }

    private void BuildClips()
    {
        System.Random rnd = new System.Random(12345);
        _clips["gun"] = MakeClip("gun", (int)(SampleRate * 0.18f), i =>
        {
            float t = (float)i / SampleRate;
            float env = Mathf.Exp(-t * 28f);
            float noise = (float)(rnd.NextDouble() * 2.0 - 1.0);
            float body = Mathf.Sin(2f * Mathf.PI * (90f - t * 220f) * t) * 0.55f;
            return (noise * 0.75f + body) * env * 0.9f;
        });
        _clips["sniper"] = MakeClip("sniper", (int)(SampleRate * 0.42f), i =>
        {
            float t = (float)i / SampleRate;
            float env = Mathf.Exp(-t * 11f);
            float noise = (float)(rnd.NextDouble() * 2.0 - 1.0);
            float body = Mathf.Sin(2f * Mathf.PI * (70f - t * 120f) * t) * 0.8f;
            return (noise * 0.85f + body) * env;
        });
        _clips["shotgun"] = MakeClip("shotgun", (int)(SampleRate * 0.30f), i =>
        {
            float t = (float)i / SampleRate;
            float env = Mathf.Exp(-t * 16f);
            float noise = (float)(rnd.NextDouble() * 2.0 - 1.0);
            return (noise * 0.9f + Mathf.Sin(2f * Mathf.PI * 60f * t) * 0.5f) * env;
        });
        _clips["reload"] = MakeClip("reload", (int)(SampleRate * 0.55f), i =>
        {
            float t = (float)i / SampleRate;
            float click = 0f;
            if (Math.Abs(t - 0.08f) < 0.012f || Math.Abs(t - 0.28f) < 0.012f || Math.Abs(t - 0.47f) < 0.012f)
                click = (float)(rnd.NextDouble() * 2.0 - 1.0) * 0.7f * Mathf.Exp(-Math.Abs(t - 0.08f) * 120f);
            return click + Mathf.Sin(2f * Mathf.PI * 180f * t) * 0.08f;
        });
        _clips["hit"] = MakeClip("hit", (int)(SampleRate * 0.07f), i =>
        {
            float t = (float)i / SampleRate;
            return Mathf.Sin(2f * Mathf.PI * 1250f * t) * Mathf.Exp(-t * 45f) * 0.55f;
        });
        _clips["kill"] = MakeClip("kill", (int)(SampleRate * 0.16f), i =>
        {
            float t = (float)i / SampleRate;
            float f = t < 0.08f ? 880f : 1320f;
            return Mathf.Sin(2f * Mathf.PI * f * t) * Mathf.Exp(-t * 18f) * 0.5f;
        });
        _clips["explosion"] = MakeClip("explosion", (int)(SampleRate * 0.85f), i =>
        {
            float t = (float)i / SampleRate;
            float env = Mathf.Exp(-t * 4.2f);
            float noise = (float)(rnd.NextDouble() * 2.0 - 1.0);
            float low = Mathf.Sin(2f * Mathf.PI * (55f - t * 25f) * t) * 0.9f;
            return (noise * 0.55f + low) * env;
        });
        _clips["pickup"] = MakeClip("pickup", (int)(SampleRate * 0.22f), i =>
        {
            float t = (float)i / SampleRate;
            float f = t < 0.11f ? 660f : 990f;
            return Mathf.Sin(2f * Mathf.PI * f * t) * Mathf.Exp(-t * 9f) * 0.45f;
        });
        _clips["heal"] = MakeClip("heal", (int)(SampleRate * 0.35f), i =>
        {
            float t = (float)i / SampleRate;
            return (Mathf.Sin(2f * Mathf.PI * 520f * t) + Mathf.Sin(2f * Mathf.PI * 780f * t) * 0.5f) * Mathf.Exp(-t * 6f) * 0.35f;
        });
        _clips["whistle"] = MakeClip("whistle", (int)(SampleRate * 1.4f), i =>
        {
            float t = (float)i / SampleRate;
            float f = 1400f - t * 650f;
            return Mathf.Sin(2f * Mathf.PI * f * t) * Mathf.Sin(Mathf.PI * Mathf.Clamp01(t / 1.4f)) * 0.28f;
        });
        _clips["victory"] = MakeClip("victory", (int)(SampleRate * 1.2f), i =>
        {
            float t = (float)i / SampleRate;
            float[] notes = new float[] { 523f, 659f, 784f, 1046f };
            int idx = Mathf.Clamp((int)(t / 0.28f), 0, 3);
            return Mathf.Sin(2f * Mathf.PI * notes[idx] * t) * Mathf.Exp(-(t - idx * 0.28f) * 5f) * 0.5f;
        });
        _clips["defeat"] = MakeClip("defeat", (int)(SampleRate * 1.1f), i =>
        {
            float t = (float)i / SampleRate;
            float[] notes = new float[] { 392f, 330f, 262f, 196f };
            int idx = Mathf.Clamp((int)(t / 0.26f), 0, 3);
            return Mathf.Sin(2f * Mathf.PI * notes[idx] * t) * Mathf.Exp(-(t - idx * 0.26f) * 4f) * 0.5f;
        });
        _clips["empty"] = MakeClip("empty", (int)(SampleRate * 0.06f), i =>
        {
            float t = (float)i / SampleRate;
            return Mathf.Sin(2f * Mathf.PI * 2200f * t) * Mathf.Exp(-t * 80f) * 0.35f;
        });
        _clips["jump"] = MakeClip("jump", (int)(SampleRate * 0.12f), i =>
        {
            float t = (float)i / SampleRate;
            return Mathf.Sin(2f * Mathf.PI * (300f + t * 900f) * t) * Mathf.Exp(-t * 10f) * 0.18f;
        });
    }

    public void Play(string name, float volume = 1f, float pitch = 1f)
    {
        AudioClip clip;
        if (!_clips.TryGetValue(name, out clip) || clip == null) return;
        _source.pitch = pitch;
        _source.PlayOneShot(clip, Mathf.Clamp01(volume));
    }

    public void PlayGun(string weaponId)
    {
        if (weaponId == "awm") Play("sniper", 1f, 0.9f);
        else if (weaponId == "shotgun") Play("shotgun", 1f, 1f);
        else Play("gun", 0.9f, 1f);
    }
}
