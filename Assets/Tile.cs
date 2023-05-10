using BoardGame;
using Unity.VisualScripting;
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

    private bool portalExists = false;
    private bool checkpointExists = false;

    [SerializeField] public int cost = 1;

    // This function is called when something has changed on the board. All 
    // tiles have been created before it is called.
    public override void OnSetup(Board board) {
        // 2. Each tile has a unique 'coordinate'
        Vector2Int key = Coordinate;
        
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
       
        // 4. Other tiles can be accessed through the 'board' instance
        if (board.TryGetTile(new Vector2Int(2, 1), out Tile otherTile)) {
            
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