using System.Collections;
using Multiplayer;
using RiptideNetworking;
using SharedLibrary;
using UnityEngine;
using UnityEngine.Events;

namespace Player
{
    [RequireComponent(typeof(PlayerMover))]
    public class Player : MonoBehaviour
    {
        [SerializeField] private float _respawnTime;
        [SerializeField] private float _maxHealth;
        public UnityAction<Player> Respawned; 
        private float _health;
        private PlayerMover _playerMover;
        
        public ushort Id { get; private set; }
        public string UserName { get; private set; }
        public Team Team { get; private set; }
        public float MaxHealth => _maxHealth;

        private void Start()
        {
            _playerMover = GetComponent<PlayerMover>();
            DontDestroyOnLoad(gameObject);
            _health = _maxHealth;
        }

        public void Init(ushort id, string userName, Team team)
        {
            Id = id;
            UserName = userName;
            Team = team;
        }
        
        private void OnDestroy()
        {
            PlayerSpawner.RemovePlayer(this);
        }

        public void TakeDamage(float damage)
        {
            _health -= damage;
            if (_health <= 0f)
            {
                _health = 0f;
                Die();
            }
            else
                SendHealthChanged();
        }

        private void Die()
        {
            _playerMover.Enabled(false);
            StartCoroutine(DelayedRespawn());
            SendDied();
        }

        private IEnumerator DelayedRespawn()
        {
            yield return new WaitForSeconds(_respawnTime);

            InstantRespawn();
        }
        
        private void InstantRespawn()
        { 
            Respawned?.Invoke(this);
            _playerMover.Enabled(true);

            _health = _maxHealth;
            SendRespawned();
        }
        
        private void SendHealthChanged()
        {
            var message = Message.Create(MessageSendMode.reliable, ServerToClientId.PlayerHealthChanged);
            message.AddFloat(_health);
            Networking.Server.Send(message, Id);
        }
        
        private void SendDied()
        {
            var message = Message.Create(MessageSendMode.reliable, ServerToClientId.PlayerDied);
            message.AddUShort(Id);
            message.AddVector3(transform.position);
            Networking.Server.SendToAll(message);
        }
        
        private void SendRespawned()
        {
            var message = Message.Create(MessageSendMode.reliable, ServerToClientId.PlayerRespawned);
            message.AddUShort(Id);
            message.AddVector3(transform.position);
            Networking.Server.SendToAll(message);
        }
    }
}