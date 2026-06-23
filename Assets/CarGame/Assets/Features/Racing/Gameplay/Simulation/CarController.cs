using Assets.CarGame.Assets.Features.Racing.Scripts.Data;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class CarController : MonoBehaviour
{
    [Header("Car Physics")]
    [SerializeField] private float currentSpeed;
    [SerializeField] private float currentTurnSpeed;

    private Rigidbody _rb;
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

    public void SetPhysicsSpeed(float forwardSpeed, float turnInput)
    {
        _forwardSpeed = forwardSpeed;
        _turnInput = turnInput;
    }

    private void FixedUpdate()
    {
        if (_rb == null || _model == null) return;

        if (Mathf.Abs(_turnInput) > 0.05f && Mathf.Abs(_forwardSpeed) > 0.1f)
        {
            float turnAmount = _turnInput * _model.MaxTurnSpeed * Time.fixedDeltaTime;
            if (_forwardSpeed < 0f) turnAmount *= -1f;
            Quaternion turnRotation = Quaternion.Euler(0f, turnAmount, 0f);
            _rb.MoveRotation(_rb.rotation * turnRotation);
        }

        Vector3 forwardVelocity = transform.forward * _forwardSpeed;
        _rb.velocity = new Vector3(forwardVelocity.x, _rb.velocity.y, forwardVelocity.z);
    }
}
