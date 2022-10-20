using UnityEngine;

namespace Morkwa.Test.Data
{
    [CreateAssetMenu(fileName = "GameConfiguration", menuName = "ScriptableObject/GameConfiguration")]
    public class GameConfiguration : ScriptableObject
    {
        [Header("Game Field Settings")]
        [SerializeField] private Vector2 _sizeField;
        public Vector2 SizeField => _sizeField;

        [SerializeField, Range(0, 100)] private int _countObstacle;
        public int CountObstacle => _countObstacle;

        [Header("Charecters Settings")]
        [SerializeField] private float _commonSpeed;
        public float CommonSpeed => _commonSpeed;

        [SerializeField] private float _commonAcceleration;
        public float CommonAcceleration => _commonAcceleration;

        [SerializeField] private float _transitionTimePatrolStatus;
        public float TransitionTimePatrolStatus => _transitionTimePatrolStatus;

        [Header("Game Settings")]
        [SerializeField, Range(0, 35)] private int _countEnemy;
        public int CountEnemy => _countEnemy;

        [SerializeField] private float _noisePerSecond;
        public float NoisePerSecond => _noisePerSecond;

        [SerializeField] private float _noiseReductionLevel;
        public float NoiseReductionLevel => _noiseReductionLevel;

        [SerializeField] private float _noiseDetection;
        public float NoiseDetection => _noiseDetection;
    }
}