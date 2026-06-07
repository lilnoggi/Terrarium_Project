using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "New Item", menuName = "Terrarium/ItemData")]
public class ItemData : ScriptableObject
{
    [Header("UI Info")]
    [SerializeField] private string _itemName;
    [SerializeField] private Sprite _itemIcon;

    public enum ItemCategory { Soil, Plant, Decor, Food, Colony }
    [SerializeField] private ItemCategory _category;
    
    [Header("Placement Data")]
    [SerializeField] private GameObject _prefabToSpawn;
    [SerializeField] private TileBase _tileToPaint;

    [Header("Simulation Stats (LATER)")]
    [SerializeField] private int _happinessBoost = 0;
    [SerializeField] private int _foodValue = 0;

    // -----------------------------------------------------------

    // --- GETTERS ---
    public string ItemName => _itemName;
    public Sprite ItemIcon => _itemIcon;
    public ItemCategory Category => _category;
    public GameObject PrefabToSpawn => _prefabToSpawn;
    public TileBase TileToPaint => _tileToPaint;
    public int HappinessBoost => _happinessBoost;
    public int FoodValue => _foodValue;
}
