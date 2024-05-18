using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBasicController : MonoBehaviour
{
    private Rigidbody _rigidbody;
	private CapsuleCollider _capsuleCollider;

    [Header("References")]
    [SerializeField] private BasicInput _basicInput;
	public Transform CameraFollow;

    [Header("Movement")]
    [SerializeField] private float _movementMultiplier = 30.0f;
    [SerializeField] private float _rotationSpeedMultiplier = 180.0f;
    [SerializeField] private float _pitchSpeedMultiplier = 180.0f;
    [SerializeField] private float _playerLookInputLerpTime = 0.35f;
	[SerializeField] private Vector2 _pitchClamp = new Vector2(-45.0f, 45.0f);
    [SerializeField] private float _runMultiplier = 2.0f;

	[Header("Ground Check")]
	[SerializeField] private bool _isPlayerGrounded = true;
	[SerializeField] [Range(0.0f, 1.8f)] private float _groundCheckRadiusMultiplier = 0.9f;
	[SerializeField] [Range(-0.95f, 1.05f)] private float _groundCheckDistance = 0.05f;

	private RaycastHit _groundCheckHit = new RaycastHit();

	[Header("Gravity")]
	[SerializeField] private float _gravityFallCurrent = -100.0f;
	[SerializeField] private float _gravityFallMin = -100.0f;
	[SerializeField] private float _gravityFallMax = -500.0f;
	[SerializeField][Range(-5.0f, -35.0f)] private float _gravityFallIncrementAmount = -20.0f;
	[SerializeField] private float _gravityFallIncrementTime = -0.05f;
	[SerializeField] private float _playerFallTimer = -0.0f;
	[SerializeField] private float _gravity = -0.0f;


	private Vector3 _playerMoveInput = Vector3.zero;
	private Vector3 _playerLookInput = Vector3.zero;
	private Vector3 _previousPlayerLookInput = Vector3.zero;
	private float _cameraPitch = 0.0f;

	private void Awake()
	{
		_rigidbody = GetComponent<Rigidbody>();
		_capsuleCollider = GetComponent<CapsuleCollider>();
	}

	private void FixedUpdate()
	{
		_playerLookInput = GetLookInput();
		PlayerLook();
		PitchCamera();

		_playerMoveInput = GetMoveInput();
		_isPlayerGrounded = PlayerGroundCheck();
		_playerMoveInput.y = PlayerGravity();
		_playerMoveInput = PlayerRun();

		_playerMoveInput = PlayerMove();

		_rigidbody.AddRelativeForce(_playerMoveInput, ForceMode.Force);
	}

	private Vector3 GetMoveInput()
	{
		return new Vector3(_basicInput.MoveInput.x, 0.0f, _basicInput.MoveInput.y);
	}

	private Vector3 GetLookInput()
	{
		_previousPlayerLookInput = _playerLookInput;
		_playerLookInput = new Vector3(_basicInput.LookInput.x, (_basicInput.InverMouseY ? -_basicInput.LookInput.y : _basicInput.LookInput.y), 0.0f);
		return Vector3.Lerp(_previousPlayerLookInput, _playerLookInput * Time.deltaTime, _playerLookInputLerpTime);
	}

	private Vector3 PlayerMove()
	{
		Vector3 calculatedPlayerMovement = new Vector3(
			_playerMoveInput.x * _movementMultiplier * _rigidbody.mass, 
			_playerMoveInput.y * _rigidbody.mass, 
			_playerMoveInput.z * _movementMultiplier * _rigidbody.mass);

		return calculatedPlayerMovement;
	}

	private Vector3 PlayerRun()
	{
		Vector3 calculatedPlayerRunSpeed = _playerMoveInput;
		if (_basicInput.IsRunning)
		{
			calculatedPlayerRunSpeed.x *= _runMultiplier;
			calculatedPlayerRunSpeed.z *= _runMultiplier;
		}
		return calculatedPlayerRunSpeed;
	}

	private void PlayerLook()
	{
		_rigidbody.rotation = Quaternion.Euler(0.0f, _rigidbody.rotation.eulerAngles.y + (_playerLookInput.x * _rotationSpeedMultiplier), 0.0f);
	}

	private void PitchCamera()
	{
		Vector3 rotationValues = CameraFollow.rotation.eulerAngles;
		_cameraPitch += _playerLookInput.y * _pitchSpeedMultiplier;
		_cameraPitch = Mathf.Clamp(_cameraPitch, _pitchClamp.x, _pitchClamp.y);

		CameraFollow.rotation = Quaternion.Euler(_cameraPitch, rotationValues.y, rotationValues.z);
	}

	private bool PlayerGroundCheck()
	{
		float sphereCastRadius = _capsuleCollider.radius * _groundCheckRadiusMultiplier;
		float sphereCastTravelDistance = _capsuleCollider.bounds.extents.y - sphereCastRadius + _groundCheckDistance;
		return Physics.SphereCast(_rigidbody.position, sphereCastRadius, Vector3.down, out _groundCheckHit, sphereCastTravelDistance);
	}

	private float PlayerGravity()
	{
		if (_isPlayerGrounded)
		{
			_gravity = 0.0f;
			_gravityFallCurrent = _gravityFallMin;
		}
		else
		{
			_playerFallTimer -= Time.fixedDeltaTime;
			if (_playerFallTimer < 0.0f)
			{
				if (_gravityFallCurrent > _gravityFallMax)
				{
					_gravityFallCurrent += _gravityFallIncrementAmount;
				}

				_playerFallTimer = _gravityFallIncrementTime;
				_gravity = _gravityFallCurrent;
			}
		}

		return _gravity;
	}
}
