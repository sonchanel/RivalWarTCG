using UnityEngine;

public class Lane : MonoBehaviour
{
    public int laneIndex;
    public LaneKind kind;

    public GridCell playerCell; // nơi player local đặt
    public GridCell enemyCell;  // nơi hiển thị đối thủ

    public void Init(int index, LaneKind kind, Transform playerSlot, Transform enemySlot)
    {
        laneIndex = index;
        this.kind = kind;

        playerCell = playerSlot.GetComponent<GridCell>();
        if (playerCell == null) playerCell = playerSlot.gameObject.AddComponent<GridCell>();
        playerCell.Init(index, kind, true);  // true = cho phép summon

        enemyCell = enemySlot.GetComponent<GridCell>();
        if (enemyCell == null) enemyCell = enemySlot.gameObject.AddComponent<GridCell>();
        enemyCell.Init(index, kind, false); // false = chỉ hiển thị
    }
}
