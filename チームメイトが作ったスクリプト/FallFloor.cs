using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallFloor : GimikManager
{
    [SerializeField, Header("落ちるまでの時間(s)")]
    private float _fallTime;

    private float _fallNowTime;

    

    private void FixedUpdate()
    {
        
    }
}
