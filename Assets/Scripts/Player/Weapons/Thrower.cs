using System.Collections;
using Projectiles;
using SharedLibrary;
using UnityEngine;

namespace Player.Weapons
{
    public class Thrower : Weapon
    {
        [SerializeField] private Player _player;
        [SerializeField] private WeaponType _weapon;
        [SerializeField] private float _reloadTime;
        private bool _canShoot = true;
        private readonly float _yGravity = Physics.gravity.y;

        public override void Shoot(Vector3 target, float angle)
        {
            if (_canShoot)
            {
                transform.localEulerAngles = new Vector3(-angle, 0f, 0f);
                var targetVelocity = Throw(target, angle);
                ProjectileSpawner.Spawn(_player, _weapon, transform.position, targetVelocity);
                StartCoroutine(Reload());
            }
        }
        
        private Vector3 Throw(Vector3 target, float angle)
        {
            var fromTo = target - transform.position;
            var fromToXZ = new Vector3(fromTo.x, 0f, fromTo.z);
            var x = fromToXZ.magnitude;
            var y = fromTo.y;
            var angleInRadians = angle * Mathf.PI / 180;
            var v2 = (_yGravity * x * x) /
                     (2 * (y - Mathf.Tan(angleInRadians) * x) * Mathf.Pow(Mathf.Cos(angleInRadians), 2));
            var v = Mathf.Sqrt(Mathf.Abs(v2));
            var velocity = transform.forward * v;
            return  velocity;
        }

        private IEnumerator Reload()
        {
            _canShoot = false;
            yield return new WaitForSeconds(_reloadTime);
            _canShoot = true;
        }
    }
}