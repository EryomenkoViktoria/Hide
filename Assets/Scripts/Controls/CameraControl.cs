using Morkwa.Test.Data;
using UnityEngine;
using Zenject;

namespace Morkwa.Test.Camera
{
    public class CameraControl : MonoBehaviour
    {
        private SettingGame _settings;

        private Transform _player;
        private Vector3 _defaultPosition;
        private bool _isPlayerEnable;

        [Inject]
        public void Construct(SettingGame settingGame)
        {
            _settings = settingGame;
            _settings.CameraControl = this;
        }

        public void SetGameObject(Transform gameObject)
        {
            _player = gameObject;

            SetDefaultPosition();
        }

        private void SetDefaultPosition()
        {
            _defaultPosition = transform.position - _player.position;
            _isPlayerEnable = true;
        }

        private void FixedUpdate()
        {
            if (!_isPlayerEnable)
                return;

            transform.position = _player.position + _defaultPosition;
        }
    }
}