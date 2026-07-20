using Assets.CarGame.Assets.Features.Racing.Scripts.Data;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class CarController : MonoBehaviour
{
    [Header("Car Physics")]
    [SerializeField] private float currentSpeed = 0f;
    [SerializeField] private float currentTurnSpeed = 0f;
    [SerializeField] private float _surfaceModifier = 1f;

    private Rigidbody _rb;
    private float _rawInputSpeed;
    private float _forwardSpeed;
    private float _turnInput;
    private Racing_Gameplay_Model _model;
    public Racing_Gameplay_Model Model => _model;

    public float CurrentSpeed => currentSpeed;
    public float CurrentTurnSpeed => currentTurnSpeed;
    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();


        if (_rb == null) return;

        _rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        _rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
    }

    public void InitializeStats(CarConfigData config)
    {
        if (config == null) return;

        currentSpeed = config.MaxSpeed;
        currentTurnSpeed = config.MaxTurnSpeed;
    }

    public void InjectModel(Racing_Gameplay_Model model)
    {
        if (model == null) return;

        _model = model; 
    }

    public void SetSurfaceModifier (float modifier)
    {
        _surfaceModifier = modifier;
        _forwardSpeed = _rawInputSpeed * _surfaceModifier;
    }

    public void SetPhysicsSpeed(float forwardSpeed, float turnInput)
    {
        _forwardSpeed = forwardSpeed * _surfaceModifier;
        _turnInput = turnInput;
        _forwardSpeed = _rawInputSpeed * _surfaceModifier;
    }

    public void SetVericalInput(float value)
    {
        _rawInputSpeed = value * currentSpeed;
        _forwardSpeed = _rawInputSpeed * _surfaceModifier;
    }

    public void SetHorizontalInput(float value)
    {
        _turnInput = value;
    }

    private void FixedUpdate()
    {
        if (_rb == null || _model == null) return;

        if (Mathf.Abs (_turnInput) > 0.05f)
        {
            float turnAmount = _turnInput * currentTurnSpeed * Time.fixedDeltaTime;
            if (_forwardSpeed < 0f) turnAmount *= -1f;
            Quaternion turnRotation = Quaternion.Euler (0f, turnAmount, 0f);
            _rb.MoveRotation (_rb.rotation * turnRotation);
        }

        if (Mathf.Abs (_forwardSpeed) < 0.05f)
        {
            _rb.velocity = new Vector3 (0f, _rb.velocity.y, 0f);
            _rb.angularVelocity = Vector3.zero;
        }
        else
        {
            Vector3 vector = transform.forward * _forwardSpeed;
            _rb.velocity = new Vector3 (vector.x, _rb.velocity.y, vector.z);
        }
    }
}
