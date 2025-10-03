using System.Collections.Generic;
using UnityEngine;

public class ArcRenderer : MonoBehaviour
{
    [Header("Camera dùng cho World Space Canvas")]
    public Camera renderCamera;
    public GameObject arrowPrefab;
    public GameObject dotPrefab;
    public int poolSize = 50;
    private List<GameObject> dotPool = new List<GameObject>();
    private GameObject arrowInstance;

    public float spacing = 50;
    public float arrowAngleAdjustment = 0;
    public int dotsToSkip = 1;
    private Vector3 arrowDirection;
    public float baseScreenWidth = 1920f;
    [SerializeField] private float spacingScale;

    void Start()
    {
        if (renderCamera == null) {
            var canvas = GetComponentInParent<Canvas>();
            if (canvas != null && canvas.renderMode == RenderMode.ScreenSpaceCamera)
                renderCamera = canvas.worldCamera;
        }
        Vector3 mousePos = Input.mousePosition;
        
        mousePos.z = 0f; 

        // Khởi tạo arrow ngay tại vị trí chuột
        arrowInstance = Instantiate(arrowPrefab, mousePos, Quaternion.identity, transform);
        //arrowInstance = Instantiate(arrowPrefab, transform);
        //arrowInstance.transform.localPosition = mousePos; // Vector3.zero is the same as new Vector3 (0,0,0). This also works for Vector2
        InitializeDotPool(poolSize);

        spacingScale = Screen.width / baseScreenWidth; // Scales our dot spacing based on the current width of the screen
    }

    void OnEnable()
    {
        if (arrowInstance != null)
        {
            arrowInstance.transform.position = Input.mousePosition; // hoặc mousePos
            arrowInstance.transform.rotation = Quaternion.identity;
        }
        foreach (var dot in dotPool)
        {
            dot.transform.localPosition = Vector3.zero;
            dot.SetActive(false);
        }
        spacingScale = Screen.width / baseScreenWidth;
    }

    void Update()
    {
        Vector3 mousePos = Input.mousePosition;
        // Nếu có camera (World Space), chuyển mousePos sang world position
        if (renderCamera != null)
        {
            mousePos.z = Mathf.Abs(renderCamera.transform.position.z);
            mousePos = renderCamera.ScreenToWorldPoint(mousePos);
        }
        else
        {
            mousePos.z = 0;
        }

        Vector3 startPos = transform.position;
        Vector3 midPoint = CalculateMidPoint(startPos, mousePos);

        UpdateArc(startPos, midPoint, mousePos);
        PositionAndRotateArrow(mousePos);
    }

    void UpdateArc(Vector3 start, Vector3 mid, Vector3 end)
    {
        int numDots = Mathf.CeilToInt(Vector3.Distance(start, end) / (spacing * spacingScale));

        for (int i = 0; i < numDots && i < dotPool.Count; i++)
        {
            float t = i / (float)numDots;
            t = Mathf.Clamp(t, 0f, 1f); // Ensure t stays within the range [0, 1]

            Vector3 position = QuadraticBezierPoint(start, mid, end, t);

            if (i != numDots - dotsToSkip)
            {
                dotPool[i].transform.position = position;
                dotPool[i].SetActive(true);
                Debug.Log($"Dot {i} pos: {position}");
            }
            if (i == numDots - (dotsToSkip + 1) && i - dotsToSkip + 1 >= 0)
            {
                arrowDirection = dotPool[i].transform.position;
            }
        }

        // Deactivate unused dots (fix: tắt cả dot dư, kể cả i=0)
        for (int i = numDots - dotsToSkip; i < dotPool.Count; i++)
        {
            dotPool[i].SetActive(false);
        }
    }

    void PositionAndRotateArrow(Vector3 position)
    {
        arrowInstance.transform.position = position;
        Vector3 direction = arrowDirection - position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        angle += arrowAngleAdjustment;
        arrowInstance.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward); // The same as (0,0,1)
    }
    
    Vector3 CalculateMidPoint(Vector3 start, Vector3 end)
    {
        Vector3 midpoint = (start + end) / 2;
        float arcHeight = Vector3.Distance(start, end) / 3f;
        midpoint.y += arcHeight;
        return midpoint;
    }

    Vector3 QuadraticBezierPoint(Vector3 start, Vector3 control, Vector3 end, float t)
    {
        float u = 1 - t;
        float tt = t * t;
        float uu = u * u;

        Vector3 point = uu * start;
        point += 2 * u * t * control;
        point += tt * end;
        return point;
    }

    void InitializeDotPool(int count)
    {
        for (int i = 0; i < count; i++)
        {
            GameObject dot = Instantiate(dotPrefab, Vector3.zero, Quaternion.identity, transform);
            dot.SetActive(false);
            dotPool.Add(dot);
        }
    }
}
