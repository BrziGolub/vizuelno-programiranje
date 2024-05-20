using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Animator _animator;
	private CapsuleCollider _capsuleCollider;

	[SerializeField] private BasicInput _basicsInput;

	[SerializeField] private float _rotationSpeed = 500.0f;

	public bool IsAnimationBusy = false;
	public Quaternion DesiredRotation;

	private bool _isCrouching = false;

	private void Start()
	{
		_animator = GetComponent<Animator>();
		_capsuleCollider = GetComponent<CapsuleCollider>();
	}

	private void Update()
	{
		// Move forward calculation based on input axiis
		Vector3 movement = new Vector3(_basicsInput.MoveInput.x, 0.0f, _basicsInput.MoveInput.y);
		float move_amount = Mathf.Clamp01(Mathf.Abs(movement.x) + Mathf.Abs(movement.z));

		_animator.SetFloat("MoveAmount", move_amount, 0.25f, Time.deltaTime);

		_animator.SetBool("IsRunning", _basicsInput.IsRunning);

		if (IsAnimationBusy) return;

		// Setting rotation of character
		if (move_amount > 0.0f)
		{
			Vector3 cam = Camera.main.transform.forward;
			movement = Quaternion.LookRotation(new Vector3(cam.x, 0.0f, cam.z)) * movement;

			float turn_angle = Mathf.Abs(Vector3.SignedAngle(transform.forward, movement, Vector3.up));

			if (turn_angle > 165.0f && !_basicsInput.IsCrouching)
			{
				if (_basicsInput.IsRunning)
				{
					_animator.CrossFade("Sprint Turn 180", 0.25f);
				}
                else
                {
					_animator.CrossFade("Walk Turn 180", 0.15f);
				}

				Vector3 anim_rotation = _animator.rootRotation.eulerAngles;
				DesiredRotation = Quaternion.Euler(new Vector3(anim_rotation.x, anim_rotation.y + 180, anim_rotation.z));
            }

			Quaternion target_rotation = Quaternion.LookRotation(movement);
			transform.rotation = Quaternion.RotateTowards(transform.rotation, target_rotation, _rotationSpeed * Time.deltaTime);
		}

		if (_basicsInput.IsCrouching && !_isCrouching)
		{
			if (_basicsInput.IsRunning)
			{
				_animator.CrossFade("Slide", 0.25f);

				Vector3 anim_rotation = _animator.rootRotation.eulerAngles;
				DesiredRotation = Quaternion.Euler(new Vector3(anim_rotation.x, anim_rotation.y, anim_rotation.z));

				_capsuleCollider.height = 1.75f;
				_capsuleCollider.center = new Vector3(0.0f, 0.875f, 0.0f);
			}
			else
			{
				_animator.CrossFade("Locomotion_Crouch", 0.25f);

				_capsuleCollider.height = 3;
				_capsuleCollider.radius = 0.77f;
				_capsuleCollider.center = new Vector3(0.0f, 1.5f, 0.0f);
			}

			_isCrouching = true;
		}
		else if (!_basicsInput.IsCrouching && _isCrouching)
		{
			_animator.CrossFade("Locomotion", 0.25f);
			_isCrouching = false;
			_capsuleCollider.height = 4;
			_capsuleCollider.radius = 0.5f;
			_capsuleCollider.center = new Vector3(0.0f, 2.0f, 0.0f);
		}
	}
}
