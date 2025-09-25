using UnityEngine;

public class GridCell : MonoBehaviour
{
    public Vector2 gridIndex;
    public bool cellFull = false;
    public GameObject objectInCell;

    // --- NEW: check can place object
    public bool CanPlace(GameObject obj)
    {
        return !cellFull;
    }

    // --- NEW: place object into cell
    public void PlaceObject(GameObject obj)
    {
        if (cellFull) return;
        objectInCell = obj;
        obj.transform.position = transform.position;
        obj.transform.SetParent(transform);
        cellFull = true;
    }

    // --- NEW: remove object
    public void RemoveObject(bool destroy = true)
    {
        if (objectInCell == null) return;
        if (destroy) Destroy(objectInCell);
        else objectInCell.transform.SetParent(null);
        objectInCell = null;
        cellFull = false;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
