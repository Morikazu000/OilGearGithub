using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossCollision : SmallEnemyManager
{

    [SerializeField, Header("ボスが上から落ちてきた時の攻撃力")]
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
