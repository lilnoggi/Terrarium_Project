using UnityEditor.EditorTools;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// IPointerEnterHandler & IPointerExitHandler used to detect
/// when the mouse enters or leaves the UI     
/// </summary>
public class AutoHideUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private RectTransform _rectTransform;

    [Header("Slide Settings")]
    [Tooltip("The Y position when the hotbar is up and visible")]
    [SerializeField] private float _shownYPosition = 0f;
    
    [Tooltip("The Y position when hidden (make negative)")]
    [SerializeField] private float _hiddenYPosition = -120f;
    
    [Tooltip("How fast it slides up and down")]
    [SerializeField] private float _slideSpeed = 12f;

    private float _targetYPosition;

    // --------------------------------------------------------------

    private void Start()
    {
        _rectTransform = GetComponent<RectTransform>();

        // Start the game with the hotbar hidden
        _targetYPosition = _hiddenYPosition;
    }

    private void Update()
    {
        // Smoothly glide the UI towards the target Y position every frame
        Vector2 currentPos = _rectTransform.anchoredPosition;
        float newY = Mathf.Lerp(currentPos.y, _targetYPosition, Time.deltaTime * _slideSpeed);
        _rectTransform.anchoredPosition = new Vector2(currentPos.x, newY);
    }

    // When the mouse touches the UI...
    public void OnPointerEnter(PointerEventData eventData)
    {
        _targetYPosition = _shownYPosition;
    }

    // When the mouse leaves the UI...
    public void OnPointerExit(PointerEventData eventData)
    {
        _targetYPosition = _hiddenYPosition;
    }
}