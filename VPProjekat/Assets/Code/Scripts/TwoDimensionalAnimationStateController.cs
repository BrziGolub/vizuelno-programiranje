using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TwoDimensionalAnimationStateController : MonoBehaviour
{
    private Animator animator;

	private float velocityZ = 0.0f;
	private float velocityX = 0.0f;

	private float maxWalkVelocity = 0.5f;
	private float maxRunVelocity = 2.0f;

	[SerializeField] private float acceleration = 1.9f;
	[SerializeField] private float deceleration = 2.0f;

	private int velocityZHash;
	private int velocityXHash;

	private void Start()
	{
		animator = GetComponent<Animator>();

		velocityZHash = Animator.StringToHash("velocityZ");
		velocityXHash = Animator.StringToHash("velocityX");
	}

	private void ChangeVelocity(bool forwardPressed, bool leftPressed, bool rightPressed, bool runPressed, float currentMaxVelocity)
	{
		// Acceleration
		if (forwardPressed && velocityZ < currentMaxVelocity)
		{
			velocityZ += Time.deltaTime * acceleration;
		}

		if (leftPressed && velocityX > -currentMaxVelocity)
		{
			velocityX -= Time.deltaTime * acceleration;
		}

		if (rightPressed && velocityX < currentMaxVelocity)
		{
			velocityX += Time.deltaTime * acceleration;
		}

		// Deceleration
		if (!forwardPressed && velocityZ > 0.0f)
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
		}
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
		bool forwardPressed = Input.GetKey(KeyCode.W);
		bool leftPressed = Input.GetKey(KeyCode.A);
		bool rightPressed = Input.GetKey(KeyCode.D);
		bool runPressed = Input.GetKey(KeyCode.LeftShift);

		float currentMaxVelocity = runPressed ? maxRunVelocity : maxWalkVelocity;

		ChangeVelocity(forwardPressed, leftPressed, rightPressed, runPressed, currentMaxVelocity);
		LockOrResetVelocity(forwardPressed, leftPressed, rightPressed, runPressed, currentMaxVelocity);

		animator.SetFloat(velocityZHash, velocityZ);
		animator.SetFloat(velocityXHash, velocityX);
	}
}
