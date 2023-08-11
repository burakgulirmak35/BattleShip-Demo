using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GameController : MonoBehaviour
{
    public static GameController Instance { get; private set; }
    private enum GameState
    {
        Placement,
        Battle,
        GameOver
    }
    [SerializeField] private BattleShipSO[] battleShipsSO;

    private GameState gameState;
    private int placedShipsCount;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        BeginShipPlacement();
    }

    #region  GameState
    private void BeginShipPlacement()
    {
        gameState = GameState.Placement;
        placedShipsCount = 0;
        Map.Instance.SetMapState(MapState.Placement);
        UpdateCursor();
        UIManager.Instance.MessageText("Place Your Ships");
    }

    private void WaitForOpponentPlacement()
    {
        gameState = GameState.Placement;
        Map.Instance.SetMapState(MapState.Disabled);
        UIManager.Instance.MessageText("Enemy Turn");
    }

    private void StartTurn()
    {
        gameState = GameState.Battle;
        Map.Instance.SetMapState(MapState.Attack);
        UIManager.Instance.MessageText("Your Turn");
    }

    private void WaitForOpponentTurn()
    {
        gameState = GameState.Battle;
        Map.Instance.SetMapState(MapState.Disabled);
        UIManager.Instance.MessageText("Enemy Turn");
    }

    private void ShowResult()
    {
        gameState = GameState.GameOver;
        Map.Instance.SetMapState(MapState.Disabled);
        UIManager.Instance.MessageText("Game Over");
    }
    #endregion

    #region SeciliGemi ve Rotasyonu
    private bool _placeShipHorizontally;
    public bool PlaceShipHorizontally
    {
        get { return _placeShipHorizontally; }
        set { _placeShipHorizontally = value; UpdateCursor(); }
    }
    private void UpdateCursor()
    {
        Map.Instance.SetShipCursor(battleShipsSO[placedShipsCount], PlaceShipHorizontally);
    }
    #endregion

}
