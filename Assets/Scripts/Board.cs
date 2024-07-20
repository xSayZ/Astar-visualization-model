using BoardGame;
using System.Collections.Generic;
using UnityEngine;
using Utility;

public class Board : BoardParent
{
    [Header("Settings")]
    public int maxSteps;
    [Space(5)]
    [Header("References")]
    [SerializeField] List<Tile> checkpoints;
    [SerializeField] Tile startTile;
    
    // Getters
    public List<Tile> GetCheckpoints() => checkpoints;
    public Tile GetStartTile() => startTile;
    public int GetMaxSteps() => maxSteps;
    
    // Internal references
    Dijkstras dijkstras;
    Pathfinding pathFinding;
    
    public override void SetupBoard()
    {
        dijkstras = new Dijkstras(this);
        pathFinding = new Pathfinding(this);
        
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
            
            // Resets the path, so it doesn't still visualize a path after it has been toggled off
            tile.IsPath = false;
            
            // Sets the tile to the startTile
            if (tile.IsStartPoint) { startTile = tile; }   
            if(tile.portalExit != null) { tile.portalExit.SetActive(false); }
        }

        // If there is a startTile and there are checkpoints, run Djikstras
        if (startTile != null && checkpoints.Count > 0)
        {
            dijkstras.EvaluateDijkstra(startTile);
            // For every tile in checkpoints, get the shortest path
            foreach (Tile checkpoint in checkpoints)
            {
                pathFinding.GetShortestPath(startTile, checkpoint);
            }
        }         
    }
}