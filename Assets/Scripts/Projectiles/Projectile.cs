using Multiplayer;
using RiptideNetworking;
using SharedLibrary;
using UnityEngine;

namespace Projectiles
{
    public abstract class Projectile : MonoBehaviour
    {
        [SerializeField] protected float _damage;
        private Vector3 _targetVelocity;
        public ushort Id { get; private set; }
        protected Player.Player _player;

        public virtual void Init(ushort id, Player.Player player, Vector3 targetVelocity)
        {
            Id = id;
            _player = player;
            transform.rotation = Quaternion.LookRotation(transform.forward);
            _targetVelocity = targetVelocity;
        }

        protected virtual void FixedUpdate()
        {
            SendMovement();
        }

        private void OnDestroy()
        {
            ProjectileSpawner.Projectiles.Remove(Id);
        }

        protected void SendMovement()
        {
            if (Networking.CurrentTick % 2 != 0)
            {
                return;
            }
            var message = Message.Create(MessageSendMode.unreliable, ServerToClientId.ProjectileMovement);
            message.AddUShort(Id);
            message.AddUShort(Networking.CurrentTick);
            message.AddVector3(transform.position);
            message.AddQuaternion(transform.rotation);
            Networking.Server.SendToAll(message);
        }

        protected void SendCollided()
        {
            var message = Message.Create(MessageSendMode.reliable, ServerToClientId.ProjectileCollided);
            message.AddUShort(Id);
            message.AddVector3(transform.position);
            Networking.Server.SendToAll(message);
        }
        
         protected void SendHitmarker(Player.Player player)
            {
                var message = Message.Create(MessageSendMode.reliable, ServerToClientId.ProjectileHitMarker);
                message.AddUShort(player.Id);
                Networking.Server.SendToAll(message);
            }
    }
}