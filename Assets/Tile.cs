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

    public GameObject portal;
    public GameObject checkPoint;
    public GameObject pathObject;

    private Vector2Int key;

    private bool portalExists = false;
    private bool checkpointExists = false;

    [SerializeField] public int cost = 1;
    [SerializeField] public int costToStart;

    public bool IsPath;
    public Tile previousTile;



    public List<Tile> GetNeighbours(Board board)
    {
        List<Tile> neighbours = new List<Tile>();

        if (board.TryGetTile(new Vector2Int(key.x - 1, key.y), out Tile neighbourLeft))
        {
            if (!neighbourLeft.IsBlocked)
            {
                neighbours.Add(neighbourLeft);
            }
        }
        if (board.TryGetTile(new Vector2Int(key.x + 1, key.y), out Tile neighbourRight))
        {
            if (!neighbourRight.IsBlocked)
            {
                neighbours.Add(neighbourRight);
            }
        }
        if (board.TryGetTile(new Vector2Int(key.x, key.y - 1), out Tile neighbourDown))
        {
            if (!neighbourDown.IsBlocked)
            {
                neighbours.Add(neighbourDown);
            }
        }
        if (board.TryGetTile(new Vector2Int(key.x, key.y + 1), out Tile neighbourUp))
        {
            if (!neighbourUp.IsBlocked)
            {
                neighbours.Add(neighbourUp);
            }
        }

        return neighbours;
    }

    public void ResetProperties()
    {
        if (IsObstacle(out int penalty))
        {
            cost = penalty;
        } else
        {
            cost = 1;
        }
        costToStart = int.MaxValue;
        previousTile = null;
        IsPath = false;
    }

    // This function is called when something has changed on the board. All 
    // tiles have been created before it is called.
    public override void OnSetup(Board board) {

        // 2. Each tile has a unique 'coordinate'
        key = Coordinate;
        
        // 3. Tiles can have different modifiers
        if (IsBlocked) {
            
        }
        
        if (IsObstacle(out int penalty)) {
            cost = penalty;
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
   
        if (IsPortal(out Vector2Int destination)) {
            portal.SetActive(true);

            if (!portalExists)
            {
                portal = Instantiate(portal, transform.position + new Vector3(0, 0.6f), transform.rotation, transform);
                portalExists = true;
            }

        }
        else
        {
            portal.SetActive(false);
        }
        if (IsPath)
        {
            pathObject.SetActive(true);
        }
        else
        {
            pathObject.SetActive(false);
        }
       
        // DO NOT REMOVE
        if(board.TryGetTile(new Vector2Int(2, 1), out Tile otherTile))
        {

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
    public override void OnUpdate(Board board) {
        
    }
}