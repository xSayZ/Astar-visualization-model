using BoardGame;
using System.Collections.Generic;
using UnityEngine;

public class Tile : TileParent {

    // 1. TileParent extends MonoBehavior, so you can add member variables here
    // to store data.
    public Material regularMaterial;
    public Material portalMaterial;
    public Material obstacleMaterial;
    public Material checkpointMaterial;
    public Material startpointMaterial;
    public Material blockedMaterial;
    public Material stepsMaterial;

    public GameObject portal;
    public GameObject portalExit;
    public GameObject checkPoint;
    public GameObject pathObject;
    public GameObject rangeObject;
    public GameObject blockedObject;
    public GameObject obstacleObject;

    private Vector2Int key;

    private bool portalExists = false;
    private bool checkpointExists = false;
    private bool blockedExists = false;
    private bool obstacleExists = false;

    public int cost = 1;
    public int costToStart;

    public bool IsPath;
    public Tile previousTile;

    public bool PortalExitIsBlocked { get; set; }

    public List<Tile> GetNeighbours(Board board)
    {
        List<Tile> neighbours = new List<Tile>();

        if (board.TryGetTile(new Vector2Int(coordinate.x - 1, coordinate.y), out Tile neighbourLeft))
        {
            if (!neighbourLeft.IsBlocked)
            {
                neighbours.Add(neighbourLeft);
            }
        }
        if (board.TryGetTile(new Vector2Int(coordinate.x + 1, coordinate.y), out Tile neighbourRight))
        {
            if (!neighbourRight.IsBlocked)
            {
                neighbours.Add(neighbourRight);
            }
        }
        if (board.TryGetTile(new Vector2Int(coordinate.x, coordinate.y - 1), out Tile neighbourDown))
        {
            if (!neighbourDown.IsBlocked)
            {
                neighbours.Add(neighbourDown);
            }
        }
        if (board.TryGetTile(new Vector2Int(coordinate.x, coordinate.y + 1), out Tile neighbourUp))
        {
            if (!neighbourUp.IsBlocked)
            {
                neighbours.Add(neighbourUp);
            }
        }
        if (IsPortal(out Vector2Int destination))
        {
            board.TryGetTile(new Vector2Int(destination.x, destination.y), out var portalDestination);
            // Check if the portal destination is blocked before adding it to the neighbors.
            if (!portalDestination.IsBlocked)
            {
                neighbours.Add(portalDestination);
            }
        }

        return neighbours;
    }

    // This function is called when something has changed on the board. All 
    // tiles have been created before it is called.
    public override void OnSetup(Board board) {

        previousTile = this;

        // 2. Each tile has a unique 'coordinate'
        key = Coordinate;
        
        // 3. Tiles can have different modifiers
        if (IsBlocked) {

            costToStart = int.MaxValue;

            blockedObject.SetActive(true);
            
            if (!blockedExists)
            {
                blockedObject = Instantiate(blockedObject, transform.position, blockedObject.transform.rotation);
                blockedObject.transform.parent = transform;
                blockedExists = true;
            }
        }
        else
        {
            blockedObject.SetActive(false);
        }
        
        if (IsObstacle(out int penalty)) {
            cost = penalty;

            obstacleObject.SetActive(true);
            if (!obstacleExists)
            {
                obstacleObject = Instantiate(obstacleObject, transform.position, obstacleObject.transform.rotation);
                obstacleObject.transform.parent = transform;
                obstacleExists = true;
            }
            
        }
        else
        {
            obstacleObject.SetActive(false);
        }
          
        if (IsCheckPoint) {
            checkPoint.SetActive(true);

            if (!checkpointExists)
            {
                checkPoint = Instantiate(checkPoint, transform.position + new Vector3(0, 1.25f), transform.rotation, transform);
                checkpointExists = true;
            }
        }
        else
        {
            checkPoint.SetActive(false);
        }
        
        if (IsStartPoint) {
            
        }

        if (IsPortal(out Vector2Int destination))
        {
            Tile destinationTile;
            if (board.TryGetTile(destination, out destinationTile))
            {
                if (destinationTile.IsBlocked)
                {
                    // If the destination tile is blocked or has a portal exit, roads should not go through it.
                    destinationTile.portalExit.SetActive(false);
                }
                else
                {
                    portal.SetActive(true);

                    if (!portalExists)
                    {
                        portal = Instantiate(portal, transform.position + new Vector3(0, 0.6f), Quaternion.identity, transform);
                        portalExists = true;
                    }

                    // DO NOT REMOVE
                    destinationTile.portalExit.SetActive(true);
                }
            }
        }
        else
        {
            portal.SetActive(false);
        }

        if (IsPath)
        {
            pathObject.SetActive(true);

            if (obstacleExists)
            {
                // Raise the y-value of the path object to avoid the obstacle
                float yOffset = 0.165f; // Adjust this value to set the desired height offset
                Vector3 raisedPosition = new Vector3(pathObject.transform.position.x, yOffset, pathObject.transform.position.z);
                pathObject.transform.position = raisedPosition;
            }
        }
        else
        {
            pathObject.SetActive(false);
        
        }
        
        // 5. Change the material color if this tile is blocked
        if (TryGetComponent<MeshRenderer>(out var meshRenderer)) {
            if (IsBlocked) {
                meshRenderer.sharedMaterial = blockedMaterial;
            } else if (IsObstacle(out penalty)) {
                meshRenderer.sharedMaterial = obstacleMaterial;
            } else if (IsCheckPoint) { 
                meshRenderer.sharedMaterial = checkpointMaterial;
            } else if (IsPortal(out destination)) {
                meshRenderer.sharedMaterial = portalMaterial;
            } else if (IsStartPoint) {
                meshRenderer.sharedMaterial = startpointMaterial;
            } 
            else {
                meshRenderer.sharedMaterial = regularMaterial; 
            }
        }
    }

    // This function is called during the regular 'Update' step, but also gives
    // you access to the 'board' instance.
    public override void OnUpdate(Board board)
    {
        if (costToStart <= 0) {
                // Tile is not reachable, so disable the rangeObject visualization
                rangeObject.SetActive(false);
            }
        else if (costToStart <= board.maxSteps) {
            // Tile is reachable and within maxSteps, so enable the rangeObject visualization
            rangeObject.SetActive(true);
        }
        else {
            // Tile is reachable, but beyond maxSteps, so disable the rangeObject visualization
            rangeObject.SetActive(false);
        }
    }
}

