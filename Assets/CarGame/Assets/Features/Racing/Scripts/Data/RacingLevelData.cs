using UnityEngine;

namespace Assets.CarGame.Assets.Features.Racing.Scripts.Data
{
    [CreateAssetMenu(fileName = "RacingLevelData", menuName = "CarGame/Racing/LevelData", order = 1)]
    public class RacingLevelData : BaseLevelData
    {
        /// <summary>
        /// Характеристики карты(уровня)
        /// </summary>

        [Header("Racing Settings")]
        [SerializeField] private GameObject trackPrefab;
        [SerializeField] private float targetTime = 0f;

        // использование характеристик уровня в других класса
        public GameObject TrackPrefab => trackPrefab;
        public float TargetTime => targetTime;
    }
}
