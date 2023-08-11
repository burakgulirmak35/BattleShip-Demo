using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Tilemaps;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    public static GameController Instance { get; private set; }

    private enum GameState
    {
        Placement,
        Battle,
        GameOver
    }

    private GameState gameState;
    private int placedShipsCount;

    private void Awake()
    {
        Instance = this;
    }

    private void BeginShipPlacement()
    {
        gameState = GameState.Placement;
        UIManager.Instance.MessageText("Place Your Ships");
    }

    private void WaitForOpponentPlacement()
    {
        gameState = GameState.Placement;
        UIManager.Instance.MessageText("Place Your Ships");
        placedShipsCount = 0;
        Map.Instance.SetPlacementMode();
    }

    private void StartTurn()
    {
        gameState = GameState.Battle;
        UIManager.Instance.MessageText("Place Your Ships");
    }

    private void WaitForOpponentTurn()
    {
        gameState = GameState.Battle;
        UIManager.Instance.MessageText("Place Your Ships");
    }

    private void ShowResult()
    {
        gameState = GameState.GameOver;
        UIManager.Instance.MessageText("Place Your Ships");
    }
}
