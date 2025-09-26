using UnityEngine;
using UnityEngine.EventSystems;
using RivalWarCard;

public class CardMovement : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
{
    private RectTransform rectTransform;
    private Canvas canvas;
    private Vector2 originalLocalPointerPosition;
    private Vector3 originalPanelLocalPosition;
    private Vector3 originalScale;
    private int currentState = 0;
    private Quaternion originalRotation;
    private Vector3 originalPosition;
    [SerializeField] private float selectScale = 1.1f;
    [SerializeField] private Vector2 cardPlay;
    [SerializeField] private Vector3 playPosition;
    [SerializeField] private GameObject glowEffect;
    [SerializeField] private GameObject playArrow;
    [SerializeField] private float lerpFator = 0.1f;

    // --- NEW: reference to CardDisplay on same object
    private CardDisplay cardDisplay;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        originalScale = rectTransform.localScale;
        originalPosition = rectTransform.localPosition;
        originalRotation = rectTransform.localRotation;

        cardDisplay = GetComponent<CardDisplay>();
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        switch (currentState)
        {
            case 1:
                HandleHoverState();
                break;
            case 2:
                HandleDrageState();
                if (!Input.GetMouseButton(0))
                {
                    // khi nhả chuột sau drag: cố gắng đặt nếu đang ở Play zone (y > cardPlay.y)
                    if (rectTransform.localPosition.y > cardPlay.y)
                        TryPlaceOnLane();
                    else
                        TransitionToState0();
                }
                break;
            case 3:
                HandlePlayState();
                if (!Input.GetMouseButton(0))
                {
                    // khi nhả chuột ở trạng thái play: thực hiện đặt
                    TryPlaceOnLane();
                }
                break;
        }
    }
    private void TransitionToState0()
    {
        currentState = 0;
        rectTransform.localScale = originalScale;
        rectTransform.localPosition = originalPosition;
        rectTransform.localRotation = originalRotation;
        if (glowEffect != null) glowEffect.SetActive(false);
        if (playArrow != null) playArrow.SetActive(false);

    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (currentState == 0)
        {
            originalPosition = rectTransform.localPosition;
            originalRotation = rectTransform.localRotation;
            originalScale = rectTransform.localScale;

            currentState = 1;
        }
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        if (currentState == 1)
        {
            TransitionToState0();
        }
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        if (currentState == 1)
        {
            currentState = 2;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.GetComponent<RectTransform>(), eventData.position, eventData.pressEventCamera, out originalLocalPointerPosition);
            originalPanelLocalPosition = rectTransform.localPosition;
        }
    }
    public void OnDrag(PointerEventData eventData)
    {
        if (currentState == 2)
        {
            Vector2 localPointerPosition;
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.GetComponent<RectTransform>(), eventData.position, eventData.pressEventCamera, out localPointerPosition))
            {
                rectTransform.position = Vector3.Lerp(rectTransform.position,Input.mousePosition,lerpFator);

                if (rectTransform.localPosition.y > cardPlay.y)
                {
                    currentState = 3;
                    if (playArrow != null) playArrow.SetActive(true);
                    rectTransform.localPosition = Vector3.Lerp(rectTransform.position,playPosition,lerpFator);
                }
            }
        }
    }
    private void HandleHoverState()
    {
        if (glowEffect != null) glowEffect.SetActive(true);
        rectTransform.localScale = originalScale * selectScale;

    }
    private void HandleDrageState()
    {
        rectTransform.localRotation = Quaternion.identity;
    }
    private void HandlePlayState()
    {
        rectTransform.localPosition = Vector3.Lerp(rectTransform.localPosition, playPosition, lerpFator);
        rectTransform.localRotation = Quaternion.identity;

        if (Input.mousePosition.y < cardPlay.y)
        {
            currentState = 2;
            if (playArrow != null) playArrow.SetActive(false);

            rectTransform.position = Vector3.Lerp(rectTransform.position, Input.mousePosition, lerpFator);
        }
    }

    // --- REPLACE only the TryPlaceOnLane() method in your CardMovement.cs file with this implementation ---
    private void TryPlaceOnLane()
    {
        // đảm bảo có BoardManager và cardDisplay
        if (BoardManager.Instance == null || cardDisplay == null)
        {
            TransitionToState0();
            return;
        }

        Vector2 mousePos = Input.mousePosition;
        Camera cam = (canvas != null) ? canvas.worldCamera : Camera.main;

        // duyệt qua các lane đã cache trong BoardManager
        int lanesCount = BoardManager.Instance.LaneCount;
        for (int i = 0; i < lanesCount; i++)
        {
            GridCell cell = BoardManager.Instance.GetLane(i);
            if (cell == null) continue;

            RectTransform laneRect = cell.GetComponent<RectTransform>();
            if (laneRect == null) continue;

            // kiểm tra con trỏ chuột có nằm trong rect của lane này không
            if (RectTransformUtility.RectangleContainsScreenPoint(laneRect, mousePos, cam))
            {
                // thử đặt card qua BoardManager (BoardManager sẽ gọi GridCell.CanPlace & PlaceCard)
                bool placed = BoardManager.Instance.TryPlaceCard(cardDisplay, i);
                if (placed)
                {
                    // nếu đặt thành công -> remove khỏi hand
                    var hand = UnityEngine.Object.FindFirstObjectByType<HandManager>();
                    if (hand != null)
                    {
                        hand.RemoveCardFromHand(this.gameObject);
                    }

                    // disable tương tác kéo trên card đã đặt
                    var cg = GetComponent<CanvasGroup>();
                    if (cg != null) cg.blocksRaycasts = false;

                    currentState = 0;
                    return;
                }
                else
                {
                    // Lane bị chặn vì loại lane / đã đầy -> break (đã hit lane chứa chuột)
                    break;
                }
            }
        }

        // nếu không đặt được ở lane nào -> trả về vị trí cũ (hand)
        TransitionToState0();
    }

}
