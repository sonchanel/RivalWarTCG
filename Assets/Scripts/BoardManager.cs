using UnityEngine;
using UnityEngine.UI;
using RivalWarCard; // để dùng CardDisplay, Card, Attribute


public class BoardManager : MonoBehaviour
{
    public static BoardManager Instance;

    [Header("Grid UI chứa các ô (mỗi ô = 1 làn)")]
    public GridLayoutGroup grid; // gán trong Inspector: object có GridLayoutGroup với đúng số ô (children)
    public int totalLanes = 6;   // mặc định 6 (4 ground + 2 water)

    private GridCell[] lanes;
    private LaneKind[] laneKinds;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        // thiết lập loại lane: 4 Ground + 2 Water (có thể sửa trong Inspector nếu muốn)
        laneKinds = new LaneKind[totalLanes];
        for (int i = 0; i < totalLanes; i++)
        {
            laneKinds[i] = (i < 4) ? LaneKind.Ground : LaneKind.Water;
        }

        lanes = new GridCell[totalLanes];
    }

    private void Start()
    {
        CacheGridCells();
    }

    void CacheGridCells()
    {
        if (grid == null)
        {
            Debug.LogError("BoardManager: Chưa gán GridLayoutGroup (grid). Vui lòng gán object chứa các ô vào field 'grid'.");
            return;
        }

        int childCount = grid.transform.childCount;
        if (childCount < totalLanes)
        {
            Debug.LogWarning($"BoardManager: Grid có {childCount} child nhưng totalLanes = {totalLanes}. Những lane vượt quá childCount sẽ bị null.");
        }

        for (int i = 0; i < totalLanes; i++)
        {
            if (i >= childCount)
            {
                lanes[i] = null;
                continue;
            }

            Transform t = grid.transform.GetChild(i);
            GridCell cell = t.GetComponent<GridCell>();
            if (cell == null)
            {
                // thêm component GridCell nếu chưa có
                cell = t.gameObject.AddComponent<GridCell>();
            }

            // gán thuộc tính lane
            cell.kind = (i < 4) ? LaneKind.Ground : LaneKind.Water;
            cell.gridIndex = new Vector2(i, 0);

            // rename object cho dễ debug
            t.name = $"Lane_{i}_{cell.kind}";

            lanes[i] = cell;
        }
    }

    // trả về GridCell (lane)
    public GridCell GetLane(int index)
    {
        if (index < 0 || index >= lanes.Length) return null;
        return lanes[index];
    }

    // số lane hiện tại (dựa trên cấu hình)
    public int LaneCount => lanes != null ? lanes.Length : 0;

    // Thử đặt card (CardDisplay) vào laneIndex
    public bool TryPlaceCard(CardDisplay cardDisplay, int laneIndex)
    {
        if (cardDisplay == null)
        {
            Debug.LogError("TryPlaceCard: cardDisplay = null");
            return false;
        }

        GridCell lane = GetLane(laneIndex);
        if (lane == null)
        {
            // lane không tồn tại (ví dụ grid child < totalLanes)
            Debug.Log($"TryPlaceCard: Lane {laneIndex} không tồn tại.");
            return false;
        }

        if (lane.CanPlace(cardDisplay.cardData))
        {
            lane.PlaceCard(cardDisplay);
            Debug.Log($"Placed {cardDisplay.cardData.cardName} at Lane {laneIndex} ({lane.kind})");
            return true;
        }
        else
        {
            Debug.Log($"{cardDisplay.cardData.cardName} không thể đặt vào lane {lane.kind} hoặc lane đã đầy.");
            return false;
        }
    }

    // remove card khỏi lane (gọi khi unit chết hoặc rollback)
    public void RemoveCardFromLane(int laneIndex, bool destroyObject = true)
    {
        GridCell lane = GetLane(laneIndex);
        if (lane != null) lane.ClearCell(destroyObject);
    }
}
