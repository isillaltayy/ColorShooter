using System;
using DG.Tweening;
using UnityEngine;

namespace __ColorShooter.Scripts.Controllers
{
    public class ColorChangeController : MonoBehaviour
    {
        private Color _boxColor;
        private Material _material;
    
        public static Action<Color> OnColorChanged;
        public static Action StartSpawning;
    
        private static bool _isSpawning=false;

        private Color GetColor()
        {
            return GetComponent<Renderer>().material.color;
        }
        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out BulletColorController colorController))
            {
                //if game is finished, don't spawn new bullets
                if (!GameManager.Instance.IsGameFinished)
                    return;
                
                colorController.ChangeColor(GetColor());
                if (!_isSpawning)
                {
                    _isSpawning = true;
                    StartSpawning?.Invoke();
                }
            }
        }
        private void OnTriggerStay(Collider other)
        {
            if (other.TryGetComponent(out BulletColorController colorController))
            {
                gameObject.transform.DOScale(new Vector3(2f, 0.5f, 2f), 0.5f).OnComplete(() =>
                {
                    _isSpawning = false;
                });
            }
        }
        private void OnTriggerExit(Collider other)
        {
            if (other.TryGetComponent(out BulletColorController colorController))
            {
                gameObject.transform.DOScale(new Vector3(2f, 0.1f, 2f), 0.5f);
            }
        }
    }
}
