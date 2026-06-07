using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ItemSlot : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] private Image _iconImage;
    private ItemData _itemData;

    private Transform _originalParent;
    private CanvasGroup _canvasGroup;

    // -----------------------------------------------

    private void Start()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
        if (_canvasGroup == null)
        {
            _canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
    }

    // UIManager will call this
    public void SetupItems(ItemData data)
    {
        _itemData = data;
        _iconImage.sprite = data.ItemIcon;
    }

    public ItemData GetItemData()
    {
        return _itemData;
    }

    // --- STANDARD CLICKING (Plants/Decor) ---
    public void OnClick_SelectItem()
    {
        // Tell the UIManager what was clicked
        UIManager.Instance.SelectItem(_itemData);
    }

    // --- DRAGGING LOGIC (Soil) ---
    public void OnBeginDrag(PointerEventData eventData)
    {
        // Only allow dragging if it is a soil category OR Colony
        if (_itemData.Category != ItemData.ItemCategory.Soil &&
            _itemData.Category != ItemData.ItemCategory.Colony) return;

        _originalParent = transform.parent; // Remember the folder

        // Move the icon to the very top layer so it doesn't hide behind other UI
        transform.SetParent(GetComponentInParent<Canvas>().transform);
        transform.SetAsLastSibling();

        // Turn off the icon's solidness so the Jar can "feel" the mouse underneath
        _canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (_itemData.Category != ItemData.ItemCategory.Soil &&
            _itemData.Category != ItemData.ItemCategory.Colony) return;

        // Make the icon follow the mouse
        transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (_itemData.Category != ItemData.ItemCategory.Soil &&
            _itemData.Category != ItemData.ItemCategory.Colony) return;

        // Snap the icon back to its folder when let go
        transform.SetParent(_originalParent);

        // Force the icon to center itself inside the UI Item Box
        transform.localPosition = Vector3.zero;

        _canvasGroup.blocksRaycasts = true; // Make jar solid again

        // Translate the screen mouse click into a 2D World Coordinate
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0; // Ignore the Z axis since this is a 2D game
        Vector2 mousePos2D = new Vector2(mouseWorldPos.x, mouseWorldPos.y);

        // Shoow a tiny laser directly into the screen at that exact pixel
        RaycastHit2D hit = Physics2D.Raycast(mousePos2D, Vector2.zero);

        // Did the laser hit anything?
        if (hit.collider != null)
        {
            // Check the tag. Was the Terrarium hit?
            if (hit.collider.CompareTag("Terrarium"))
            {
                // Was SOIL dropped?
                if (_itemData.Category == ItemData.ItemCategory.Soil)
                {
                    // Tell the Environment Manager to add soil at this location
                    EnvironmentManager.Instance.FillTerrariumWithSoil();
                }
                // Was COLONY dropped?
                else if (_itemData.Category == ItemData.ItemCategory.Colony)
                {
                    // Tell the Formicarium Manager to spawn the starter colony test tube at this location
                    FormicariumManager.Instance.SpawnStarterColony(mouseWorldPos, _itemData.PrefabToSpawn);
                }
            }
        }
    }
}
