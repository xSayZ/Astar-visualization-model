using BoardGame;
using System.Collections.Generic;
using UnityEngine;

public class Board : BoardParent
{
    [SerializeField] private float checkpointNum;
    [SerializeField] private List<Tile> checkpoints;
    [SerializeField] private Tile startTile;

    // This function is called whenever the board or any tile inside the board
    // is modified.
    public override void SetupBoard()
    {

        // 1. Get the size of the board
        var boardSize = BoardSize;

        checkpoints.Clear();

        // 2. Iterate over all tiles
        foreach (Tile tile in Tiles)
        {
            tile.ResetProperties();
            if (tile.IsCheckPoint)
            {
                checkpointNum++;
                checkpoints.Add(tile);
            }
            if (tile.IsStartPoint)
                startTile = tile;
        }

        if (startTile != null)
            startTile.cost = 1;

        foreach (Tile checkpointTile in checkpoints)
        {
            if (startTile == null)
                return;
            var shortPath = GetShortestPath(checkpointTile);
            foreach (var test in shortPath)
            {
                test.IsPath = true;
                Debug.Log(test);
            }
        }
    }
    private void EvaluateDijkstra(Tile startTile)
    {
        var searchTiles = new List<Tile>();
        searchTiles.Add(startTile);

        while (searchTiles.Count > 0)
        {
            searchTiles.Sort((x, y) => x.cost.CompareTo(y.cost));
            Tile currentSearchTile = searchTiles[0];
            searchTiles.Remove(currentSearchTile);

            var currentNeighbours = currentSearchTile.GetNeighbours(this);
            foreach (Tile neighbouringTile in currentNeighbours)
            {
                if (!neighbouringTile.evaluated && !neighbouringTile.IsBlocked)
                {
                    var searchCost = currentSearchTile.cost + neighbouringTile.cost;
                    if (searchCost > neighbouringTile.cost)
                    {
                        neighbouringTile.cost = currentSearchTile.cost;
                        neighbouringTile.previousTile = currentSearchTile;
                        searchTiles.Add(neighbouringTile);
                    }
                }
            }

            currentSearchTile.evaluated = true;
        }
    }

    private List<Tile> GetShortestPath(Tile checkpointTile)
    {
        var path = new List<Tile>();
        EvaluateDijkstra(startTile);
        Tile tile = checkpointTile;

        while (tile != null)
        {
            path.Add(tile);
            tile = tile.previousTile;
        }
        path.Reverse();
        return path;
    }
}