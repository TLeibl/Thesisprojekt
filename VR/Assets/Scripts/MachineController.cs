using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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


    //method to switch lamp on = change material 
    public void LampSwitchOn()
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
}
