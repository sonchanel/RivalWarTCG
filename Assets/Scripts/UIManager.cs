using UnityEngine;

public class UIManager : MonoBehaviour {
    public static UIManager Instance;

    public GameObject attackUI;
    public GameObject defenseUI;

    public Camera attackCam;
    public Camera defenseCam;

    void Awake() { Instance = this; }

    // Helper: tắt/bật toàn bộ object và các con
    public static void SetActiveRecursively(GameObject obj, bool active)
    {
        if (obj == null) return;
        obj.SetActive(active);
        foreach (Transform child in obj.transform)
        {
            SetActiveRecursively(child.gameObject, active);
        }
    }

    public void SetupUI(PlayerSide side) {
        bool isAttack = side == PlayerSide.Attack;
        bool isDefense = side == PlayerSide.Defense;
        SetActiveRecursively(attackUI, isAttack);
        SetActiveRecursively(defenseUI, isDefense);

        // Tắt BoardManager của UI bị tắt
        var boardAT = GameObject.Find("BoardManagerAT");
        if (boardAT != null) boardAT.SetActive(isAttack);
        var boardDF = GameObject.Find("BoardManagerDF");
        if (boardDF != null) boardDF.SetActive(isDefense);

        // Helper: tắt blocksRaycasts cho toàn bộ CanvasGroup con khi tắt UI
        void SetAllCanvasGroups(GameObject ui, bool enable)
        {
            if (ui == null) return;
            var groups = ui.GetComponentsInChildren<CanvasGroup>(true);
            foreach (var cg in groups)
            {
                cg.blocksRaycasts = enable;
                cg.interactable = enable;
            }
            var raycasters = ui.GetComponentsInChildren<UnityEngine.UI.GraphicRaycaster>(true);
            foreach (var gr in raycasters)
            {
                gr.enabled = enable;
            }
        }
        SetAllCanvasGroups(attackUI, isAttack);
        SetAllCanvasGroups(defenseUI, isDefense);

        // if (attackCam != null) attackCam.gameObject.SetActive(isAttack);
        // if (defenseCam != null) defenseCam.gameObject.SetActive(isDefense);
    }
}
