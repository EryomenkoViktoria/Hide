using Morkwa.Test.Mechanics.AI;
using Morkwa.Test.Mechanics.Audio;
using UnityEngine;
using UnityEngine.AI;

namespace Morkwa.Test.Mechanics.Characters
{
    public class Enemy : MonoBehaviour, IEnemy
    {
        [SerializeField] private MeshRenderer _meshRenderer;
        [SerializeField] private NavMeshAgent _agent;
        [SerializeField] private Animator _animator;
        [SerializeField] private AIView _aIView;
        [SerializeField] private float _timeWait = 3f;
        [SerializeField] private Color32 _normalColor = Color.white;
        [SerializeField] private Color32 _hunterColor = Color.red;
        [SerializeField] private AudioClip _stepSound;
        [SerializeField] private AudioSource _stepSource;
        [SerializeField] private float _defaultVolume = 1f;
        [SerializeField] private StatusEnemy _currentStatusEnemy;

        private Game _game;
        private Spawner _spawner;
        private AudioManager _audio;
        private bool _patrolStatus;
        private bool _movingStatus;
        private Vector3 _bugPosition;
        private float _timeBug;
        private bool _isAlertPlay;
        private int _randomPosition;

        public void SetInfo(Game game)
        {
            _game = game;
            _game.SetEnemy(this);

            _spawner = _game.GetSpawner();
            _audio = _game.GetAudioManager();

            ChangeColor(_normalColor);
            SetNewStatusPatrol(true);

            SetSpeedEmeny(_game.GameConfiguration.CommonSpeed, _game.GameConfiguration.CommonAcceleration);
        }

        private void FixedUpdate()
        {
            if (!_game.GetPlayGameStatus())
                return;

            Patrol();
            HunterStatusActive();
        }

        private void ChangeColor(Color32 color)
        {
            _meshRenderer.material.color = color;
        }

        private void SetSpeedEmeny(float speed, float acceleration)
        {
            _agent.speed = speed;
            _agent.acceleration = acceleration;
        }

        private void Patrol()
        {
            if (!_patrolStatus)
                return;

            if (_movingStatus)
            {
                _agent.SetDestination(_spawner.WallsList[_randomPosition]);

                PlayStepSound();

                if (Vector3.Distance(transform.position, _spawner.WallsList[_randomPosition]) < 0.3f)
                {
                    _animator.SetInteger("State", 0);
                    _timeWait = Time.time;
                    _movingStatus = false;
                }

                if (Vector3.Distance(transform.position, _bugPosition) < 0.4f && Time.time - _timeBug > 1.5f)
                {
                    SetNewRandomPointForPatrol();
                    _timeBug = Time.time;
                }
            }
            else
            {
                if (Time.time - _timeWait > 2.0f)
                {
                    SetNewRandomPointForPatrol();
                    _movingStatus = true;
                    _bugPosition = transform.position;
                    _timeBug = Time.time;
                }
            }
        }

        private void HunterStatusActive()
        {
            if ((_audio.GetCurrectNoise() >= _audio.GetNoiseDetection() || _aIView.IsSeeing))
            {
                NewStatusEnemy(StatusEnemy.Hunter);

                _agent.SetDestination(_game.Player.transform.position);
                PlayStepSound();
            }

            if (!_isAlertPlay && _aIView.IsSeeing)
            {
                _isAlertPlay = true;
                _audio.PlaySoundAlert();
            }
        }

        private void NewStatusEnemy(StatusEnemy status)
        {
            if (_currentStatusEnemy == status)
                return;

            switch (status)
            {
                case StatusEnemy.Patrol:
                    ChangeColor(_normalColor);
                    SetNewStatusPatrol(true);

                    break;
                case StatusEnemy.Hunter:
                    ChangeColor(_hunterColor);
                    SetNewStatusPatrol(false);
                    _animator.SetInteger("State", 1);
                    break;
                default:
                    break;
            }

            _currentStatusEnemy = status;
        }

        private void SetNewStatusPatrol(bool isOn)
        {
            _patrolStatus = isOn;
        }

        private void PlayStepSound()
        {
            if (_stepSource.isPlaying)
                return;

            _stepSource.PlayOneShot(_stepSound, _defaultVolume);
        }

        private void SetNewRandomPointForPatrol()
        {
            _randomPosition = Random.Range(0, _spawner.WallsList.Count);
            _agent.SetDestination(_spawner.WallsList[_randomPosition]);
            _animator.SetInteger("State", 1);
        }
    }
}