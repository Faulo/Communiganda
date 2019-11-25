using UnityEngine;
using UnityEngine.Audio;

[System.Serializable]
public class Sound
{
    public string name;
    [HideInInspector]
    public AudioSource source;
    public AudioMixerGroup mixerGroup;
    public AudioClip clip;
    [Range(0f, 1f)]
    public float volume;
    [Range(0f, 2f)]
    public float pitch;
    public bool loop;
    public bool playOnAwake;
    public bool ignoreGamePaused;

    public bool threeDSound;
    [Range(0f, 1f)]
    public float spatialBlend;
    public float maxRange;
    public GameObject sourceObject;
}

[System.Serializable]
public class SoundContainer
{
    public string name;
    public AudioClip[] clips;
    public Sound[] sounds;
}
