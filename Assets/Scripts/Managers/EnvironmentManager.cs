using UnityEngine;
using UnityEngine.Tilemaps;

public class EnvironmentManager : MonoBehaviour
{
    // --- SINGLETON SCRIPT ---
    public static EnvironmentManager Instance { get; private set; }

    [Header("Grid Dimensions")]
    [SerializeField] private int _width = 30;
    [SerializeField] private int _height = 20;

    [Header("Terrain Generation")]
    [SerializeField] private float _bumpiness = 0.1f; // How wide the hills are
    [SerializeField] private float _hillHeight = 5f; // How tall the hills are
    [SerializeField] private int _baseHeight = 10; // The minimum height of the dirt

    [Header("Tilemap Setup")]
    [SerializeField] private Tilemap _dirtTilemap;
    [SerializeField] private RuleTile _terrainRuleTile;

    [Header("Tunnel Visuals")]
    [SerializeField] private Tilemap _tunnelTilemap;
    [SerializeField] private TileBase _tunnelBackgroundTile;

    // This is the invisible map that the Ants will read
    // 0 = Air, 1 = Grass, 2 = Light Dirt, 3 = Deep Dirt
    [SerializeField] private int[,] _grid;

    // Safety lock
    private bool _hasSoil = false;

    // ----------------------------------------------------------------------------------------------

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

    private void Start()
    {
        // Initialise the empty array
        _grid = new int[_width, _height];
    }

    // The Invisible array (math part)
    public void FillTerrariumWithSoil()
    {
        // SAFETY LOCK: If there is already soil in the jar, stop the code
        if (_hasSoil) return;

        // Pick a random seed so the terrarium is unique every time Play is pressed
        float surfaceSeed = Random.Range(0f, 10000f);

        // Read across the terrarium from left to right (X axis)
        for (int x = 0; x < _width; x++)
        {
            // Calculate the wavy surface level
            float surfaceNoise = Mathf.PerlinNoise(x * _bumpiness + surfaceSeed, 0);
            int surfaceLevel = Mathf.RoundToInt(_baseHeight + (surfaceNoise * _hillHeight));

            // Read from the bottom to the top of this column (Y axis)
            for (int y = 0; y < _height; y++)
            {
                if (y > surfaceLevel)
                {
                    _grid[x, y] = 0; // Air
                }
                else
                {
                    _grid[x, y] = 1; // Dirt
                }
            }
        }

        DrawTiles();

        // Lock the jar so no more soil can be spawned
        _hasSoil = true;
    }

    // Reads the invisible array and paints the actual tiles
    private void DrawTiles()
    {
        for (int x = 0; x < _width; x++)
        {
            for (int y = 0; y < _height; y++)
            {
                if (_grid[x, y] == 1)
                {
                    _dirtTilemap.SetTile(new Vector3Int(x, y, 0), _terrainRuleTile);
                }
            }
        }
    }

    // --- SURFACE DETECTION LOGIC ---
    // This looks down a column and finds the first empty air block right above the dirt
    public Vector3Int GetSurfaceCell(int x)
    {
        // SAFETY CHECK: Did the player click completely outside the jar?
        if (x < 0 || x >= _width)
        {
            return new Vector3Int(-99, -99, 0);
        }

        // Start at the very top of the jar and look down
        for (int y = _height - 1; y >= 0; y--)
        {
            // The moment something is hit that IS NOT air (0)
            if (_grid[x, y] != 0)
            {
                // Return the coordinate of the air block directly ABOVE it
                return new Vector3Int(x, y + 1, 0);
            }
        }

        // Fallback in case the column is completely empty
        return new Vector3Int(x, 0, 0);
    }

    // --- TERRAIN EDITING LOGIC ---
    public void RemoveDirt(Vector3Int centerCell, int radius)
    {
        // Loop through a square area around the clicked cell
        for (int x = -radius; x <= radius; x++)
        {
            for (int y = -radius; y <= radius; y++)
            {
                // Check if this specific tile is within a circle
                if (x * x + y * y <= radius * radius)
                {
                    Vector3Int targetCell = new Vector3Int(centerCell.x + x, centerCell.y + y, 0);

                    // SAFETY CHECK: Still inside the jar?
                    if (targetCell.x >= 0 && targetCell.x < _width && targetCell.y >= 0 && targetCell.y < _height)
                    {
                        if (_grid[targetCell.x, targetCell.y] != 0)
                        {
                            _grid[targetCell.x, targetCell.y] = 0;
                            _dirtTilemap.SetTile(targetCell, null);

                            // Paint the dark background tunnel wall
                            _tunnelTilemap.SetTile(targetCell, _tunnelBackgroundTile);
                        }
                    }
                }
            }
        }
    }

    public void AddDirt(Vector3Int centerCell, int radius)
    {
        for (int x = -radius; x <= radius; x++)
        {
            for (int y = -radius; y <= radius; y++)
            {
                if (x * x + y * y <= radius * radius)
                {
                    Vector3Int targetCell = new Vector3Int(centerCell.x + x, centerCell.y + y, 0);

                    if (targetCell.x >= 0 && targetCell.x < _width && targetCell.y >= 0 && targetCell.y < _height)
                    {
                        if (_grid[targetCell.x, targetCell.y] == 0)
                        {
                            _grid[targetCell.x, targetCell.y] = 1;
                            _dirtTilemap.SetTile(targetCell, _terrainRuleTile);
                        }
                    }
                }
            }
        }
    }

    // --- CLEAR THE JAR ---
    public void ClearTerrarium()
    {
        // Reset the math array to 0 (Air)
        for (int x = 0; x < _width; x++)
        {
            for (int y = 0; y < _height; y++)
            {
                _grid[x, y] = 0;
            }
        }

        // Clear the visual dirt tiles
        _dirtTilemap.ClearAllTiles();

        // Destroy anything the player spawned
        GameObject[] plants = GameObject.FindGameObjectsWithTag("Decor");
        foreach (GameObject plant in plants)
        {
            Destroy(plant);
        }

        // Unlock the jar so soil can be spawned again
        _hasSoil = false;
    }

    public Tilemap GetDirtTilemap()
    {
        return _dirtTilemap;
    }
}
