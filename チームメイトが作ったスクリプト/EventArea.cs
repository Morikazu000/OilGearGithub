using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventArea : EventManager
{
    //”ÍˆÍ•`‰æLineRendererŠi”[—p
    LineRenderer _lineRenderer;



    private void Start()
    {
        base.Start();

        _lineRenderer = GetComponent<LineRenderer>();
    }



    
}
