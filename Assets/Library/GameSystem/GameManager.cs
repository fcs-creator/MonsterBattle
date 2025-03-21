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
    public const float ACTION_FORCE_SCALE = 8;                                  //アクションの力の補正倍率
    public const float JUMP_FORCE_SCALE = 3.5f;                                 //ジャンプの力の補正倍率
    public const float KNOCKBACK_FORCE = 150;                                   //ノックバックの力
    public const float MAX_VELOCITY_X = 50;                                     //最高速度_X
    public const float MAX_VELOCITY_Y = 50;                                     //最高速度_Y
    public const float DEAD_LINE_X = 40;                                        //死亡ラインX
    public const float DEAD_LINE_Y_UP = 40;                                     //死亡ライン上Y
    public const float DEAD_LINE_Y_DOWN = -20;                                  //死亡ライン下Y
    public const int ENEMY_CHECK_FREAKENCE = 10;                                //索敵の頻度
    public const int LAND_VELOCITY = 10;                                        //着地時に速度を0にするための閾値
    public const float FLOATING_VELOCITY_RESISTANCE_RATE = 0.985f;               //浮遊時に速度を減少させる割合

                                                                               
    //アクション
    public const float START_INTERVAL = 1;                                      //アクション開始までの待ち時間
    public const float ACTION_INTERVAL_FORWARD = 1.5f;                          //前移動
    public const float ACTION_INTERVAL_BACKWARD = 1.5f;                         //後移動
    public const float ACTION_INTERVAL_JUMP = 1.5f;                             //ジャンプ
    public const float ACTION_INTERVAL_ATTACK = 1.0f;                           //武器で攻撃
    public const float ACTION_INTERVAL_SHOT = 0.5f;                             //武器を投げる
    public const float ACTION_INTERVAL_SWITCH_WEAPON = 0.25f;                   //武器を切り替える
    public const float ACTION_INTERVAL_GUARD = 1.0f;                            //ガード
    public const float ACTION_INTERVAL_MAGIC = 1.0f;                            //魔法
    public const float ACTION_INTERVAL_MOVE = 1.0f;                             //自由移動
    public const float ACTION_INTERVAL_FLOATING = 1.0f;                         //浮遊

    //状態フラグの判定調整
    public const float BACKSTEP_CANCELATION_VELOCITY = 0.2f;                    //バックステップ判定を解除する速度


    public static readonly Vector2 FORWARD_JUMP_DIRECTION = new Vector2(0.4f, 0.6f);    //前ジャンプの方向
    public static readonly Vector2 BACKWARD_JUMP_DIRECTION = new Vector2(-0.4f, 0.6f);  //後ジャンプの方向

    //ボディ
    public const float MASS_MAGNIFICATION = 5f;                                //面積に対する重さの倍率
    public const float MASS_MAX = 100;                                          //最大質量
    public const float MASS_MIN = 0.01f;                                        //最小質量

    //武器
    public const float WEAPON_ONHIT_ADD_DIRECTION_Y = 0.25f;                    //武器が当たったときの上方向への吹き飛ばしの加算値
    public const float WEAPON_DAMAGE_REDUCATION_RATE_ON_GUARDING = 0.2f;        //ガード時の武器のダメージの軽減率
    public const float WEAPON_STRIKE_FORCE_REDUCATION_RATE_ON_GUARDING = 0.2f;  //ガード時の武器による吹き飛ばしの軽減率
    public const float WEAPON_STRIKE_FORCE = 360;                               //武器が当たったときに吹き飛ばす力
    public const float WEAPON_SHOT_FORCE_SCALE = 5;                             //武器を投げる力の補正倍率
    public const float WEAPON_DAMAGE_SCALE = 2.2f;                              //武器のダメージ倍率
    public const float WEAPON_GRAVITY_SCALE = 3;                                //武器にかかる重力
    public const float DEFAULT_RETURN_TIME= 0.5f;                               //初期位置に戻るのにかかる秒数
    public const float DEFAULT_RETURN_WAIT_TIME = 0.5f;                         //初期位置に戻った後の待ち時間
    public const float WEAPON_INTERVAL_DRAWING = 1;                             //抜刀後の時間
    public const float MASS_WEAPON_MAGNIFICATION = 1;                           //面積に対する重さの倍率
    public const float MASS_WEAPON_MAX = 30;                                    //最大質量
    public const float MASS_WEAPON_MIN = 3;                                     //最小質量
    public const int   WEAPON_CLONE_MAX = 10;                                   //クローンの最大数
    public const float WEAPON_CLONE_SCALE_RATE = 0.5f;                          //クローンのスケール倍率
    public const float WEAPON_CLONE_DESTROY_DURATION = 5;                       //クローンの破棄までの時間
    public const float WEAPON_INTERVAL_CLONE = 3.0f;                            //クローン後の待ち時間
    public const float WEAPON_MAX_VELOCITY_X = 120;                             //武器の最高速度
    public const float WEAPON_MAX_VELOCITY_Y = 120;                             //武器の最高速度
    public const float WEAPON_HIT_VELOCITY_REDUCATION_RATE = 0.6f;              //武器の当たった相手の速度を弱める倍率
    public const float WEAPON_MIN_USE_TIME = 0.2f;                              //武器の動作にかけることのできる最小時間
    public const float WEAPON_MAX_USE_TIME = 15f;                               //武器の動作にかけることのできる最小時間
    public const float WEAPON_MOVE_MAX_DISTANCE = 5;



    //ガード
    public const float GUARD_DURATION = 2f;                                               //ガードの継続時間
    public const float GUARD_STUN_DURATION = 4f;                                            //ガードが決まった時のスタン時間
    public const float GUARD_FORCE_SCALE = 50;                                              //ガードが決まった時に吹き飛ばす力(向きは相手の逆ベクトル)
    public const GuardType GUARD_DEFAULT_TYPE = GuardType.Shield;                           //ガードの初期タイプ

    //シールド
    public const string SHIELD_SPRITE_RESOURCE_PATH = "Textures/Guard/Shield";              //シールドのスプライトのパス
    public static readonly Vector2 SHIELD_DEFAULT_OFFSET = new Vector2(0,0);                //シールドの初期オフセット
    public static readonly Color SHIELD_DEFALUT_COLOR = new Color(0.2f, 1f, 0f, 0.5f);     //シールドの初期色
    public const float SHIELD_DEFALUT_SCALE = 2.5f;                                         //シールドの初期スケール
    public const float SHIELD_MAX_OFFSET = 5;                                               //シールドの最大の距離
    public const float SHIELD_MIN_SCALE = 1;                                                //シールドの最小スケール
    public const float SHIELD_MAX_SCALE = 3.5f;                                             //シールドの最大スケール
                                                                
    //リフレクター
    public const string REFLECTOR_SPRITE_RESOURCE_PATH = "Textures/Guard/Reflector";        //リフレクターのスプライトのパス
    public static readonly Vector2 REFLECTOR_DEFAULT_OFFSET = new Vector2(4, 0);            //リフレクターの初期オフセット
    public static readonly Color REFLECTOR_DEFALUT_COLOR = new Color(0f, 0.6f, 1f, 0.75f);  //リフレクターの初期色
    public const float REFLECTOR_DEFAULT_SCALE = 2;                                         //リフレクターの初期スケール
    public const float REFLECTOR_MIN_OFFSET = 3;                                            //リフレクターの最大オフセット
    public const float REFLECTOR_MAX_OFFSET = 6;                                            //リフレクターの最大オフセット
    public const float REFLECTOR_MIN_SCALE = 1;                                             //リフレクターの最小スケール
    public const float REFLECTOR_MAX_SCALE = 3;                                             //リフレクターの最大スケール
    public const float REFLECT_WEAPON_FORCE = 100;                                          //武器を反射した時の力の強さ
    public const float REFLECT_DAMAGE_RATE = 2f;                                            //武器を反射した時基本ダメージ倍率

    //魔法
    public const float MAGIC_FORCE = 200;                                       //魔法が当たったとき吹き飛ばす力
    public const float MAGIC_REFLECT_INCREACE_DAMAGE_RATE = 1.25f;              //魔法が反射した時のダメージ倍率
    public const float MAGIC_REFLECT_FORCE = 100;                               //魔法が反射した時の力の強さ                    
    public const float FIREBALL_CHANT_TIME = 3.0f;                              //炎魔法を撃つための詠唱時間
    public const float FIREBALL_END_INTERBAL = 2.0f;                            //炎魔法を撃った後の隙

    public const float THUNDER_CHANT_TIME = 3.0f;                               //雷魔法を撃つための詠唱時間
    public const float THUNDER_END_INTERBAL = 2.0f;                             //雷魔法を撃った後の隙

    public const float ICE_MAX_NUM = 5;                                         //氷魔法の生成個数
    public const float ICE_GEN_INTERVAL = 0.5f;                                 //氷魔法の生成間隔
    public const float ICE_CHANT_TIME = 3.0f;                                   //氷魔法を撃つための詠唱時間
    public const float ICE_END_INTERBAL = 2.0f;                                 //氷魔法を撃った後の隙
    public const float ICE_BALL_DAMAGE = 25;                                    //氷魔法のボール状態のダメージ
    public const float ICE_DAMAGE = 10;                                         //氷魔法のダメージ
    public const float ICE_DESTROY_WAIT_TIME = 3;                               //氷魔法発動が破棄されるまでの待ち時間

    public const float FIREBALL_GEN_INTERVAL = 0.2f;                            //炎魔法のの生成間隔
    public const int FIREBALL_MAX_NUM = 5;                                      //ファイヤーボールの生成最大数
    public const float FIREBALL_DAMAGE = 7.5f;                                  //ファイアーボールダメージ値
    public const float FIREBALL_DESTOROY_WAIT_TIME = 0.75f;                     //発動後に破棄されるまでの待ち時間
    public const float FIREBALL_SHOT_GROUPING = 0.12f;                          //集弾率(小さいほど正確に狙う)
    public const float FIREBALL_SHOT_ADJUST_Y = 0.1f;                           //発射時のY軸の調整値
    public const float THUNDER_DAMAGE = 20;                                     //サンダーダメージ値
    public const float THUNDER_DESTOROY_WAIT_TIME = 0.5f;                       //発動後に破棄されるまでの待ち時間
    

    //ステージの壁
    public const float WALL_FORCE = 150;                                        //壁に当たった時に受ける力
    public const float WALL_DAMAGE = 10;                                        //壁に当たった時のダメージ値

    //UI
    public static readonly Vector2 HPBAR_OFFSET = new Vector2(0, 5f);            //HPバーの表示オフセット位置
    public static readonly Vector2 ACTIONBAR_OFFSET = new Vector2(0,-5f);        //アクションバーの表示オフセット位置
    public const string HPBAR_RESOUCE_PATH = "Prefabs/UI/HpBar";                 //HPバーのプレハブのパス
    public const string ACTIONBAR_RESOUCE_PATH = "Prefabs/UI/ActionBar";         //アクションバーのプレハブのパス                     

    //VFX
    public const VFX VFX_HIT_S = VFX.HitS;                                      //弱ヒット時に使用するVFX
    public const VFX VFX_HIT_M = VFX.HitM;                                      //中ヒット時に使用するVFX
    public const VFX VFX_HIT_L = VFX.HitL;                                      //大ヒット時に使用するVFX
    public const VFX VFX_GUARD = VFX.Guard;                                     //ガード時に使用するVFX
    public const VFX VFX_DEAD = VFX.Dead;                                       //ガード時に使用するVFX
    public const VFX VFX_HIT_WALL = VFX.HitWall;                                //壁に当たった時に使用するVFX
    public const VFX VFX_HIT_MAGIC = VFX.HitWall;                                //壁に当たった時に使用するVFX

    public static readonly Vector3 VFX_HIT_S_SCALE = new Vector3(10, 10, 1);    //弱ヒット時のVFXのスケール
    public static readonly Vector3 VFX_HIT_M_SCALE = new Vector3(10, 10, 1);    //中ヒット時のVFXのスケール
    public static readonly Vector3 VFX_HIT_L_SCALE = new Vector3(10, 10, 1);    //大ヒット時のVFXのスケール
    public static readonly Vector3 VFX_GUARD_SCALE = new Vector3(20, 20, 1);    //ガード時のVFXのスケール
    public static readonly Vector3 VFX_DEAD_SCALE = new Vector3(10, 10, 1);     //死亡時のVFXのスケール
    public static readonly Vector3 VFX_WALL_SCALE = new Vector3(10, 10, 1);     //壁に当たった時のVFXのスケール
    public static readonly Vector3 VFX_MAGIC_SCALE = new Vector3(20, 20, 1);    //魔法当たった時のVFXのスケール

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
    public const SE SE_SWITCH_WEAPON = SE.SwitchWeapon;                         //武器を切り替えた時のSE
    public const SE SE_MAGIC_FIRE = SE.MagicFire;                               //魔法炎
    public const SE SE_MAGIC_THUNDER = SE.MagicThunder;                         //魔法雷
    public const SE SE_MAGIC_ICE = SE.MagicIce;                                 //魔法氷
    public const SE SE_REFLECT = SE.Reflect;                                    //リフレクター
    public const SE SE_MAGIC_CHANT = SE.MagicChant;                             //魔法詠唱
    public const SE SE_HIT_MAGIC = SE.HitMagic;                                 //魔法に当たったとき
    public const SE SE_ICE_SHOT = SE.WeaponShot;                                //氷魔法が放たれた時

}

public class GameManager : MonoBehaviour
{
    public static int AliveMonstersNum { get; private set; }

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

        //生きているモンスターの数
        AliveMonstersNum = allMonsters.Length;

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

            //Awakeを呼ぶために一度全てアクティブ状態にする
            monster.transform.Find("Body").gameObject.SetActive(true);
            monster.transform.Find("Guard").gameObject.SetActive(true);

            foreach (var w in monster.Weapons) 
            {
                w.gameObject.SetActive(true);
            }

            //カメラの追従のターゲットを設定
            //GameObject objCameraTartget = new GameObject();
            //objCameraTartget.name = "CameraFollowTarget";
            //objCameraTartget.transform.position = new Vector3(0,0,0);
            //objCameraTartget.transform.rotation = Quaternion.identity;
            //objCameraTartget.transform.SetParent(monster.transform);
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
            await Task.Yield();
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

        AliveMonstersNum = allMonsters.Length - deadMonsters;

        if (AliveMonstersNum <= 1)
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
