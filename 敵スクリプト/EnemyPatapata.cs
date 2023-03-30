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
    /// ã‰ºˆÚ“®
    /// </summary>
    private void Updown()
    {
        //”g‚Ì‹‚ß‚é®  y=Asin2ƒÎf(t-x/v) ‚ğ‚à‚Æ‚É‚µ‚Äí‚Á‚½®
        float _width = Mathf.Sin(Mathf.PI * patapataParameter._movespeed * Time.time) * patapataParameter._moverange;
        float _height = Mathf.Cos(Mathf.PI * patapataParameter._heightspeed * Time.time) * patapataParameter._heightrange;
        //‚Í‚¿‚Ìš‚É“®‚­®
        transform.position = new Vector2(transform.position.x + _width / 100, transform.position.y + _height / 100);

    }
}
