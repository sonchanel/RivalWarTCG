using UnityEngine;
using UnityEngine.UI;
using RivalWarCard; // để dùng CardDisplay, Card, Attribute


public class BoardManager : MonoBehaviour
{

    public Lane GetLane(int index)
    {
        if (index < 0 || lanes == null || index >= lanes.Length) return null;
        return lanes[index];
    }

    [Header("Grid UI: mỗi lane có 2 slot (PlayerSlot, EnemySlot)")]
    public Transform lanesRoot;   // parent chứa tất cả lane
    public int totalLanes = 6;

    private Lane[] lanes;

    private void Start()
    {
        CacheLanes();
    }

    void CacheLanes()
    {
        int childCount = lanesRoot.childCount;
        if (childCount < totalLanes)
        {
            Debug.LogWarning($"LanesRoot có {childCount} child nhưng totalLanes = {totalLanes}");
        }

        lanes = new Lane[totalLanes];

        for (int i = 0; i < totalLanes; i++)
        {
            if (i >= childCount) continue;

            Transform laneObj = lanesRoot.GetChild(i);

            Lane lane = laneObj.GetComponent<Lane>();
            if (lane == null) lane = laneObj.gameObject.AddComponent<Lane>();

            // giả sử laneObj có 2 child: [0] = PlayerSlot, [1] = EnemySlot
            Transform playerSlot = laneObj.GetChild(0);
            Transform enemySlot  = laneObj.GetChild(1);

            LaneKind kind = (i < 4) ? LaneKind.Ground : LaneKind.Water;
            lane.Init(i, kind, playerSlot, enemySlot);

            laneObj.name = $"Lane_{i}_{kind}";
            lanes[i] = lane;
        }
    }

    public bool TryPlaceCard(CardDisplay cardDisplay, int laneIndex, PlayerSide localSide)
    {
        if (cardDisplay == null) return false;
        if (laneIndex < 0 || laneIndex >= lanes.Length) return false;

        GridCell cell;
        if (localSide == PlayerSide.Attack)
        {
            cell = lanes[laneIndex].playerCell;
        }
        else if (localSide == PlayerSide.Defense)
        {
            cell = lanes[laneIndex].playerCell;
        }
        else
        {
            return false;
        }

        Debug.Log("Placing card on lanesRoot: " + lanesRoot.name + " cell: " + cell.transform.name + " localSide: " + localSide);

        if (cell.CanPlace(cardDisplay.cardData))
        {
            cell.PlaceCard(cardDisplay);
            return true;
        }
        return false;
    }

    public void ShowEnemyCard(CardDisplay cardDisplay, int laneIndex)
    {
        if (cardDisplay == null) return;
        if (laneIndex < 0 || laneIndex >= lanes.Length) return;

        lanes[laneIndex].enemyCell.ShowEnemyCard(cardDisplay);
    }

    public void RemoveCard(Side side, int laneIndex, bool destroyObject = true)
    {
        if (laneIndex < 0 || lanes == null || laneIndex >= lanes.Length) return;
        var lane = lanes[laneIndex];
        if (side == Side.Player && lane.playerCell != null)
            lane.playerCell.ClearCell(destroyObject);
        else if (side == Side.Enemy && lane.enemyCell != null)
            lane.enemyCell.ClearCell(destroyObject);
    }
}
