using UnityEngine;
using UnityEngine.InputSystem;

public class RacingInputReader : MonoBehaviour
{
    [Header ("Dependencies")]
    [SerializeField] private Racing_Gameplay_View gameplayView;

    private InputAction _moveAction;
    private InputAction _turnAction;
    private bool _isInitialized = false;

    private void Start ()
    {
        if (InputSystem.actions != null)
        {
            var playerMap = InputSystem.actions.FindActionMap ("Player");
            if (playerMap != null)
            {
                _moveAction = playerMap.FindAction ("Move");
                _turnAction = playerMap.FindAction ("Turn");

                playerMap.Enable ();
                _isInitialized = true;
            }
            else
            {
                Debug.LogError ("(InputReader) Карта действий 'Player' не найдена в настройках проекта!");
            }
        }

        if (gameplayView == null)
        {
            gameplayView = GetComponent<Racing_Gameplay_View> ();
        }
    }

    private void FixedUpdate ()
    {
        if (!_isInitialized || gameplayView == null) return;

        float currentMove = 0f;
        float currentTurn = 0f;

        if (_moveAction != null)
        {
            currentMove = _moveAction.ReadValue<float> ();
        }

        if (_turnAction != null)
        {
            currentTurn = _turnAction.ReadValue<float> ();

            if (Gamepad.current != null)
            {
                bool leftPressed = Gamepad.current.dpad.left.isPressed;
                bool rightPressed = Gamepad.current.dpad.right.isPressed;

                if (leftPressed && rightPressed)
                {
                    currentTurn = 1f;
                }
                else if (leftPressed)
                {
                    currentTurn = -1f;
                }
                else if (rightPressed)
                {
                    currentTurn = 1f;
                }
            }
        }

        if (Gamepad.current != null && Mathf.Abs (currentMove) < 0.01f)
        {
            bool forwardPressed = Gamepad.current.rightTrigger.isPressed;
            bool backPressed = Gamepad.current.leftTrigger.isPressed;

            if (forwardPressed) currentMove = 1f;
            else if (backPressed) currentMove = -1f;
        }


        Vector2 combinedInput = new Vector2 (currentTurn, currentMove);

        gameplayView.HandleNewInput (combinedInput);
    }
}
