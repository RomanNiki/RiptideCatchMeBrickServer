using Player.Weapons;
using UnityEngine;

namespace Player
{
    public class PlayerShooter : MonoBehaviour
    {
        [SerializeField] private Weapon _weapon;
        
        public void PrimaryUsePressed(Vector3 target, float angle)
        {
            _weapon.Shoot(target, angle);
        }
    }
}