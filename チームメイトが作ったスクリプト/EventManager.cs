using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    //�v���C���[��Transform�i�[�p
    private Transform _playerTransform;



    protected void Start()
    {
        //�v���C���[��Transform�i�[
        _playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
    }
}
