using BoardGame;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Board : BoardParent
{
    [Header("Set amount of Max Steps")]
    public int maxSteps;
    [Space]
    [SerializeField] private List<Tile> checkpoints;
    [SerializeField] private Tile startTile;

    // This function is called whenever the board or any tile inside the board
    // is modified.
    public override void SetupBoard()
    {
        // List of checkpoints
        checkpoints = new List<Tile>();

        // 1. Get the size of the board
        var boardSize = BoardSize;

        checkpoints.Clear();

        // 2. Iterate over all tiles
        foreach (Tile tile in Tiles)
        {
            // Makes sure that the cost of the obstacle tile is always set to the correct value
            if(tile.IsObstacle(out int penalty)) { tile.cost = penalty; }
            else { tile.cost = 1; }
            // Adds the tiles to the checkpoint list if they are checkpoints
            if (tile.IsCheckPoint) {
                checkpoints.Add(tile);
            }
            // Resets the path so it doesn't still visualize a path after it has been toggled off
            tile.IsPath = false;
            // Sets the tile to the startTile
            if (tile.IsStartPoint) { startTile = tile; }   
            if(tile.portalExit != null) { tile.portalExit.SetActive(false); }
        }

        // If there is a startTile and there are checkpoints, run Djikstras
        if (startTile != null && checkpoints.Count > 0)
        {
            EvaluateDijkstra(startTile);
            // For every tile in checkpoints, get the shortest path
            foreach (Tile checkpoint in checkpoints)
            {
                GetShortestPath();
            }
        }         
    }

    /// <summary>
    /// Djikstras Algortihm to find the shortest path to any specified tile used in GetShortestPath
    /// </summary>
    /// <param name="startTile"> The Start Tile</param>
    private void EvaluateDijkstra(Tile start)
    {
        foreach(Tile tile in Tiles)
        {
            tile.previousTile = null;
            tile.costToStart = int.MaxValue;
        }

        // The list of tiles to be searched
        var searchTiles = new List<Tile>();

        // The list of tiles completed
        var completedTiles = new List<Tile>();

        // Adds the first tile, the start tile, to the list of tiles to be searched
        searchTiles.Add(startTile);

        startTile.costToStart = 0;
        startTile.previousTile = startTile;

        int moved = 0;
        int checkpointsFound = 0;

        /// <summary>
        /// Runs as long as all checkpoints have not been found, or
        /// the amount of steps have not reached maxSteps, and
        /// the amount of tiles to be searched are more than 0
        /// </summary>
        while ((checkpoints.Count >= checkpointsFound || maxSteps > moved) && searchTiles.Count > 0)
        {
            // Gets the neighbours from Tile.cs
            var currentNeighbours = searchTiles[0].GetNeighbours(this);

            // Update counting variables for amount of checkpoints
            if (searchTiles[0].IsCheckPoint)
            {
                checkpointsFound++;
                Debug.Log("Checkpoints: " + checkpointsFound);
            }

            // Update counting variable for amount of tiles moved 
            if(searchTiles[0].costToStart > moved)
            {
                moved = searchTiles[0].costToStart;
            }

            // Runs for each neighbour in the current tile
            foreach (Tile neighbouringTile in currentNeighbours)
            {
                // Makes sure that it has not searched any neighbouring tiles already
                if (!searchTiles.Contains(neighbouringTile) && !completedTiles.Contains(neighbouringTile))
                {
                    neighbouringTile.previousTile = searchTiles[0]; // Sets previous tile to the current search tile
                    searchTiles.Add(neighbouringTile); // Adds the neighbouring tile to the tiles to be searched
                    neighbouringTile.costToStart = neighbouringTile.previousTile.costToStart + neighbouringTile.cost; // Updated the cost of the neighbours cost
                                                                                                                      // to the current evaluating tiles cost to start
                }
                /// <summary>
                /// If the previous was not true and the neighbours cost to start is higher than the 
                /// evalutating tiles cost to start summed with the neighbouring tiles cost
                /// </summary>
                else if (neighbouringTile.costToStart > searchTiles[0].costToStart + neighbouringTile.cost)
                {
                    neighbouringTile.previousTile = searchTiles[0];
                    searchTiles.Add(neighbouringTile);
                    neighbouringTile.costToStart = neighbouringTile.previousTile.costToStart + neighbouringTile.cost;
                }
            }

            completedTiles.Add(searchTiles[0]); // The tile that has been evaluated needs to be added to the list of completed tiles
            searchTiles.Remove(searchTiles[0]); // Removes the tile from the search tiles

            //Writes out if not all checkpoints could be found.
            if (!searchTiles.Any() && checkpointsFound != checkpoints.Count)
            {
                Debug.Log("Couldn't reach all checkpoints!");
            }

        }
    }

    /// <summary>
    /// Gets the shortest path from the starting tile to the Checkpoint(s)
    /// </summary>
    private void GetShortestPath()
    {
        // Makes sure that the start tile is not null, will return an error and return the code
        if (startTile == null)
        {
            Debug.LogError("Start tile is null");
            return;
        }

        foreach (Tile checkpoint in checkpoints)
        {
            // Sets the current tile to checkpoint
            Tile current = checkpoint;

            if(current.previousTile != null)
            {
                // If the current isnt startTile
                while (current != startTile && !current.IsBlocked)
                {
                    current = current.previousTile;

                    current.IsPath = true;
                    
                }
            }

        }
    }
}