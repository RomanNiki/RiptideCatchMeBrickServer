using System.Collections.Generic;
using Projectiles;
using SharedLibrary;
using UnityEngine;

[CreateAssetMenu(menuName = "PrefabContainer/New PrefabContainer")]
public class PrefabContainer : ScriptableObject
{
    [SerializeField] private Player.Player _playerPrefab;
    public Player.Player PlayerPrefab => _playerPrefab;

    [SerializeField] private List<Projectile> _projectiles;
    
    public Projectile GetProjectile(WeaponType type)
    {
        var value = (int) type;
        return _projectiles[value];
    }
}
