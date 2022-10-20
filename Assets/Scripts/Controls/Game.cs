using Morkwa.Test.Camera;
using Morkwa.Test.Data;
using Morkwa.Test.Mechanics.Audio;
using Morkwa.Test.Mechanics.Characters;
using Morkwa.Test.UI;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Morkwa.Test.Mechanics
{
    public enum StatusEnemy
    {
        None = 0,
        Patrol = 1,
        Hunter = 2
    }

    public class Game : MonoBehaviour, IGame
    {
        [SerializeField] private GameConfiguration _gameConfig;
        public GameConfiguration GameConfiguration => _gameConfig;
        private SettingGame _settings;
        private Spawner _spawner;
        private AudioManager _audioManager;
        private UIManager _uIManager;
        private CameraControl _cameraControl;
        private bool _isPlayGame;
        public Player Player { get; private set; }
        private List<Enemy> Enemies = new List<Enemy>();

        [Inject]
        public void Construct(SettingGame settingGame)
        {
            _settings = settingGame;
            _settings.Game = this;
        }

        private void Awake()
        {
            _spawner = _settings.Spawner;
            _audioManager = _settings.AudioManager;
            _uIManager = _settings.UIManager;
            _cameraControl = _settings.CameraControl;
        }

        private void Start()
        {
            ClearEnemiesList();
            _spawner.SetGameConfiguration(_gameConfig.SizeField, _gameConfig.CountObstacle, _gameConfig.CountEnemy);
            SetStatusActiveGame(true);
            SetTimeScale(false);
        }

        public void StartGame()
        {
            SetTimeScale(true);
        }

        private void SetStatusActiveGame(bool isOn)
        {

            _isPlayGame = isOn;
        }

        public void SetPlayer(Player player)
        {

            Player = player;
            _cameraControl.SetGameObject(Player.transform);
        }

        public void SetEnemy(Enemy enemy)
        {
            Enemies.Add(enemy);
        }

        private void ClearEnemiesList()
        {
            Enemies.Clear();
        }

        public Spawner GetSpawner()
        {
            return _spawner;
        }

        public Transform GetObjectAreFollowing()
        {
            return Player.transform;
        }

        public bool GetPlayGameStatus()
        {
            return _isPlayGame;
        }

        public AudioManager GetAudioManager()
        {
            return _audioManager;
        }

        public void GameOver(bool isWin)
        {
            SetStatusActiveGame(false);

            _uIManager.GameOver(isWin);

            if (isWin)
            {
                _audioManager.PlaySoundFinish();
            }
            else
            {
                _audioManager.PlaySoundGameOver();
            }

            SetTimeScale(false);
        }

        private void SetTimeScale(bool isPlay)
        {
            NewMethod(isPlay);
        }

        private static void NewMethod(bool isPlay)
        {
            if (isPlay)
                Time.timeScale = 1f;
            else
                Time.timeScale = 0f;
        }
    }
}