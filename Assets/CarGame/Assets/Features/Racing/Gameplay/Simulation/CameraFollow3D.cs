using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class CameraFollow3D : MonoBehaviour
{
    [Header("Follow Settings")]
    [SerializeField] private Vector3 offset = new Vector3(0f, 5f, -10f);
    [SerializeField] private float smoothSpeed = 0.125f;

    private Transform _target;
    private void LateUpdate()
    {
        if (_target == null)
        {
            var carPhysics = Object.FindAnyObjectByType<CarController>();
            if (carPhysics != null)
            {
                _target = carPhysics.transform;
                Debug.Log("[Camera] Машина заспавнилась! Камера успешно захватила цель.");
            }
         
            return;
        }

        
        Vector3 desiredPosition = _target.position + offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPosition;

        transform.LookAt(_target.position + Vector3.up * 1f);
    }
}
