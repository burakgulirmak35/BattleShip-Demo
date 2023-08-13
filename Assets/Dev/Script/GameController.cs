using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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
    private bool[,] shots;
    private Player[] players = new Player[2];

    private void Awake()
    {
        Instance = this;

        for (int i = 0; i < players.Length; i++)
        {
            players[i] = new Player();
            players[i].SetPlacemant(mapSize);
        }
    }

    private void Start()
    {
        Map.Instance.SetMap(mapSize);
        UIManager.Instance.Rotate += Event_RotateShip;
        shots = new bool[mapSize, mapSize];
        BeginShipPlacement();
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

    private void EnemyPlacement()
    {
        gameState = GameState.EnemyTurn;
        Map.Instance.SetMapState(MapState.Disabled);
        UIManager.Instance.MessageText("Enemy Turn");
    }

    public void EnemyTurn()
    {
        gameState = GameState.EnemyTurn;
        Map.Instance.SetMapState(MapState.Disabled);
        UIManager.Instance.MessageText("Enemy Turn");
        EnemyShoot();
    }

    public void GameOver()
    {
        gameState = GameState.GameOver;
        Map.Instance.SetMapState(MapState.Disabled);
        UIManager.Instance.MessageText("Game Over");
    }
    #endregion

    #region PlaceShip
    public void PlaceShip(Vector3Int coordinate, int playerID = 0)
    {
        int size = Map.Instance.GetCurretBattleShip().ShipSize;
        int shipWidth = _placeShipHorizontally ? size : 1;
        int shipHeight = _placeShipHorizontally ? 1 : size;

        if ((playerID == 1) && (coordinate.x < 0 || coordinate.x + (shipWidth - 1) >= mapSize || coordinate.y - (shipHeight - 1) < mapSize / 2)) { return; }
        if (coordinate.x < 0 || coordinate.x + (shipWidth - 1) >= mapSize || coordinate.y - (shipHeight - 1) < 0) { return; }

        for (int i = 0; i < size; i++)
        {
            if (PlaceShipHorizontally)
            {
                Vector3Int checkCorrdinate = coordinate + new Vector3Int(i, 0, 0);
                if (!players[playerID].isEmptyCell(checkCorrdinate)) { return; }
            }
            else
            {
                Vector3Int checkCorrdinate = coordinate + new Vector3Int(0, -i, 0);
                if (!players[playerID].isEmptyCell(checkCorrdinate)) { return; }
            }
        }
        // gemiyi yerlestirebiliriz

        ShipID++;
        for (int i = 0; i < size; i++)
        {
            if (PlaceShipHorizontally)
            {
                players[playerID].SetCell(coordinate + new Vector3Int(i, 0, 0), ShipID);
            }
            else
            {
                players[playerID].SetCell(coordinate + new Vector3Int(0, -i, 0), ShipID);
            }
        }
        if (playerID == 0)
        {
            Map.Instance.SetShip(coordinate, _placeShipHorizontally);
        }
        UpdateCursor();
    }

    public void CheckReady()
    {
        if (ShipID >= battleShipsSO.Length)
        {
            EnemyPlaceShips();
            TakeTurn();
        }
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

    #region Shoot
    public void Shoot(Vector3Int coordinate, int targetPlayerID)
    {
        if (shots[coordinate.x, coordinate.y]) { UIManager.Instance.MessageText("Buraya zaten attin"); return; }
        if (players[targetPlayerID].isHit(coordinate))
        {
            Map.Instance.SetMarker(coordinate, Marker.Hit);
            if (players[targetPlayerID].isGameOver()) { GameOver(); return; }
            if (targetPlayerID == 1)
            {
                TakeTurn();
            }
            else
            {
                EnemyTurn();
            }
        }
        else
        {
            if (targetPlayerID == 1)
            {
                EnemyTurn();
            }
            else
            {
                TakeTurn();
            }
            Map.Instance.SetMarker(coordinate, Marker.Miss);
        }
        shots[coordinate.x, coordinate.y] = true;
    }

    #endregion

    #region Enemy
    private void EnemyPlaceShips()
    {
        ShipID = 0;
        EnemyPlacement();
        UpdateCursor();

        while (ShipID < battleShipsSO.Length)
        {
            PlaceShipHorizontally = Random.value > 0.5f;
            Vector3Int _randomCell = new Vector3Int(Random.Range(0, mapSize), Random.Range(mapSize / 2, mapSize), 0);
            PlaceShip(_randomCell, 1);
        }
    }

    private void EnemyShoot()
    {
        Vector3Int _randomCell = new Vector3Int(Random.Range(0, mapSize), Random.Range(0, mapSize / 2), 0);
        if (shots[_randomCell.x, _randomCell.y]) { EnemyShoot(); return; }
        Shoot(_randomCell, 0);
    }
    #endregion

    private class Player
    {
        private int[,] placement;
        private int[] hit;
        private int lostShipCount;

        public void SetPlacemant(int _mapSize)
        {
            placement = new int[_mapSize, _mapSize];
            hit = new int[GameController.Instance.battleShipsSO.Length];
        }

        public bool isEmptyCell(Vector3Int coordinate)
        {
            if (placement[coordinate.x, coordinate.y] > 0) { return false; }
            return true;
        }

        public void SetCell(Vector3Int coordinate, int ShipID)
        {
            placement[coordinate.x, coordinate.y] = ShipID;
        }

        public bool isHit(Vector3Int coordinate)
        {
            if (placement[coordinate.x, coordinate.y] > 0)
            {
                int _shipID = placement[coordinate.x, coordinate.y] - 1;
                hit[_shipID]++;
                if (hit[_shipID] < GameController.Instance.battleShipsSO[_shipID].ShipSize)
                {
                    UIManager.Instance.MessageText("Amiral yara aldı");
                }
                else
                {
                    UIManager.Instance.MessageText(GameController.Instance.battleShipsSO[_shipID].ShipName + " batti");
                    lostShipCount++;
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool isGameOver()
        {
            return lostShipCount >= GameController.Instance.battleShipsSO.Length;
        }

        private class ShipData
        {
            public Vector3Int shipCoordinate;
            public bool isHorizontally;
        }

    }
}
