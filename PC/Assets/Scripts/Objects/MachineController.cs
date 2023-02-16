using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class MachineController : MonoBehaviour
{
    //interactables
    [SerializeField] private GameObject stopButton = null;

    //events
    private int interacted = 0; //increased when interactable (e.g. button and lever) used - triggers events
    private int triggerEventAt = 0; //value at which event is triggered

    //warning lamp and materials
    [SerializeField] private GameObject warningLamp = null;
    [SerializeField] private Material warningLampOn = null;
    [SerializeField] private Material warningLampOff = null;
    private MeshRenderer lampRenderer = null;

    private bool warningLampEnabled = false;
    public bool WarningLampEnabled
    {
        get { return warningLampEnabled; }
        set { warningLampEnabled = value; }
    }


    private void Start()
    {
        lampRenderer = warningLamp.GetComponent<MeshRenderer>(); //set warning lamp renderer

        triggerEventAt = Random.Range(1, 15); //initialize triggerEventAt value
    }


    // Update is called once per frame
    private void Update()
    {
        if(interacted == triggerEventAt)
        {
            interacted = 0; //reset
            warningLampEnabled = true;
            LampSwitchOn(); //switch lamp on
            triggerEventAt = Random.Range(1, 15); //reset triggerEventAt value to random new value
        }
    }


    //method called by supervisor to start alarm
    [PunRPC]
    protected void StartAlarmManually()
    {
        LampSwitchOn(); //call method to switch on lamp
    }


    //method to switch lamp on = change material 
    private void LampSwitchOn()
    {
        StartCoroutine(LampBlinking());
    }


    //method to let lamp blink on and off
    private IEnumerator LampBlinking()
    {
        lampRenderer.material = warningLampOn;
        yield return new WaitForSeconds(4f);
        lampRenderer.material = warningLampOff;
        if(warningLampEnabled)
            StartCoroutine(LampBlinking());
    }


    //method to switch lamp off = change material - used by StopButton when pressed
    public void LampSwitchOff()
    {
        warningLampEnabled = false;
        lampRenderer.material = warningLampOff;
    }

    //called by buttons - if selected increase interacted value
    public void InteractedWithButton()
    {
        interacted += 1; 
    }
}
