using BoardGame;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Board : BoardParent
{
    [SerializeField] private List<Tile> checkpoints;
    [SerializeField] private Tile startTile;

    private int numberOfCheckpoints;
    [SerializeField] private int maxSteps;

    // This function is called whenever the board or any tile inside the board
    // is modified.
    public override void SetupBoard()
    {
        checkpoints = new List<Tile>();

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
                numberOfCheckpoints++;
            }

            // Sets the tile to the startTile
            if (tile.IsStartPoint)
            {
                startTile = tile;
            }   
        }

        // If there is a startTile, always make sure its cost is '1'
        if (startTile != null)
            startTile.cost = 1;

        // For every tile in checkpoints, get the shortest path and set its path
        foreach (Tile checkpointTile in checkpoints)
        {
            if (startTile == null)
            {
                Debug.LogError("Start tile is null");
                return;
            }

            EvaluateDijkstra(startTile);
            GetShortestPath();
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
        var completedTiles = new List<Tile>();

        searchTiles.Add(startTile);

        startTile.costToStart = 0;
        startTile.previousTile = startTile;

        int checkpointsFound = 0;
        int moved = 0;

        // Only runs if there are tiles to be searched
        while (searchTiles.Any() && (numberOfCheckpoints >= checkpointsFound || maxSteps > moved))
        {
            // Gets the neighbours
            var currentNeighbours = searchTiles[0].GetNeighbours(this);

            if (searchTiles[0].IsCheckPoint)
            {
                checkpointsFound++;
                Debug.Log("Checkpoints: " + checkpointsFound);
            }
            if(searchTiles[0].costToStart > moved)
            {
                moved = searchTiles[0].costToStart;
            }

            foreach (Tile neighbouringTile in currentNeighbours)
            {
                if (!searchTiles.Contains(neighbouringTile) && !completedTiles.Contains(neighbouringTile))
                {
                    neighbouringTile.previousTile = searchTiles[0];
                    searchTiles.Add(neighbouringTile);
                    neighbouringTile.costToStart = neighbouringTile.previousTile.costToStart + neighbouringTile.cost;
                }
                //else
                //{
                //    if (neighbouringTile.costToStart > searchTiles[0].cost + neighbouringTile.cost)
                //    {
                //        var dont = false;
                //        if (!dont)
                //        {
                //            neighbouringTile.previousTile = searchTiles[0];
                //            searchTiles.Add(neighbouringTile);
                //            neighbouringTile.costToStart = neighbouringTile.previousTile.costToStart + neighbouringTile.cost;
                //            Debug.Log("Test");
                //            dont = true;
                //        }
                //    }
                //}
            }

            completedTiles.Add(searchTiles[0]);
            searchTiles.Remove(searchTiles[0]);
           
        }
    }

    /// <summary>
    /// Gets the shortest path from the starting tile to the Checkpoint(s)
    /// </summary>
    private void GetShortestPath()
    {
        foreach(Tile checkpoint in checkpoints)
        {
            Debug.Log("Get Shortest Path, for-each running");
            Tile current = checkpoint;

           Debug.Log(checkpoint);
           while(current != startTile)
           {
               current = current.previousTile;
               Debug.Log("Get Shortest Path, while-roop running");
               current.IsPath = true;
           }

        }
    }
}