using UnityEngine;

[CreateAssetMenu(menuName = "PrefabContainer/New PrefabContainer")]
public class PrefabContainer : ScriptableObject
{
    [Header("Prefabs")] [SerializeField] private Player.Player _playerPrefab;

    public Player.Player PlayerPrefab => _playerPrefab;
}