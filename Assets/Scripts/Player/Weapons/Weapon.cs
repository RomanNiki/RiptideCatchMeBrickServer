using UnityEngine;

namespace Player.Weapons
{
    public abstract class Weapon : MonoBehaviour
    {
        public abstract void Shoot(Vector3 target, float angle);
    }
}