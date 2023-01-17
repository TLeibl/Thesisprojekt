using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

//class to play background sounds in current scenario
public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource = null;
    [SerializeField] private AudioSource ambienteSound = null;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void StopBackgroundMusic()
    {
        ambienteSound.Stop();
    }
}
