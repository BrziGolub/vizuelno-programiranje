using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TwoDimensionalAnimationStateController : MonoBehaviour
{
	[Header("References")]
	[SerializeField] private BasicInput _basicInput;

    private Animator animator;

	private float velocityZ = 0.0f;
	private float velocityX = 0.0f;
	private bool isWalking = false;
	private bool isRunning = false;

	private float scaleVelocityZ = 0.0f;
	private float scaleVelocityX = 0.0f;

	private float maxWalkVelocity = 1.0f;
	private float maxRunVelocity = 2.0f;

	[Header("Speed")]
	[SerializeField] private float acceleration = 1.9f;
	[SerializeField] private float deceleration = 2.0f;

	public float AccelerationZ => acceleration * scaleVelocityZ;
	public float AccelerationX => acceleration * scaleVelocityX;

	private int velocityZHash;
	private int velocityXHash;
	private int isWalkingHash;
	private int isRunningHash;

	private void Start()
	{
		animator = GetComponent<Animator>();

		velocityZHash = Animator.StringToHash("velocityZ");
		velocityXHash = Animator.StringToHash("velocityX");
		isWalkingHash = Animator.StringToHash("isWalking");
		isRunningHash = Animator.StringToHash("isRunning");
	}

	private void ChangeVelocity(bool forwardPressed, bool leftPressed, bool rightPressed, bool runPressed, float currentMaxVelocity)
	{
		/*// Acceleration
		if (forwardPressed && velocityZ < currentMaxVelocity)
		{
			velocityZ += Time.deltaTime * AccelerationZ;
		}

		if (leftPressed && velocityX > -currentMaxVelocity)
		{
			velocityX += Time.deltaTime * AccelerationX;
		}

		if (rightPressed && velocityX < currentMaxVelocity)
		{
			velocityX += Time.deltaTime * AccelerationX;
		}*/
		velocityZ = scaleVelocityZ;
		velocityX = scaleVelocityX;

		// Deceleration
		/*if (!forwardPressed && velocityZ > 0.0f)
		{
			velocityZ -= Time.deltaTime * deceleration;
		}

		if (!leftPressed && velocityX < 0.0f)
		{
			velocityX += Time.deltaTime * deceleration;
		}

		if (!rightPressed && velocityX > 0.0f)
		{
			velocityX -= Time.deltaTime * deceleration;
		}*/
	}

	private void LockOrResetVelocity(bool forwardPressed, bool leftPressed, bool rightPressed, bool runPressed, float currentMaxVelocity)
	{
		if (!forwardPressed && velocityZ < 0.0f)
		{
			velocityZ = 0.0f;
		}

		if (!leftPressed && !rightPressed && velocityX != 0.0f && (velocityX > -0.01 && velocityX < 0.01f))
		{
			velocityX = 0.0f;
		}

		if (forwardPressed && runPressed && velocityZ > currentMaxVelocity)
		{
			velocityZ = currentMaxVelocity;
		}
		else if (forwardPressed && velocityZ > currentMaxVelocity)
		{
			velocityZ -= Time.deltaTime * deceleration;

			if (velocityZ > currentMaxVelocity && velocityZ < (currentMaxVelocity + 0.01f))
			{
				velocityZ = currentMaxVelocity;
			}
		}
		else if (forwardPressed && velocityZ < currentMaxVelocity && velocityZ > (currentMaxVelocity - 0.01f))
		{
			velocityZ = currentMaxVelocity;
		}

		if (rightPressed && runPressed && velocityX > currentMaxVelocity)
		{
			velocityX = currentMaxVelocity;
		}
		else if (rightPressed && velocityX > currentMaxVelocity)
		{
			velocityX -= Time.deltaTime * deceleration;

			if (velocityX > currentMaxVelocity && velocityX < (currentMaxVelocity + 0.01f))
			{
				velocityX = currentMaxVelocity;
			}
		}
		else if (rightPressed && velocityX < currentMaxVelocity && velocityX > (currentMaxVelocity - 0.01f))
		{
			velocityX = currentMaxVelocity;
		}

		if (leftPressed && runPressed && velocityX < -currentMaxVelocity)
		{
			velocityX = -currentMaxVelocity;
		}
		else if (leftPressed && velocityX < -currentMaxVelocity)
		{
			velocityX += Time.deltaTime * deceleration;

			if (velocityX < -currentMaxVelocity && velocityX > (-currentMaxVelocity - 0.01f))
			{
				velocityX = -currentMaxVelocity;
			}
		}
		else if (leftPressed && velocityX > -currentMaxVelocity && velocityX < (-currentMaxVelocity + 0.01f))
		{
			velocityX = -currentMaxVelocity;
		}
	}

	private void Update()
	{
		bool forwardPressed = _basicInput.MoveInput.y > 0.0f;
		bool leftPressed = _basicInput.MoveInput.x < 0.0f;
		bool rightPressed = _basicInput.MoveInput.x > 0.0f;
		bool runPressed = Input.GetKey(KeyCode.LeftShift);

		scaleVelocityZ = _basicInput.MoveInput.y;
		scaleVelocityX = _basicInput.MoveInput.x;

		isWalking = scaleVelocityZ != 0.0f || scaleVelocityX != 0.0f;
		isRunning = _basicInput.IsRunning;

		float currentMaxVelocity = runPressed ? maxRunVelocity : maxWalkVelocity;

		ChangeVelocity(forwardPressed, leftPressed, rightPressed, runPressed, currentMaxVelocity);
		LockOrResetVelocity(forwardPressed, leftPressed, rightPressed, runPressed, currentMaxVelocity);

		animator.SetBool(isWalkingHash, isWalking);
		animator.SetBool(isRunningHash, isRunning);
		animator.SetFloat(velocityZHash, scaleVelocityZ);
		animator.SetFloat(velocityXHash, scaleVelocityX);
	}
}
