using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public enum ShipType
{
    Scout,
    Cruiser,
    Carrier,
    None,
}

public enum Marker
{
    Target,
    Hit,
    Miss
}

public enum GameMode
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
    [SerializeField] private Tile[] markers;
    [SerializeField] private BattleShip[] battleShip;
    [Space]
    [SerializeField] private Tile cursorTile;
    [SerializeField] private int size = 8;

    private GameMode gameMode;

    private Grid grid;
    private Vector3Int minCoordinate;
    private Vector3Int maxCoordinate;

    private void Start()
    {
        Instance = this;
        grid = GetComponent<Grid>();
        Camera.main.transform.position = new Vector3Int(size / 2, size, -10);
        SetPlacementMode();
    }

    private void Update()
    {
        if (gameMode.Equals(GameMode.Disabled)) return;

        Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int coordinate = grid.WorldToCell(pos);

        coordinate.Clamp(minCoordinate, maxCoordinate);

        cursorLayer.ClearAllTiles();
        cursorLayer.SetTile(coordinate, cursorTile);


    }

    private void OnMouseDown()
    {
        Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int coordinate = grid.WorldToCell(pos);

        coordinate.Clamp(minCoordinate, maxCoordinate);

        if (gameMode.Equals(GameMode.Placement))
        {
            // controller.PlaceShip(coordinate);
        }
        else if (gameMode.Equals(GameMode.Attack))
        {
            // controller.TakeTurn(coordinate - new Vector3Int(0, size, 0));
        }
    }

    private void SetDisabled()
    {
        gameMode = GameMode.Disabled;
        cursorLayer.ClearAllTiles();
    }

    public void SetPlacementMode()
    {
        gameMode = GameMode.Placement;
        minCoordinate = new Vector3Int(0, 0, 0);
        maxCoordinate = new Vector3Int(size - 1, size - 1, 0);
    }

    private void SetAttackMode()
    {
        gameMode = GameMode.Attack;
        cursorTile = markers[(int)Marker.Target];
        minCoordinate = new Vector3Int(0, size, 0);
        maxCoordinate = new Vector3Int(size - 1, size + size - 1, 0);
    }

    private void SetMarker(int index, Marker marker, bool radar)
    {
        Vector3Int coordinate = new Vector3Int(index % size, Mathf.FloorToInt(index / size), 0);
        SetMarker(coordinate, marker, radar);
    }

    private void SetMarker(Vector3Int coordinate, Marker marker, bool radar)
    {
        if (radar)
        {
            coordinate += new Vector3Int(0, size, 0); // offset position
        }

        markerLayer.SetTile(coordinate, markers[(int)marker]);
    }

    private void SetShipCursor(ShipType shipType, bool horizontal)
    {
        int index = (int)shipType;
        cursorTile = battleShip[index].shipTile[horizontal ? 0 : 1];
    }

    private void SetShip(ShipType shipType, Vector3Int coordinate, bool horizontal)
    {
        int index = (int)shipType;
        Tile tile = battleShip[index].shipTile[horizontal ? 0 : 1];
        fleetLayer.SetTile(coordinate, tile);
    }
}
