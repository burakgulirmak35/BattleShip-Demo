using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public enum Marker
{
    Target,
    Hit,
    Miss
}

public enum MapState
{
    Disabled,
    Placement,
    Attack
}

public class Map : MonoBehaviour
{
    public static Map Instance { get; private set; }

    [SerializeField] private Tilemap cursorLayer;
    [SerializeField] private Tilemap markerLayer;
    [SerializeField] private Tilemap radarLayer;
    [SerializeField] private Tilemap fleetLayer;
    [SerializeField] private Tilemap oceanLayer;
    [Space]
    [SerializeField] private Tile[] markers;
    [SerializeField] private Tile oceanTile;
    [SerializeField] private Tile radarTile;

    private Tile cursorTile;
    private int mapSize;
    [Space]
    private BattleShipSO currentBattleShip;
    private MapState mapState;
    private Grid grid;
    private Vector3Int minCoordinate;
    private Vector3Int maxCoordinate;

    private void Awake()
    {
        Instance = this;
        grid = GetComponent<Grid>();

    }

    public void SetMap(int _mapSize)
    {
        mapSize = _mapSize;
        Camera.main.transform.position = new Vector3Int(mapSize / 2, mapSize / 2, -10);
        oceanLayer.ClearAllTiles();

        for (int x = 0; x < mapSize; x++)
        {
            for (int y = mapSize / 2; y < mapSize; y++)
            {
                radarLayer.SetTile(new Vector3Int(x, y, 0), radarTile);
            }

            for (int y = 0; y < mapSize; y++)
            {
                oceanLayer.SetTile(new Vector3Int(x, y, 0), oceanTile);
            }
        }
    }

    public void ReviveArea(Vector3Int _coordinate)
    {
        radarLayer.SetTile(_coordinate, null);
    }

    private void Update()
    {
        if (mapState.Equals(MapState.Disabled)) return;

        Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int coordinate = grid.WorldToCell(pos);

        coordinate.Clamp(minCoordinate, maxCoordinate);
        cursorLayer.ClearAllTiles();
        cursorLayer.SetTile(coordinate, cursorTile);

        if (Input.GetMouseButtonDown(0))
        {
            switch (mapState)
            {
                case MapState.Placement:
                    GameController.Instance.PlaceShip(coordinate);
                    GameController.Instance.CheckReady();
                    break;
                case MapState.Attack:
                    GameController.Instance.Shoot(coordinate, 0, 1);
                    break;
                default:
                    break;
            }
        }
    }

    #region MapState
    public void SetMapState(MapState state)
    {
        mapState = state;
        switch (mapState)
        {
            case MapState.Disabled:
                cursorLayer.ClearAllTiles();
                break;
            case MapState.Placement:
                minCoordinate = new Vector3Int(0, 0, 0);
                maxCoordinate = new Vector3Int(mapSize - 1, mapSize / 2 - 1, 0);
                break;
            case MapState.Attack:
                cursorTile = markers[(int)Marker.Target];
                minCoordinate = new Vector3Int(0, mapSize / 2, 0);
                maxCoordinate = new Vector3Int(mapSize - 1, mapSize - 1, 0);
                break;
            default:
                break;
        }
    }
    #endregion

    #region Ship Ghost And Placement
    public void SetShipCursor(BattleShipSO battleShipSO, bool horizontal)
    {
        cursorTile = battleShipSO.ship[horizontal ? 0 : 1];
        currentBattleShip = battleShipSO;
    }

    public void SetShip(Vector3Int coordinate, bool horizontal)
    {
        Tile tile = currentBattleShip.ship[horizontal ? 0 : 1];
        fleetLayer.SetTile(coordinate, tile);
    }
    #endregion

    #region Marker
    public void SetMarker(Vector3Int coordinate, Marker marker)
    {
        markerLayer.SetTile(coordinate, markers[(int)marker]);
    }
    #endregion

    public BattleShipSO GetCurretBattleShip()
    {
        return currentBattleShip;
    }
}
