using Morkwa.Test.Mechanics.Audio;
using Morkwa.Test.OptionalVisual;
using UnityEngine;

namespace Morkwa.Test.Mechanics.Characters
{
    public class Player : MonoBehaviour
    {
        private Game _game;
        private AudioManager _audio;
        private float _speed;
        private float _turnCoeficent = 10;

        [SerializeField] private Rigidbody _body;
        [SerializeField] private Animator _animator;

        private Vector3 position;
        private Vector3 moveDirection;
        private float _distanceNoise = 0.2f;
        
        [SerializeField] private AudioClip _stepsSound;
        [SerializeField] private AudioSource _stepSource;

        public void SetInfo(Game game)
        {
            _game = game;
            _game.SetPlayer(this);
            _audio = _game.GetAudioManager();
            moveDirection = Vector3.zero;
            position = _body.position;

            SetConfigPlayer();
        }

        private void SetConfigPlayer()
        {
            _speed = _game.GameConfiguration.CommonSpeed;
        }

        private void FixedUpdate()
        {
            if (!_game.GetPlayGameStatus())
                return;

            Move();
            AddNoiseValue();
        }

        private void Move()
        {
            moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")) * _speed;

            if (moveDirection != Vector3.zero)
                SetAnimatorStatus(true);
            else
                SetAnimatorStatus(false);

            Vector3 direction = Vector3.RotateTowards(transform.forward, moveDirection, _speed * _turnCoeficent, 0.0f);
            transform.localRotation = Quaternion.LookRotation(direction);

            _body.MovePosition(_body.position + moveDirection * Time.fixedDeltaTime);
        }

        private void AddNoiseValue()
        {
            if (Vector3.Distance(position, _body.transform.position) > _distanceNoise)
            {
                _audio.AddNoiseValue();
                position = _body.transform.position;
            }
        }

        private void SetAnimatorStatus(bool isOn)
        {
            if (isOn)
            {
                _animator.SetInteger("State", 1);

                if (!_stepSource.isPlaying)
                {
                    _stepSource.clip = _stepsSound;
                    _stepSource.Play();
                }
            }
            else
            {
                _animator.SetInteger("State", 0);
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.TryGetComponent(out Enemy enemy))
            {
                _game.GameOver(false);
                moveDirection = Vector3.zero;
            }

            if (collision.gameObject.TryGetComponent(out Exit exit))
            {
                _game.GameOver(true);
            }
        }
    }
}