using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class DrivingCommands : MonoBehaviour
{
   
}

public class MoveCommand : ICommandInputs
{
    private readonly Action<float> _setVerticalInput;
    private readonly InputAction.CallbackContext _context;


    public MoveCommand(Action<float> setVerticalInput, InputAction.CallbackContext context)
    {
        _setVerticalInput = setVerticalInput;
        _context = context;
    }

    public void Execute()
    {
        float value = _context.ReadValue<float>();
        _setVerticalInput?.Invoke(value);
    }
}

public class TurnCommand : ICommandInputs
{
    private readonly Action<float> _setHorizontalInput;
    private readonly InputAction.CallbackContext _context;
    public TurnCommand(Action<float> setHorizontalInput, InputAction.CallbackContext context)
    {
        _setHorizontalInput = setHorizontalInput;
        _context = context;
    }
    public void Execute()
    {
        float value = _context.ReadValue<float>();
        _setHorizontalInput?.Invoke(value);
    }
}
