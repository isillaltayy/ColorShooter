using __ColorShooter.Scripts.ColorControllers;
using ColorControllers;
using UnityEngine;

namespace __ColorShooter.Scripts.Controllers
{
    public class BulletColorController : MonoBehaviour
    { 
        public  static BulletColorController instance; 
        private Color _selectedColor;
    
        private  PlayerTargetChooser playerTargetChooser;

        private void Start()
        {
            playerTargetChooser = GetComponent<PlayerTargetChooser>();
        }

        private void OnEnable()
        {
            ColorChangeController.OnColorChanged+= ChangeColor;
        }
        private void OnDisable()
        {
            ColorChangeController.OnColorChanged-= ChangeColor;
        }
        public void ChangeColor(Color obj)
        {
            _selectedColor = obj;
            playerTargetChooser.ChangeColor(_selectedColor);
        }
        public Color BoxColor
        {
            get => _selectedColor;
            set
            {
                _selectedColor = value;
            }
        }
        private void Awake()
        {
            if (instance == null)
                instance = this;
            else
                Destroy(gameObject);
        }
    }
}
