using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationStateController : MonoBehaviour
{
    Animator animator;
	int isWalkingHash;
	int isRunningHash;

	private void Start()
	{
		animator = GetComponent<Animator>();
		isWalkingHash = Animator.StringToHash("isWalking");
		isRunningHash = Animator.StringToHash("isRunning");
	}

	private void Update()
	{
		bool isWalking = animator.GetBool(isWalkingHash);
		bool isRunning = animator.GetBool(isRunningHash);
		bool forwardPressed = Input.GetKey("w");
		bool runPressed = Input.GetKey("left shift");

		if (!isWalking && forwardPressed)
		{
			animator.SetBool(isWalkingHash, true);
		}
		
		if (isWalking && !forwardPressed)
		{
			animator.SetBool(isWalkingHash, false);
		}

		if (!isRunning && (forwardPressed && runPressed))
		{
			animator.SetBool(isRunningHash, true);
		}

		if (isRunning && (!forwardPressed || !runPressed))
		{
			animator.SetBool(isRunningHash, false);
		}
	}
}
