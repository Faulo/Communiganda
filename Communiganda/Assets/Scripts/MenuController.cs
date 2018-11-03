using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MenuController : MonoBehaviour
{
    public TextMeshProUGUI titleText;
    public string titleTextString;
    public float titleTextDelay = 1f;
    public float titleTextIntervall = .1f;
    [Space]
    public TextMeshProUGUI anyKeyText;
    public AnimationCurve anyKeyFadeInOutCurve;
    [Space]
    public Image fadeInOutImage;
    public AnimationCurve fadeInAnimationCurve;
    public AnimationCurve fadeOutAnimationCurve;

    private bool canStartGame = false;
    Color transparent = new Color(1, 1, 1, 0);

    private void Awake()
    {
        anyKeyText.color = transparent;
        titleText.text = "";
        fadeInOutImage.color = Color.black;
        fadeInOutImage.enabled = true;
        titleText.fontSharedMaterial.DisableKeyword("GLOW_ON");

    }

    void Start()
    {
        StartCoroutine(Intro());
    }

    private void Update()
    {
        if (Input.anyKeyDown && canStartGame)
        {
            canStartGame = false;
            StopAllCoroutines();
            StartCoroutine(StartGame());
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            AudioManager.instance.PlayRandomSound("Mumble");
        }
    }

    private IEnumerator Intro()
    {
        AudioManager.instance.GetAudioSource("MusicMenu").volume = 0f;
        AudioManager.instance.FadeToAudioCostum("MusicMenu", AudioManager.instance.GetOriginalVolume("MusicMenu"), 2f, false);
        yield return StartCoroutine(Utility.instance.LerpColorRoutine(fadeInOutImage, transparent, titleTextDelay, false, fadeInAnimationCurve));
        for (int i = 0; i < titleTextString.Length; i++)
        {
            titleText.text += titleTextString[i];
            yield return new WaitForSeconds(titleTextIntervall);
        }
        yield return new WaitForSeconds(1f);
        canStartGame = true;
        while (true)
        {
            yield return StartCoroutine(Utility.instance.LerpColorRoutine(anyKeyText, Color.white, 1f, false, anyKeyFadeInOutCurve));
            yield return StartCoroutine(Utility.instance.LerpColorRoutine(anyKeyText, transparent, 1f, false, anyKeyFadeInOutCurve));
        }
    }

    private IEnumerator StartGame()
    {
        StartCoroutine(AnimateTextGlow());
        StartCoroutine(AudioManager.instance.FadeOutAudioCostumRoutine("MusicMenu", .5f));
        AudioManager.instance.PlaySound("StartGame");

        StartCoroutine(Utility.instance.LerpColorRoutine(anyKeyText, transparent, 1f, false, anyKeyFadeInOutCurve));
        StartCoroutine(Utility.instance.LerpColorRoutine(titleText, transparent, 1f, false, anyKeyFadeInOutCurve));
        yield return StartCoroutine(Utility.instance.LerpColorRoutine(fadeInOutImage, Color.black, titleTextDelay, false, fadeOutAnimationCurve));
        Utility.instance.TransitionToScene(1);
    }

    private IEnumerator AnimateTextGlow()
    {
        Material textMat = titleText.fontSharedMaterial;
        textMat.EnableKeyword("GLOW_ON");
        int steps = Mathf.RoundToInt(titleTextDelay / Time.fixedDeltaTime);
        if (steps < 2) steps = 2;
        float progress = 0f;
        for (int i = 0; i < steps; i++)
        {
            progress = Utility.instance.Remap(i, 0, steps - 1, 0f, 1f);
            float value = Mathf.Lerp(0, 1, progress);
            textMat.SetFloat("_GlowOffset", value);
            textMat.SetFloat("_GlowInner", value);
            textMat.SetFloat("_GlowOuter", value);
            yield return new WaitForFixedUpdate();
        }
    }
}
