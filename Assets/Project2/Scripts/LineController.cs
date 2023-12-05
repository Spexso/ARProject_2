using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineController : MonoBehaviour
{
    // LineRenderer Reference
    private LineRenderer lr;

    // Points to where lines will be rendered
    private Transform[] points;

    

    // Start is called before the first frame update

    private void Awake()
    {
        // Gets LineRenderer Component on lr 
        lr = GetComponent<LineRenderer>();
    }

    // Initializes points that will be rendered by LineRenderer
    public void SetUpLine(Transform[] points)
    {
        lr.positionCount = points.Length;
        this.points = points;

        for (int i = 0; i < points.Length; i++)
        {
            lr.SetPosition(i, this.points[i].position);
        }
    }


    // Update is called once per frame
    void Update()
    {

    }
}