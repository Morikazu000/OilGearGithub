using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraincheckSingle : MonoBehaviour
{

    [SerializeField, Header("カメラ")]
    private GameObject _camera;

    [SerializeField, Header("敵生成ディレイ時間")]
    private float _derayTime;

    private float _derayCount = default;
    private ObjectInCameraSingle _Incamera;
    void Start()
    {
        _Incamera = _camera.GetComponent<ObjectInCameraSingle>();
    }

    void FixedUpdate()
    {
        _derayCount += Time.fixedDeltaTime;
        if (_derayCount > _derayTime)
        {
            for (int i = 0; i < _Incamera._spawnCount; i++)
            {
                if (_Incamera._insideCamera[i] == true)
                {
                    _Incamera._spawnChildren[i].GetComponent<EnemypoolSingle>().InstantiateEnemy();
                }
            }
            DerayReset();
        }
    }

    private void DerayReset()
    {
        _derayCount = 0;
    }
}
