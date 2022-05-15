using Multiplayer;
using UnityEngine;

namespace Player
{
    public class Player : MonoBehaviour
    {
        public ushort Id { get; private set; }
        public string UserName { get; private set; }
        public Networking Networking { get; private set; }

        public void Init(ushort id, string userName, Networking networking)
        {
            Id = id;
            UserName = userName;
            Networking = networking;
        }
        
        private void OnDestroy()
        {
            PlayerSpawner.RemovePlayer(Id);
        }
    }
}