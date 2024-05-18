using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBasicController : MonoBehaviour
{
    private Rigidbody _rigidbody;

    [Header("References")]
    [SerializeField] private BasicInput _basicInput;
	public Transform CameraFollow;

    [Header("Movement")]
    [SerializeField] private float _movementMultiplier = 30.0f;
    [SerializeField] private float _rotationSpeedMultiplier = 180.0f;
    [SerializeField] private float _pitchSpeedMultiplier = 180.0f;
    [SerializeField] private float _playerLookInputLerpTime = 0.35f;
	[SerializeField] private Vector2 _pitchClamp = new Vector2(-45.0f, 45.0f);

	private Vector3 _playerMoveInput = Vector3.zero;
	private Vector3 _playerLookInput = Vector3.zero;
	private Vector3 _previousPlayerLookInput = Vector3.zero;
	private float _cameraPitch = 0.0f;

	private void Awake()
	{
		_rigidbody = GetComponent<Rigidbody>();
	}

	private void FixedUpdate()
	{
		_playerLookInput = GetLookInput();
		PlayerLook();
		PitchCamera();

		_playerMoveInput = GetMoveInput();
		PlayerMove();

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

	private void PlayerMove()
	{
		_playerMoveInput = new Vector3(
			_playerMoveInput.x * _movementMultiplier * _rigidbody.mass, 
			_playerMoveInput.y, 
			_playerMoveInput.z * _movementMultiplier * _rigidbody.mass);
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
}
