using UnityEngine;

namespace Projectiles
{
    [RequireComponent(typeof(Rigidbody))]
    public class Brick : Projectile
    {
        [SerializeField] private float _angularVelocityModifier = 5f;
        private Rigidbody _rigidbody;

        public override void Init(ushort id, Player.Player player, Vector3 targetVelocity)
        {
            _rigidbody = GetComponent<Rigidbody>();
            base.Init(id, player, targetVelocity);
            _rigidbody.velocity = targetVelocity;
            _rigidbody.angularVelocity = new Vector3(Random.value, Random.value, Random.value) * _angularVelocityModifier;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out Player.Player hitPlayer))
            {
                if (_player.Id == hitPlayer.Id)
                {
                    return;
                }
                if (hitPlayer.Id != _player.Id)
                {
                    SendHitmarker(hitPlayer);
                    hitPlayer.TakeDamage(_damage);
                }
            }

            SendCollided();
            Destroy(gameObject);
        }
    }
}