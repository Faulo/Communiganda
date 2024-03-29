using System;
using System.Collections;
using UnityEngine;

namespace Communiganda {
    public class AudioManager : MonoBehaviour {
        public Sound[] sounds;
        public SoundContainer[] soundContainers;

        public static AudioManager instance;
        AudioSource oneShotSource;

        void Awake() {
            instance = this;
            var editorSource = GetComponent<AudioSource>();
            if (editorSource) {
                Destroy(editorSource);
            }

            oneShotSource = gameObject.AddComponent<AudioSource>();
            foreach (var s in sounds) {
                AudioSource newSource;
                if (s.threeDSound) {
                    if (s.sourceObject == null) {
#if UNITY_EDITOR
                        Debug.LogError("No source object specified for 3D sound effect with name: " + s.name);
                        UnityEditor.EditorApplication.isPlaying = false;
#endif
#if UNITY_STANDALONE

#endif
                        break;
                    }
                    newSource = s.sourceObject.AddComponent<AudioSource>();
                    newSource.rolloffMode = AudioRolloffMode.Linear;

                } else {
                    newSource = gameObject.AddComponent<AudioSource>();
                }

                s.source = newSource;
                s.source.clip = s.clip;
                s.source.volume = s.volume;
                s.source.pitch = s.pitch;
                s.source.outputAudioMixerGroup = s.mixerGroup;
                s.source.loop = s.loop;
                s.source.playOnAwake = s.playOnAwake;
                s.source.ignoreListenerPause = s.ignoreGamePaused;
                s.source.spatialBlend = s.spatialBlend;
                s.source.minDistance = 0f;
                s.source.maxDistance = s.maxRange;
                s.source.spatialize = true;
                s.source.dopplerLevel = 0;

                if (s.playOnAwake) {
                    s.source.Play();
                }
            }
        }

        public void PlaySound(string name) {
            var s = Array.Find(sounds, sound => sound.name == name);
            if (s != null) {
                SetAudiosourceToOriginalVolume(name);
                s.source.time = 0f;
                s.source.Play();
            } else {
                print("Cannot find sound with name: " + name);
            }
        }

        public void PlaySoundAtProgressPoint(string name, float progress) {
            var s = Array.Find(sounds, sound => sound.name == name);
            if (s != null) {
                s.source.Stop();
                SetAudiosourceToOriginalVolume(name);
                s.source.Play();
                s.source.time = Utility.instance.Remap(progress, 0f, 1f, 0f, s.source.clip.length);
            } else {
                print("Cannot find sound with name: " + name);
            }
        }

        public void SetSoundToProgressPoint(string name, float progress) {
            var s = Array.Find(sounds, sound => sound.name == name);
            if (s != null) {
                s.source.time = Utility.instance.Remap(progress, 0f, 1f, 0f, s.source.clip.length);
            } else {
                print("Cannot find sound with name: " + name);
            }
        }

        public void PlaySound(string name, float pitchRange) {
            var s = Array.Find(sounds, sound => sound.name == name);
            if (s != null) {
                SetAudiosourceToOriginalVolume(name);
                s.source.time = 0f;
                s.source.pitch = GetOriginalPitch(name) + UnityEngine.Random.Range(-pitchRange, pitchRange);
                s.source.Play();
            } else {
                print("Cannot find sound with name: " + name);
            }
        }

        public void PlaySound(AudioClip clip, float volume = 1f) {
            oneShotSource.PlayOneShot(clip, volume);
        }

        public void PlayRandomSound(string soundContainerName, float volume) {
            PlayRandomSound(soundContainerName, volume, 0);
        }
        public void PlayRandomSound(string soundContainerName, float volume, float pitchRange) {
            PlayRandomSound(soundContainerName, volume, 1f - pitchRange, 1f + pitchRange);
        }
        public void PlayRandomSound(string soundContainerName, float volume, float pitchMin, float pitchMax) {
            var soundContainer = Array.Find(soundContainers, cont => cont.name == soundContainerName);
            if (soundContainer != null) {
                int randomIndex = UnityEngine.Random.Range(0, soundContainer.clips.Length);
                oneShotSource.pitch = UnityEngine.Random.Range(pitchMin, pitchMax);
                oneShotSource.PlayOneShot(soundContainer.clips[randomIndex], volume);
            } else {
                Debug.LogError("Cannot find sound container with name " + soundContainerName);
            }
        }
        public AudioSource GetAudioSource(string name) {
            var s = Array.Find(sounds, sound => sound.name == name);
            if (s != null) {
                return s.source;
            } else {
                return null;
            }
        }

        public GameObject Get3DSourceObject(string name) {
            var s = Array.Find(sounds, sound => sound.name == name);
            if (s != null) {
                if (s.threeDSound == true) {
                    return s.sourceObject;
                } else {
                    return null;
                }
            } else {
                Debug.LogError("Cannot find sound with name: " + name);
                return null;
            }
        }

        public float GetSoundDuration(string name) {
            var s = Array.Find(sounds, sound => sound.name == name);
            if (s != null) {
                return s.clip.length;
            } else {
                Debug.LogError("Cannot find sound with name: " + name);
                return 0f;
            }
        }

        public void SetAudiosourceToOriginalVolume(string name) {
            GetAudioSource(name).volume = GetOriginalVolume(name);
        }

        public float GetOriginalVolume(string name) {
            var sound = Array.Find(sounds, s => s.name == name);
            if (sound != null) {
                return sound.volume;
            } else {
                Debug.LogError("Cannot find sound with name: " + name + "\n Volume returned is 0f!");
                return 0f;
            }
        }

        public float GetOriginalPitch(string name) {
            var sound = Array.Find(sounds, s => s.name == name);
            if (sound != null) {
                return sound.pitch;
            } else {
                Debug.LogError("Cannot find sound with name: " + name + "\n Pitch returned is 0f!");
                return 0f;
            }
        }

        public string[] GetAllSoundNames() {
            string[] names = new string[sounds.Length];
            for (int i = 0; i < sounds.Length; i++) {
                names[i] = sounds[i].name;
            }
            return names;
        }

        #region Costum Audio Fade

        public void FadeOutAudioCostum(string soundName, float duration, bool stopPlayingOnFadeOutComplete = true) {
            StartCoroutine(FadeOutAudioCostumRoutine(soundName, duration, stopPlayingOnFadeOutComplete));
        }
        public IEnumerator FadeOutAudioCostumRoutine(string soundName, float duration, bool stopPlayingOnFadeOutComplete = true) {
            var source = GetAudioSource(soundName);
            int steps = Mathf.RoundToInt(duration / Time.fixedDeltaTime);
            float progress = 0f;
            float startingVolume = source.volume;
            float endVolume = 0f;
            float newVolume = 0f;
            for (int i = 0; i < steps; i++) {
                progress = Utility.instance.Remap(i, 0, steps - 1, 0f, 1f);
                newVolume = Mathf.Lerp(startingVolume, endVolume, progress);
                source.volume = newVolume;
                yield return new WaitForFixedUpdate();
            }
            if (stopPlayingOnFadeOutComplete) {
                source.Stop();
            }
        }

        public void FadeOutAudioCostum(string soundName, float duration, Action onComplete, bool stopPlayingOnFadeOutComplete = true) {
            StartCoroutine(FadeOutAudioCostumRoutine(soundName, duration, onComplete, stopPlayingOnFadeOutComplete));
        }
        public IEnumerator FadeOutAudioCostumRoutine(string soundName, float duration, Action onComplete, bool stopPlayingOnFadeOutComplete = true) {
            var source = GetAudioSource(soundName);
            int steps = Mathf.RoundToInt(duration / Time.fixedDeltaTime);
            float progress = 0f;
            float startingVolume = source.volume;
            float endVolume = 0f;
            float newVolume = 0f;
            for (int i = 0; i < steps; i++) {
                progress = Utility.instance.Remap(i, 0, steps - 1, 0f, 1f);
                newVolume = Mathf.Lerp(startingVolume, endVolume, progress);
                source.volume = newVolume;
                yield return new WaitForFixedUpdate();
            }
            onComplete.Invoke();
            if (stopPlayingOnFadeOutComplete) {
                source.Stop();
            }
        }

        public void FadeToAudioCostum(string soundName, float targetVolume, float duration, bool stopPlayingOnFadeOutComplete = true) {
            StartCoroutine(FadeToAudioCostumRoutine(soundName, targetVolume, duration, stopPlayingOnFadeOutComplete));
        }
        public IEnumerator FadeToAudioCostumRoutine(string soundName, float targetVolume, float duration, bool stopPlayingOnFadeOutComplete = true) {
            var source = GetAudioSource(soundName);
            int steps = Mathf.RoundToInt(duration / Time.fixedDeltaTime);
            float progress = 0f;
            float startingVolume = source.volume;
            float endVolume = targetVolume;
            float newVolume = 0f;
            for (int i = 0; i < steps; i++) {
                progress = Utility.instance.Remap(i, 0, steps - 1, 0f, 1f);
                newVolume = Mathf.Lerp(startingVolume, endVolume, progress);
                source.volume = newVolume;
                yield return new WaitForFixedUpdate();
            }
            if (stopPlayingOnFadeOutComplete) {
                source.Stop();
            }
        }

        public void FadeToAudioCostum(string soundName, float targetVolume, float duration, Action onComplete, bool stopPlayingOnFadeOutComplete = true) {
            StartCoroutine(FadeToAudioCostumRoutine(soundName, targetVolume, duration, onComplete, stopPlayingOnFadeOutComplete));
        }
        public IEnumerator FadeToAudioCostumRoutine(string soundName, float targetVolume, float duration, Action onComplete, bool stopPlayingOnFadeOutComplete = true) {
            var source = GetAudioSource(soundName);
            int steps = Mathf.RoundToInt(duration / Time.fixedDeltaTime);
            float progress = 0f;
            float startingVolume = source.volume;
            float endVolume = targetVolume;
            float newVolume = 0f;
            for (int i = 0; i < steps; i++) {
                progress = Utility.instance.Remap(i, 0, steps - 1, 0f, 1f);
                newVolume = Mathf.Lerp(startingVolume, endVolume, progress);
                source.volume = newVolume;
                yield return new WaitForFixedUpdate();
            }
            onComplete.Invoke();
            if (stopPlayingOnFadeOutComplete) {
                source.Stop();
            }
        }

        #endregion
    }
}