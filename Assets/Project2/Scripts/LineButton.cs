using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineButton : MonoBehaviour
{
    [SerializeField] private GameObject LineRendererObj;
    [SerializeField] private LineRenderer LineRenderer;
    private bool stateL = true;

    // Start is called before the first frame update
    void Start()
    {
        // Find the GameObject by its name
        LineRendererObj = GameObject.Find("LineRenderer");

        if (LineRendererObj != null)
        {
            // Access the components or do other operations with the GameObject
            // For example, get the Transform component
            LineRenderer = LineRendererObj.GetComponent<LineRenderer>();
        }
        else
        {
            Debug.LogError("GameObject not found!");
        }
    }


    public void OnClickL()
    {
        Debug.Log("Line Button interacted!");

        if (stateL)
        {
            stateL = false;
            LineRenderer.enabled = false;
            Debug.Log("Show line");
        }
        else
        {
            stateL = true;
            LineRenderer.enabled = true;
            Debug.Log("Hide line");
        }
    }
}
