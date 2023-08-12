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
    [SerializeField] private Tilemap fleetLayer;
    [SerializeField] private Tilemap markerLayer;
    [Space]
    [SerializeField] public Tile[] markers;
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

    private void Start()
    {
        mapSize = GameController.Instance.GetMapSize();
        Camera.main.transform.position = new Vector3Int(mapSize / 2, mapSize, -10);
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
                    break;
                case MapState.Attack:
                    SetMarker(coordinate - new Vector3Int(0, mapSize, 0), Marker.Hit, true);
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
                maxCoordinate = new Vector3Int(mapSize - 1, mapSize - 1, 0);
                break;
            case MapState.Attack:
                cursorTile = markers[(int)Marker.Target];
                minCoordinate = new Vector3Int(0, mapSize, 0);
                maxCoordinate = new Vector3Int(mapSize - 1, mapSize + mapSize - 1, 0);
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
    private void SetMarker(int index, Marker marker, bool radar)
    {
        Vector3Int coordinate = new Vector3Int(index % mapSize, Mathf.FloorToInt(index / mapSize), 0);
        SetMarker(coordinate, marker, radar);
    }

    private void SetMarker(Vector3Int coordinate, Marker marker, bool radar)
    {
        if (radar)
        {
            coordinate += new Vector3Int(0, mapSize, 0); // offset position
        }
        markerLayer.SetTile(coordinate, markers[(int)marker]);
    }
    #endregion

    public BattleShipSO GetCurretBattleShip()
    {
        return currentBattleShip;
    }
}
