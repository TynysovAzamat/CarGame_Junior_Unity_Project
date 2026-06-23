using UnityEngine;

namespace Assets.CarGame.Assets.Features.Racing.Gameplay.Simulation
{
    public class SurfaceZoneTrigger : MonoBehaviour
    {
        public enum ZoneType { Dirt, SpeedBoost, FinishLine}

        [Header("Zone Configuration")]
        [SerializeField] private ZoneType zoneType;

        [Tooltip("Dirt zone - 0.5 speed, SpeedBoost zone - 2x speed")]
        [SerializeField] private float speedModifier = 1f;

        private void OnTriggerEnter(Collider other)
        {
            CarController car = other.GetComponent<CarController>();
            
            if (car == null)
            {
                car = other.GetComponentInParent<CarController>();
            }

            if (car == null) return;

            if (car.Model == null)
            {
                return;
            }

            switch (zoneType)
            {
                case ZoneType.Dirt:
                    car.Model.ApplySurfaceModifier(speedModifier); break;

                case ZoneType.SpeedBoost:
                    car.Model.ApplySurfaceModifier(speedModifier); break;

                case ZoneType.FinishLine:
                    car.Model.CompleteRace();
                    break;
            }

        }

        private void OnTriggerExit(Collider other)
        {
            CarController car = other.GetComponent<CarController>();
            if (car == null || car.Model == null) return;

            if (zoneType == ZoneType.Dirt || zoneType == ZoneType.SpeedBoost)
            {
                car.Model.ApplySurfaceModifier(1f);
            }
        }
    }
}
