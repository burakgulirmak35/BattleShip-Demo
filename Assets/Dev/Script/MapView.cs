using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapView : MonoBehaviour
{
    [SerializeField] private Tilemap fleepLayer;
    [SerializeField] private Tilemap markerLayer;
    [SerializeField] private Tilemap cursorLayer;

    [SerializeField] private Tile[] cursorTiles;
    [SerializeField] private Tile cursorTile;

    [SerializeField] private int size = 8;

    private Grid grid;
    [SerializeField] private Vector3Int minCoordinate;
    [SerializeField] private Vector3Int maxCoordinate;

    private void Awake()
    {
        grid = GetComponent<Grid>();
    }

    private void Update()
    {
        Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int coordinate = grid.WorldToCell(pos);

        coordinate.Clamp(minCoordinate, maxCoordinate);
        cursorLayer.ClearAllTiles();
        cursorLayer.SetTile(coordinate, cursorTile);
    }

}
