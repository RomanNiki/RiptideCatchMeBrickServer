using System.Collections.Generic;
using System.Linq;
using Multiplayer;
using RiptideNetworking;
using SharedLibrary;
using UnityEngine;

namespace Player
{
    public class PlayerSpawner : MonoBehaviour
    {
        [SerializeField] private SpawnPoint[] _spawnPoints;
        [SerializeField] private PrefabContainer _prefabContainerScriptableObject;
        private static PrefabContainer _prefabContainer;
        public static readonly Dictionary<ushort, PlayerComponents> Players = new Dictionary<ushort, PlayerComponents>();
        private static SpawnPoint[] _staticSpawnPoints;

        private void Start()
        {
            _staticSpawnPoints = _spawnPoints;
            _prefabContainer =_prefabContainerScriptableObject;
        }
        
        private static Player Spawn(ushort fromClientId, string username)
        {
            var team = fromClientId % 2 == 0 ? Team.Red : Team.Green;
            foreach (var otherPlayer in Players)
            {
                PlayerSpawnedMessage(fromClientId, otherPlayer.Value.Client);
            }
            
            var player = Instantiate(_prefabContainer.PlayerPrefab, GetRandomPosition(team), Quaternion.identity);
            player.name = $"Player{fromClientId} {(string.IsNullOrEmpty(username) ? "Guest" : username)}";
            var userName = string.IsNullOrEmpty(username) ? $"Guest {fromClientId}" : username;
            player.Init(fromClientId, userName, team);
          
            Players.Add(player.Id, new PlayerComponents(player, player.GetComponent<PlayerMover>(), player.GetComponent<PlayerShooter>()));
            player.Respawned += TeleportToTeamSpawnPoint;
            PlayerSpawnedMessage(player);
         
            return player;
        }

        private static Vector3 GetRandomPosition(Team team)
        {
            var positions = _staticSpawnPoints.Where(x=>x.GetTeam == team).ToList();
            return positions[Random.Range(0, positions.Count)].transform.position;
        }
        
        private static void TeleportToTeamSpawnPoint(Player player)
        {
            Players[player.Id].Mover.Teleport(GetRandomPosition(player.Team));
        }
        
        public static void RemovePlayer(Player player)
        {
            player.Respawned -= TeleportToTeamSpawnPoint;
            Players.Remove(player.Id);
        }

        #region Messages

        private static void PlayerSpawnedMessage(Player player)
        {
            var message = AddSpawnData(Message.Create(MessageSendMode.reliable, ServerToClientId.PlayerSpawned), player);
            Networking.Server.SendToAll(message);
        }

        private static void PlayerSpawnedMessage(ushort toClientId, Player player)
        {
            var message = AddSpawnData(Message.Create(MessageSendMode.reliable, ServerToClientId.PlayerSpawned), player);
            Networking.Server.Send(message, toClientId);
        }

        private static Message AddSpawnData(Message message, Player player)
        {
            message.AddUShort(player.Id);
            message.Add(player.UserName);
            message.AddVector3(player.transform.position);
            message.AddByte((byte)player.Team);
            message.AddFloat(player.MaxHealth);
            return message;
        }
        
        [MessageHandler((ushort) ClientToServerId.Name)]
        private static void ChangeName(ushort fromClientId, Message message)
        {
            Spawn(fromClientId, message.GetString());
        }

        [MessageHandler((ushort) ClientToServerId.Input)]
        private static void Input(ushort fromClientId, Message message)
        {
            if (Players.TryGetValue(fromClientId, out var player))
            {
                player.Mover.SetInput(message.GetVector2(),message.GetBools(2), message.GetVector2());
            }
        }
        
        [MessageHandler((ushort)ClientToServerId.PrimaryUse)]
        private static void PrimaryUse(ushort fromClientId, Message message)
        {
            if (Players.TryGetValue(fromClientId, out var player))
                player.Shooter.PrimaryUsePressed(message.GetVector3(), message.GetFloat());
        }
        #endregion
    }
}