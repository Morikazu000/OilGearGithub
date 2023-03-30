using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NomaBehaviour : MonoBehaviour
{
    //fixedの最大FPS
    const float MAX_FIXED_FPS = 50;

    //１秒
    const float ONE_SECONDS = 1;


    protected float GetFixedDeltaTime()
    {
        #region ローカル変数

        //FPS取得用
        float fps;

        //変換後のDeltaTime取得用
        float deltaTime;

        #endregion


        //fps取得
        fps = ONE_SECONDS / Time.deltaTime;

        //fpsがFixedの最大値以上か
        if (fps > MAX_FIXED_FPS)
        {
            //fpsをFixedの最大値に修正
            fps = MAX_FIXED_FPS;
        }

        //１フレーム当たりの秒数に変換
        deltaTime = ONE_SECONDS / fps;

        return deltaTime;
    }
}
