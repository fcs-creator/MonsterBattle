using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEngine;

public static class Parameters
{
    //モンスター
    public const float GRAVITY = -9.81f * 2.5f;                                 //重力
    public const float KNOCKBACK_FORCE = 2;                                     //ノックバックの力
    public const float MAX_VELOCITY_X = 50;                                     //最高速度_X
    public const float MAX_VELOCITY_Y = 50;                                     //最高速度_Y
    public const float DEAD_LINE_X = 25;                                        //死亡ラインX
    public const float DEAD_LINE_Y_UP = 20;                                     //死亡ライン上Y
    public const float DEAD_LINE_Y_DOWN = -10;                                  //死亡ライン下Y
    public const int ENEMY_CHECK_FREAKENCE = 20;                                //索敵の頻度
    
    //アクション
    public const float ACTION_INTERVAL_DASH = 1.0f;                             //ダッシュ
    public const float ACTION_INTERVAL_BACKSTEP = 1.0f;                         //バックステップ
    public const float ACTION_INTERVAL_JUMP = 1.0f;                             //ジャンプ
    public const float ACTION_INTERVAL_ATTACK = 1.0f;                           //攻撃
    public const float ACTION_INTERVAL_GUARD = 1.0f;                            //ガード
    public const float ACTION_INTERVAL_MAGIC = 1.0f;                            //魔法
    public const float BACKSTEP_CANCELATION_VELOCITY = 0.2f;                    //バックステップ判定を解除する速度

    //ボディ
    public const float MASS_MAGNIFICATION = 40.0f;                              //面積に対する重さの倍率

    //武器
    public const float WEAPON_ONHIT_ADD_DIRECTION_Y = 0.7f;                     //武器が当たったときの上方向への吹き飛ばしの加算値
    public const float WEAPON_DAMAGE_REDUCATION_RATE_ON_GUARDING = 0.2f;        //ガード時の武器のダメージの軽減率
    public const float WEAPON_STRIKE_FORCE_REDUCATION_RATE_ON_GUARDING = 0.2f;  //ガード時の武器による吹き飛ばしの軽減率
    public const float WEAPON_STRIKE_FORCE = 20;                                //武器が当たったときに吹き飛ばす力
    public const float WEAPON_DAMAGE = 20;                                      //武器によるダメージ値
    public const float DEFAULT_RETURN_TIME= 0.5f;                               //初期位置に戻るのにかかる秒数
    public const float DEFAULT_RETURN_WAIT_TIME = 0.5f;                         //初期位置に戻った後の待ち時間

    //魔法
    public const float FIREBALL_DAMAGE = 5;                                     //ダメージ値
    public const float FIREBALL_DESTOROY_WAIT_TIME = 0.5f;                      //発動後に破棄されるまでの待ち時間
    public const float FIREBALL_SHOT_GROUPING = 0.3f;                           //集弾率(小さいほど正確に狙う)

    public const float THUNDER_DAMAGE = 20;                                     //ダメージ値
    public const float THUNDER_DESTOROY_WAIT_TIME = 0.5f;                       //発動後に破棄されるまでの待ち時間
}

public class GameManager : MonoBehaviour
{
    bool gameSet;
    Monster[] allMonsters;

    [SerializeField] UIRestult result;
    [SerializeField] MagicBook magicBook;
    [SerializeField] GameObject hpBarPrefab;

    void Awake()
    {
        gameSet = false;

        //UIResultのついたオブジェクトを全て探して先頭を取得(非アクティブのオブジェクトが対象のためこの記述)
        result = FindObjectsByType<UIRestult>(FindObjectsInactive.Include, FindObjectsSortMode.None)[0];　
        result.gameObject.SetActive(false);

        //全てのモンスターを取得(非アクティブのモンスターは除外)
        allMonsters = FindObjectsByType<Monster>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);

        //全てのモンスターに対して処理を行う
        foreach (Monster monster in allMonsters)
        {
            //敵のリストの作成
            List<Monster> otherMonsters = new List<Monster>(allMonsters.OrderBy(obj => Vector3.Distance(obj.transform.position, monster.transform.position)).ToList<Monster>());
            otherMonsters.Remove(monster);
            monster.Enemies = otherMonsters;
            if (otherMonsters.Count > 0)
                monster.Enemy = otherMonsters[0];

            //魔法書を設定
            monster.MagicBook = magicBook;

            //HPバーを設定
            GameObject objHpBar = Instantiate(hpBarPrefab, Vector3.zero, Quaternion.identity);
            UIHPBar hpBar = objHpBar.GetComponent<UIHPBar>();
            hpBar.Character = monster.transform;
            hpBar.Offset = new Vector3(0, 2.75f, 0);
            monster.SetHpBar(hpBar);
            objHpBar.transform.SetParent(GameObject.Find("UIPlay").transform);
            objHpBar.transform.localScale = new Vector3(1, 1, 1);

            //カメラの追従のターゲットを設定
            GameObject objCameraTartget = new GameObject();
            objCameraTartget.name = "CameraFollowTarget";
            objCameraTartget.transform.position = new Vector3(0,0,0);
            objCameraTartget.transform.rotation = Quaternion.identity;
            objCameraTartget.transform.SetParent(monster.transform);
        }
    }

    void Update()
    {
        if (gameSet) return;

        GameSetCheck();
    }

    //ゲームの終了判定
    void GameSetCheck() 
    {
        int deadMonsters = 0;

        foreach (Monster monster in allMonsters)
        {
            if (monster.IsDead)
            {
                deadMonsters++;
            }
        }

        int ariveMonstersNum = allMonsters.Length - deadMonsters;

        if (ariveMonstersNum <= 1)
        {
            gameSet = true;

            //生き残ったモンスターを取得
            foreach (Monster monster in allMonsters)
            {
                if (!monster.IsDead)
                {
                    result.SetWinMonster(monster);
                    break;
                }
            }

            //リザルト画面を表示
            result.gameObject.SetActive(true);
        }
    }
}
