using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Swordcollision : SmallEnemyManager
{
    private SwordParameter swordParameter;
    void Start()
    {

        swordParameter = Resources.Load<SwordParameter>(characterCommonParameter._myParameterObjName);
        base.Start();

        EnemyCollisionStart();
    }

    private void Update()
    {
        PlayerBoxCollision();    
    }

}
