using UnityEngine;

public class FormicariumManager : MonoBehaviour
{
    public static FormicariumManager Instance { get; private set; }

    public bool hasColonySpawned = false;

    // --------------------------------------------------------------------

    private void Awake()
    {
        // Singleton Instance
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        Instance = this;
    }

    // The ItemSlot script will call this when a Test Tube is dropped on the jar
    public void SpawnStarterColony(Vector3 dropPosition, GameObject testTubePrefab)
    {
        if (hasColonySpawned)
        {
            Debug.Log("A colony already lives here.");
            return;
        }

        // Ask the EnvironmentManager where the top of the dirt is.
        Vector3Int cellPos = EnvironmentManager.Instance.GetDirtTilemap().WorldToCell(dropPosition);
        Vector3Int surfaceCell = EnvironmentManager.Instance.GetSurfaceCell(cellPos.x);

        if (surfaceCell.x != -99)
        {
            // Calculate exactly where the grass is
            Vector3 spawnPos = EnvironmentManager.Instance.GetDirtTilemap().GetCellCenterWorld(surfaceCell);
            // spawnPos.y += 0.5f; // Raise up slightly so the ants don't get stuck

            // Spawn the Test Tube
            Instantiate(testTubePrefab, spawnPos, Quaternion.identity);

            hasColonySpawned = true;
            Debug.Log("The colony has been born!");
        }
    }
}
