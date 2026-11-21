using UnityEngine;

public static class SoundEffects
{
    private const int SampleRate = 44100;

    private static AudioClip attackClip;
    private static AudioClip hitClip;
    private static AudioClip enemyDeathClip;
    private static AudioClip playerHurtClip;
    private static AudioClip playerDeathClip;
    private static AudioClip spikeClip;
    private static AudioClip portalClip;

    public static void PlayAttack(Vector3 position)
    {
        Play(ref attackClip, "Attack", position, 0.18f, 0.12f, 720f, 280f, 0.08f);
    }

    public static void PlayHit(Vector3 position)
    {
        Play(ref hitClip, "Hit", position, 0.2f, 0.1f, 180f, 90f, 0.16f);
    }

    public static void PlayEnemyDeath(Vector3 position)
    {
        Play(ref enemyDeathClip, "EnemyDeath", position, 0.24f, 0.3f, 260f, 70f, 0.08f);
    }

    public static void PlayPlayerHurt(Vector3 position)
    {
        Play(ref playerHurtClip, "PlayerHurt", position, 0.22f, 0.18f, 440f, 180f, 0.2f);
    }

    public static void PlayPlayerDeath(Vector3 position)
    {
        Play(ref playerDeathClip, "PlayerDeath", position, 0.3f, 0.5f, 360f, 55f, 0.12f);
    }

    public static void PlaySpike(Vector3 position)
    {
        Play(ref spikeClip, "Spike", position, 0.22f, 0.1f, 900f, 260f, 0.3f);
    }

    public static void PlayPortal(Vector3 position)
    {
        Play(ref portalClip, "Portal", position, 0.2f, 0.55f, 180f, 900f, 0.05f);
    }

    private static void Play(
        ref AudioClip clip,
        string name,
        Vector3 position,
        float volume,
        float duration,
        float startFrequency,
        float endFrequency,
        float noiseAmount)
    {
        clip ??= CreateClip(name, duration, startFrequency, endFrequency, noiseAmount);

        GameObject audioObject = new($"SoundEffect_{name}");
        audioObject.transform.position = position;
        Object.DontDestroyOnLoad(audioObject);

        AudioSource source = audioObject.AddComponent<AudioSource>();
        source.playOnAwake = false;
        source.spatialBlend = 0f;
        source.volume = volume;
        source.PlayOneShot(clip);
        Object.Destroy(audioObject, clip.length);
    }

    private static AudioClip CreateClip(
        string name,
        float duration,
        float startFrequency,
        float endFrequency,
        float noiseAmount)
    {
        int sampleCount = Mathf.CeilToInt(SampleRate * duration);
        float[] samples = new float[sampleCount];

        for (int i = 0; i < sampleCount; i++)
        {
            float progress = (float)i / sampleCount;
            float frequency = Mathf.Lerp(startFrequency, endFrequency, progress);
            float envelope = Mathf.Sin(Mathf.PI * progress);
            float tone = Mathf.Sin(2f * Mathf.PI * frequency * i / SampleRate);
            float noise = (Random.value * 2f - 1f) * noiseAmount;
            samples[i] = (tone + noise) * envelope;
        }

        AudioClip clip = AudioClip.Create(name, sampleCount, 1, SampleRate, false);
        clip.SetData(samples, 0);
        return clip;
    }
}
