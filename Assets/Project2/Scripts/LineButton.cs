using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineButton : MonoBehaviour
{
    // GameObject Reference from scene
    [SerializeField] private GameObject LineRendererObj;

    // LineRenderer Component Reference
    [SerializeField] private LineRenderer LineRenderer;

    // Bool variable to check state of button
    private bool stateL = true;

    // Start is called before the first frame update
    void Start()
    {
        // Find the GameObject by its name
        LineRendererObj = GameObject.Find("LineRenderer");

        if (LineRendererObj != null)
        {
            // Access the components or do other operations with the GameObject
            LineRenderer = LineRendererObj.GetComponent<LineRenderer>();
        }
        else
        {
            Debug.LogError("GameObject not found!");
        }
    }

    // Function to hide rendered line on scene
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
