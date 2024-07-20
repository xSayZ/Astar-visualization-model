using UnityEngine;

public class Pathfinding {
    readonly Board board;
    
    public Pathfinding(Board board) {
        this.board = board;
    }
    
    /// <summary>
    /// Gets the shortest path from the starting tile to the Checkpoint(s)
    /// </summary>
    public void GetShortestPath(Tile startTile, Tile checkpoint)
    {
        // Makes sure that the start tile is not null, will return an error and return the code
        if (startTile == null)
        {
            Debug.LogError("Start tile is null");
            return;
        }

        Tile currentTile = checkpoint;
        
        if (currentTile.previousTile != null)
        {
            while (currentTile != startTile && !currentTile.IsBlocked)
            {
                currentTile = currentTile.previousTile;
                currentTile.IsPath = true;
            }
        }
    }
}