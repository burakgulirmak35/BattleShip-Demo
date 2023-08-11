using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(menuName = "Scriptable/BattleShipSO", order = 0)]
public class BattleShipSO : ScriptableObject
{
    public Tile[] ship;
}