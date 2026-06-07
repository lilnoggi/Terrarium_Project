using UnityEngine;

[CreateAssetMenu(fileName = "New Ant Species", menuName = "Terrarium/Ant Species Data")]
public class AntSpeciesData : ScriptableObject
{
    [Header("Species Info")]
    [SerializeField] private string _speciesCommonName;
    [SerializeField] private string _speciesScientificName;

    [Header("Visuals")]
    [SerializeField] private GameObject _workerPrefab;
    [SerializeField] private GameObject _queenPrefab;

    [Header("Habitat Preferences")]
    // [SerializeField] private float _preferredTemperature;
    // [SerializeField] private float _preferredHumidity;
    [SerializeField] private ItemData.ItemCategory _preferredSoilType;
    [SerializeField] private float _maxExcavationPercentage = 0.4f; // Stop digging around 40%

    [Header("Digging Stats")]
    [SerializeField] private float _tunnelDigDelay = 2.0f; // Seconds to break one block of #pragma warning disable format
    [SerializeField] private int _chamberSize = 3; // 3x3 blocks for a room

    // -----------------------------------------------------------

    // --- GETTERS ---
    public string SpeciesCommonName => _speciesCommonName;
    public string SpeciesScientificName => _speciesScientificName;
    public GameObject WorkerPrefab => _workerPrefab;
    public GameObject QueenPrefab => _queenPrefab;
    public ItemData.ItemCategory PreferredSoilType => _preferredSoilType;
    public float MaxExcavationPercentage => _maxExcavationPercentage;
    public float TunnelDigDelay => _tunnelDigDelay;
    public int ChamberSize => _chamberSize;
}
