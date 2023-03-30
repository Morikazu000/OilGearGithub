using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterManagerNoma : NomaBehaviour
{
    //当たり判定のRayを使用する際は、StartメソッドにCollisionStart()
    //体力を使用する際は、UpdateまたはFixedUpdateでInvincibleTimeUpdate()を実行してください

    [SerializeField, Header("キャラクターの最大HP")]
    private int _characterMaxHP;

    //実際にもっているキャラクターのHP
    private int _nowCharacterHP;

    [SerializeField, Header("攻撃をくらった時の無敵時間")]
    private float _invincibleTime = 2.5f;

    //被攻撃フラグ
    protected bool _isDamage = false;

    //攻撃をされたときの無敵中のフラグ
    private bool _isInvincible;
    private float _elapsedTime;

    #region 衝突判定に関する変数

    //それぞれの方向へrayを飛ばした際の衝突フラグ
    protected bool _isGroundTouch = default;
    protected bool _isRoofTouch = default;
    protected bool _isRightWallTouch = default;
    protected bool _isLeftWallTouch = default;

    //Colliderのサイズの半分
    private Vector2 _halfColliderSize = default;
    //とりつけた当たり判定の中心(Colliderを手で動かすようなときに使ってください)
    private Vector2 _colliderCenter = default;

    //Rayの長さ
    float _Raydistance = 0.2f;
    //Rayの始点であるColliderの端から内側にずらす距離
    float _shiftEdgePos = 0.03f;

    //Rayの始点を射出方向にずらす距離
    float _shiftInjectionPos = 0.02f;
    //ずらした分Rayが当たった時に動かさなきゃいけない距離
    float _shiftTransformPos = 0;
    #endregion


    private void OnEnable()
    {
        InisializedHP();

        Collider collider;
    }

    #region 衝突関連

    /// <summary>
    /// メソッドを使う際は必ずこれをStartで実行してください
    /// </summary>
    protected void CollisionStart()
    {
        BoxCollider2D boxCollider = this.GetComponent<BoxCollider2D>();

        //自オブジェクトColliderの中心(ローカル座標)と大きさをとる
        _colliderCenter = boxCollider.offset;
        _halfColliderSize = this.transform.lossyScale * boxCollider.size / 2;
    }

    /// <summary>
    /// 接地フラグを返しつつ、接地位置よりめり込まないようにするメソッド
    /// </summary>
    public GameObject GroundRay()
    {
        //2本のRayの始点を計算する
        Vector2 rightRayPosition = new Vector2(this.gameObject.transform.position.x + _halfColliderSize.x - _shiftEdgePos, this.gameObject.transform.position.y - _halfColliderSize.y - _shiftInjectionPos);
        Vector2 leftRayPosition = new Vector2(this.gameObject.transform.position.x - _halfColliderSize.x + _shiftEdgePos, this.gameObject.transform.position.y - _halfColliderSize.y - _shiftInjectionPos);

        //Rayの当たったオブジェクトの情報を格納
        RaycastHit2D rightRayhit = Physics2D.Raycast(rightRayPosition, Vector2.down, _Raydistance);
        RaycastHit2D leftRayhit = Physics2D.Raycast(leftRayPosition, Vector2.down, _Raydistance);

        //デバッグ用
        Debug.DrawRay(rightRayPosition, Vector2.down * _Raydistance, Color.red);
        Debug.DrawRay(leftRayPosition, Vector2.down * _Raydistance, Color.red);

        //右のRayが当たった時の処理
        if (rightRayhit.collider != null && (rightRayhit.collider.CompareTag("Ground") || rightRayhit.collider.CompareTag("GimikGround")))
        {
            _isGroundTouch = true;
            ShiftTransformPos(rightRayhit.point, rightRayPosition);

            /*-------------------------------*/
            //<野間追記>

            //地面のコライダー上端の座標取得用
            float groundTopPositionY;

            //地面取得用
            GameObject rayHitGround;

            //地面のコライダー取得用
            BoxCollider2D groundCollider;


            //地面取得
            rayHitGround = rightRayhit.collider.gameObject;

            //地面のコライダー取得
            groundCollider = rayHitGround.GetComponent<BoxCollider2D>();

            //地面のコライダー上端の座標取得
            groundTopPositionY = rayHitGround.transform.position.y + rayHitGround.transform.lossyScale.y * groundCollider.size.y / 2;

            /*-------------------------------*/

            //自分のオブジェクトをRayの当たった位置に変更
            this.gameObject.transform.position = new Vector3(this.gameObject.transform.position.x, groundTopPositionY + _halfColliderSize.y, 0);

            return rightRayhit.collider.gameObject;
        }
        //左のRayが当たった時の処理
        if (leftRayhit.collider != null && (leftRayhit.collider.CompareTag("Ground") || leftRayhit.collider.CompareTag("GimikGround")))
        {
            /*-------------------------------*/
            //<野間追記>

            //地面のコライダー上端の座標取得用
            float groundTopPositionY;

            //地面取得用
            GameObject rayHitGround;

            //地面のコライダー取得用
            BoxCollider2D groundCollider;


            //地面取得
            rayHitGround = leftRayhit.collider.gameObject;

            //地面のコライダー取得
            groundCollider = rayHitGround.GetComponent<BoxCollider2D>();

            //地面のコライダー上端の座標取得
            groundTopPositionY = rayHitGround.transform.position.y + rayHitGround.transform.lossyScale.y * groundCollider.size.y / 2;

            /*-------------------------------*/

            _isGroundTouch = true;
            ShiftTransformPos(leftRayhit.point, leftRayPosition);

            //自分のオブジェクトをRayの当たった位置に変更
            this.gameObject.transform.position = new Vector3(this.gameObject.transform.position.x, groundTopPositionY + _halfColliderSize.y, 0);

            return leftRayhit.collider.gameObject;
        }
        //当たってないときの処理
        _isGroundTouch = false;

        return null;
    }

    /// <summary>
    /// 天井接触フラグを返しつつ、接触位置よりめり込まないようにするメソッド
    /// </summary>
    protected void RoofRay()
    {
        //2本のRayの始点を計算する
        Vector2 rightRayPosition = new Vector2(this.gameObject.transform.position.x + _halfColliderSize.x - _shiftEdgePos, this.gameObject.transform.position.y + _halfColliderSize.y + _shiftInjectionPos);
        Vector2 leftRayPosition = new Vector2(this.gameObject.transform.position.x - _halfColliderSize.x + _shiftEdgePos, this.gameObject.transform.position.y + _halfColliderSize.y + _shiftInjectionPos);

        //Rayの当たったオブジェクトの情報を格納
        RaycastHit2D rightRayhit = Physics2D.Raycast(rightRayPosition, Vector2.up, _Raydistance);
        RaycastHit2D leftRayhit = Physics2D.Raycast(leftRayPosition, Vector2.up, _Raydistance);

        //右のRayが当たった時の処理
        if (rightRayhit.collider != null && rightRayhit.collider.CompareTag("Ground"))
        {
            _isRoofTouch = true;
            ShiftTransformPos(rightRayhit.point, rightRayPosition);

            //自分のオブジェクトをRayの当たった位置に変更
            this.gameObject.transform.position = new Vector3(this.gameObject.transform.position.x, rightRayhit.point.y - _halfColliderSize.y - _shiftTransformPos, 0);

            return;
        }
        //左のRayが当たった時の処理
        if (leftRayhit.collider != null && leftRayhit.collider.CompareTag("Ground"))
        {
            _isRoofTouch = true;
            ShiftTransformPos(leftRayhit.point, leftRayPosition);

            //自分のオブジェクトをRayの当たった位置に変更
            this.gameObject.transform.position = new Vector3(this.gameObject.transform.position.x, leftRayhit.point.y - _halfColliderSize.y - _shiftTransformPos, 0);

            return;
        }

        //当たってないときの処理
        _isRoofTouch = false;
    }

    /// <summary>
    /// 右の壁への接触フラグを返しつつ、接触位置よりめり込まないようにするメソッド
    /// </summary>
    protected void RightWallRay()
    {
        //2本のRayの始点を計算する
        Vector2 upperRayPosition = new Vector2(this.gameObject.transform.position.x + _halfColliderSize.x + _shiftInjectionPos, this.gameObject.transform.position.y + _halfColliderSize.y - _shiftEdgePos);
        Vector2 underRayPosition = new Vector2(this.gameObject.transform.position.x + _halfColliderSize.x + _shiftInjectionPos, this.gameObject.transform.position.y - _halfColliderSize.y + _shiftEdgePos);

        //Rayの当たったオブジェクトの情報を格納
        RaycastHit2D upperRayhit = Physics2D.Raycast(upperRayPosition, Vector2.right, _Raydistance);
        RaycastHit2D underRayhit = Physics2D.Raycast(underRayPosition, Vector2.right, _Raydistance);

        //上のRayが当たった時の処理
        if (upperRayhit.collider != null && upperRayhit.collider.CompareTag("Ground"))
        {
            _isRightWallTouch = true;
            ShiftTransformPos(upperRayhit.point, upperRayPosition);

            //自分のオブジェクトをRayの当たった位置に変更
            this.gameObject.transform.position = new Vector3(upperRayhit.point.x - _halfColliderSize.x - _shiftTransformPos, this.gameObject.transform.position.y, 0);

            return;
        }
        if (underRayhit.collider != null && underRayhit.collider.CompareTag("Ground"))
        {
            _isRightWallTouch = true;
            ShiftTransformPos(underRayhit.point, underRayPosition);

            //自分のオブジェクトをRayの当たった位置に変更
            this.gameObject.transform.position = new Vector3(underRayhit.point.x - _halfColliderSize.x - _shiftTransformPos, this.gameObject.transform.position.y, 0);

            return;
        }

        //当たってないときの処理
        _isRightWallTouch = false;
    }

    /// <summary>
    /// 左の壁への接触フラグを返しつつ、接触位置よりめり込まないようにするメソッド
    /// </summary>
    protected void LeftWallRay()
    {
        //2本のRayの始点を計算する
        Vector2 upperRayPosition = new Vector2(this.gameObject.transform.position.x - _halfColliderSize.x - _shiftInjectionPos, this.gameObject.transform.position.y + _halfColliderSize.y - _shiftEdgePos);
        Vector2 underRayPosition = new Vector2(this.gameObject.transform.position.x - _halfColliderSize.x - _shiftInjectionPos, this.gameObject.transform.position.y - _halfColliderSize.y + _shiftEdgePos);

        //Rayの当たったオブジェクトの情報を格納
        RaycastHit2D upperRayhit = Physics2D.Raycast(upperRayPosition, Vector2.left, _Raydistance);
        RaycastHit2D underRayhit = Physics2D.Raycast(underRayPosition, Vector2.left, _Raydistance);

        //上のRayが当たった時の処理
        if (upperRayhit.collider != null && upperRayhit.collider.CompareTag("Ground"))
        {
            _isLeftWallTouch = true;
            ShiftTransformPos(upperRayhit.point, upperRayPosition);

            //自分のオブジェクトをRayの当たった位置に変更
            this.gameObject.transform.position = new Vector3(upperRayhit.point.x + _halfColliderSize.x + _shiftTransformPos, this.gameObject.transform.position.y, 0);

            return;
        }
        //下のRayが当たった時の処理
        if (underRayhit.collider != null && underRayhit.collider.CompareTag("Ground"))
        {
            _isLeftWallTouch = true;
            ShiftTransformPos(underRayhit.point, underRayPosition);

            //自分のオブジェクトをRayの当たった位置に変更
            this.gameObject.transform.position = new Vector3(underRayhit.point.x + _halfColliderSize.x + _shiftTransformPos, this.gameObject.transform.position.y, 0);

            return;
        }

        //当たってないときの処理
        _isLeftWallTouch = false;
    }

    /// <summary>
    /// Rayの発射地点をずらした分、戻さなければならないオブジェクトの距離を格納するメソッド
    /// </summary>
    private void ShiftTransformPos(Vector2 rayHitPoint, Vector2 rayStartPos)
    {
        if (rayHitPoint == rayStartPos)
        {
            _shiftTransformPos = _shiftInjectionPos;
        }
        else
        {
            _shiftTransformPos = 0;
        }
    }

    /*---------------野間追記---------------*/

    //衝突判定enum
    public enum COLLISION
    {
        GROUND,
        ROOF,
        RIGHT_WALL,
        LEFT_WALL,
    }



    /// <summary>
    /// 四方の取りたい衝突判定を取得するメソッド
    /// </summary>
    /// <param name="collision">取得したい衝突判定</param>
    /// <returns>衝突フラグ</returns>
    public bool GetIsCollision(COLLISION collision)
    {
        //衝突フラグ取得用
        bool isCollision = default;


        //とりたい判定により分岐
        switch (collision)
        {
            case COLLISION.GROUND:
                //接地判定
                GroundRay();

                //接地フラグ取得
                isCollision = _isGroundTouch;

                break;


            case COLLISION.ROOF:
                //天井判定
                RoofRay();

                //天井フラグ取得
                isCollision = _isRoofTouch;

                break;


            case COLLISION.RIGHT_WALL:
                //右壁判定
                RightWallRay();

                //右壁フラグ取得
                isCollision = _isRightWallTouch;

                break;


            case COLLISION.LEFT_WALL:
                //左壁判定
                LeftWallRay();

                //左壁フラグ取得
                isCollision = _isLeftWallTouch;

                break;
        }

        return isCollision;
    }

    #endregion


    #region HP関連
    /// <summary>
    /// HPから引数を引き、HPが0以下になったら非アクティブにする
    /// </summary>
    public void TakesDamage(int damageValue)
    {
        if(_isInvincible == false)
        {
            //被攻撃フラグtrue
            _isDamage = true;

            //ダメージを計算する
            _nowCharacterHP -= damageValue;

            //体力がなくなったらこのキャラを倒す
            if (_nowCharacterHP <= 0)
            {
                this.gameObject.SetActive(false);
            }

            //無敵時間にはいる
            _isInvincible = true;
            _elapsedTime = 0;
        }
    }

    /// <summary>
    /// HPの初期化
    /// </summary>
    private void InisializedHP()
    {
        _nowCharacterHP = _characterMaxHP;
        _isInvincible = false;
    }

    /// <summary>
    /// 無敵時間中の処理(体力を扱う場合、UpdateやFixedUpdateで実行してください)
    /// </summary>
    protected void InvincibleTimeUpdate()
    {
        if(_isInvincible == true)
        {
            _elapsedTime += Time.fixedDeltaTime;
            //計測している時間が無敵時間を超えたら無敵を解除する
            if (_elapsedTime >= _invincibleTime)
            {
                _isInvincible = false;
            }
        }
    }



    /// <summary>
    /// 現在のHPを返すメソッド
    /// </summary>
    /// <returns>現在のHP</returns>
    public int GetNowHp()
    {
        return _nowCharacterHP;
    }



    /// <summary>
    /// HP減少メソッド
    /// </summary>
    /// <param name="decreaceValue">減少値</param>
    public void DecreaceHp(int decreaceValue)
    {
        //HPを減らす
        _nowCharacterHP -= decreaceValue;
    }



    /// <summary>
    /// 最大HPを返すメソッド
    /// </summary>
    /// <returns>最大HP</returns>
    public int GetMaxHp()
    {
        return _characterMaxHP;
    }

    #endregion
}

