using UnityEngine;

namespace Models
{
    public class Player : MonoBehaviour
    {
        public ushort Id { get; private set; }
        public string UserName { get; private set; }

        public void Init(ushort id, string userName)
        {
            Id = id;
            UserName = userName;
        }
        
        private void OnDestroy()
        {
            PlayerSpawner.Players.Remove(Id);
        }
    }
}