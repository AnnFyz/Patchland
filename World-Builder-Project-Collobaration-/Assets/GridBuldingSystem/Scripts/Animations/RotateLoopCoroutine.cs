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

    private void Start()
    {
        rotateCoroutine = StartCoroutine(RotateLoop());
    }

    private void OnDisable()
    {
        if (rotateCoroutine != null)
            StopCoroutine(rotateCoroutine);
    }

    private IEnumerator RotateLoop()
    {
        //while (true)
        //{
        //    float elapsed = 0f;
        //    Quaternion startRot = transform.localRotation;
        //    Quaternion endRot = startRot * Quaternion.Euler(Vector3.up * 90f);

        //    while (elapsed < duration)
        //    {
        //        elapsed += Time.deltaTime;
        //        Debug.Log($"Rotating... t={elapsed / duration:F2}", this);
        //        float t = elapsed / duration;
        //        transform.localRotation = Quaternion.Slerp(startRot, endRot, t);
        //        yield return null;
        //    }

        //   // transform.localRotation = endRot;
        //}


        while (true)
        {
            float elapsed = 0f;

            Quaternion startRotation = transform.localRotation;

            Vector3 rotationEuler = new Vector3(
                rotateX ? 90f : 0f,
                rotateY ? 90f : 0f,
                rotateZ ? 90f : 0f
            );

            Quaternion endRotation = startRotation * Quaternion.Euler(rotationEuler);

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;
                transform.localRotation = Quaternion.Slerp(startRotation, endRotation, t);
                yield return null;
            }

            transform.localRotation = endRotation;
        }
    }
}
