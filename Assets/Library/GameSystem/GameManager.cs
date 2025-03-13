using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;

public static class Parameters
{
    //モンスター
    public const float GRAVITY_SCALE = 3;                                       //重力
    public const float ACTION_FORCE_SCALE = 5;                                  //アクションの力の補正倍率
    public const float JUMP_FORCE_SCALE = 3f;                                   //ジャンプの力の補正倍率
    public const float KNOCKBACK_FORCE = 40;                                    //ノックバックの力
    public const float KNOCKBACK_RANDOM_RANGE_X = 0.3f;                         //ノックバックのランダムX方向の範囲
    public const float MAX_VELOCITY_X = 50;                                     //最高速度_X
    public const float MAX_VELOCITY_Y = 50;                                     //最高速度_Y
    public const float DEAD_LINE_X = 40;                                        //死亡ラインX
    public const float DEAD_LINE_Y_UP = 40;                                     //死亡ライン上Y
    public const float DEAD_LINE_Y_DOWN = -20;                                  //死亡ライン下Y
    public const int ENEMY_CHECK_FREAKENCE = 20;                                //索敵の頻度
    public const int LAND_VELOCITY = 10;                                        //着地時に速度を0にするための閾値

    //アクション
    public const float START_INTERVAL = 0;                                      //アクション開始までの待ち時間
    public const float ACTION_INTERVAL_DASH = 2.0f;                             //ダッシュ
    public const float ACTION_INTERVAL_BACKSTEP = 1.0f;                         //バックステップ
    public const float ACTION_INTERVAL_JUMP = 1.0f;                             //ジャンプ
    public const float ACTION_INTERVAL_ATTACK = 1.0f;                           //武器で攻撃
    public const float ACTION_INTERVAL_SHOT = 0.5f;                             //武器を投げる
    public const float ACTION_INTERVAL_GUARD = 2.0f;                            //ガード
    public const float ACTION_INTERVAL_MAGIC = 1.0f;                            //魔法
    public const float ACTION_INTERVAL_FLOATING = 1.0f;                         //浮遊
    public const float BACKSTEP_CANCELATION_VELOCITY = 0.2f;                    //バックステップ判定を解除する速度

    public static readonly Vector2 FORWARD_JUMP_DIRECTION = new Vector2(0.3f, 0.7f);    //前ジャンプの方向
    public static readonly Vector2 BACKWARD_JUMP_DIRECTION = new Vector2(-0.3f, 0.7f);  //後ジャンプの方向

    //ボディ
    public const float MASS_MAGNIFICATION = 3;                                  //面積に対する重さの倍率
    public const float MASS_MAX = 100;                                          //最大質量
    public const float MASS_MIN = 5;                                            //最小質量

    //武器
    public const float WEAPON_ONHIT_ADD_DIRECTION_Y = 0.25f;                    //武器が当たったときの上方向への吹き飛ばしの加算値
    public const float WEAPON_DAMAGE_REDUCATION_RATE_ON_GUARDING = 0.2f;        //ガード時の武器のダメージの軽減率
    public const float WEAPON_STRIKE_FORCE_REDUCATION_RATE_ON_GUARDING = 0.2f;  //ガード時の武器による吹き飛ばしの軽減率
    public const float WEAPON_STRIKE_FORCE = 300;                               //武器が当たったときに吹き飛ばす力
    public const float WEAPON_SHOT_FORCE_SCALE = 5;                             //武器を投げる力の補正倍率
    public const float WEAPON_DAMAGE = 15;                                      //武器によるダメージ値
    public const float WEAPON_DAMAGE_SCALE = 3.0f;                              //武器のダメージ倍率
    public const float WEAPON_GRAVITY_SCALE = 3;                                //武器にかかる重力
    public const float DEFAULT_RETURN_TIME= 0.5f;                               //初期位置に戻るのにかかる秒数
    public const float DEFAULT_RETURN_WAIT_TIME = 0.5f;                         //初期位置に戻った後の待ち時間
    public const float WEAPON_INTERVAL_DRAWING = 1;                             //抜刀後の時間
    public const float MASS_WEAPON_MAGNIFICATION = 1;                           //面積に対する重さの倍率
    public const float MASS_WEAPON_MAX = 30;                                    //最大質量
    public const float MASS_WEAPON_MIN = 3;                                     //最小質量
    

    //ガード
    public const float GUARD_DURATION = 1.0f;                                   //継続時間
    public const float GUARD_STUN_DURATION = 3f;                                //ガードが決まった時のスタン時間
    public const float GUARD_FORCE_SCALE = 50;                                  //ガードが決まった時に吹き飛ばす力(向きは相手の逆ベクトル)

    //魔法
    public const float FIREBALL_DAMAGE = 5;                                     //ファイアーボールダメージ値
    public const float FIREBALL_DESTOROY_WAIT_TIME = 0.75f;                     //発動後に破棄されるまでの待ち時間
    public const float FIREBALL_SHOT_GROUPING = 0.12f;                          //集弾率(小さいほど正確に狙う)
    public const float FIREBALL_SHOT_ADJUST_Y = 0.1f;                           //発射時のY軸の調整値
    public const float THUNDER_DAMAGE = 20;                                     //サンダーダメージ値
    public const float THUNDER_DESTOROY_WAIT_TIME = 0.5f;                       //発動後に破棄されるまでの待ち時間

    //ステージの壁
    public const float WALL_FORCE = 50;                                         //壁に当たった時に受ける力
    public const float WALL_DAMAGE = 10;                                        //壁に当たった時のダメージ値

    //UI
    public static readonly Vector2 HPBAR_OFFSET = new Vector2(0, 5f);            //HPバーの表示オフセット位置
    public static readonly Vector2 ACTIONBAR_OFFSET = new Vector2(0,-5f);        //アクションバーの表示オフセット位置

    //VFX
    public const VFX VFX_HIT_S = VFX.HitS;                                      //弱ヒット時に使用するVFX
    public const VFX VFX_HIT_M = VFX.HitM;                                      //中ヒット時に使用するVFX
    public const VFX VFX_HIT_L = VFX.HitL;                                      //大ヒット時に使用するVFX
    public const VFX VFX_GUARD = VFX.Guard;                                     //ガード時に使用するVFX
    public const VFX VFX_DEAD = VFX.Dead;                                       //ガード時に使用するVFX
    public const VFX VFX_HIT_WALL = VFX.HitWall;                                //壁に当たった時に使用するVFX

    public static readonly Vector3 VFX_HIT_S_SCALE = new Vector3(10, 10, 1);    //弱ヒット時のVFXのスケール
    public static readonly Vector3 VFX_HIT_M_SCALE = new Vector3(10, 10, 1);    //中ヒット時のVFXのスケール
    public static readonly Vector3 VFX_HIT_L_SCALE = new Vector3(10, 10, 1);    //大ヒット時のVFXのスケール
    public static readonly Vector3 VFX_GUARD_SCALE = new Vector3(20, 20, 1);    //ガード時のVFXのスケール
    public static readonly Vector3 VFX_DEAD_SCALE = new Vector3(10, 10, 1);     //死亡時のVFXのスケール
    public static readonly Vector3 VFX_WALL_SCALE = new Vector3(10, 10, 1);     //壁に当たった時のVFXのスケール

    //BGM
    public const BGM BGM_BATTLE = BGM.Battle;                                   //バトルBGM
    public const BGM BGM_RESULT = BGM.Result;                                   //リザルトBGM

    //SE
    public const SE SE_HIT_STRIKE_S = SE.HitStrikeS;                            //弱ヒット時のSE
    public const SE SE_HIT_STRIKE_M = SE.HitStrikeM;                            //中ヒット時のSE
    public const SE SE_HIT_STRIKW_L = SE.HitStrikeL;                            //大ヒット時のSE
    public const SE SE_PARRY = SE.Parry;                                        //パリィ時のSE
    public const SE SE_STAN = SE.Stan;                                          //スタン時のSE
    public const SE SE_GUARD = SE.Guard;                                        //ガード時のSE
    public const SE SE_DEAD = SE.Dead;                                          //死亡時のSE
    public const SE SE_JUMP = SE.Jump;                                          //ジャンプ時のSE
    public const SE SE_LAND = SE.None;                                          //着地時のSE
    public const SE SE_WEAPON_SHOT = SE.WeaponShot;                             //武器を投げた時のSE
    public const SE SE_WEAPON_DRAWING = SE.WeaponDrawing;                       //武器を抜刀時のSE
    public const SE SE_COLLIDE_BODY = SE.CollideBody;                           //ボディ同士がぶつかった時のSE
    public const SE SE_HIT_WALL = SE.HitWall;                                   //ボディ同士がぶつかった時のSE
}

public class GameManager : MonoBehaviour
{
    bool gameSet;
    Monster[] allMonsters;

    [SerializeField] UIRestult result;
    [SerializeField] MagicBook magicBook;
    [SerializeField] GameObject hpBarPrefab;
    [SerializeField] GameObject actionBarPrefab;

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
            List<Monster> otherMonsters = new List<Monster>(allMonsters.OrderBy(obj => Vector3.Distance(obj.transform.position, monster.transform.position)).ToList());
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
            hpBar.Offset = Parameters.HPBAR_OFFSET;
            monster.SetHpBar(hpBar);
            objHpBar.transform.SetParent(GameObject.Find("UIPlay").transform);
            objHpBar.transform.localScale = new Vector3(1, 1, 1);

            //アクションテキストを設定
            GameObject actionBarObj = Instantiate(actionBarPrefab, Vector3.zero, Quaternion.identity);
            UIActionBar actionBar = actionBarObj.GetComponent<UIActionBar>();
            actionBar.Character = monster.transform;
            actionBar.Offset = Parameters.ACTIONBAR_OFFSET;
            monster.SetActionBar(actionBar);
            actionBar.transform.SetParent(GameObject.Find("UIPlay").transform);
            actionBar.transform.localScale = new Vector3(1, 1, 1);

            //カメラの追従のターゲットを設定
            GameObject objCameraTartget = new GameObject();
            objCameraTartget.name = "CameraFollowTarget";
            objCameraTartget.transform.position = new Vector3(0,0,0);
            objCameraTartget.transform.rotation = Quaternion.identity;
            objCameraTartget.transform.SetParent(monster.transform);
        }
    }

    void Start()
    {
        _ = GameStart();
    }

    async Task GameStart() 
    {
        //BGMの再生
        AudioManager.Instance.PlayBGM(Parameters.BGM_BATTLE);

        while (!gameSet) 
        {
            foreach (var monster in allMonsters)
            {
                _ = monster.Action(); // 各モンスターのActionを呼び出し
            }

            // 指定された時間（ミリ秒）だけ待機
            await Task.Delay(5000);
        }

        foreach (var monster in allMonsters)
        {
            monster.CancelActions();
        }

        //BGMの停止
        AudioManager.Instance.StopBGM();

        AudioManager.Instance.PlayBGM(Parameters.BGM_RESULT);
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
