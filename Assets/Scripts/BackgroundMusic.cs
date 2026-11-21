using UnityEngine;

public class BackgroundMusic : MonoBehaviour
{
    private const int SampleRate = 44100;
    private const float TrackDuration = 16f;

    private static GameObject musicObject;

    private void Awake()
    {
        if (musicObject == null)
        {
            musicObject = GameObject.Find("BackgroundMusic");
        }

        if (musicObject != null)
        {
            AudioSource existingSource = musicObject.GetComponent<AudioSource>();
            if (existingSource != null && !existingSource.isPlaying)
            {
                existingSource.Play();
            }

            Destroy(this);
            return;
        }

        musicObject = new GameObject("BackgroundMusic");
        DontDestroyOnLoad(musicObject);

        AudioSource source = musicObject.AddComponent<AudioSource>();
        source.clip = CreateTrack();
        source.loop = true;
        source.playOnAwake = false;
        source.mute = false;
        source.spatialBlend = 0f;
        source.volume = 0.45f;
        source.Play();
    }

    private static AudioClip CreateTrack()
    {
        int sampleCount = Mathf.CeilToInt(SampleRate * TrackDuration);
        float[] samples = new float[sampleCount];
        float[] roots = { 55f, 65.41f, 73.42f, 49f, 55f, 65.41f, 73.42f, 49f };
        float[] melody = { 220f, 261.63f, 293.66f, 329.63f, 293.66f, 261.63f, 220f, 196f };

        for (int i = 0; i < sampleCount; i++)
        {
            float time = (float)i / SampleRate;
            int section = Mathf.FloorToInt(time / 2f) % roots.Length;
            float sectionTime = time % 2f;
            float root = roots[section];
            float melodyFrequency = melody[section];

            float bass = Mathf.Sin(2f * Mathf.PI * root * time) * 0.3f;
            float fifth = Mathf.Sin(2f * Mathf.PI * root * 1.5f * time) * 0.12f;
            float pad = Mathf.Sin(2f * Mathf.PI * root * 2f * time + Mathf.Sin(time * 0.7f) * 0.4f) * 0.08f;
            float melodyEnvelope = Mathf.Sin(Mathf.PI * Mathf.Clamp01(sectionTime / 2f));
            float melodyTone = Mathf.Sin(2f * Mathf.PI * melodyFrequency * time) * melodyEnvelope * 0.12f;
            float pulse = Mathf.Sin(2f * Mathf.PI * 2f * sectionTime) * 0.025f;
            float loopEnvelope = Mathf.Sin(Mathf.PI * time / TrackDuration);

            samples[i] = (bass + fifth + pad + melodyTone + pulse) * loopEnvelope;
        }

        AudioClip clip = AudioClip.Create("DungeonBackgroundTrack", sampleCount, 1, SampleRate, false);
        clip.SetData(samples, 0);
        return clip;
    }
}
