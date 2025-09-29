using UnityEngine;
using System.Linq;
using RivalWarCard; // để dùng Card, Attribute
using DG.Tweening;
using System.Collections;


public enum LaneKind
{
    Ground,
    Water
}
public enum Side
{
    Player,
    Enemy
}

public class GridCell : MonoBehaviour
{
    public int laneIndex;
    public LaneKind kind;

    private CardDisplay currentCard;

    public void Init(int lane, LaneKind kind, bool canSummon)
    {
        this.laneIndex = lane;
        this.kind = kind;
    }

    public bool CanPlace(Card cardData)
    {
    // Đã có card thì không cho đặt
    if (currentCard != null) return false;
    // Nếu là lane nước thì card phải có thuộc tính Swim
    if (kind == LaneKind.Water && (cardData.attribute == null || !cardData.attribute.Contains(Attribute.Swim))) return false;
    // Nếu là lane đất thì luôn cho phép (không cần kiểm tra attribute)
    return true;
    }

    public void PlaceCard(CardDisplay display)
    {
        currentCard = display;
        display.transform.SetParent(transform, false);
        display.transform.localPosition = Vector3.zero;
        display.transform.localScale = new Vector3(60f,60f,1f);
    }

    public void ShowEnemyCard(CardDisplay display)
    {
        // bỏ check summon → vì chỉ hiển thị
        currentCard = display;
        display.transform.SetParent(transform, false);
    }

    public void ClearCell(bool destroyObject = true)
    {
        if (currentCard != null)
        {
            if (destroyObject) Destroy(currentCard.gameObject);
            else currentCard.transform.SetParent(null);
            currentCard = null;
        }
    }
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
