using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Zenject;
using Morkwa.Test.Data;
using TMPro;
using Morkwa.Test.Mechanics;

namespace Morkwa.Test.UI
{
    public class UIManager : MonoBehaviour
    {
        private SettingGame _settings;
        private Game _game;

        [SerializeField] private Slider _noiseSlider;
        [SerializeField] private GameObject _startPanel;
        [SerializeField] private GameObject _pausePanel;
        [SerializeField] private Button _playGame;
        [SerializeField] private Button _replayGame;
        [SerializeField] private Button _quitGame;
        [SerializeField] private TextMeshProUGUI _textGameOver;

        [SerializeField] private string _uWin = "You Win!!";
        [SerializeField] private string _uLoose = "You Loose!( ";
        [SerializeField] private int _gameSceneIndex = 0;

        [Inject]
        public void Construct(SettingGame settingGame)
        {
            _settings = settingGame;
            _settings.UIManager = this;
        }

        private void OnEnable()
        {
            _game = _settings.Game;

            _playGame.onClick.AddListener(PlayGame);
            _replayGame.onClick.AddListener(ReplayGame);
            _quitGame.onClick.AddListener(Quit);
        }

        private void OnDisable()
        {
            _playGame.onClick.RemoveListener(PlayGame);
            _replayGame.onClick.RemoveListener(ReplayGame);
            _quitGame.onClick.RemoveListener(Quit);
        }

        private void Start()
        {
            DefauleState();
        }

        private void DefauleState()
        {
            _startPanel.SetActive(true);
        }

        public void SetNoiseDetected(float value)
        {
            _noiseSlider.maxValue = value;
        }

        public void SetValueNoiseSlider(float value)
        {
            _noiseSlider.value = value;
        }

        public void GameOver(bool isWin)
        {
            OpenPausePanel(true);

            if (isWin)
            {
                _textGameOver.text = _uWin;
            }
            else
            {
                _textGameOver.text = _uLoose;
            }
        }

        public void OpenPausePanel(bool isOpen)
        {
            _pausePanel.SetActive(isOpen);
        }

        private void PlayGame()
        {
            _startPanel.SetActive(false);
            _game.StartGame();
        }

        private void ReplayGame()
        {
            SceneManager.LoadScene(_gameSceneIndex);
        }

        private void Quit()
        {
            Application.Quit();
        }
    }
}