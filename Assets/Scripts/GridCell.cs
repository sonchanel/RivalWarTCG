using UnityEngine;
using System.Linq;
using RivalWarCard; // để dùng Card, Attribute

public enum LaneKind
{
    Ground,
    Water
}

public class GridCell : MonoBehaviour
{
    public Vector2 gridIndex;          // vị trí trong grid (index.x = laneIndex)
    public bool cellFull = false;      // đã có card chưa
    public GameObject objectInCell;    // card hiện tại trong cell (GameObject của CardDisplay)
    public LaneKind kind = LaneKind.Ground; // loại lane (đất / nước)

    /// <summary>
    /// Kiểm tra card có đặt được không
    /// </summary>
    public bool CanPlace(Card card)
    {
        if (cellFull) return false;
        if (card == null) return false;

        if (kind == LaneKind.Ground)
            return true;

        if (kind == LaneKind.Water)
            return (card.attribute != null && card.attribute.Contains(Attribute.Swim));

        return false;
    }

    /// <summary>
    /// Đặt card vào cell (cardDisplay là object UI đang kéo)
    /// </summary>
    public void PlaceCard(CardDisplay cardDisplay)
    {
        if (cardDisplay == null) return;

        // nếu cellFull (bảo vệ)
        if (cellFull)
        {
            Debug.LogWarning($"GridCell.PlaceCard: cell {gridIndex.x} đã đầy.");
            return;
        }

        objectInCell = cardDisplay.gameObject;
        cellFull = true;

        // parent card vào ô (UI) và căn giữa
        var rt = cardDisplay.transform as RectTransform;
        if (rt != null)
        {
            cardDisplay.transform.SetParent(transform, false);
            rt.anchoredPosition = Vector2.zero;
            rt.localRotation = Quaternion.identity;
        }
        else
        {
            cardDisplay.transform.SetParent(transform, false);
            cardDisplay.transform.localPosition = Vector3.zero;
            cardDisplay.transform.localRotation = Quaternion.identity;
        }
    }

    /// <summary>
    /// Xoá card khỏi cell
    /// </summary>
    public void ClearCell(bool destroyObject = true)
    {
        if (objectInCell != null)
        {
            if (destroyObject)
            {
                Destroy(objectInCell);
            }
            else
            {
                // unparent và reset transform
                objectInCell.transform.SetParent(null);
            }
        }

        objectInCell = null;
        cellFull = false;
    }

    // helper
    public bool IsEmpty() => !cellFull;
}









// using UnityEngine;
// using System.Linq; // để dùng Contains
// using RivalWarCard; // để dùng Card, Attribute

// public enum LaneKind
// {
//     Ground,
//     Water
// }
// public class GridCell : MonoBehaviour
// {
//     public Vector2 gridIndex;
//     public bool cellFull = false;
//     public GameObject objectInCell;
//     public LaneKind kind = LaneKind.Ground; // Loại lane (đất / nước)

//     // --- NEW: check can place object
//     public bool CanPlace(GameObject obj)
//     {
//         return !cellFull;
//     }

//     // --- NEW: place object into cell
//     public void PlaceObject(GameObject obj)
//     {
//         if (cellFull) return;
//         objectInCell = obj;
//         obj.transform.position = transform.position;
//         obj.transform.SetParent(transform);
//         cellFull = true;
//     }

//     // --- NEW: remove object
//     public void RemoveObject(bool destroy = true)
//     {
//         if (objectInCell == null) return;
//         if (destroy) Destroy(objectInCell);
//         else objectInCell.transform.SetParent(null);
//         objectInCell = null;
//         cellFull = false;
//     }

//     // Start is called once before the first execution of Update after the MonoBehaviour is created
//     void Start()
//     {

//     }

//     // Update is called once per frame
//     void Update()
//     {

//     }
// }
