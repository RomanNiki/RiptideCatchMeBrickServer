using RiptideNetworking;
using RiptideNetworking.Utils;
using UnityEngine;

namespace Multiplayer
{
    public class Networking : MonoBehaviour
    {
        [SerializeField] private ushort _port;
        [SerializeField] private ushort _maxClientCount;
        [SerializeField] private int _targetFrameRate = 60;

        public Server Server { get; set; }

        private void Start()
        {
            Application.targetFrameRate = _targetFrameRate;
            RiptideLogger.Initialize(Debug.Log, Debug.Log, Debug.LogWarning, Debug.LogError, false);
            Server = new Server();
            Server.Start(_port, _maxClientCount);
            Server.ClientDisconnected += PlayerLeft;
        }

        private void FixedUpdate()
        {
            Server.Tick();
        }

        private void OnApplicationQuit()
        {
            Server.Stop();
        }
        
        private static void PlayerLeft(object sender, ClientDisconnectedEventArgs e)
        {
            Destroy(PlayerSpawner.Players[e.Id].gameObject);
        }
    }
}
