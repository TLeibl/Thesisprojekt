using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

//class to play background sounds in current scenario
public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource = null;
    //arachnophobia
    [SerializeField] private AudioSource cellarBackgroundSounds = null;
    //machine learning
    [SerializeField] private AudioSource machineBackgroundSounds = null;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void StopBackgroundMusic()
    {
        if ((NetworkingManager.Scenario)PhotonNetwork.CurrentRoom.CustomProperties["ChosenScenario"] == NetworkingManager.Scenario.Arachnophobia)
            cellarBackgroundSounds.Stop();
        else if ((NetworkingManager.Scenario)PhotonNetwork.CurrentRoom.CustomProperties["ChosenScenario"] == NetworkingManager.Scenario.MachineOperating)
            machineBackgroundSounds.Stop();
    }
}
