using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GameController : MonoBehaviour
{
    public static GameController Instance { get; private set; }
    private enum GameState
    {
        Placement,
        Battle,
        EnemyTurn,
        GameOver
    }
    [SerializeField] private BattleShipSO[] battleShipsSO;
    [SerializeField] private int mapSize;

    private GameState gameState;
    private int ShipID;
    private int[,] placement;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        Map.Instance.SetMap(mapSize);
        BeginShipPlacement();

        UIManager.Instance.Rotate += Event_RotateShip;
        placement = new int[mapSize, mapSize];
    }

    #region  GameState
    private void BeginShipPlacement()
    {
        gameState = GameState.Placement;
        ShipID = 0;
        Map.Instance.SetMapState(MapState.Placement);
        UpdateCursor();
        UIManager.Instance.MessageText("Place Your Ships");
    }

    private void TakeTurn()
    {
        gameState = GameState.Battle;
        Map.Instance.SetMapState(MapState.Attack);
        UIManager.Instance.MessageText("Your Turn");
    }

    private void EnemyTurn()
    {
        gameState = GameState.EnemyTurn;
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

    #region PlaceShip
    public void PlaceShip(Vector3Int coordinate, bool topSide = false)
    {
        int size = Map.Instance.GetCurretBattleShip().ShipSize;
        int shipWidth = _placeShipHorizontally ? size : 1;
        int shipHeight = _placeShipHorizontally ? 1 : size;

        if (topSide && (coordinate.x < 0 || coordinate.x + (shipWidth - 1) >= mapSize || coordinate.y - (shipHeight - 1) < mapSize / 2))
        {
            return;
        }
        if (coordinate.x < 0 || coordinate.x + (shipWidth - 1) >= mapSize || coordinate.y - (shipHeight - 1) < 0)
        {
            return;
        }

        for (int i = 0; i < size; i++)
        {
            if (PlaceShipHorizontally)
            {
                Vector3Int checkCorrdinate = coordinate + new Vector3Int(i, 0, 0);
                if (!isEmptyCell(checkCorrdinate)) { return; }
            }
            else
            {
                Vector3Int checkCorrdinate = coordinate + new Vector3Int(0, -i, 0);
                if (!isEmptyCell(checkCorrdinate)) { return; }
            }
        }
        // gemiyi yerlestirebiliriz

        ShipID++;
        for (int i = 0; i < size; i++)
        {
            if (PlaceShipHorizontally)
            {
                SetCell(coordinate + new Vector3Int(i, 0, 0));
            }
            else
            {
                SetCell(coordinate + new Vector3Int(0, -i, 0));
            }
        }

        Map.Instance.SetShip(coordinate, _placeShipHorizontally);
        UpdateCursor();
    }

    public void CheckReady()
    {
        if (ShipID >= battleShipsSO.Length)
        {
            PlaceEnemyShips();
            TakeTurn();
        }
    }

    private bool isEmptyCell(Vector3Int coordinate)
    {
        if (placement[coordinate.x, coordinate.y] > 0) { return false; }
        return true;
    }

    private void SetCell(Vector3Int coordinate)
    {
        placement[coordinate.x, coordinate.y] = (int)ShipID;
    }
    #endregion

    #region SelectedShip and Rotation
    private bool _placeShipHorizontally;
    private bool PlaceShipHorizontally
    {
        get { return _placeShipHorizontally; }
        set { _placeShipHorizontally = value; UpdateCursor(); }
    }
    private void UpdateCursor()
    {
        if (ShipID < battleShipsSO.Length)
            Map.Instance.SetShipCursor(battleShipsSO[ShipID], PlaceShipHorizontally);
    }
    private void Event_RotateShip(object sender, System.EventArgs e)
    {
        if (gameState.Equals(GameState.Placement))
            PlaceShipHorizontally = !PlaceShipHorizontally;
    }
    #endregion

    #region Enemy
    private void PlaceEnemyShips()
    {
        ShipID = 0;
        EnemyTurn();
        UpdateCursor();

        int i = 0;
        while (ShipID < battleShipsSO.Length)
        {
            PlaceShipHorizontally = Random.value > 0.5f;
            Vector3Int randomCell = new Vector3Int(Random.Range(0, mapSize), Random.Range(mapSize / 2, mapSize), 0);
            PlaceShip(randomCell, true);

            i++;
            if (i > 20)
            {
                return;
            }
        }
    }
    #endregion
}
