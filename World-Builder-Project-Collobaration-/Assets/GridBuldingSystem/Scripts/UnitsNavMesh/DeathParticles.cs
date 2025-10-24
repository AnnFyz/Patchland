using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Unity.Collections.AllocatorManager;
using static UnityEngine.ParticleSystem;

public class DeathParticles : MonoBehaviour
{
    int oldBlockAmount = 0;
    int currentBlockAmount = 0;
    // Factory method to create and initialize a DeathParticles instance
    public static DeathParticles Create(Vector3 worldPosition, Transform parent, GameObject prefab, Quaternion rotation)
    {
        GameObject obj = Instantiate(prefab, worldPosition, rotation);
        DeathParticles deathParticles = obj.GetComponent<DeathParticles>();
        deathParticles.transform.SetParent(parent);
        parent.GetComponent<BlockPrefab>().OnBlockHeightChanged += deathParticles.UpdatePosition;
        return deathParticles;
    }

    private void UpdatePosition(int newHeight)
    {
        gameObject.GetComponent<ParticleSystem>().Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        transform.position = new Vector3(
               transform.position.x,
               transform.parent.position.y - BlockPrefab.Offset.y,
               transform.position.z
           );

        gameObject.GetComponent<ParticleSystem>().Play();
    }

    public void Play(AudioClip clip, float volume)
    {
        gameObject.GetComponent<ParticleSystem>().Play();
        if (clip == null) return;
        gameObject.AddComponent<AudioSource>().clip = clip;
        gameObject.GetComponent<AudioSource>().volume = volume;
        gameObject.GetComponent<AudioSource>().loop = false;
        gameObject.GetComponent<AudioSource>().Play();
    }

    void OnParticleSystemStopped()
    {
        GetComponentInParent<BlockPrefab>().OnBlockHeightChanged -= UpdatePosition;
        Destroy(gameObject);
    }
}
