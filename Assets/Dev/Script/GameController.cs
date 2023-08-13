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
    private Player[] players;
    private int playerCount = 2;
    public System.DateTime startTime;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        players = new Player[playerCount];
        for (int i = 0; i < players.Length; i++)
        {
            players[i] = new Player();
            players[i].SetPlacemant(mapSize);
        }

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
        startTime = System.DateTime.Now;
    }

    private void PlayerTurn()
    {
        gameState = GameState.Battle;
        Map.Instance.SetMapState(MapState.Attack);
        // UIManager.Instance.MessageText("Your Turn");
    }

    private void EnemyPlacement()
    {
        gameState = GameState.EnemyTurn;
        Map.Instance.SetMapState(MapState.Disabled);
        // UIManager.Instance.MessageText("Enemy Turn");
    }

    public void EnemyTurn()
    {
        gameState = GameState.EnemyTurn;
        Map.Instance.SetMapState(MapState.Disabled);
        // UIManager.Instance.MessageText("Enemy Turn");
        EnemyShoot();
    }

    public void GameOver(int playerID)
    {

        gameState = GameState.GameOver;
        Map.Instance.SetMapState(MapState.Disabled);
        UIManager.Instance.MessageText("Game Over");

        System.TimeSpan span = (System.DateTime.Now - startTime);
        string Time = System.String.Format("{0} min, {1} sec", span.Minutes, span.Seconds);

        if (playerID == 1)
        {
            UIManager.Instance.EndGame("You Win", players[0].GetHitCount(), players[0].GetMissCount(), Time);
        }
        else
        {
            UIManager.Instance.EndGame("You Lost", players[0].GetHitCount(), players[0].GetMissCount(), Time);
        }

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
        Map.Instance.SetShip(coordinate, _placeShipHorizontally);
        UpdateCursor();
    }

    public void CheckReady()
    {
        if (ShipID >= battleShipsSO.Length)
        {
            EnemyPlaceShips();
            PlayerTurn();
            UIManager.Instance.MessageText("Your Turn");
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
    public void Shoot(Vector3Int coordinate, int playerID, int targetPlayerID)
    {
        if (shots[coordinate.x, coordinate.y]) { UIManager.Instance.MessageText("Buraya zaten attin"); return; }
        if (players[targetPlayerID].isHit(coordinate))
        {
            Map.Instance.SetMarker(coordinate, Marker.Hit);
            if (players[targetPlayerID].isGameOver()) { GameOver(targetPlayerID); return; }
            players[playerID].shotResult(true);
            PassTurn(playerID);
        }
        else
        {
            players[playerID].shotResult(false);
            Map.Instance.SetMarker(coordinate, Marker.Miss);
            PassTurn(targetPlayerID);
        }

        UIManager.Instance.UpdateText(players[0].GetHitCount(), players[0].GetMissCount());
        shots[coordinate.x, coordinate.y] = true;
    }

    private void PassTurn(int playerID)
    {
        switch (playerID)
        {
            case 0:
                PlayerTurn();
                break;
            case 1:
                EnemyTurn();
                break;
        }
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
        Shoot(_randomCell, 1, 0);
    }
    #endregion

    private class Player
    {
        private int[,] placement;
        private int[] hit;
        private int lostShipCount;
        private int mapSize;

        private int hitCount;
        private int missCount;

        public void shotResult(bool hit)
        {
            if (hit) { hitCount++; }
            else { missCount++; }
        }

        public void SetPlacemant(int _mapSize)
        {
            mapSize = _mapSize;
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
                int shipID = placement[coordinate.x, coordinate.y];
                hit[shipID - 1]++;
                if (hit[shipID - 1] < GameController.Instance.battleShipsSO[shipID - 1].ShipSize)
                {
                    UIManager.Instance.MessageText("Amiral yara aldı");
                }
                else
                {
                    UIManager.Instance.MessageText(GameController.Instance.battleShipsSO[shipID - 1].ShipName + " batti");
                    lostShipCount++;
                    ReviveShip(shipID);
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

        public int GetHitCount()
        {
            return hitCount;
        }

        public int GetMissCount()
        {
            return missCount;
        }


        private void ReviveShip(int _shipID)
        {
            for (int i = 0; i < mapSize; i++)
            {
                for (int j = 0; j < mapSize; j++)
                {
                    if (placement[i, j] == _shipID)
                    {
                        Map.Instance.ReviveArea(new Vector3Int(i, j, 0));
                    }
                }
            }
        }

    }
}
