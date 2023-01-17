using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.InputSystem;
#if UNITY_ANDROID
using UnityEngine.XR;
#endif


//Script used to control the animator of the VR character.
public class CharAC : MonoBehaviour
{
    //[SerializeField] private InputActionReference move; //use reference of the InputAction for walking
    [SerializeField] private Animator animator; //the VR character animator
    #if UNITY_ANDROID
    private InputDevice targetDevice; //the device used to get an input value (here: left hand controller)
    #endif
    private float currentMoveValue = 0.0f; //current movement value (Vector2 between x and y -1 to 1)


    //sounds
    private AudioSource audioSource = null; //current audio source
    [SerializeField] private AudioClip[] footstepSounds; // an array of footstep sounds that will be randomly selected from.


    private void Start()
    {
        //try and set left hand controller as target device (to enable/disable walk animation)
        TrySetTargetDevice();

        //set sound audio source
        audioSource = GetComponent<AudioSource>();
    }


    private void Update()
    {
        #if UNITY_ANDROID
        //if targetDevice is not valid yet - set it
        if (!targetDevice.isValid)
            TrySetTargetDevice();
        #endif

        UpdateWalkAnimation();
    }


    //enalbe/disable walk animation
    private void UpdateWalkAnimation()
    {
#if UNITY_ANDROID
        //get Vector2 value of left controller joystick input 
        targetDevice.TryGetFeatureValue(CommonUsages.primary2DAxis, out Vector2 joystickValue);

        //enable/disable Walk animation

        //check whether movement is forwards or backwards
        bool isWalkingForward = joystickValue.y > 0; //if greater zero bool is true = char should walk forward, else walk backwards
        bool isStandingStill = joystickValue.y == 0; //if zero bool is true = char is standing still

        if (isWalkingForward) //start animation and move forward if true
        {
            this.animator.SetBool("isWalking", true); //start animation
            this.animator.SetFloat("animSpeed", 1.0f); //set speed of animation
            PlayRandomSound(footstepSounds); //play footstep sounds
        }
        else if (isStandingStill) //stop animation if true
        {
            this.animator.SetBool("isWalking", false);
            this.animator.SetFloat("animSpeed", 0.0f);
        }
        else //start animation and move backwards
        {
            this.animator.SetBool("isWalking", true);
            this.animator.SetFloat("animSpeed", -1.0f);
            PlayRandomSound(footstepSounds); //sounds
        }
#endif
    }


    //method used to set the left hand controller as target device
    private void TrySetTargetDevice()
    {
#if UNITY_ANDROID
        //get all usable devices (headset and controllers) 
        List<InputDevice> devices = new List<InputDevice>();

        //get left hand controller out of devices because we only need input values for this one (for starting walking animation)
        InputDeviceCharacteristics leftControllerCharacteristics = InputDeviceCharacteristics.Left | InputDeviceCharacteristics.Controller;
        InputDevices.GetDevicesWithCharacteristics(leftControllerCharacteristics, devices);


        //if left controller not found - end method
        if (devices.Count == 0)
        {
            return;
        }

        //set left hand controller as target device
        targetDevice = devices[0];
#endif
    }


    //method called when a random sound out of a sound array is needed (e.g. when walking)
    private void PlayRandomSound(AudioClip[] array)
    {
        // pick & play a random footstep sound from the array,
        // excluding sound at index 0
        int n = Random.Range(1, array.Length);
        audioSource.clip = array[n];
        audioSource.PlayOneShot(audioSource.clip);
        // move picked sound to index 0 so it's not picked next time
        array[n] = array[0];
        array[0] = audioSource.clip;
    }
}
