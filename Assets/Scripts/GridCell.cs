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

public class GridCell : MonoBehaviour
{
    public Vector2 gridIndex;          // vị trí trong grid (index.x = laneIndex)
    public bool cellFull = false;      // đã có card chưa
    public GameObject objectInCell;    // card hiện tại trong cell (GameObject của CardDisplay)
    public LaneKind kind = LaneKind.Ground; // loại lane (đất / nước)

    public CardDisplay attackCard;   // card of Team.Attack
    public CardDisplay defendCard;   // card of Team.Defend

    public bool IsEmptyForTeam(Team t)
    {
        return (t == Team.Attack) ? (attackCard == null) : (defendCard == null);
    }

    /// <summary>
    /// Kiểm tra card có đặt được không
    /// </summary>
    public bool CanPlace(Card card)
    {
        if (card == null) return false;

        // check lane kind for Swimming
        if (kind == LaneKind.Water && (card.attribute == null || !card.attribute.Contains(Attribute.Swim)))
            return false;

        // check slot empty for that team
        return (card.team == Team.Attack) ? (attackCard == null) : (defendCard == null);
    }

    /// <summary>
    /// Đặt card vào cell (cardDisplay là object UI đang kéo)
    /// </summary>
    // public void PlaceCard(CardDisplay cardDisplay)
    // {
    //     if (cardDisplay == null) return;

    //     // nếu cellFull (bảo vệ)
    //     if (cellFull)
    //     {
    //         Debug.LogWarning($"GridCell.PlaceCard: cell {gridIndex.x} đã đầy.");
    //         return;
    //     }

    //     objectInCell = cardDisplay.gameObject;
    //     cellFull = true;

    //     // parent card vào ô (UI) và căn giữa
    //     var rt = cardDisplay.transform as RectTransform;
    //     if (rt != null)
    //     {
    //         cardDisplay.transform.SetParent(transform, false);
    //         rt.anchoredPosition = Vector2.zero;
    //         rt.localRotation = Quaternion.identity;
    //         rt.transform.localScale = new Vector3(60f, 60f, 1f);
    //     }
    //     else
    //     {
    //         cardDisplay.transform.SetParent(transform, false);
    //         cardDisplay.transform.localPosition = Vector3.zero;
    //         cardDisplay.transform.localRotation = Quaternion.identity;
    //     }
    // }
    public void PlaceCard(CardDisplay cardDisplay)
    {
        if (cardDisplay == null) return;
        if (!CanPlace(cardDisplay.cardData)) return;

        // parent & set anchored position offset so attacker/defender appear distinct
        var rt = cardDisplay.transform as RectTransform;
        cardDisplay.transform.SetParent(transform, true); // keep world pos for smooth move
        cardDisplay.InitializeRuntime();

        if (cardDisplay.cardData.team == Team.Attack)
        {
            attackCard = cardDisplay;
            // animate move+scale to left side
            Vector3 targetWorld = transform.TransformPoint(new Vector3(0f, 0f, 0f)); // tweak offset as needed
            StartCoroutine(AnimateMoveAndScale(rt, targetWorld, new Vector3(60f,60f,1f), 0.25f));
        }
        else
        {
            defendCard = cardDisplay;
            Vector3 targetWorld = transform.TransformPoint(new Vector3(0f, 0f, 0f));
            StartCoroutine(AnimateMoveAndScale(rt, targetWorld, new Vector3(60f,60f,1f), 0.25f));
        }

        // disable raycasts on placed card
        var cg = cardDisplay.GetComponent<CanvasGroup>();
        if (cg != null) cg.blocksRaycasts = false;
    }
    private IEnumerator AnimateMove(RectTransform rt, Vector3 startPos, Vector3 targetPos,Vector3 startScale, Vector3 targetScale, float duration)
    {
        float time = 0f;
        while (time < duration)
        {
            time += Time.deltaTime;
            float t = Mathf.Clamp01(time / duration);

            // smooth step
            float smooth = t * t * (3f - 2f * t);

            rt.position = Vector3.Lerp(startPos, targetPos, smooth);
            rt.localScale = Vector3.Lerp(startScale, targetScale, smooth);

            yield return null;
        }

        // đảm bảo chốt đúng vị trí cuối
        rt.position = targetPos;
        rt.localScale = targetScale;
        rt.localRotation = Quaternion.identity;
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

    public void RemoveAttackCard(bool destroy=true)
    {
        if (attackCard != null)
        {
            if (destroy) Destroy(attackCard.gameObject);
            else attackCard.transform.SetParent(null);
            attackCard = null;
        }
    }

    public void RemoveDefendCard(bool destroy=true)
    {
        if (defendCard != null)
        {
            if (destroy) Destroy(defendCard.gameObject);
            else defendCard.transform.SetParent(null);
            defendCard = null;
        }
    }

    // helper coroutine used above
    private System.Collections.IEnumerator AnimateMoveAndScale(RectTransform rt, Vector3 targetWorld, Vector3 targetScale, float duration)
    {
        if (rt == null) yield break;
        Vector3 startPos = rt.position;
        Vector3 startScale = rt.localScale;
        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            float p = Mathf.SmoothStep(0f,1f, t / duration);
            rt.position = Vector3.Lerp(startPos, targetWorld, p);
            rt.localScale = Vector3.Lerp(startScale, targetScale, p);
            yield return null;
        }
        rt.position = targetWorld;
        rt.localScale = targetScale;
    }

    // helper
    public bool IsLaneEmpty() => attackCard == null && defendCard == null;
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
