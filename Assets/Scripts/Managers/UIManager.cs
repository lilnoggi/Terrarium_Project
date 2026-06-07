using System;
using Unity.VisualScripting;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    // --- SINGLETON SCRIPT ---
    public static UIManager Instance { get; private set; }

    [Header("Managers")]
    [SerializeField] private PlayerTools _playerTools;

    [Header("Dynamic UI Generation")]
    [SerializeField] private GameObject _itemSlotPrefab;
    [SerializeField] private ItemData[] _allItemDatabase;

    [Header("Content Folders (Grid Layouts)")]
    [SerializeField] private Transform _soilContent;
    [SerializeField] private Transform _plantsContent;
    [SerializeField] private Transform _decorContent;
    [SerializeField] private Transform _foodContent;
    [SerializeField] private Transform _colonyContent;

    // --- GAME MODES ---
    public enum GameMode { Build, Dig }
    public GameMode CurrentMode = GameMode.Build;

    [Header("Mode Panels")]
    [SerializeField] private GameObject _hotbarPanel;
    [SerializeField] private GameObject _digPanel;

    // ----------------------------------------------------------------------

    private void Awake()
    {
        // If there is already a UIManager in the scene, destroy this duplicate
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        // Otherwise, continue
        Instance = this;
    }

    private void Start()
    {
        // Force the game into Build Mode
        CurrentMode = GameMode.Build;

        // Force the correct panels to turn on and off
        _hotbarPanel.SetActive(true);
        _digPanel.SetActive(false);

        GenerateHotbarButtons();
    }

    // LINK TO TROWEL BUTTON
    public void ToggleGameMode()
    {
        if (CurrentMode == GameMode.Build)
        {
            // Switch to Digging
            CurrentMode = GameMode.Dig;
            _hotbarPanel.SetActive(false);
            _digPanel.SetActive(true);
        }
        else
        {
            // Switch back to building
            CurrentMode = GameMode.Build;
            _hotbarPanel.SetActive(true);
            _digPanel.SetActive(false);
        }
    }

    private void GenerateHotbarButtons()
    {
        // Loop through every single data card
        foreach (ItemData item in _allItemDatabase)
        {
            // Figure out which folder this belongs in based on its category
            Transform targetFolder = _soilContent; // Default fallback

            if (item.Category == ItemData.ItemCategory.Plant) targetFolder = _plantsContent;
            else if (item.Category == ItemData.ItemCategory.Decor) targetFolder = _decorContent;
            else if (item.Category == ItemData.ItemCategory.Food) targetFolder = _foodContent;
            else if (item.Category == ItemData.ItemCategory.Colony) targetFolder = _colonyContent;
            // Spawn the prefab inside that specific folder
            GameObject newButton = Instantiate(_itemSlotPrefab, targetFolder);

            // Give the button its specific data (Icon, Name, etc)
            newButton.GetComponentInChildren<ItemSlot>().SetupItems(item);
        }
    }

    // --- ITEM SELECTION ---
    public void SelectItem(ItemData selectedItem)
    {
        // Tell the Player Tools to hold this item
        _playerTools.EquipItem(selectedItem);
    }

    // --- BUTTON EVENT METHODS ---
    public void OnFillSoilPressed()
    {
        EnvironmentManager.Instance.FillTerrariumWithSoil();
    }

    public void OnClearTerrariumPressed()
    {
        EnvironmentManager.Instance.ClearTerrarium();
    }

    // --- BRUSH SIZE BUTTONS ---
    public void SetSmallBrush()
    {
        _playerTools.brushRadius = 0; // 1x1 block
    }

    public void SetMediumBrush()
    {
        _playerTools.brushRadius = 2; // Roughly 5x5 circular area
    }

    public void SetLargeBrush()
    {
        _playerTools.brushRadius = 4; // Roughly 9x9 circular area
    }
}
