using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationStateController : MonoBehaviour
{
    private Animator animator;

	private int velocityHash;

	private float velocity = 0.0f;

	[SerializeField] private float acceleration = 0.1f;
	[SerializeField] private float deceleration = 0.5f;

	private void Start()
	{
		animator = GetComponent<Animator>();

		velocityHash = Animator.StringToHash("velocity");
	}

	private void Update()
	{
		bool forwardPressed = Input.GetKey("w");
		bool runPressed = Input.GetKey("left shift");

		if (forwardPressed && velocity < 1.0f)
		{
			velocity += Time.deltaTime * acceleration;
		}

		if (!forwardPressed && velocity > 0.0f)
		{
			velocity -= Time.deltaTime * deceleration;
		}

		if (!forwardPressed && velocity < 0.0f)
		{
			velocity = 0.0f;
		}

		animator.SetFloat(velocityHash, velocity);
	}
}
