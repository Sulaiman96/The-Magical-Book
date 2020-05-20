using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.PlayerLoop;

public class PlayRandomSound : MonoBehaviour
{
    [SerializeField] private AudioClip[] audioClips;
    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void SetRandomClip()
    {
        audioSource.clip = audioClips[UnityEngine.Random.Range(0, audioClips.Length-1)];
        audioSource.PlayOneShot(audioSource.clip);
    }
}
