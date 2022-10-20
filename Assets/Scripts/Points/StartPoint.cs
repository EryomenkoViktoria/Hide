using System.Collections;
using UnityEngine;
using DG.Tweening;

namespace Morkwa.Test.OptionalVisual
{
    public class StartPoint : MonoBehaviour
    {
        [SerializeField] private float _timeOpenPoint = 1f;
        [SerializeField] private float _clousedPosition = -1f;

        private void Start()
        {
            StartCoroutine(MoveSatrtPoint());
        }

        private IEnumerator MoveSatrtPoint()
        {
            gameObject.transform.DOMoveY(_clousedPosition, _timeOpenPoint);
            yield return new WaitForSeconds(_timeOpenPoint);

            DestroyGameObject();
        }

        private void DestroyGameObject()
        {
            Destroy(gameObject);
        }
    }
}