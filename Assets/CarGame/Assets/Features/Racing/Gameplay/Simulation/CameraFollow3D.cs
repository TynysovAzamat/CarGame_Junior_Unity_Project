using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class CameraFollow3D : MonoBehaviour
{
    [Header("Follow Target")]
    [SerializeField] private Transform _target;
    [SerializeField] private CarController _controller;
    [SerializeField] private Camera _camera;

    [Header("Position Settings")]
    [SerializeField] private Vector3 _offset = new Vector3(0f, 5f, -10f);
    [SerializeField] private float _smoothSpeed = 0.125f;

    [Header("Collision Avoidance")]
    [SerializeField] private LayerMask _collisionLayers;
    [SerializeField] private float _cameraRadius;
    [SerializeField] private float _minDistanceOfTarget;

    [Header("Dynamic FOV Settings")]
    [SerializeField] private float _minFov;
    [SerializeField] private float _maxFov;
    [SerializeField] private float _minSpeed;
    [SerializeField] private float _maxSpeed;
    [SerializeField] private float _fovSmoothSpeed;

    private Vector3 _calculatedPosition;
    private void Start()
    {
        _camera = GetComponent<Camera>();
        if (_camera == null) _camera = Camera.main;
    }

    public void Init(Transform playerTransform, CarController carController)
    {
        _target = playerTransform;
        _controller = carController;
    }

    private void LateUpdate()
    {
        if (_target == null || _target.Equals(null))
        {
            _target = null;
            _controller = null;

            var carPhysics = Object.FindAnyObjectByType<CarController>();
            if (carPhysics != null)
            {
                _target = carPhysics.transform;
                Debug.Log("[Camera] Машина заспавнилась! Камера успешно захватила цель.");
            }
         
            return;
        }

        RayDistance();
        MovingCalculations();
    }

    private void MovingCalculations()
    {
        transform.position = Vector3.Lerp(transform.position, _calculatedPosition, _smoothSpeed * Time.deltaTime);

        transform.LookAt(_target.position + Vector3.up * 1f);

        if (_controller != null && _camera != null)
        {
            float speedPct = Mathf.InverseLerp(_minSpeed, _maxSpeed, _controller.CurrentSpeed);
            float targetFov = Mathf.Lerp(_minFov, _maxFov, speedPct);
            _camera.fieldOfView = Mathf.Lerp(_camera.fieldOfView, targetFov, _fovSmoothSpeed * Time.deltaTime);
        }
    }
    private void RayDistance()
    {
        Vector3 targetPosition = _target.position;
        Vector3 desiredPosition = targetPosition + _offset;

        Vector3 rayDirection = desiredPosition - targetPosition;
        float maxDistance = rayDirection.magnitude;
        rayDirection.Normalize();

        float finalDistance = maxDistance;

        if (Physics.SphereCast(targetPosition, _cameraRadius, rayDirection, out RaycastHit hit, maxDistance, _collisionLayers))
        {
            finalDistance = hit.distance;
        }

        finalDistance = Mathf.Max(finalDistance, _minDistanceOfTarget);

        _calculatedPosition = targetPosition + (rayDirection * finalDistance);
    }
}
