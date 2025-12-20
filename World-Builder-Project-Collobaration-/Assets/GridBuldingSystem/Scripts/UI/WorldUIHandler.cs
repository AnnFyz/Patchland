using UnityEngine;
using TMPro;

public class WorldUIHandler : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float maxDistance = 25f;

    [Header("Debug")]
    [SerializeField] private bool isVisible;
    [SerializeField] private float currentDistance;

    private Camera cam;
    private float maxDistanceSqr;

    [SerializeField] TextMeshProUGUI UITextMeshPro;
    private void Awake()
    {
        UITextMeshPro= GetComponentInChildren<TextMeshProUGUI>();
        cam = Camera.main;
        maxDistanceSqr = maxDistance * maxDistance;
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

        // --- Example usage ---
        if (isCloseEnough && isVisible)
        {
            // Object is close AND visible
            Debug.DrawLine(camPos, transform.position, Color.green);
            UITextMeshPro.alpha = 1f;
        }
        else
        {
            Debug.DrawLine(camPos, transform.position, Color.red);
            UITextMeshPro.alpha = 0f;
        }
    }
}

