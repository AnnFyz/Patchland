using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    static SoundManager audio;
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip clip;
    private void Awake()
    {
        if(audio == null)
        {
            audio = this;
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        else
        {
            Destroy(this);
            return;
        }

        DontDestroyOnLoad(audio);
    }

    private void Start()
    {
        audioSource.clip = clip;
        audioSource.loop = true;
        audioSource.volume = 0.1f;
        audioSource.reverbZoneMix = 0.6f;
        audioSource.Play();
    }
}
