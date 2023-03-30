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
    /// �㉺�ړ�
    /// </summary>
    private void Updown()
    {
        //�g�̋��߂鎮  y=Asin2��f(t-x/v) �����Ƃɂ��č������
        float _width = Mathf.Sin(Mathf.PI * patapataParameter._movespeed * Time.time) * patapataParameter._moverange;
        float _height = Mathf.Cos(Mathf.PI * patapataParameter._heightspeed * Time.time) * patapataParameter._heightrange;
        //�͂��̎��ɓ�����
        transform.position = new Vector2(transform.position.x + _width / 100, transform.position.y + _height / 100);

    }
}
