using UnityEngine;

namespace __ColorShooter.Scripts.Controllers
{
    public class PlayerMovementController : MonoBehaviour
    {
        [SerializeField] private float speed = 10f;
    
        private Animator _animator;
        private static readonly int IsRunning = Animator.StringToHash("isRunning");

        [SerializeField] private float _inputOffset = 2;
        private Vector3 _firstMousePosition;
        private Vector3 _preMousePosition;
        private Vector3 _currentMousePosition;
        private Vector3 _deltaMousePosition;
        private Vector3 _direction;
        private float _magnitued;
        private Vector3 objectPosition;
        private bool _isGameStarted=false;

        [SerializeField] private Vector3 playerStartPosition;
        private bool _stopPlayer;

        private void OnEnable()
        {
            Debug.LogError(gameObject.name+GameManager.Instance);
            GameManager.Instance.OnGameStart += OnGameStart;
            GameManager.Instance.OnGameOver += ResetPlayerMovement;
            GameManager.Instance.OnGameFinishedSuccessfully += ResetPlayerMovement;
        }
        private void OnDisable()
        {
            GameManager.Instance.OnGameStart -= OnGameStart;
            GameManager.Instance.OnGameOver -= ResetPlayerMovement;
            GameManager.Instance.OnGameFinishedSuccessfully -= ResetPlayerMovement;
        }

        private void OnGameStart()
        {
            _stopPlayer = false;
            _isGameStarted = true;
            _animator = GetComponent<Animator>();
            transform.position = playerStartPosition;
            objectPosition = transform.position;
        }
        private void ResetPlayerMovement()
        {
            _stopPlayer = true;
            DefaultValues();
        }
        private void Update()
        {
            if (_isGameStarted)
            {
                PlayerMove();
                AnimationControl();
            }
        }
        private void PlayerMove()
        {
            if(_stopPlayer)
                return;
            
            if (Input.GetMouseButtonDown(0))
            {
                _firstMousePosition = Input.mousePosition;
                _preMousePosition = _firstMousePosition;
                _currentMousePosition = _preMousePosition;
            }
            if (Input.GetMouseButton(0))
            {
                _preMousePosition = _currentMousePosition;
                _currentMousePosition = Input.mousePosition;
                _deltaMousePosition = _currentMousePosition - _preMousePosition;

                if ((_firstMousePosition.x + _inputOffset) < Input.mousePosition.x || (_firstMousePosition.x - _inputOffset) > Input.mousePosition.x
                    || (_firstMousePosition.y + _inputOffset) < Input.mousePosition.y || (_firstMousePosition.y - _inputOffset) > Input.mousePosition.y)
                {
                    var directionLocal = (_currentMousePosition - _firstMousePosition);
                    _direction = directionLocal.normalized;
                    _magnitued = directionLocal.magnitude;
                }
            }
            if (Input.GetMouseButtonUp(0))
            {
                DefaultValues();
            }
        
            LerpMovement();
            SetBounds();
        }

        private void SetBounds()
        {
            var position = transform.position;
            objectPosition.x = Mathf.Clamp(position.x, -4f, 4f);
            objectPosition.z = Mathf.Clamp(position.z, 1.4f, 3f);
            position = objectPosition;
            transform.position = position;
        }

        private void LerpMovement()
        {
            var position = transform.position;
            Vector3 targetPosition = position + new Vector3(_deltaMousePosition.x,0,_deltaMousePosition.y);
            position = Vector3.Lerp(position, targetPosition, speed * Time.deltaTime);
            transform.position = position;
        }

        private void DefaultValues()
        {
            _firstMousePosition = Vector3.zero;
            _preMousePosition = Vector3.zero;
            _currentMousePosition = Vector3.zero;
            _deltaMousePosition = Vector3.zero;
            _magnitued = 0f;
        }
        private void AnimationControl()
        {
            if (Input.GetMouseButton(0))
            {
                _animator.SetBool(IsRunning, true);
            }
            else
            {
                _animator.SetBool(IsRunning, false);
            }
        }
    }
}


