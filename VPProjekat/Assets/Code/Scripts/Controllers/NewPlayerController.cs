using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class NewPlayerController : MonoBehaviour
{
	private InputActions inputActions;

    private new Rigidbody rigidbody;
	private CapsuleCollider capsuleCollider;

	[Header("References")]
	[SerializeField] private Animator animator;

	[Header("Parameters")]
	[SerializeField] private float walkSpeed = 1.0f;
	[SerializeField] private float runSpeed = 1.0f;
	[SerializeField] private float slideSpeed = 1.0f;
	[SerializeField] private float jumpVelocity = 1.0f;
	[SerializeField] private float maxForce = 1.0f;
	[SerializeField] private float rotationSpeed = 1.0f;
	//[SerializeField] private float lookSensitivity = 1.0f;
	[SerializeField] private float gravityVelocity = 1.0f;
	[SerializeField] private float timeBeforeStartFalling = 1.0f;

	private Vector2 moveInput = Vector2.zero;
	private Vector2 lookInput = Vector2.zero;
	private bool isGrounded = true;
	private bool isMoving = false;
	private bool isRunning = false;
	private float currentSpeed = 0.0f;
	private bool isInAir = false;
	private float inAirTime = 0.0f;
	private bool isSliding = false;
	private Vector3 momentumDirection = Vector3.zero;

	// Power up
	public float speedMultiplier { get; private set; } = 1.0f;
	public float powerUpTimer { get; private set; } = 0.0f;
	public float outputSpeed { get; private set; }

	private bool canSlide => isMoving && isRunning && isGrounded;
	private bool canJump => isGrounded && !isSliding;
	private bool canMove => isMoving && !isSliding;
	private bool canRotate => canMove;

	// Animator hash fields
	private int moveAmountHash;
	private int isMovingHash;
	private int isRunningHash;
	private int isGroundedHash;

	private void Awake()
	{
		rigidbody = GetComponent<Rigidbody>();
		capsuleCollider = GetComponent<CapsuleCollider>();
	}

	private void Start()
	{
		Cursor.visible = false;
		Cursor.lockState = CursorLockMode.Locked;

		// setup
		currentSpeed = walkSpeed;
		slideSpeed = runSpeed;
		outputSpeed = 0.0f;

		moveAmountHash = Animator.StringToHash("MoveAmount");
		isMovingHash = Animator.StringToHash("IsMoving");
		isRunningHash = Animator.StringToHash("IsRunning");
		isGroundedHash = Animator.StringToHash("IsGrounded");
	}

	private void Update()
	{
		// Power up timer
		if (powerUpTimer > 0.0f)
		{
			powerUpTimer -= Time.deltaTime;
		}
		else if (powerUpTimer < 0.0f)
		{
			powerUpTimer = 0.0f;
			speedMultiplier = 1.0f;
		}

		// Counting time spent in air
		if (!isGrounded)
		{
			if (!isInAir && inAirTime > timeBeforeStartFalling)
			{
				// Falling animation transition
				animator.CrossFade("Jump Midair", 0.2f);
				isInAir = true;
			}

			inAirTime += Time.deltaTime;
		}

		Vector3 movement = new Vector3(moveInput.x, 0.0f, moveInput.y);
		float moveAmount = Mathf.Clamp01(Mathf.Abs(movement.x) + Mathf.Abs(movement.z));

		animator.SetFloat(moveAmountHash, moveAmount, 0.25f, Time.deltaTime);
		animator.SetBool(isMovingHash, isMoving);
		animator.SetBool(isRunningHash, isRunning);
		animator.SetBool(isGroundedHash, isGrounded);
	}

	private void FixedUpdate()
	{
		// Handling Translation of character
		AmplifyGravity();
		Move();
	}

	private void LateUpdate()
	{
		// Handling rotation of character
		Rotate();
	}

	private void AmplifyGravity()
	{
		Vector3 velocityChange = Vector3.down * gravityVelocity;
		rigidbody.AddForce(velocityChange, ForceMode.VelocityChange);
	}

	private void Move()
	{
		Vector3 direction = Vector3.zero;
		float speed = currentSpeed;

		if (canMove)
		{
			// Calculate move Direction
			Transform cameraTransform = Camera.main.transform;
			Vector3 localDirection = new Vector3(moveInput.x, 0.0f, moveInput.y);
			Vector3 worldDirection = (cameraTransform.forward * localDirection.z + cameraTransform.right * localDirection.x).normalized;
			worldDirection.y = 0.0f;
			direction = worldDirection;
			momentumDirection = worldDirection;
		}

		if (isSliding)
		{
			direction = momentumDirection;
			speed = slideSpeed;
		}

		outputSpeed = speed * speedMultiplier;
		Vector3 targetVelocity = direction * outputSpeed;

		if (direction == Vector3.zero) outputSpeed = 0.0f;

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
		if (!canRotate) return;

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
		if (canJump)
		{
			Vector3 velocity = rigidbody.velocity;
			velocity.y = jumpVelocity;
			rigidbody.velocity = velocity;

			isInAir = true;

			// Jump animation transition
			animator.CrossFade("Jump Up", 0.0f);
		}
	}

	private void Crouch(InputAction.CallbackContext context)
	{
		if (canSlide)
		{
			currentSpeed = runSpeed;

			animator.CrossFade("Slide", 0.2f);
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
		
		// Subscribe to crouch input
		inputActions.BasicActions.Crouch.performed += Crouch;
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

		// Unsubscribe to crouch input
		inputActions.BasicActions.Crouch.performed -= Crouch;

		// Deactivate basic actions map
		inputActions.BasicActions.Disable();
	}

	private void ResizeCollider()
	{
		if (isSliding)
		{
			capsuleCollider.center = new Vector3(0.0f, 1.0f, 0.0f);
			capsuleCollider.radius = 0.7f;
			capsuleCollider.height = 2.0f;
			//capsuleCollider.direction = 2;
			return;
		}

		capsuleCollider.center = new Vector3(0.0f, 2.0f, 0.0f);
		capsuleCollider.radius = 0.5f;
		capsuleCollider.height = 4.0f;
		//capsuleCollider.direction = 1;
	}

	public void SetGroundedState(bool state)
	{
		if (state && isGrounded != state)
		{
			inAirTime = 0.0f;
			isInAir = false;
		}

		isGrounded = state;
	}

	public void SetSlideState(bool state)
	{
		isSliding = state;

		if (isSliding) ResizeCollider();
		else ResizeCollider();
	}

	public void CollectPowerUp(float multiplier, float timer)
	{
		speedMultiplier = multiplier;
		powerUpTimer = timer;
	}
}
