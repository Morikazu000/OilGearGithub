using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NomaBehaviour : MonoBehaviour
{
    //fixed�̍ő�FPS
    const float MAX_FIXED_FPS = 50;

    //�P�b
    const float ONE_SECONDS = 1;


    protected float GetFixedDeltaTime()
    {
        #region ���[�J���ϐ�

        //FPS�擾�p
        float fps;

        //�ϊ����DeltaTime�擾�p
        float deltaTime;

        #endregion


        //fps�擾
        fps = ONE_SECONDS / Time.deltaTime;

        //fps��Fixed�̍ő�l�ȏォ
        if (fps > MAX_FIXED_FPS)
        {
            //fps��Fixed�̍ő�l�ɏC��
            fps = MAX_FIXED_FPS;
        }

        //�P�t���[��������̕b���ɕϊ�
        deltaTime = ONE_SECONDS / fps;

        return deltaTime;
    }
}
