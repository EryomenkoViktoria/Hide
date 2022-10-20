using Morkwa.Test.Data;
using Morkwa.Test.UI;
using System.Collections;
using UnityEngine;
using Zenject;

namespace Morkwa.Test.Mechanics.Audio
{
    public class AudioManager : MonoBehaviour
    {
        private SettingGame _settings;

        private UIManager _uIManager;
        private Game _game;

        [SerializeField] private AudioClip _alertSound;
        [SerializeField] private AudioClip _winSound;
        [SerializeField] private AudioClip _loseSound;

        [SerializeField] private AudioSource _soundSource;
        [SerializeField] private float _defaultVolume = 1f;

        private float _timeUpdate = 0.2f;
        [SerializeField] private float _timeNoiseCoeficentAdd;
        [SerializeField] private float _timeNoiseCoeficentRemove;
        private float _transitionTimePatrolStatus;
        public float CurrentNoise { get; private set; }

        private float _noiseDetection;
        private float _noisePerSecond;
        private float _noiseReductionLevel;
        private bool _isAlertPlay;

        [Inject]
        public void Construct(SettingGame settingGame)
        {
            _settings = settingGame;
            _settings.AudioManager = this;
        }

        private void Awake()
        {
            _uIManager = _settings.UIManager;
            _game = _settings.Game;
        }

        private void Start()
        {
            SetConfigAudioNoise();
        }

        private void SetConfigAudioNoise()
        {
            _noiseDetection = _game.GameConfiguration.NoiseDetection;
            _noisePerSecond = _game.GameConfiguration.NoisePerSecond;
            _noiseReductionLevel = _game.GameConfiguration.NoiseReductionLevel;

            _timeNoiseCoeficentAdd = _noisePerSecond * _timeUpdate;
            _timeNoiseCoeficentRemove = (_noiseReductionLevel * _timeUpdate) / 10f;
            _transitionTimePatrolStatus = _game.GameConfiguration.TransitionTimePatrolStatus;
            _uIManager.SetNoiseDetected(_noiseDetection);
        }

        public void AddNoiseValue()
        {
            if (CurrentNoise <= _noiseDetection)
                CurrentNoise += _timeNoiseCoeficentAdd;
        }

        private void FixedUpdate()
        {
            Noise();
        }

        private void Noise()
        {
            if (CurrentNoise >= _noiseDetection)
            {
                PlaySoundAlert();
            }

            if (CurrentNoise >= 0)
                CurrentNoise -= _timeNoiseCoeficentRemove;

            _uIManager.SetValueNoiseSlider(CurrentNoise);
        }

        public void PlaySoundAlert()
        {
            if (!_isAlertPlay)
            {
                PlaySound(_alertSound);
                _isAlertPlay = true;
            }
        }

        public void PlaySoundFinish()
        {
            PlaySound(_winSound);
        }

        public void PlaySoundGameOver()
        {
            PlaySound(_loseSound);
        }

        private void PlaySound(AudioClip clip)
        {
            _soundSource.PlayOneShot(clip, _defaultVolume);
        }

        public float GetCurrectNoise()
        {
            return CurrentNoise;
        }

        public float GetNoiseDetection()
        {
            return _noiseDetection;
        }
    }
}