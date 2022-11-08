using System;
using UnityEngine;

/**
 * This class is used to control the VR character.
 * It is based on the OVRPlayerController script of the Oculus SDK 
 * and was adapted to the necessary functions for this application.
 */
public class VRCharController : MonoBehaviour
{
    private int currentFeedbackValue = 0; //current feedback value (can be 0-100)

	//Quit
	public KeyCode quitKey = KeyCode.Escape; //key to quit application

	//Movement
	public float Acceleration = 0.1f; //The rate acceleration during movement.
	public float Damping = 0.3f; //The rate of damping on movement.
	public float BackAndSideDampen = 0.5f; //The rate of additional damping when moving sideways or backwards.
	[Tooltip("How many fixed speeds to use with linear movement? 0=linear control")]
	public int FixedSpeedSteps; //How many fixed speeds to use with linear movement? 0=linear control
	public bool EnableLinearMovement = true; //When true, user input will be applied to linear movement. Set this to false whenever the player controller needs to ignore input for linear movement.

	//Rotation
	public float RotationAmount = 1.5f; //The rate of rotation when using a gamepad.
	public float RotationRatchet = 45.0f; //The rate of rotation when using the keyboard.
	[Tooltip("The player will rotate in fixed steps if Snap Rotation is enabled.")]
	public bool SnapRotation = true; //The player will rotate in fixed steps if Snap Rotation is enabled.
	public bool EnableRotation = true; //When true, user input will be applied to rotation. Set this to false whenever the player controller needs to ignore input for rotation.
	public bool RotationEitherThumbstick = false; //Rotation defaults to secondary thumbstick. You can allow either here. Note that this won't behave well if EnableLinearMovement is true.
	/// <summary>
	/// [Deprecated] When enabled, snap rotation will happen about the guardian rather
	/// than the player/camera viewpoint.
	/// </summary>
	[Tooltip("[Deprecated] When enabled, snap rotation will happen about the center of the " +
		"guardian rather than the center of the player/camera viewpoint. This (legacy) " +
		"option should be left off except for edge cases that require extreme behavioral " +
		"backwards compatibility.")]
	public bool RotateAroundGuardianCenter = false;

	//Hmd
	public bool HmdResetsY = true; //If true, reset the initial yaw of the player controller when the Hmd pose is recentered.
	public bool HmdRotatesY = true; //If true, tracking data from a child OVRCameraRig will update the direction of movement.

	//Modifies the strength of gravity.
	public float GravityModifier = 0.379f;

	// If true, each OVRPlayerController will use the player's physical height.
	public bool useProfileData = true;

	/// <summary>
	/// The CameraHeight is the actual height of the HMD and can be used to adjust the height of the character controller, which will affect the
	/// ability of the character to move into areas with a low ceiling.
	/// </summary>
	[NonSerialized]
	public float CameraHeight;

	/// <summary>
	/// This event is raised after the character controller is moved. This is used by the OVRAvatarLocomotion script to keep the avatar transform synchronized
	/// with the OVRPlayerController.
	/// </summary>
	public event Action<Transform> TransformUpdated;

	/// <summary>
	/// This bool is set to true whenever the player controller has been teleported. It is reset after every frame. Some systems, such as
	/// CharacterCameraConstraint, test this boolean in order to disable logic that moves the character controller immediately
	/// following the teleport.
	/// </summary>
	[NonSerialized] // This doesn't need to be visible in the inspector.
	public bool Teleported;

	/// <summary>
	/// This event is raised immediately after the camera transform has been updated, but before movement is updated.
	/// </summary>
	public event Action CameraUpdated;

	/// <summary>
	/// This event is raised right before the character controller is actually moved in order to provide other systems the opportunity to
	/// move the character controller in response to things other than user input, such as movement of the HMD. See CharacterCameraConstraint.cs
	/// for an example of this.
	/// </summary>
	public event Action PreCharacterMove;

	//General
	protected CharacterController Controller = null;
	protected OVRCameraRig CameraRig = null;

	private float MoveScale = 1.0f;
	private Vector3 MoveThrottle = Vector3.zero;
	private OVRPose? InitialPose;
	public float InitialYRotation { get; private set; }
	private float MoveScaleMultiplier = 1.0f;
	private float RotationScaleMultiplier = 1.0f;
	private bool SkipMouseRotation = true; // It is rare to want to use mouse movement in VR, so ignore the mouse by default.
	private bool HaltUpdateMovement = false;
	private bool prevHatLeft = false;
	private bool prevHatRight = false;
	private float SimulationRate = 60f;
	private float buttonRotation = 0f;
	private bool ReadyToSnapTurn; // Set to true when a snap turn has occurred, code requires one frame of centered thumbstick to enable another snap turn.
	private bool playerControllerEnabled = false;

	private void Start()
	{
		// Add eye-depth as a camera offset from the player controller
		var p = CameraRig.transform.localPosition;
		p.z = OVRManager.profile.eyeDepth;
		CameraRig.transform.localPosition = p;
	}

	private void Awake()
	{
		Controller = gameObject.GetComponent<CharacterController>();

		if (Controller == null)
			Debug.LogWarning("OVRPlayerController: No CharacterController attached.");

		//Use OVRCameraRig to set rotations to cameras and to be influenced by rotation
		OVRCameraRig[] CameraRigs = gameObject.GetComponentsInChildren<OVRCameraRig>();

		if (CameraRigs.Length == 0)
			Debug.LogWarning("OVRPlayerController: No OVRCameraRig attached.");
		else if (CameraRigs.Length > 1)
			Debug.LogWarning("OVRPlayerController: More then 1 OVRCameraRig attached.");
		else
			CameraRig = CameraRigs[0];

		InitialYRotation = transform.rotation.eulerAngles.y;
	}

	private void OnEnable()
	{
	}

	private void OnDisable()
	{
		if (playerControllerEnabled)
		{
			OVRManager.display.RecenteredPose -= ResetOrientation;

			if (CameraRig != null)
			{
				CameraRig.UpdatedAnchors -= UpdateTransform;
			}
			playerControllerEnabled = false;
		}
	}


	//--------------------------------------UPDATE--------------------------------------

	private void Update()
	{
		if (!playerControllerEnabled)
		{
			if (OVRManager.OVRManagerinitialized)
			{
				OVRManager.display.RecenteredPose += ResetOrientation;

				if (CameraRig != null)
				{
					CameraRig.UpdatedAnchors += UpdateTransform;
				}
				playerControllerEnabled = true;
			}
			else
				return;
		}


#if ENABLE_LEGACY_INPUT_MANAGER

		//Use keys to ratchet rotation
		if (Input.GetKeyDown(KeyCode.Q))
			buttonRotation -= RotationRatchet;

		if (Input.GetKeyDown(KeyCode.E))
			buttonRotation += RotationRatchet;
#endif


#if !UNITY_ANDROID || UNITY_EDITOR
		// Escape Application
		if (Input.GetKeyDown(quitKey))
			Application.Quit();
#endif
	}

	protected virtual void UpdateController()
	{
		if (useProfileData)
		{
			if (InitialPose == null)
			{
				// Save the initial pose so it can be recovered if useProfileData
				// is turned off later.
				InitialPose = new OVRPose()
				{
					position = CameraRig.transform.localPosition,
					orientation = CameraRig.transform.localRotation
				};
			}

			var p = CameraRig.transform.localPosition;
			if (OVRManager.instance.trackingOriginType == OVRManager.TrackingOrigin.EyeLevel)
			{
				p.y = OVRManager.profile.eyeHeight - (0.5f * Controller.height) + Controller.center.y;
			}
			else if (OVRManager.instance.trackingOriginType == OVRManager.TrackingOrigin.FloorLevel)
			{
				p.y = -(0.5f * Controller.height) + Controller.center.y;
			}
			CameraRig.transform.localPosition = p;
		}
		else if (InitialPose != null)
		{
			// Return to the initial pose if useProfileData was turned off at runtime
			CameraRig.transform.localPosition = InitialPose.Value.position;
			CameraRig.transform.localRotation = InitialPose.Value.orientation;
			InitialPose = null;
		}

		CameraHeight = CameraRig.centerEyeAnchor.localPosition.y;

		if (CameraUpdated != null)
		{
			CameraUpdated();
		}

		UpdateMovement();

		Vector3 moveDirection = Vector3.zero;

		float motorDamp = (1.0f + (Damping * SimulationRate * Time.deltaTime));

		MoveThrottle.x /= motorDamp;
		MoveThrottle.y = (MoveThrottle.y > 0.0f) ? (MoveThrottle.y / motorDamp) : MoveThrottle.y;
		MoveThrottle.z /= motorDamp;

		moveDirection += MoveThrottle * SimulationRate * Time.deltaTime;


		if (MoveThrottle.y <= transform.lossyScale.y * 0.001f)
		{
			// Offset correction for uneven ground
			float bumpUpOffset = Mathf.Max(Controller.stepOffset, new Vector3(moveDirection.x, 0, moveDirection.z).magnitude);
			moveDirection -= bumpUpOffset * Vector3.up;
		}

		if (PreCharacterMove != null)
		{
			PreCharacterMove();
			Teleported = false;
		}

		Vector3 predictedXZ = Vector3.Scale((Controller.transform.localPosition + moveDirection), new Vector3(1, 0, 1));

		// Move contoller
		Controller.Move(moveDirection);
		Vector3 actualXZ = Vector3.Scale(Controller.transform.localPosition, new Vector3(1, 0, 1));

		if (predictedXZ != actualXZ)
			MoveThrottle += (actualXZ - predictedXZ) / (SimulationRate * Time.deltaTime);
	}


	//--------------------------------------MOVEMENT AND GESTURES--------------------------------------

	//called in Update method to update movement when using the left thumbstick
	public virtual void UpdateMovement()
	{
#if ENABLE_LEGACY_INPUT_MANAGER
		if (HaltUpdateMovement)
			return;

		if (EnableLinearMovement)
		{
			bool moveForward = Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow);
			bool moveLeft = Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow);
			bool moveRight = Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow);
			bool moveBack = Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow);

			bool dpad_move = false;

			if (OVRInput.Get(OVRInput.Button.DpadUp))
			{
				moveForward = true;
				dpad_move = true;

			}

			if (OVRInput.Get(OVRInput.Button.DpadDown))
			{
				moveBack = true;
				dpad_move = true;
			}

			MoveScale = 1.0f;

			if ((moveForward && moveLeft) || (moveForward && moveRight) ||
				(moveBack && moveLeft) || (moveBack && moveRight))
				MoveScale = 0.70710678f;

			MoveScale *= SimulationRate * Time.deltaTime;

			//Compute this for key movement
			float moveInfluence = Acceleration * 0.1f * MoveScale * MoveScaleMultiplier;

			//Run
			if (dpad_move || Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
				moveInfluence *= 2.0f;

			Quaternion ort = transform.rotation;
			Vector3 ortEuler = ort.eulerAngles;
			ortEuler.z = ortEuler.x = 0f;
			ort = Quaternion.Euler(ortEuler);

			if (moveForward)
				MoveThrottle += ort * (transform.lossyScale.z * moveInfluence * Vector3.forward);
			if (moveBack)
				MoveThrottle += ort * (transform.lossyScale.z * moveInfluence * BackAndSideDampen * Vector3.back);
			if (moveLeft)
				MoveThrottle += ort * (transform.lossyScale.x * moveInfluence * BackAndSideDampen * Vector3.left);
			if (moveRight)
				MoveThrottle += ort * (transform.lossyScale.x * moveInfluence * BackAndSideDampen * Vector3.right);



			moveInfluence = Acceleration * 0.1f * MoveScale * MoveScaleMultiplier;

#if !UNITY_ANDROID // LeftTrigger not available on Android game pad
			moveInfluence *= 1.0f + OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger);
#endif

			Vector2 primaryAxis = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick);

			// If speed quantization is enabled, adjust the input to the number of fixed speed steps.
			if (FixedSpeedSteps > 0)
			{
				primaryAxis.y = Mathf.Round(primaryAxis.y * FixedSpeedSteps) / FixedSpeedSteps;
				primaryAxis.x = Mathf.Round(primaryAxis.x * FixedSpeedSteps) / FixedSpeedSteps;
			}

			if (primaryAxis.y > 0.0f)
				MoveThrottle += ort * (primaryAxis.y * transform.lossyScale.z * moveInfluence * Vector3.forward);

			if (primaryAxis.y < 0.0f)
				MoveThrottle += ort * (Mathf.Abs(primaryAxis.y) * transform.lossyScale.z * moveInfluence *
									   BackAndSideDampen * Vector3.back);

			if (primaryAxis.x < 0.0f)
				MoveThrottle += ort * (Mathf.Abs(primaryAxis.x) * transform.lossyScale.x * moveInfluence *
									   BackAndSideDampen * Vector3.left);

			if (primaryAxis.x > 0.0f)
				MoveThrottle += ort * (primaryAxis.x * transform.lossyScale.x * moveInfluence * BackAndSideDampen *
									   Vector3.right);
		}

		if (EnableRotation)
		{
			Vector3 euler = RotateAroundGuardianCenter ? transform.rotation.eulerAngles : Vector3.zero;
			float rotateInfluence = SimulationRate * Time.deltaTime * RotationAmount * RotationScaleMultiplier;

			bool curHatLeft = OVRInput.Get(OVRInput.Button.PrimaryShoulder);

			if (curHatLeft && !prevHatLeft)
				euler.y -= RotationRatchet;

			prevHatLeft = curHatLeft;

			bool curHatRight = OVRInput.Get(OVRInput.Button.SecondaryShoulder);

			if (curHatRight && !prevHatRight)
				euler.y += RotationRatchet;

			prevHatRight = curHatRight;

			euler.y += buttonRotation;
			buttonRotation = 0f;


#if !UNITY_ANDROID || UNITY_EDITOR
			if (!SkipMouseRotation)
				euler.y += Input.GetAxis("Mouse X") * rotateInfluence * 3.25f;
#endif

			if (SnapRotation)
			{
				if (OVRInput.Get(OVRInput.Button.SecondaryThumbstickLeft) ||
					(RotationEitherThumbstick && OVRInput.Get(OVRInput.Button.PrimaryThumbstickLeft)))
				{
					if (ReadyToSnapTurn)
					{
						euler.y -= RotationRatchet;
						ReadyToSnapTurn = false;
					}
				}
				else if (OVRInput.Get(OVRInput.Button.SecondaryThumbstickRight) ||
					(RotationEitherThumbstick && OVRInput.Get(OVRInput.Button.PrimaryThumbstickRight)))
				{
					if (ReadyToSnapTurn)
					{
						euler.y += RotationRatchet;
						ReadyToSnapTurn = false;
					}
				}
				else
				{
					ReadyToSnapTurn = true;
				}
			}
			else
			{
				Vector2 secondaryAxis = OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick);
				if (RotationEitherThumbstick)
				{
					Vector2 altSecondaryAxis = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick);
					if (secondaryAxis.sqrMagnitude < altSecondaryAxis.sqrMagnitude)
					{
						secondaryAxis = altSecondaryAxis;
					}
				}
				euler.y += secondaryAxis.x * rotateInfluence;
			}

			if (RotateAroundGuardianCenter)
			{
				transform.rotation = Quaternion.Euler(euler);
			}
			else
			{
				transform.RotateAround(CameraRig.centerEyeAnchor.position, Vector3.up, euler.y);
			}
		}
#endif
	}



	//--------------------------------------UPDATE--------------------------------------

	/// <summary>
	/// Invoked by OVRCameraRig's UpdatedAnchors callback. Allows the Hmd rotation to update the facing direction of the player.
	/// </summary>
	public void UpdateTransform(OVRCameraRig rig)
	{
		Transform root = CameraRig.trackingSpace;
		Transform centerEye = CameraRig.centerEyeAnchor;

		if (HmdRotatesY && !Teleported)
		{
			Vector3 prevPos = root.position;
			Quaternion prevRot = root.rotation;

			transform.rotation = Quaternion.Euler(0.0f, centerEye.rotation.eulerAngles.y, 0.0f);

			root.position = prevPos;
			root.rotation = prevRot;
		}

		UpdateController();
		if (TransformUpdated != null)
		{
			TransformUpdated(root);
		}
	}

	/// <summary>
	/// Stop this instance.
	/// </summary>
	public void Stop()
	{
		Controller.Move(Vector3.zero);
		MoveThrottle = Vector3.zero;
	}


	//method to update the feedback value via right trigger button
	private void UpdateFeedback()
	{
		//TODO GameManager-Methoden nutzen!
	}



	//--------------------------------------GETTER & SETTER--------------------------------------

	/// <summary>
	/// Gets the move scale multiplier.
	/// </summary>
	/// <param name="moveScaleMultiplier">Move scale multiplier.</param>
	public void GetMoveScaleMultiplier(ref float moveScaleMultiplier)
	{
		moveScaleMultiplier = MoveScaleMultiplier;
	}

	/// <summary>
	/// Sets the move scale multiplier.
	/// </summary>
	/// <param name="moveScaleMultiplier">Move scale multiplier.</param>
	public void SetMoveScaleMultiplier(float moveScaleMultiplier)
	{
		MoveScaleMultiplier = moveScaleMultiplier;
	}

	/// <summary>
	/// Gets the rotation scale multiplier.
	/// </summary>
	/// <param name="rotationScaleMultiplier">Rotation scale multiplier.</param>
	public void GetRotationScaleMultiplier(ref float rotationScaleMultiplier)
	{
		rotationScaleMultiplier = RotationScaleMultiplier;
	}

	/// <summary>
	/// Sets the rotation scale multiplier.
	/// </summary>
	/// <param name="rotationScaleMultiplier">Rotation scale multiplier.</param>
	public void SetRotationScaleMultiplier(float rotationScaleMultiplier)
	{
		RotationScaleMultiplier = rotationScaleMultiplier;
	}

	/// <summary>
	/// Gets the allow mouse rotation.
	/// </summary>
	/// <param name="skipMouseRotation">Allow mouse rotation.</param>
	public void GetSkipMouseRotation(ref bool skipMouseRotation)
	{
		skipMouseRotation = SkipMouseRotation;
	}

	/// <summary>
	/// Sets the allow mouse rotation.
	/// </summary>
	/// <param name="skipMouseRotation">If set to <c>true</c> allow mouse rotation.</param>
	public void SetSkipMouseRotation(bool skipMouseRotation)
	{
		SkipMouseRotation = skipMouseRotation;
	}

	/// <summary>
	/// Gets the halt update movement.
	/// </summary>
	/// <param name="haltUpdateMovement">Halt update movement.</param>
	public void GetHaltUpdateMovement(ref bool haltUpdateMovement)
	{
		haltUpdateMovement = HaltUpdateMovement;
	}

	/// <summary>
	/// Sets the halt update movement.
	/// </summary>
	/// <param name="haltUpdateMovement">If set to <c>true</c> halt update movement.</param>
	public void SetHaltUpdateMovement(bool haltUpdateMovement)
	{
		HaltUpdateMovement = haltUpdateMovement;
	}

	/// <summary>
	/// Resets the player look rotation when the device orientation is reset.
	/// </summary>
	public void ResetOrientation()
	{
		if (HmdResetsY && !HmdRotatesY)
		{
			Vector3 euler = transform.rotation.eulerAngles;
			euler.y = InitialYRotation;
			transform.rotation = Quaternion.Euler(euler);
		}
	}
}
