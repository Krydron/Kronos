/**************************************************************************************************************
* <Name> Class
*
* The header file for the <Name> class.
* 
* This class 
* 
*
* Created by: <Owen Clifton> 
* Date: <need to add>
*
***************************************************************************************************************/

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public enum MapType
{
    Map1,
    Map2,
    Map3,
}
public class DrawUI : MonoBehaviour
{
    [SerializeField] MapType map;
    private GameObject drawMap;
    private GameObject drawAreaCamera;
    private GameObject lineRenderer;
    private List<GameObject> lineRendererList = new List<GameObject>();
    RawImage renderTexture;
    Vector3 pointInDrawArea;

    //[SerializeField] private GameObject debugCube;

    private InputAction mouseClickAction;
    private InputAction keyPressAction;

    bool drawing;

    Transform parent;

    void OnEnable()
    {
        //TODO: change to OnInteract()
        mouseClickAction = new InputAction(type: InputActionType.Button, binding: "<Mouse>/leftButton");
        mouseClickAction.Enable();

        keyPressAction = new InputAction(type: InputActionType.Button, binding: "<Keyboard>/Delete");
        keyPressAction.performed += ctx => {
            //currentPoints.Clear();
            foreach (GameObject line in lineRendererList)
            {
                Destroy(line);
            }
        };
        keyPressAction.Enable();

        switch (map)
        {
            case MapType.Map1:
                drawMap = GameObject.Find("DrawMap1");
                break;
            case MapType.Map2:
                drawMap = GameObject.Find("DrawMap2");
                break;
            case MapType.Map3:
                drawMap = GameObject.Find("DrawMap3");
                break;
            default:
                break;
        }
        if (drawMap == null) { Debug.Log("Draw Map is null"); }
        drawAreaCamera = drawMap.transform.Find("Camera").gameObject;
        lineRenderer = drawMap.transform.Find("Line").gameObject;
    }

    void OnDisable()
    {
        mouseClickAction.Disable();
        keyPressAction.Disable();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        renderTexture = GetComponent<RawImage>();
        drawing = false;
        parent = lineRenderer.transform.parent;
    }

    void DrawLine()
    {
        Vector3 parentPos = parent.position;
        lineRendererList[lineRendererList.Count - 1].GetComponent<LineRenderer>().positionCount++;
        lineRendererList[lineRendererList.Count - 1].GetComponent<LineRenderer>().SetPosition(lineRendererList[lineRendererList.Count - 1].GetComponent<LineRenderer>().positionCount - 1, new Vector3(pointInDrawArea.x-parentPos.x, 0, pointInDrawArea.z-parentPos.z));
    }

    void UpdateMouse()
    {
        if (drawMap == null) { Debug.Log("Draw Map is null"); }
        if (drawAreaCamera == null) { Debug.Log("Draw Area Camera is null"); return; }
        RectTransform rectTransform = renderTexture.rectTransform;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, Input.mousePosition, null, out Vector2 localPoint);

        Vector2 viewportPosition = new(
            (localPoint.x + rectTransform.rect.width * 0.5f) / rectTransform.rect.width,
            (localPoint.y + rectTransform.rect.height * 0.5f) / rectTransform.rect.height
        );


        pointInDrawArea = drawAreaCamera.GetComponent<Camera>().ViewportToWorldPoint(new(viewportPosition.x, viewportPosition.y, 5));
        //debugCube.transform.position = pointInDrawArea;
    }

    void Update()
    {
        UpdateMouse();
        if (drawing) { DrawLine(); }
        
    }

    public void Clear()
    {
        foreach (GameObject line in lineRendererList) { Destroy(line); }
    }

    private void LateUpdate()
    {
        if (!mouseClickAction.IsPressed() && drawing)
        {
            drawing = false;
        };
        if (mouseClickAction.IsPressed() && !drawing)
        {
            lineRendererList.Add(Instantiate(lineRenderer, lineRenderer.transform.position, Quaternion.identity, parent));
            drawing = true;
        };
    }
}
