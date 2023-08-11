using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(menuName = "Scriptable/BattleShip", order = 0)]
public class BattleShip : ScriptableObject
{
    public Tile[] shipTile;
}