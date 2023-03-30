using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    //プレイヤーのTransform格納用
    private Transform _playerTransform;



    protected void Start()
    {
        //プレイヤーのTransform格納
        _playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
    }
}
