using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public enum ShipType
{
    None,
    Scout,
    Cruiser,
    Carrier
}

public enum Marker
{
    Target,
    Hit,
    Miss
}

enum GameMode
{
    Disabled,
    Placement,
    Attack
}

public class Map : MonoBehaviour
{
    [SerializeField] private GameController controller;
    [SerializeField] private Tilemap cursorLayer;
    [SerializeField] private Tilemap fleetLayer;
    [SerializeField] private Tilemap debugLayer;
    [SerializeField] private Tilemap markerLayer;
    [SerializeField] private Tile[] cursorTiles;
    [SerializeField] private Tile cursorTile;
    [SerializeField] private int size = 8;

    private GameMode gameMode;

    private Grid grid;
    private Vector3Int minCoordinate;
    private Vector3Int maxCoordinate;

    private void Start()
    {
        controller = FindObjectOfType<GameController>();
        grid = GetComponent<Grid>();
        SetPlacementMode();
    }

    private Update()
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

    private void SetPlacementMode()
    {
        gameMode = GameMode.Placement;
        minCoordinate = new Vector3Int(0, 0, 0);
        maxCoordinate = new Vector3Int(size - 1, size - 1, 0);
    }

    private void SetAttackMode()
    {
        gameMode = GameMode.Attack;
        cursorTile = cursorTiles[(int)Marker.Target];
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

        markerLayer.SetTile(coordinate, cursorTiles[(int)marker]);
    }

    private void SetShipCursor(ShipType shipType, bool horizontal)
    {
        int index = ((int)shipType - 1) * 2 + (horizontal ? 0 : 1);
        cursorTile = cursorTiles[index];
    }

    private void SetDebugTile(Vector3Int coordinate, Tile tile)
    {
        debugLayer.SetTile(coordinate, tile);
    }

    private void SetShip(ShipType shipType, Vector3Int coordinate, bool horizontal)
    {
        int index = ((int)shipType - 1) * 2 + (horizontal ? 0 : 1);
        Tile tile = cursorTiles[index];
        fleetLayer.SetTile(coordinate, tile);
    }
}
