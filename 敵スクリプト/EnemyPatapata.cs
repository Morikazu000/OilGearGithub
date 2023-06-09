using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPatapata : SmallEnemyManager
{
    private EnemyPatapataParameter patapataParameter;
    private float _patapataNowHp = default;

    private void Start()
    {

        patapataParameter = Resources.Load<EnemyPatapataParameter>(characterCommonParameter._myParameterObjName);
        base.Start();

        EnemyCollisionStart();
    }
    void FixedUpdate()
    {

        _patapataNowHp = GetNowHp();

        if (_patapataNowHp < 0)
        {
            gameObject.SetActive(false);
        }

        Updown();
        PlayerBoxCollision();

        InvincibleTimeUpdate();
    }

    /// <summary>
    /// 上下移動
    /// </summary>
    private void Updown()
    {
        //波の求める式  y=Asin2πf(t-x/v) をもとにして削った式
        float _width = Mathf.Sin(Mathf.PI * patapataParameter._movespeed * Time.time) * patapataParameter._moverange;
        float _height = Mathf.Cos(Mathf.PI * patapataParameter._heightspeed * Time.time) * patapataParameter._heightrange;
        //はちの字に動く式
        transform.position = new Vector2(transform.position.x + _width / 100, transform.position.y + _height / 100);

    }
}
