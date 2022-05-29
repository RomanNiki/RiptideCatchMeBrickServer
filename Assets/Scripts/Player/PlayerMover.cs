using Multiplayer;
using RiptideNetworking;
using SharedLibrary;
using UnityEngine;

namespace Player
{
    [RequireComponent(typeof(CharacterController))]
    [RequireComponent(typeof(Player))]
    public class PlayerMover : MonoBehaviour
    {
        [SerializeField] private float _gravity;
        [SerializeField] private float _movementSpeed;
        [SerializeField] private float _jumpHeight;
        [SerializeField] private float _rotateSpeed = 5f;
        private Player _player;
        private CharacterController _controller;

        private float _gravityAcceleration;
        private float _moveSpeed;
        private float _jumpSpeed;

        private Vector2 _moveInput;
        private bool[] _inputs;
        private float _yVelocity;
        private Vector2 _throwJoystickInput;
        private bool _didTeleport;

        public void Enabled(bool value)
        {
            enabled = value;
            _controller.enabled = value;
        }
        
        private void Start()
        {
            _player = GetComponent<Player>();
            _controller = GetComponent<CharacterController>();
            Initialize();
            _inputs = new bool[6];
        }

        private void FixedUpdate()
        {
            Move(_moveInput, _inputs[0], _inputs[1], _throwJoystickInput);
        }

        private void Initialize()
        {
            _gravityAcceleration = _gravity * Time.fixedDeltaTime * Time.fixedDeltaTime;
            _moveSpeed = _movementSpeed * Time.fixedDeltaTime;
            _jumpSpeed = Mathf.Sqrt(_jumpHeight * -2f * _gravityAcceleration);
        }
        
        public void Teleport(Vector3 toPosition)
        {
            var isEnabled = _controller.enabled;
            _controller.enabled = false;
            transform.position = toPosition;
            _controller.enabled = isEnabled;

            _didTeleport = true;
        }

        private void Move(Vector2 inputDirection, bool jump, bool sprint, Vector2 throwJoystickInput)
        {
            var moveDirection = new Vector3(inputDirection.x, 0f, inputDirection.y);

            moveDirection *= _moveSpeed;

            if (sprint)
            {
                moveDirection *= 2f;
            }

            if (_controller.isGrounded)
            {
                _yVelocity = 0f;
                if (jump)
                {
                    _yVelocity = _jumpSpeed;
                }
            }

            _yVelocity += _gravityAcceleration;
            moveDirection.y = _yVelocity;
            _controller.Move(moveDirection);
            if (_throwJoystickInput.magnitude > 0.2f)
            {
                RotateCharacter(new Vector3(throwJoystickInput.x, 0f, throwJoystickInput.y));
            }
            else
            {
                RotateCharacter(new Vector3(inputDirection.x, 0f, inputDirection.y));
            }
            SendMovement();
        }

        private void RotateCharacter(Vector3 moveDirection)
        {
            if (Vector3.Angle(transform.forward, moveDirection) <= 0) return;
            var newDirection = Vector3.RotateTowards(transform.forward, moveDirection, _rotateSpeed, 0);
            transform.rotation = Quaternion.LookRotation(newDirection);
        }

        public void SetInput(Vector2 moveInput,bool[] inputs, Vector2 rotation)
        {
            _throwJoystickInput = rotation;
            _moveInput = moveInput;
            _inputs = inputs;
        }
    
        private void SendMovement()
        {
            if (Networking.CurrentTick % 2 != 0)
            {
                return;
            }
            var message = Message.Create(MessageSendMode.unreliable, ServerToClientId.PlayerMovement);
            message.AddUShort(_player.Id);
            message.AddUShort(Networking.CurrentTick);
            message.AddVector3(transform.position);
            message.AddQuaternion(transform.rotation);
            message.AddBool(_didTeleport);
            Networking.Server.SendToAll(message);
        }
    }
}