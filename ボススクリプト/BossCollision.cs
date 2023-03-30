using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossCollision : SmallEnemyManager
{

    [SerializeField, Header("ƒ{ƒX‚ªã‚©‚ç—‚¿‚Ä‚«‚½‚ÌUŒ‚—Í")]
    private int _bossstampdamage;
    private CharacterManager CM;

    void Start()
    {
        CM = _playerObj.GetComponent<CharacterManager>();
        EnemyCollisionStart();
    }

    private void FixedUpdate()
    {
        PlayerBoxCollision();
        InvincibleTimeUpdate();
    }
}
