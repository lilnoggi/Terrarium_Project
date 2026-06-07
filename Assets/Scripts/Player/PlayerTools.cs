using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.EventSystems;
using Unity.VisualScripting;

public class PlayerTools : MonoBehaviour
{
    // --- TOOL STATES ---
    public enum ToolMode { Dig, AddDirt, PlacePlant }

    [Header("Current Tool")]
    public ToolMode currentMode = ToolMode.Dig;
    public int brushRadius = 0;

    [Header("Currently Equipped")]
    [SerializeField] private ItemData _equippedItem; // What is currently in hand

    [Header("References")]
    [SerializeField] private Tilemap _dirtTilemap;

    [Header("Decorations")]
    [SerializeField] private GameObject _plantPrefab;

    [Header("Visuals")]
    [SerializeField] private GameObject _brushCursor;

    // ---------------------------------------------------------------

    private void Update()
    {
        // SAFETY CHECK: If the mouse is hovering over any UI panel or button, stop
        // Prevents from digging a hole when something is clicked
        if (EventSystem.current.IsPointerOverGameObject())
        {
            if (_brushCursor != null)
            {
                _brushCursor.SetActive(false);
            }

            return;
        }

        // --- DIG MODE LOGIC ---
        if (UIManager.Instance.CurrentMode == UIManager.GameMode.Dig)
        {
            UpdateBrushCursor();

            // Digging and Adding Dirt use Hold/Drag
            if (Input.GetMouseButton(0))
            {
                HandleTerrainEdit(isDigging: currentMode == ToolMode.Dig);
            }
        }

        // --- BUILD MODE LOGIC ---
        else if (UIManager.Instance.CurrentMode == UIManager.GameMode.Build)
        {
            // Make sure the cursor turns off when back to building
            if (_brushCursor != null) _brushCursor.SetActive(false);

            // Placing a plant requires a single precise click
            if (Input.GetMouseButtonDown(0) && currentMode == ToolMode.PlacePlant)
            {
                SpawnPlant();
            }
        }
    }

    // --- BRUSH CURSOR LOGIC ---
    private void UpdateBrushCursor()
    {
        if (_brushCursor == null)
        {
            return;
        }

        // Ensure the cursor is visible
        _brushCursor.SetActive(true);

        // Find the mouse in the world
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0;

        // Snap the mouse position to the nearest grid cell
        Vector3Int cellPos = _dirtTilemap.WorldToCell(mouseWorldPos);
        _brushCursor.transform.position = _dirtTilemap.GetCellCenterWorld(cellPos);

        // Scale the cursor based on the radius
        // Radius 0 = 1 block. Radius 2 = 5 blocks
        float diameter = (brushRadius * 2) + 1;
        _brushCursor.transform.localScale = new Vector3(diameter, diameter, 1);
    }

    // --- UI BUTTONS (DIG PANEL) ---
    public void SetDigTool()
    {
        currentMode = ToolMode.Dig;
    }

    public void SetAddDirtTool()
    {
        currentMode = ToolMode.AddDirt;
    }

    // --- TERRAIN EDITING ---
    private void HandleTerrainEdit(bool isDigging)
    {
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0;

        // Convert world position to grid cell
        Vector3Int clickCellPosition = _dirtTilemap.WorldToCell(mouseWorldPos);

        // Tell the Grid Manager what to do
        if (isDigging)
        {
            EnvironmentManager.Instance.RemoveDirt(clickCellPosition, brushRadius);
        }
        else
        {
            EnvironmentManager.Instance.AddDirt(clickCellPosition, brushRadius);
        }
    }

    // --- EQUIPPING ---
    public void EquipItem(ItemData newItem)
    {
        _equippedItem = newItem;
        Debug.Log(_equippedItem.ItemName + "equipped.");

        // Auto switch modes based on what was picked up
        if (_equippedItem.Category == ItemData.ItemCategory.Plant || 
            _equippedItem.Category == ItemData.ItemCategory.Decor)
        {
            currentMode = ToolMode.PlacePlant;
        }
    }

    // --- PLANT PLACING ---
    private void SpawnPlant()
    {
        // SAFETY CHECK: Make sure something is being held AND it has a prefab
        if (_equippedItem == null || _equippedItem.PrefabToSpawn == null) return;

        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0;
        Vector3Int clickCellPosition = _dirtTilemap.WorldToCell(mouseWorldPos);

        // Ask the GridManager where the top of the dirt is in this column
        Vector3Int surfaceCell = EnvironmentManager.Instance.GetSurfaceCell(clickCellPosition.x);

        // SAFETY CHECK: Ensure player is not clicking outside the jar
        if (surfaceCell.x != -99)
        {
            // Get the precide world coordinate of the surface block
            Vector3 surfaceWorldPos = _dirtTilemap.GetCellCenterWorld(surfaceCell);

            // --- SURFACE CHECK ---
            // How far away (vertically) is the mouse from the actual dirt line?
            float distanceToSurface = Mathf.Abs(mouseWorldPos.y - surfaceWorldPos.y); 
            
            if (distanceToSurface <= 3)
            {
                // Smooth X (Mouse), Snapped Y (Surface)
                Vector3 spawnPosition = new Vector3(mouseWorldPos.x, surfaceWorldPos.y - 0.2f, 0);

                // Spawn the actual specific GameObject prefab from the data
                Instantiate(_equippedItem.PrefabToSpawn, spawnPosition, Quaternion.identity);
            }
            else
            {
                Debug.Log("Too far from the surface to plant.");
            }
        }
    }
}
