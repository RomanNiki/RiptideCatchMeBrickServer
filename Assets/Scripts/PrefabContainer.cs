using Models;
using UnityEngine;

public class PrefabContainer : ScriptableObject
{
    [Header("Prefabs")] [SerializeField] private Player _playerPrefab;

    public Player PlayerPrefab => _playerPrefab;
}