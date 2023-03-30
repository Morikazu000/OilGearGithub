using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectInCameraSingle : MonoBehaviour
{

    [SerializeField, Header("スポーンポイントの親")]
    private GameObject _enemySpawn;
    private Bossmove bossmove;

    #region オブジェクトの配列

    [field: SerializeField, Header("敵のスポーンポイント")]
    public List<GameObject> _spawnChildren { get; private set; }

    [field: SerializeField, Header("カメラ内のスポーンポイントチェック")]
    public bool[] _insideCamera { get; private set; }

    [field: SerializeField, Header("スポーンポイントから生成された敵")]
    public List<GameObject> _spawnGrandChildren { get; private set; }


    public int _spawnCount { get; private set; }

    #endregion

    #region カメラの設定

    [SerializeField, Header("敵を消す用の枠の最大値(_maxPoint枠より大きく設定してください)")]
    private Vector2 _deathMaxpoint;
    [SerializeField, Header("敵を消す用の枠の最小値(_minPoint枠より大きく設定してください)")]
    private Vector2 _deathMinpoint;

    [SerializeField, Header("枠の最大値")]
    private Vector2 _maxPoint;
    [SerializeField, Header("枠の最小値")]
    private Vector2 _minPoint;

    //----デバック用　---- 
    public Vector2 _rightTopPoint { get; private set; }
    public Vector2 _leftUnderPoint { get; private set; }
    public Vector2 _deathRightTopPoint { get; private set; }
    public Vector2 _deathLeftUnderPoint { get; private set; }

    #endregion


    protected void Start()
    {
        //配列初期化q
        _spawnChildren = new List<GameObject>();
        //子オブジェクトの数
        _spawnCount = _enemySpawn.transform.childCount;

        //配列の長さを子オブジェクトと同じにする
        _insideCamera = new bool[_spawnCount];

        //_enemyspawnの子オブジェクト全てを_spawnchildrenの中に入れる
        foreach (Transform child in _enemySpawn.transform)
        {
            _spawnChildren.Add(child.gameObject);
        }
    }
    protected void Update()
    {
        //右上の頂点
        _rightTopPoint = new Vector2(transform.position.x + _maxPoint.x, transform.position.y + _maxPoint.y);
        //左上の頂点
        _leftUnderPoint = new Vector2(transform.position.x + _minPoint.x, transform.position.y + _minPoint.y);
        //敵を消す右上の頂点
        _deathRightTopPoint = new Vector2(transform.position.x + _deathMaxpoint.x, transform.position.y + _deathMaxpoint.y);
        //敵を消す左上の頂点
        _deathLeftUnderPoint = new Vector2(transform.position.x + _deathMinpoint.x, transform.position.y + _deathMinpoint.y);


        //　" スポーンポイントが" 設定した枠の中に入っているかどうか
        for (int i = 0; i < _spawnCount; i++)
        {
            Vector3 spawnPointPosition = _spawnChildren[i].transform.position;
            //敵を消す用の範囲の中に入っているかどうか
            if (_deathRightTopPoint.x > spawnPointPosition.x && _deathLeftUnderPoint.x < spawnPointPosition.x
                && _deathRightTopPoint.y > spawnPointPosition.y && _deathLeftUnderPoint.y < spawnPointPosition.y)
            {
                //カメラの描画範囲の中にいるかどうか
                if (_rightTopPoint.x > spawnPointPosition.x && spawnPointPosition.x > _leftUnderPoint.x
                && _rightTopPoint.y > spawnPointPosition.y && spawnPointPosition.y > _leftUnderPoint.y)
                {
                    //カメラの描画範囲の外に出たらfalseにする
                    _insideCamera[i] = false;
                }
                else
                {
                    _insideCamera[i] = true;
                    _spawnChildren[i].SetActive(true);
                }
            }
            else
            {
                //敵を消す用の枠の外に出たらfalseにする
                _insideCamera[i] = false;
            }

            //　" 生成されている敵が" を消す用の枠から出ているかどうか
            for (int j = 0; j < _spawnChildren[i].transform.childCount; j++)
            {
                GameObject instageEnemy = _spawnChildren[i].transform.GetChild(j).gameObject;
                //子オブジェクト一つ一つが敵を消す枠に入っているかどうか
                if (_deathRightTopPoint.x > instageEnemy.transform.position.x && _deathLeftUnderPoint.x < instageEnemy.transform.position.x
                    && _deathRightTopPoint.y > instageEnemy.transform.position.y && _deathLeftUnderPoint.y < instageEnemy.transform.position.y)
                {
                }
                //描画範囲外でかつ、削除する用の枠の内側にいるかどうか
                else
                {
                    _spawnChildren[i].transform.GetChild(j).gameObject.SetActive(false);

                }


            }

        }

    }

}
