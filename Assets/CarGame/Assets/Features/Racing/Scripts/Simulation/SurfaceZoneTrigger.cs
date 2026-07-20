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
            Debug.Log ($"[Trigger] Что-то вошло в зону {zoneType}: {other.gameObject.name}");
            CarController car = other.GetComponent<CarController>();
            
            if (car == null)
            {
                car = other.GetComponentInParent<CarController>();
            }

            if (car == null)
            {
                Debug.LogWarning ($"[Trigger] Объект {other.gameObject.name} не имеет компонента CarController!");
                return;
            }

            if (car.Model == null)
            {
                Debug.LogError ($"[Trigger] У CarController на объекте {car.gameObject.name} отсутствует ссылка на Model!");
                return;
            }

            Debug.Log ($"[Trigger] Успешно применили модификатор {speedModifier} для {zoneType} на машине {car.gameObject.name}");

            switch (zoneType)
            {
                case ZoneType.Dirt:
                    car.SetSurfaceModifier (speedModifier); break;
                case ZoneType.SpeedBoost:
                    car.SetSurfaceModifier (speedModifier); break;
                case ZoneType.FinishLine:
                    car.Model.CompleteRace (); break;
            }

            if (zoneType == ZoneType.Dirt || zoneType == ZoneType.SpeedBoost)
            {
                car.SetSurfaceModifier (1f); // Сбрасываем в 1f при выезде напрямую у car
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (!TryGetComponent<CarController> (out var car)) car = other.GetComponentInParent<CarController> ();
            if (car == null || car.Model == null) return;

            if (zoneType == ZoneType.Dirt || zoneType == ZoneType.SpeedBoost)
            {
                Debug.Log ($"[Trigger] Машина покинула зону {zoneType}. Сброс модификатора на 1f");
                car.SetSurfaceModifier (1f);
            }
        }
    }
}
