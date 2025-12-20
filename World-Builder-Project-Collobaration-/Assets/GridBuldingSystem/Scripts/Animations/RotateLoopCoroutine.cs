using Sirenix.OdinInspector;
using System;
using System.Collections;
using UnityEngine;

public class RotateLoopCoroutine : MonoBehaviour
{

    [Flags]
    public enum AnimationAxis
    {
        None = 0,
        X = 1,
        Y = 2,
        Z = 4
    }


    [Header("Animation Settings")]
    [SerializeField] private AnimationAxis axes = AnimationAxis.Y;
    [SerializeField, Range(1, 10f), SuffixLabel("s", true)] private float duration = 2f; // seconds per full rotation
    private Coroutine rotateCoroutine;

    private bool rotateX => (axes & AnimationAxis.X) != 0;
    private bool rotateY => (axes & AnimationAxis.Y) != 0;
    private bool rotateZ => (axes & AnimationAxis.Z) != 0;

    private void Update()
    {
        if (axes == AnimationAxis.None) return;

        float dur = Mathf.Max(0.0001f, duration);
        float deltaDegrees = (360f / dur) * Time.deltaTime;

        Quaternion delta = Quaternion.identity;

        if (rotateX) delta = Quaternion.AngleAxis(deltaDegrees, Vector3.right) * delta;
        if (rotateY) delta = Quaternion.AngleAxis(deltaDegrees, Vector3.up) * delta;
        if (rotateZ) delta = Quaternion.AngleAxis(deltaDegrees, Vector3.forward) * delta;

        // ✅ world-space application
        transform.rotation = delta * transform.rotation;
    }
}
