using Player;
using RiptideNetworking;
using RiptideNetworking.Utils;
using SharedLibrary;
using UnityEngine;

namespace Multiplayer
{
    public class Networking : MonoBehaviour
    {
        [SerializeField] private ushort _port;
        [SerializeField] private ushort _maxClientCount;
        [SerializeField] private int _targetFrameRate = 60;

        public Server Server { get; } = new Server();
        public static ushort CurrentTick { get; set; } = 0;

        private void Start()
        {
            Application.targetFrameRate = _targetFrameRate;
            RiptideLogger.Initialize(Debug.Log, Debug.Log, Debug.LogWarning, Debug.LogError, false);
            Server.Start(_port, _maxClientCount);
            Server.ClientDisconnected += PlayerLeft;
        }

        private void FixedUpdate()
        {
            Server.Tick();
            if (CurrentTick % 200 == 0)
            {
                SendSync();
            }
            CurrentTick++;
        }

        private void OnApplicationQuit()
        {
            Server.Stop();
        }

        private static void PlayerLeft(object sender, ClientDisconnectedEventArgs e)
        {
            if (PlayerSpawner.Players.TryGetValue(e.Id, out var player))
            {
                Destroy(player.gameObject);
            }
        }
        
        private void SendSync()
        {
            var message = Message.Create(MessageSendMode.unreliable, (ushort) ServerToClientId.Sync);
            message.Add(CurrentTick);
            Server.SendToAll(message);
        }
    }
}