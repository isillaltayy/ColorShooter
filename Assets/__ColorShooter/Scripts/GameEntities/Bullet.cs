using __ColorShooter.Scripts.Controllers;
using __ColorShooter.Scripts.GameEntities;
using DG.Tweening;
using NaughtyAttributes;
using UnityEngine;

namespace GameEntities
{
    public class Bullet : MonoBehaviour
    {
        [SerializeField] private float speed;
        [SerializeField] private bool isMoving;
        [SerializeField] private Renderer renderer;
        [SerializeField] private Renderer liquid;
        
        private float _endPosition;
        public delegate void MapEnd(GameObject bullet);
        public event MapEnd OnFinishedMap;
        private Color ballColor;

        private void FixedUpdate()
        {
            if (!isMoving) return;

            if (transform.position.z > _endPosition)
                Finished();
        }

        private void Finished()
        {
            ResetValues();
            OnFinishedMap?.Invoke(gameObject);
            gameObject.SetActive(false);
        }

        public void StartMoving(Transform player,float endPos, float speed)
        {
            ResetValues();
            ChangeColor();
            isMoving = true;
            this.speed = speed;
            _endPosition = endPos;
            GetComponent<Rigidbody>().velocity = player.forward * speed;
        }
        
        private void ChangeColor()
        {
            ballColor = BulletColorController.instance.BoxColor;
            gameObject.GetComponent<Renderer>().material.color = ballColor;
            renderer.material.color = new Color(ballColor.r, ballColor.g, ballColor.b, 1f);
            liquid.material.color = new Color(ballColor.r, ballColor.g, ballColor.b, 1f);
        }

        private void ResetValues()
        {
            speed = 0f;
            isMoving = false;
        }

        private void OnTriggerEnter(Collider other)
        {
            Debug.LogError("Collision " + other.gameObject.name);
            if (other.gameObject.TryGetComponent(out Enemy enemy))
            {
                enemy.CompareColor(ballColor);
                Finished();
            }
        }
    }
}
