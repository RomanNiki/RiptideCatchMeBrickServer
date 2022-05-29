using System.Collections.Generic;
using Multiplayer;
using RiptideNetworking;
using SharedLibrary;
using UnityEngine;

namespace Projectiles
{
    public class ProjectileSpawner : MonoBehaviour
    {
        [SerializeField] private PrefabContainer _prefab;
        public static Dictionary<ushort, Projectile> Projectiles = new Dictionary<ushort, Projectile>();

        private static PrefabContainer _staticPrefabContainer;
        private static ushort _nextId;
        private static ushort NextId => _nextId++;

        private void Start()
        {
            _staticPrefabContainer = _prefab;
        }

        private static void SendSpawned(ushort id, WeaponType type, Player.Player player, Vector3 position)
        {
            var message = Message.Create(MessageSendMode.reliable, ServerToClientId.ProjectileSpawned);
            message.AddUShort(id);
            message.AddByte((byte) type);
            message.AddUShort(player.Id);
            message.AddVector3(position);
            Networking.Server.SendToAll(message);
        }

        public static void Spawn(Player.Player player, WeaponType type, Vector3 position, Vector3 targetVelocity)
        {
            var projectile = Instantiate(_staticPrefabContainer.GetProjectile(type), position,
                Quaternion.identity);

            var id = NextId;
            projectile.name = $"Projectile {id}";
            projectile.Init(id, player, targetVelocity);

            SendSpawned(projectile.Id, type, player, position);
            Projectiles.Add(id, projectile);
        }
    }
}