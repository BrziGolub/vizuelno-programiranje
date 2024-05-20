using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class BasicInput : MonoBehaviour
{
    private InputActions _inputActions;

    public Vector2 MoveInput { get; private set; } = Vector2.zero;
    public Vector2 LookInput { get; private set; } = Vector2.zero;

	public bool IsMoving { get; private set; } = false;
	public bool IsRunning { get; private set; } = false;
	public bool IsCrouching { get; private set; } = false;

	public bool InvertMouseY { get; private set; } = true;

	private void SetMove(InputAction.CallbackContext context)
	{
		MoveInput = context.ReadValue<Vector2>();
		IsMoving = !(MoveInput == Vector2.zero);
	}

	private void SetLook(InputAction.CallbackContext context)
	{
		LookInput = context.ReadValue<Vector2>();
	}

	private void SetRun(InputAction.CallbackContext context)
	{
		IsRunning = context.started;
	}

	private void SetCrouch(InputAction.CallbackContext context)
	{
		IsCrouching = !IsCrouching;
	}

	private void OnEnable()
	{
		_inputActions = new InputActions();
		_inputActions.BasicActions.Enable();

		_inputActions.BasicActions.Move.performed += SetMove;
		_inputActions.BasicActions.Move.canceled += SetMove;

		_inputActions.BasicActions.Look.performed += SetLook;
		_inputActions.BasicActions.Look.canceled += SetLook;

		_inputActions.BasicActions.Run.started += SetRun;
		_inputActions.BasicActions.Run.canceled += SetRun;

		_inputActions.BasicActions.Crouch.started += SetCrouch;
	}

	private void OnDisable()
	{
		_inputActions.BasicActions.Move.performed -= SetMove;
		_inputActions.BasicActions.Move.canceled -= SetMove;

		_inputActions.BasicActions.Look.performed -= SetLook;
		_inputActions.BasicActions.Look.canceled -= SetLook;

		_inputActions.BasicActions.Run.started -= SetRun;
		_inputActions.BasicActions.Run.canceled -= SetRun;

		_inputActions.BasicActions.Crouch.started -= SetCrouch;

		_inputActions.BasicActions.Disable();
	}
}
