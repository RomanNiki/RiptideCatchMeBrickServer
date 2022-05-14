using System.Collections.Generic;
using Models;
using Multiplayer;
using RiptideNetworking;
using SharedLibrary;
using UnityEngine;

[RequireComponent(typeof(Networking))]
[RequireComponent(typeof(PrefabContainer))]
public class PlayerSpawner : MonoBehaviour
{
    [SerializeField] private Transform[] _spawnPoints;
    private static Transform[] _staticSpawnPoints;

    public static readonly Dictionary<ushort, Player> Players = new Dictionary<ushort, Player>();
    private static PrefabContainer _prefabContainer;
    private static Networking _networking;
    
    private void Start()
    {
        _staticSpawnPoints = _spawnPoints;
        _prefabContainer = GetComponent<PrefabContainer>();
        _networking = GetComponent<Networking>();
    }

    private static Player Spawn(ushort fromClientId, string username)
    {
        foreach (var otherPlayer in Players)
        {
            PlayerSpawnedMessage(otherPlayer.Key, otherPlayer.Value);
        }
            
        var player = Instantiate(_prefabContainer.PlayerPrefab, new Vector3(0, 1f), Quaternion.identity);
        player.name = $"Player{fromClientId} {(string.IsNullOrEmpty(username) ? "Guest" : username)}";
        var userName = string.IsNullOrEmpty(username) ? $"Guest {fromClientId}" : username;
        player.Init(fromClientId, userName);
        player.transform.position = _staticSpawnPoints[Random.Range(0, _staticSpawnPoints.Length)].position;
        PlayerSpawnedMessage(player);
        Players.Add(player.Id, player);
        return player;
    }

    #region Messages

    private static void PlayerSpawnedMessage(Player player)
    {
        var message = AddSpawnData(Message.Create(MessageSendMode.reliable, ServerToClientId.PlayerSpawned), player);
        _networking.Server.SendToAll(message);
    }

    private static void PlayerSpawnedMessage(ushort toClientId, Player player)
    {
        var message = AddSpawnData(Message.Create(MessageSendMode.reliable, ServerToClientId.PlayerSpawned), player);
        _networking.Server.Send(message, toClientId);
    }

    private static Message AddSpawnData(Message message, Player player)
    {
        message.AddUShort(player.Id);
        message.Add(player.UserName);
        message.AddVector3(player.transform.position);
        return message;
    }
        
    [MessageHandler((ushort) ClientToServerId.Name)]
    private static void ChangeName(ushort fromClientId, Message message)
    {
        Spawn(fromClientId, message.GetString());
    }
    #endregion
}