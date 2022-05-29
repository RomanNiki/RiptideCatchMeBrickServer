using SharedLibrary;
using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    [SerializeField] private Team _team;
    public Team GetTeam => _team;

}