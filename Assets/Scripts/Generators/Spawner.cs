using Morkwa.Test.Data;
using Morkwa.Test.Mechanics.Characters;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Zenject;

namespace Morkwa.Test.Mechanics
{
    public class Spawner : MonoBehaviour
    {
        private SettingGame _settings;
        private Game _game;

        private List<Vector3> _wallsPositions = new List<Vector3>();
        [HideInInspector] public List<Vector3> WallsList => _wallsPositions;
        [SerializeField] private NavMeshSurface _navMeshSurface;
        [SerializeField] private Transform _startPointGame;
        [SerializeField] private GameObject _playerPrefab;
        [SerializeField] private GameObject _startPointPrefab;
        [SerializeField] private GameObject _exitPrefab;
        [SerializeField] private GameObject _wallPrefab;
        [SerializeField] private GameObject[] _groundPrefabs;
        [SerializeField] private GameObject[] _obstaclePrefabs;
        [SerializeField] private GameObject[] _enemyPrefabs;

        [SerializeField] private string _nameFoldersForObstacle = "Obstacle";
        [SerializeField] private string _nameFoldersForEnemy = "Enemy";
        [SerializeField] private string _nameFoldersForGround = "Ground";
        [SerializeField] private string _nameFoldersForWalls = "Walls";

        private float _defaultHeightExit = 0.5f;
        private float _defaultHeight = 0.0f;

        private Transform _groundTransform;
        private Transform _outerWallsTransform;
        private Transform _wallsTransform;
        private Transform _enemiesTransform;

        private int _columnsCount;
        private int _rowsCount;
        private int _obstacleCount;
        private int _enemyCount;

        [Inject]
        public void Construct(SettingGame settingGame)
        {
            _settings = settingGame;
            _settings.Spawner = this;
        }

        private void OnEnable()
        {
            _game = _settings.Game;
        }

        public void SetGameConfiguration(Vector2 vector, int countObstacle, int countEnemy)
        {
            _columnsCount = (int)vector.x;
            _rowsCount = (int)vector.y;
            _obstacleCount = countObstacle;
            _enemyCount = countEnemy;

            CreateGameField();
        }

        public void CreateGameField()
        {
            FieldGeneration();
            CreateListWall();
            GeneratorRandomObjects(_wallsTransform, _nameFoldersForObstacle, _obstaclePrefabs, _obstacleCount, false);
            CreateNavMesh();
            CreatePlayerPrefab();
            GeneratorRandomObjects(_enemiesTransform, _nameFoldersForEnemy, _enemyPrefabs, _enemyCount, true);
            CreateStartPointPrefab();
            CreateExitPrefab();
        }

        private void FieldGeneration()
        {
            _groundTransform = new GameObject(_nameFoldersForGround).transform;
            _outerWallsTransform = new GameObject(_nameFoldersForWalls).transform;

            for (int x = 0; x < _columnsCount; x++)
            {
                for (int z = 0; z < _rowsCount; z++)
                {
                    GameObject toInstantiate = _groundPrefabs[Random.Range(0, _groundPrefabs.Length)];

                    if (x == 0 || x == _columnsCount - 1 || z == 0 || z == _rowsCount - 1)
                    {
                        if (z == 0)
                            Instantiate(_wallPrefab, new Vector3(x - 0.5f, _defaultHeight, z - 0.5f), Quaternion.AngleAxis(180, new Vector3(0, 1, 0))).transform.SetParent(_outerWallsTransform);
                        if (z == _rowsCount - 1)
                            Instantiate(_wallPrefab, new Vector3(x - 0.5f, _defaultHeight, z + 0.5f), Quaternion.AngleAxis(180, new Vector3(0, 1, 0))).transform.SetParent(_outerWallsTransform);
                        if (x == 0)
                            Instantiate(_wallPrefab, new Vector3(x - 0.5f, _defaultHeight, z - 0.5f), Quaternion.AngleAxis(90, new Vector3(0, 1, 0))).transform.SetParent(_outerWallsTransform);
                        if (x == _columnsCount - 1)
                            Instantiate(_wallPrefab, new Vector3(x + 0.5f, _defaultHeight, z + 0.5f), Quaternion.AngleAxis(-90, new Vector3(0, 1, 0))).transform.SetParent(_outerWallsTransform);

                    }

                    Instantiate(toInstantiate, new Vector3(x, 0, z), Quaternion.identity).transform.SetParent(_groundTransform);
                }
            }
        }

        private void ClearList()
        {
            _wallsPositions.Clear();
        }

        private void CreateListWall()
        {
            ClearList();

            for (int x = 1; x < _columnsCount - 1; x++)
            {
                for (int z = 1; z < _rowsCount - 1; z++)
                {
                    _wallsPositions.Add(new Vector3(x, 0, z));
                }
            }
        }

        private void CreateNavMesh()
        {
            _navMeshSurface.BuildNavMesh();
        }

        private Vector3 GetRandomPosition()
        {
            var randomIndex = Random.Range(0, _wallsPositions.Count);
            Vector3 randomPosition = _wallsPositions[randomIndex];
            _wallsPositions.RemoveAt(randomIndex);

            return randomPosition;
        }

        private void GeneratorRandomObjects(Transform parentTransform, string parentName, GameObject[] Prefabs, int count, bool isEnemy)
        {
            parentTransform = new GameObject(parentName).transform;

            for (int i = 0; i < count; i++)
            {
                var randomPos = GetRandomPosition();

                GameObject prefab = Prefabs[Random.Range(0, Prefabs.Length)];

                if (!isEnemy)
                    Instantiate(prefab, randomPos, Quaternion.identity).transform.SetParent(parentTransform);
                else
                    Instantiate(prefab, randomPos, Quaternion.identity).GetComponent<Enemy>().SetInfo(_game);
            }
        }

        private void CreateStartPointPrefab()
        {
            Instantiate(_startPointPrefab, _startPointGame);
        }

        private void CreatePlayerPrefab()
        {
            Instantiate(_playerPrefab, _startPointGame).GetComponent<Player>().SetInfo(_game);
        }

        private void CreateExitPrefab()
        {
            Instantiate(_exitPrefab, new Vector3(_columnsCount - 1, _defaultHeightExit, _rowsCount - 1), Quaternion.identity);
        }
    }
}