using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CustomButton : MonoBehaviour
{
    [SerializeField] private float threshold = 0.1f; //% of button press needed to activate button
    [SerializeField] private float deadZone = 0.025f; //prevent automatic press and release through bouncyness

    private bool isPressed = false; //button only pressed once
    private Vector3 startPos; //object to start button movement
    private ConfigurableJoint joint; //the joint on the object

    public UnityEvent onPressed, onReleased;


    private void Start()
    {
        //set components
        startPos = transform.localPosition;
        joint = GetComponent<ConfigurableJoint>();
    }


    private void Update()
    {
        //check if user has just pressed the button:
        //isPressed has not triggered before && if there is enough pressure on the button
        if (!isPressed && GetValue() + threshold >= 1)
            Pressed(); //if yes: call method

        //check if user has just released the button:
        //isPressed has been triggered and pressure is small enough
        if (isPressed && GetValue() - threshold <= 0)
            Released(); //if yes: call method
    }

    //method called when button is pressed.

    private void Pressed()
    {
        Debug.Log("Button pressed!");

        isPressed = true;
        onPressed.Invoke(); //invoke event
    }


    //method called when button is released.
    private void Released()
    {
        Debug.Log("Button released!");

        isPressed = false;
        onReleased.Invoke(); //invoke event
    }


    //method to check current pressure onto button.
    private float GetValue()
    {
        var value = Vector3.Distance(startPos, transform.localPosition) / joint.linearLimit.limit; //= space between starting pos of button and current pos

        if (Mathf.Abs(value) < deadZone) //if beyond limit of button - set to 0
            value = 0;

        return Mathf.Clamp(value, -1f, 1f); //clamp result to validate it
    }
}
