using System.Collections;
using UnityEngine;
using DG.Tweening;

namespace Morkwa.Test.OptionalVisual
{
    public class Exit : MonoBehaviour
    {
        [SerializeField] private float _timeMoving = 1f;
        [SerializeField] private float _startHeight = 0.5f;
        [SerializeField] private float _endHeight = 1.5f;

        private void Start()
        {
            StartMoving();
        }

        private void StartMoving()
        {
            StartCoroutine(ExitMoving());
        }

        public void StopMoving()
        {
            StopCoroutine(ExitMoving());
        }

        private IEnumerator ExitMoving()
        {
            gameObject.transform.DOMoveY(_startHeight, _timeMoving);
            gameObject.transform.DORotate(new Vector3(0, 360, 0), 0.95f, RotateMode.FastBeyond360);
            yield return new WaitForSeconds(_timeMoving);
            gameObject.transform.DOMoveY(_endHeight, _timeMoving);
            gameObject.transform.DORotate(new Vector3(0, 360, 0), 0.95f, RotateMode.FastBeyond360);
            yield return new WaitForSeconds(_timeMoving);
            StartMoving();
        }
    }
}