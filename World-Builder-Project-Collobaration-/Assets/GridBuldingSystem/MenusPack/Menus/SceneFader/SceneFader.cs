using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneFader : MonoBehaviour
{
    public Image img;
    public AnimationCurve curve;
    [SerializeField] float fadeInTime =  1f;
    [SerializeField] float fadeOutTime = 0;

    private void Start()
    {
        StartCoroutine(FadeIn());
    }
    IEnumerator FadeIn()
    {
        while(fadeInTime > 0)
        {
            fadeInTime -= Time.deltaTime;
            float a = curve.Evaluate(fadeInTime);
            img.color = new Color(191, 136, 110, a);
            yield return 0;
        }
    }
    public void FadeTo(int sceneIndex)
    {
        StartCoroutine(FadeOut(sceneIndex));
    }
    IEnumerator FadeOut(int sceneIndex)
    {
        while (fadeOutTime < 1)
        {
            fadeOutTime += Time.deltaTime * 2f;
            float a = curve.Evaluate(fadeOutTime);
            img.color = new Color(191, 136, 110, a);
            yield return 0;
        }

        SceneManager.LoadScene(sceneIndex);
    }
}
