using UnityEngine;

namespace Assets.CarGame.Assets.Features.Racing.Scripts.Data
{
    [CreateAssetMenu(fileName = "NewCarConfig", menuName = "CarGame/Racing/CarConfigData", order = 2)]
    public class CarConfigData : ScriptableObject
    {
        /// <summary>
        /// Характеристики машины
        /// </summary>
        
        [Header("Car Identification")]
        [SerializeField] private string carId;
        [SerializeField] private string carName;

        [Header("Visuals & Assets")]
        [SerializeField] private GameObject prefab;
        [SerializeField] private Sprite UiSprite;

        [Header("Car Performance")]
        [SerializeField] private float maxSpeed = 20f;
        [SerializeField] private float maxTurnSpeed = 180f;

        // использование характеристик машины в других класса
        public string CarId => carId;
        public string CarName => carName;
        public GameObject CarPrefab => prefab;
        public Sprite CarUiSprite => UiSprite;

        public float MaxSpeed => maxSpeed;
        public float MaxTurnSpeed => maxTurnSpeed;
    }
}
