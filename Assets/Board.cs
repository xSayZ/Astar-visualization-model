using BoardGame;
using System.Collections.Generic;
using UnityEngine;

public class Board : BoardParent
{
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
            // Resets all tiles to their respective default properties
            tile.ResetProperties();

            // Adds the tiles to the checkpoint list if they are checkpoints
            if (tile.IsCheckPoint)
            {
                checkpoints.Add(tile);
            }

            // Sets the tile to the startTile
            if (tile.IsStartPoint)
                startTile = tile;
        }

        // If there is a startTile, always make sure its cost is '1'
        if (startTile != null)
            startTile.cost = 1;

        // For every tile in checkpoints, get the shortest path and set its path
        foreach (Tile checkpointTile in checkpoints)
        {
            if (startTile == null)
                return;
            var shortPath = GetShortestPath(checkpointTile);
            foreach (var result in shortPath)
            {
                result.IsPath = true;
            }
        }
    }

    /// <summary>
    /// Djikstras Algortihm to find the shortest path to any specified tile used in GetShortestPath
    /// </summary>
    /// <param name="startTile"> The Start Tile</param>
    private void EvaluateDijkstra(Tile startTile)
    {

        // The list of tiles to be searched
        var searchTiles = new List<Tile>();
        searchTiles.Add(startTile);

        // Only runs if there are tiles to be searched
        while (searchTiles.Count > 0)
        {
            // Sorts all the tiles in the list based on their cost and then removes them from the list after they have been searched
            searchTiles.Sort((x, y) => x.cost.CompareTo(y.cost));
            Tile currentSearchTile = searchTiles[0];
            searchTiles.Remove(currentSearchTile);

            // Gets the neighbours
            var currentNeighbours = currentSearchTile.GetNeighbours(this);
            foreach (Tile neighbouringTile in currentNeighbours)
            {
                // Makes sure that a tile has not been searched and isn't blocked
                if (!neighbouringTile.evaluated && !neighbouringTile.IsBlocked)
                {
                    // Local variable that adds the cost of the current tile and the cost of the next tile
                    var searchCost = currentSearchTile.cost + neighbouringTile.cost;

                    // If it is less than neighbouring tile (negative in this case since int.MaxValue)
                    if (searchCost < neighbouringTile.cost)
                    {
                        // Sets the neghbouring tile to the cost of the search tile
                        neighbouringTile.cost = currentSearchTile.cost;

                        // Makes sure that the previous tile is the search tile
                        neighbouringTile.previousTile = currentSearchTile;

                        // Adds it to the list of neighbouring tiles
                        searchTiles.Add(neighbouringTile);
                    }
                }
                // Has been searched
                currentSearchTile.evaluated = true;
            }
        }
    }

    /// <summary>
    /// Gets the shortest path from the starting tile to the Checkpoint(s)
    /// </summary>
    /// <param name="checkpointTile">Checkpoint(s)</param>
    /// <returns></returns>
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