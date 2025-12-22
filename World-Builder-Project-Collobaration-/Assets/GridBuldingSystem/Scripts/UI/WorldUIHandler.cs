using UnityEngine;
using TMPro;
using System.Collections;

public class WorldUIHandler : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float maxDistance = 25f;
    [SerializeField] private float fadeDuration = 0.25f;

    [Header("Debug")]
    [SerializeField] private bool isVisible;
    [SerializeField] private float currentDistance;

    private Camera cam;
    private float maxDistanceSqr;

    TextMeshProUGUI UIText;

    private Coroutine fadeCoroutine;
    private bool isFadedIn;
    private bool isLifeTimeExpired;
    private void Awake()
    {
        UIText= GetComponentInChildren<TextMeshProUGUI>();
        cam = Camera.main;
        maxDistanceSqr = maxDistance * maxDistance;

        // start hidden
        UIText.alpha = 0f;
        isFadedIn = false;
    }

    private void Update()
    {
        if (cam == null) return;

        Vector3 camPos = cam.transform.position;

        // --- Efficient distance check (no sqrt) ---
        Vector3 diff = transform.position - camPos;
        float sqrDistance = diff.sqrMagnitude;

        currentDistance = Mathf.Sqrt(sqrDistance); // only for debug display

        bool isCloseEnough = sqrDistance <= maxDistanceSqr;

        // --- Viewport visibility check ---
        Vector3 viewportPos = cam.WorldToViewportPoint(transform.position);

        isVisible =
            viewportPos.z > 0f &&
            viewportPos.x >= 0f && viewportPos.x <= 1f &&
            viewportPos.y >= 0f && viewportPos.y <= 1f;

        bool shouldBeVisible = isCloseEnough && !isLifeTimeExpired; // && isVisible;

        if (shouldBeVisible && !isFadedIn)
        {
            StartFade(FadeIn());
        }
        else if (!shouldBeVisible && isFadedIn)
        {
            StartFade(FadeOut());
        }
    }

    public void SetText(string text, bool isLifeTimeExpired)
    {
        UIText.text = text;
        this.isLifeTimeExpired = isLifeTimeExpired;
    }

    private void StartFade(IEnumerator fadeRoutine)
    {
        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);

        fadeCoroutine = StartCoroutine(fadeRoutine);
    }

    private IEnumerator FadeIn()
    {
        isFadedIn = true;

        float startAlpha = UIText.alpha;
        float t = 0f;

        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            UIText.alpha = Mathf.Lerp(startAlpha, 1f, t / fadeDuration);
            yield return null;
        }

        UIText.alpha = 1f;
    }

    private IEnumerator FadeOut()
    {
        isFadedIn = false;

        float startAlpha = UIText.alpha;
        float t = 0f;

        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            UIText.alpha = Mathf.Lerp(startAlpha, 0f, t / fadeDuration);
            yield return null;
        }

        UIText.alpha = 0f;
    }
}


