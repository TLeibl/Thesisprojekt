using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CustomButton : MonoBehaviour
{
    private bool isPressed = false; //button only pressed once
    private Vector3 startPos; //button unpressed position

    public UnityEvent onPressed, onReleased;


    private void Start()
    {
        startPos = transform.position;
    }


    private void OnTriggerEnter(Collider other)
    {
        if (!isPressed)
        {
            Debug.Log("Button pressed");

            onPressed.Invoke();
            isPressed = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log("Button released");

        onReleased.Invoke();
        isPressed = false;
    }
}
