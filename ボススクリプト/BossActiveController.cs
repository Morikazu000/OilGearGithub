using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossActiveController : MonoBehaviour
{
    private GameObject _boss;
    private Bossmove bossmove;
    [SerializeField,Header("ìÆÇ´èoÇ∑éûä‘")]
    private float _activeTime = default;
    private float _timeCount = default;
    private bool _active = default;
    void Awake()
    {
        _boss = GameObject.Find("BossMom");
        bossmove = _boss.GetComponent<Bossmove>();
    }

    void Update()
    {
        if (_active == true)
        {
            _timeCount += Time.deltaTime;
            if (_activeTime < _timeCount)
            {
                bossmove.enabled = true;
                _active = false;
            }
        }

    }


    private void ActiveSet()
    {
        _active = true;
    }
}
