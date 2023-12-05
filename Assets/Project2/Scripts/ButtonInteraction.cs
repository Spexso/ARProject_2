using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonInteraction : MonoBehaviour
{
    [SerializeField] private GameObject LineRendererObject;
    [SerializeField] private LineRenderer OnSceneRenderer;
    private bool state = false;

    // Start is called before the first frame update
    void Start()
    {
        // Find the GameObject by its name
        LineRendererObject = GameObject.Find("LineRenderer");

        if (LineRendererObject != null)
        {
            // Access the components or do other operations with the GameObject
            // For example, get the Transform component
            OnSceneRenderer = LineRendererObject.GetComponent<LineRenderer>();
        }
        else
        {
            Debug.LogError("GameObject not found!");
        }
    }

    public void OnClick()
    {
        Debug.Log("Button interacted!");

        if (state)
        {
            state = false;
            OnSceneRenderer.enabled = false;
            Debug.Log("Hide line");
        }
        else
        {
            state = true;
            OnSceneRenderer.enabled = true;
            Debug.Log("Show line");
        }
    }
}
