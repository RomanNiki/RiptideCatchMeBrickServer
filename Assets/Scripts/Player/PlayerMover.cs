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
        [SerializeField] private Transform _camProxy;
        [SerializeField] private float _gravity;
        [SerializeField] private float _movementSpeed;
        [SerializeField] private float _jumpHeight;
        private Player _player;
        private CharacterController _controller;

        private float _gravityAcceleration;
        private float _moveSpeed;
        private float _jumpSpeed;

        private Vector2 _moveInput;
        private bool[] _inputs;
        private float _yVelocity;

        private void Start()
        {
            _player = GetComponent<Player>();
            _controller = GetComponent<CharacterController>();
            Initialize();
            _inputs = new bool[6];
        }

        private void FixedUpdate()
        {
            Move(_moveInput, _inputs[0], _inputs[1]);
        }

        private void Initialize()
        {
            _gravityAcceleration = _gravity * Time.fixedDeltaTime * Time.fixedDeltaTime;
            _moveSpeed = _movementSpeed * Time.fixedDeltaTime;
            _jumpSpeed = Mathf.Sqrt(_jumpHeight * -2f * _gravityAcceleration);
        }

        private void Move(Vector2 inputDirection, bool jump, bool sprint)
        {
            var moveDirection =
                Vector3.Normalize(_camProxy.right * inputDirection.x +
                                  Vector3.Normalize(FlattenVector3(_camProxy.forward)) * inputDirection.y);

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

            SendMovement();
        }

        private Vector3 FlattenVector3(Vector3 vector)
        {
            vector.y = 0f;
            return vector;
        }

        public void SetInput(Vector2 moveInput,bool[] inputs, Vector3 forward)
        {
            _moveInput = moveInput;
            _inputs = inputs;
            _camProxy.forward = forward;
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
            message.AddVector3(_camProxy.forward);
            _player.Networking.Server.SendToAll(message);
        }
    }
}