using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Utility {
    
    /// <summary>
    /// Dijkstra's algorithm for pathfinding is a weighted pathfinding algorithm that finds the shortest path between two points.
    /// The algorithm is based on the idea of visiting vertices in the graph in order of their distance from the start vertex.
    /// </summary>
    public class Dijkstras {
        readonly Board board;

        public Dijkstras(Board board) {
            this.board = board;
        }

        /// <summary>
        /// Evaluate the shortest path
        /// </summary>
        /// <param name="startTile">The tile used as origin of the pathfinding</param>
        public void EvaluateDijkstra(Tile startTile) {
            foreach (Tile tile in board.Tiles) {
                tile.previousTile = null;
                tile.costToStart = int.MaxValue;
            }

            var searchTiles = new List<Tile> { startTile };
            var completedTiles = new List<Tile>();

            startTile.costToStart = 0;
            startTile.previousTile = startTile;

            int moved = 0;
            int checkpointsFound = 0;

            while (board.GetCheckpoints().Count >= checkpointsFound || (board.GetMaxSteps() > moved) && searchTiles.Count > 0)
            {
                // Gets the neighbours from Tile.cs
                var currentNeighbours = searchTiles[0].GetNeighbours(board);

                // Update counting variables for amount of checkpoints
                if (searchTiles[0].IsCheckPoint) {
                    checkpointsFound++;
                }

                // Update counting variable for amount of tiles moved 
                if (searchTiles[0].costToStart > moved) {
                    moved = searchTiles[0].costToStart;
                }

                // Runs for each neighbour in the current tile
                foreach (Tile neighbouringTile in currentNeighbours) {
                    if (!searchTiles.Contains(neighbouringTile) && !completedTiles.Contains(neighbouringTile)) {
                        neighbouringTile.previousTile = searchTiles[0]; // Sets previous tile to the current search tile
                        searchTiles.Add(neighbouringTile); // Adds the neighbouring tile to the tiles to be searched
                        neighbouringTile.costToStart =
                            neighbouringTile.previousTile.costToStart +
                            neighbouringTile.cost; // Sets the cost to start for the neighbouring tile
                    }
                    else if (neighbouringTile.costToStart > searchTiles[0].costToStart + neighbouringTile.cost) {
                        neighbouringTile.previousTile = searchTiles[0];
                        searchTiles.Add(neighbouringTile);
                        neighbouringTile.costToStart =
                            neighbouringTile.previousTile.costToStart + neighbouringTile.cost;
                    }
                }

                completedTiles.Add(
                    searchTiles
                        [0]); // The tile that has been evaluated needs to be added to the list of completed tiles
                searchTiles.RemoveAt(0); // Removes the tile from the search tiles

                //Writes out if not all checkpoints could be found.
                if (!searchTiles.Any() && checkpointsFound != board.GetCheckpoints().Count) {
                    Debug.Log("Couldn't reach all checkpoints!");
                }

            }
        }
    }
}
