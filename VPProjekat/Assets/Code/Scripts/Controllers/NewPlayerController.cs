using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class NewPlayerController : MonoBehaviour
{
	private InputActions inputActions;

    private new Rigidbody rigidbody;

	[Header("References")]
	[SerializeField] private Animator animator;

	[Header("Parameters")]
	[SerializeField] private float walkSpeed = 1.0f;
	[SerializeField] private float runSpeed = 1.0f;
	[SerializeField] private float jumpVelocity = 1.0f;
	[SerializeField] private float maxForce = 1.0f;
	[SerializeField] private float rotationSpeed = 1.0f;
	[SerializeField] private float lookSensitivity = 1.0f;

	private Vector2 moveInput = Vector2.zero;
	private Vector2 lookInput = Vector2.zero;
	private bool isGrounded = true;
	private bool isMoving = false;
	private bool isRunning = false;
	private float currentSpeed = 0.0f;

	// Animator hash fields
	private int moveAmountHash;
	private int isMovingHash;
	private int isRunningHash;

	private void Awake()
	{
		rigidbody = GetComponent<Rigidbody>();
	}

	private void Start()
	{
		Cursor.visible = false;
		Cursor.lockState = CursorLockMode.Locked;

		// setup
		currentSpeed = walkSpeed;

		moveAmountHash = Animator.StringToHash("MoveAmount");
		isMovingHash = Animator.StringToHash("IsMoving");
		isRunningHash = Animator.StringToHash("IsRunning");
	}

	private void Update()
	{
		Vector3 movement = new Vector3(moveInput.x, 0.0f, moveInput.y);
		float moveAmount = Mathf.Clamp01(Mathf.Abs(movement.x) + Mathf.Abs(movement.z));

		animator.SetFloat(moveAmountHash, moveAmount, 0.25f, Time.deltaTime);
		animator.SetBool(isMovingHash, isMoving);
		animator.SetBool(isRunningHash, isRunning);
	}

	private void FixedUpdate()
	{
		// Handling Translation of character
		Move();
	}

	private void LateUpdate()
	{
		if (!isMoving) return;

		// Handling rotation of character
		Rotate();
	}

	private void Move()
	{
		// Calculate Direction
		Transform cameraTransform = Camera.main.transform;
		Vector3 localDirection = new Vector3(moveInput.x, 0.0f, moveInput.y);
		Vector3 worldDirection = (cameraTransform.forward * localDirection.z + cameraTransform.right * localDirection.x).normalized;
		worldDirection.y = 0.0f;
		Vector3 targetVelocity = worldDirection * currentSpeed;

		// Calculate Velocity change
		Vector3 currentVelocity = rigidbody.velocity;
		Vector3 velocityChange = targetVelocity - currentVelocity;
		velocityChange.y = 0.0f; // Ensure to not change y velocity with movement logic

		// Clamp magnitude of velocity change to ensure it doesn't exceed maxForce
		velocityChange = Vector3.ClampMagnitude(velocityChange, maxForce);

		// Apply force
		rigidbody.AddForce(velocityChange, ForceMode.VelocityChange);
	}
	
	private void Rotate()
	{
		Vector3 input = new Vector3(moveInput.x, 0.0f, moveInput.y);
		Vector3 cam = Camera.main.transform.forward;
		Vector3 direction = Quaternion.LookRotation(new Vector3(cam.x, 0.0f, cam.z)) * input;
		Quaternion targetRotation = Quaternion.LookRotation(direction);
		transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
	}

	private void SetMove(InputAction.CallbackContext context)
	{
		moveInput = context.ReadValue<Vector2>();
		isMoving = !(moveInput == Vector2.zero);
	}

	private void SetLook(InputAction.CallbackContext context)
	{
		lookInput = context.ReadValue<Vector2>();
	}

	private void SetRun(InputAction.CallbackContext context)
	{
		isRunning = context.performed;
		currentSpeed = isRunning ? runSpeed : walkSpeed;
	}

	private void Jump(InputAction.CallbackContext context)
	{
		if (isGrounded)
		{
			Vector3 velocity = rigidbody.velocity;
			velocity.y = jumpVelocity;
			rigidbody.velocity = velocity;
		}
	}

	private void OnEnable()
	{
		// Activate basic actions map
		inputActions = new InputActions();
		inputActions.BasicActions.Enable();

		// Subscribe to move input
		inputActions.BasicActions.Move.performed += SetMove;
		inputActions.BasicActions.Move.canceled += SetMove;

		// Subscribe to look input
		inputActions.BasicActions.Look.performed += SetLook;
		inputActions.BasicActions.Look.canceled += SetLook;

		// Subscribe to run input
		inputActions.BasicActions.Run.performed += SetRun;
		inputActions.BasicActions.Run.canceled += SetRun;

		// Subscribe to jump input
		inputActions.BasicActions.Jump.performed += Jump;
	}

	private void OnDisable()
	{
		// Unsubscribe to move input
		inputActions.BasicActions.Move.performed -= SetMove;
		inputActions.BasicActions.Move.canceled -= SetMove;

		// Unsubscribe to look input
		inputActions.BasicActions.Look.performed -= SetLook;
		inputActions.BasicActions.Look.canceled -= SetLook;

		// Unsubscribe to run input
		inputActions.BasicActions.Run.performed -= SetRun;
		inputActions.BasicActions.Run.canceled -= SetRun;

		// Unsubscribe to jump input
		inputActions.BasicActions.Jump.performed -= Jump;

		// Deactivate basic actions map
		inputActions.BasicActions.Disable();
	}

	public void SetGroundedState(bool state)
	{
		isGrounded = state;
	}
}
