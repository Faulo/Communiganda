using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Communiganda {
    public class Utility : MonoBehaviour {
        public static Utility instance;

        void Awake() {
            instance = this;
        }

        #region Transforms

        public void MoveGameObject(Transform transform, Vector3 trgPos, float duration, AnimationCurve animationCurve = null) {
            StartCoroutine(MoveGameObjectRoutine(transform, trgPos, duration, animationCurve));
        }
        public IEnumerator MoveGameObjectRoutine(Transform transform, Vector3 trgPos, float duration, AnimationCurve animationCurve = null) {
            int steps = Mathf.RoundToInt(duration / Time.fixedDeltaTime);
            if (steps < 2) {
                steps = 2;
            }

            float progress = 0f;
            var startPos = transform.position;
            for (int i = 0; i < steps; i++) {
                progress = Remap(i, 0, steps - 1, 0f, 1f);
                if (animationCurve != null) {
                    progress = animationCurve.Evaluate(progress);
                }

                transform.position = Vector3.Lerp(startPos, trgPos, progress);
                yield return new WaitForFixedUpdate();
            }
        }
        public void MoveToWaypoints(Transform transform, float duration, AnimationCurve animationCurve, params Vector2[] points) {
            StartCoroutine(MoveToWaypointsRoutine(transform, duration, animationCurve, points));
        }
        public IEnumerator MoveToWaypointsRoutine(Transform transform, float duration, AnimationCurve animationCurve, params Vector2[] points) {
            for (int j = 0; j < points.Length; j++) {
                int steps = Mathf.RoundToInt(duration / Time.fixedDeltaTime);
                if (steps < 2) {
                    steps = 2;
                }

                float progress = 0f;
                var startPos = transform.position;
                for (int i = 0; i < steps; i++) {
                    progress = Remap(i, 0, steps - 1, 0f, 1f);
                    if (animationCurve != null) {
                        progress = animationCurve.Evaluate(progress);
                    }

                    transform.position = Vector3.Lerp(startPos, points[j], progress);
                    yield return new WaitForFixedUpdate();
                }
            }
        }

        public void RotateGameObject(Transform transform, Quaternion trgRotation, float duration, AnimationCurve animationCurve = null) {
            StartCoroutine(RotateGameObjectRoutine(transform, trgRotation, duration, animationCurve));
        }
        public IEnumerator RotateGameObjectRoutine(Transform transform, Quaternion trgRotation, float duration, AnimationCurve animationCurve) {
            int steps = Mathf.RoundToInt(duration / Time.fixedDeltaTime);
            if (steps < 2) {
                steps = 2;
            }

            float progress = 0f;
            var startRotation = transform.rotation;
            for (int i = 0; i < steps; i++) {
                progress = Remap(i, 0, steps - 1, 0f, 1f);
                if (animationCurve != null) {
                    progress = animationCurve.Evaluate(progress);
                }

                transform.rotation = Quaternion.Lerp(startRotation, trgRotation, progress);
                yield return new WaitForFixedUpdate();
            }
        }

        public void ScaleGameObject(Transform transform, Vector3 targetScale, float duration, AnimationCurve animationCurve = null) {
            StartCoroutine(ScaleGameObjectRoutine(transform, targetScale, duration, animationCurve));
        }
        public IEnumerator ScaleGameObjectRoutine(Transform transform, Vector3 targetScale, float duration, AnimationCurve animationCurve = null) {
            int steps = Mathf.RoundToInt(duration / Time.fixedDeltaTime);
            if (steps < 2) {
                steps = 2;
            }

            float progress = 0f;
            var startScale = transform.localScale;
            for (int i = 0; i < steps; i++) {
                progress = Remap(i, 0, steps - 1, 0f, 1f);
                if (animationCurve != null) {
                    progress = animationCurve.Evaluate(progress);
                }

                transform.localScale = Vector3.Lerp(startScale, targetScale, progress);
                yield return new WaitForFixedUpdate();
            }
        }

        #endregion

        #region Image/Sprite/Text
        public void SimpleAnimate(SpriteRenderer spriteRend, float intervall, params Sprite[] sprites) {
            StartCoroutine(SimpleAnimateCR(spriteRend, intervall, sprites));
        }
        IEnumerator SimpleAnimateCR(SpriteRenderer spriteRend, float intervall, params Sprite[] sprites) {
            int index = 0;
            while (true) {
                spriteRend.sprite = sprites[index];
                ++index;
                if (index > sprites.Length - 1) {
                    index = 0;
                }

                yield return new WaitForSeconds(intervall);
            }
        }

        public void LerpColor(Image image, Color targetColor, float duration, bool ignoreTimeScale, AnimationCurve animationCurve = null) {
            StartCoroutine(LerpColorRoutine(image, targetColor, duration, ignoreTimeScale, animationCurve));
        }
        public IEnumerator LerpColorRoutine(Image image, Color targetColor, float duration, bool ignoreTimeScale, AnimationCurve animationCurve = null) {
            image.gameObject.SetActive(true);
            image.enabled = true;
            var startingColor = image.color;
            float progress = 0f;
            int steps = Mathf.RoundToInt(duration / Time.fixedDeltaTime);

            if (ignoreTimeScale) {
                float increment = duration / steps;
                for (int i = 0; i < steps; i++) {
                    progress = Remap(i, 0, steps - 1, 0f, 1f);
                    if (animationCurve != null) {
                        progress = animationCurve.Evaluate(progress);
                    }

                    image.color = Color.Lerp(startingColor, targetColor, progress);
                    yield return new WaitForSecondsRealtime(increment);
                }
            } else {
                for (int i = 0; i < steps; i++) {
                    progress = Remap(i, 0, steps - 1, 0f, 1f);
                    if (animationCurve != null) {
                        progress = animationCurve.Evaluate(progress);
                    }

                    image.color = Color.Lerp(startingColor, targetColor, progress);
                    yield return new WaitForFixedUpdate();
                }
            }
        }

        public void LerpColor(SpriteRenderer rend, Color targetColor, float duration, bool ignoreTimeScale, AnimationCurve animationCurve = null) {
            StartCoroutine(LerpColorRoutine(rend, targetColor, duration, ignoreTimeScale, animationCurve));
        }
        public IEnumerator LerpColorRoutine(SpriteRenderer rend, Color targetColor, float duration, bool ignoreTimeScale, AnimationCurve animationCurve = null) {
            var startingColor = rend.color;
            float progress = 0f;
            int steps = Mathf.RoundToInt(duration / Time.fixedDeltaTime);
            float increment = duration / steps;
            if (ignoreTimeScale) {
                for (int i = 0; i < steps; i++) {
                    progress = Remap(i, 0, steps - 1, 0f, 1f);
                    if (animationCurve != null) {
                        progress = animationCurve.Evaluate(progress);
                    }

                    rend.color = Color.Lerp(startingColor, targetColor, progress);
                    yield return new WaitForSecondsRealtime(increment);
                }
            } else {
                for (int i = 0; i < steps; i++) {
                    progress = Remap(i, 0, steps - 1, 0f, 1f);
                    if (animationCurve != null) {
                        progress = animationCurve.Evaluate(progress);
                    }

                    rend.color = Color.Lerp(startingColor, targetColor, progress);
                    yield return new WaitForFixedUpdate();
                }
            }
        }

        public void LerpColor(TMPro.TextMeshProUGUI textMesh, Color targetColor, float duration, bool ignoreTimeScale, AnimationCurve animationCurve = null) {
            StartCoroutine(LerpColorRoutine(textMesh, targetColor, duration, ignoreTimeScale, animationCurve));
        }
        public IEnumerator LerpColorRoutine(TMPro.TextMeshProUGUI textMesh, Color targetColor, float duration, bool ignoreTimeScale, AnimationCurve animationCurve = null) {
            var startingColor = textMesh.color;
            float progress = 0f;
            int steps = Mathf.RoundToInt(duration / Time.fixedDeltaTime);
            if (ignoreTimeScale) {
                float increment = duration / steps;
                for (int i = 0; i < steps; i++) {
                    progress = Remap(i, 0, steps - 1, 0f, 1f);
                    if (animationCurve != null) {
                        progress = animationCurve.Evaluate(progress);
                    }

                    textMesh.color = Color.Lerp(startingColor, targetColor, progress);
                    yield return new WaitForSecondsRealtime(increment);
                }
            } else {
                for (int i = 0; i < steps; i++) {
                    progress = Remap(i, 0, steps - 1, 0f, 1f);
                    if (animationCurve != null) {
                        progress = animationCurve.Evaluate(progress);
                    }

                    textMesh.color = Color.Lerp(startingColor, targetColor, progress);
                    yield return new WaitForFixedUpdate();
                }
            }
        }

        public void LerpColor(TMPro.TextMeshPro textMesh, Color targetColor, float duration, bool ignoreTimeScale, AnimationCurve animationCurve = null) {
            StartCoroutine(LerpColorRoutine(textMesh, targetColor, duration, ignoreTimeScale, animationCurve));
        }
        public IEnumerator LerpColorRoutine(TMPro.TextMeshPro textMesh, Color targetColor, float duration, bool ignoreTimeScale, AnimationCurve animationCurve = null) {
            var startingColor = textMesh.color;
            float progress = 0f;
            int steps = Mathf.RoundToInt(duration / Time.fixedDeltaTime);

            if (ignoreTimeScale) {
                float increment = duration / steps;
                for (int i = 0; i < steps; i++) {
                    progress = Remap(i, 0, steps - 1, 0f, 1f);
                    if (animationCurve != null) {
                        progress = animationCurve.Evaluate(progress);
                    }

                    textMesh.color = Color.Lerp(startingColor, targetColor, progress);
                    yield return new WaitForSecondsRealtime(increment);
                }
            } else {
                for (int i = 0; i < steps; i++) {
                    progress = Remap(i, 0, steps - 1, 0f, 1f);
                    if (animationCurve != null) {
                        progress = animationCurve.Evaluate(progress);
                    }

                    textMesh.color = Color.Lerp(startingColor, targetColor, progress);
                    yield return new WaitForFixedUpdate();
                }
            }
        }
        #endregion

        #region Audio
        public void AdjustAudioVolume(AudioSource source, float targetVolume, float duration) {
            StartCoroutine(AdjustAudioVolumeCR(source, targetVolume, duration));
        }
        public IEnumerator AdjustAudioVolumeCR(AudioSource source, float targetVolume, float duration) {
            float startingValue = source.volume;
            int steps = Mathf.RoundToInt(duration / Time.fixedDeltaTime);
            float progress = 0f;
            for (int i = 0; i < steps; i++) {
                progress = Remap(i, 0, steps - 1, 0f, 1f);
                source.volume = Mathf.Lerp(startingValue, targetVolume, progress);
                yield return new WaitForFixedUpdate();
            }
            if (targetVolume == 0f) {
                source.Stop();
                source.volume = startingValue;
            }
        }

        public void AdjustAudioPitch(AudioSource source, float targetPitch, float duration) {
            StartCoroutine(AdjustAudioPitchCR(source, targetPitch, duration));
        }
        public IEnumerator AdjustAudioPitchCR(AudioSource source, float targetPitch, float duration) {
            float startingValue = source.pitch;
            float smoothness = 0.01f;
            float progress = 0;
            float increment = smoothness / duration;

            while (progress < 1) {
                source.pitch = Mathf.Lerp(startingValue, targetPitch, progress);
                progress += increment;
                yield return new WaitForSeconds(smoothness);
            }
        }

        #endregion

        #region SceneManagement
        public void TransitionToScene(string sceneName) {
            SceneManager.LoadScene(sceneName);
        }

        public void TransitionToScene(int sceneIndex) {
            SceneManager.LoadScene(sceneIndex);
        }

        public void TransitionToScene(string sceneName, float delayBeforeTrans = 0f) {
            StartCoroutine(SceneTransition(sceneName, delayBeforeTrans));
        }
        public void TransitionToScene(int sceneIndex, float delayBeforeTrans = 0f) {
            StartCoroutine(SceneTransition(sceneIndex, delayBeforeTrans));
        }
        IEnumerator SceneTransition(string sceneName, float delayBeforeTrans) {
            yield return new WaitForSeconds(delayBeforeTrans);
            SceneManager.LoadScene(sceneName);
        }
        IEnumerator SceneTransition(int sceneIndex, float delayBeforeTrans) {
            yield return new WaitForSeconds(delayBeforeTrans);
            SceneManager.LoadScene(sceneIndex);
        }
        #endregion

        #region GUI
        public void SetSelectable(GameObject newSelectable) {
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(newSelectable);
        }
        #endregion

        #region Other
        // Checks wether to points are in the same position with set tolerance. 
        public bool V3Equals(Vector3 pos1, Vector3 pos2, float tolerance = 0.01f) {
            if (Vector3.Distance(pos1, pos2) <= tolerance) {
                return true;
            } else {
                return false;
            }
        }
        public bool V3Equals(Transform pos1, Transform pos2, float tolerance = 0.01f) {
            if (Vector3.Distance(pos1.position, pos2.position) <= tolerance) {
                return true;
            } else {
                return false;
            }
        }

        public void ToggleObjectOnOff(params GameObject[] objects) {
            foreach (var gameObj in objects) {
                gameObj.SetActive(!gameObj.activeSelf);
            }
        }

        public float Remap(float value, float from1, float to1, float from2, float to2) {
            return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
        }

        public void Quit() {
            Application.Quit();
        }

        #endregion
    }
}