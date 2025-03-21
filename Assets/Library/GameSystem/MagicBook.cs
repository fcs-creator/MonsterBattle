using System;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using Unity.Cinemachine;
using UnityEngine;

public class MagicBook : MonoBehaviour
{
    [SerializeField] GameObject fireBallPrefab;     //ファイアーボールのプレハブ
    [SerializeField] GameObject thunderPrefab;      //サンダーのプレハブ
    [SerializeField] GameObject icePrefab;          //アイスのプレハブ

    [SerializeField] GameObject magicCirclePrefab;  //魔法陣のプレハブ
    [SerializeField] bool isActiveRotating = false; //回転するかどうか
    [SerializeField] bool isActiveUpDowning = false; //上下するかどうか

    float rotationSpeed = 120.0f; //回転速度

    //上下移動
    float upDownSpeed = 1f;     //上下する速度
    public float amplitude = 1.0f; // 振幅: 上下の移動量
    Vector3 startPosition;

    //タスクをキャンセルするための共通トークン
    readonly Canceler canceler = new Canceler();

    void Start()
    {
        startPosition = transform.position;
    }

    async public Task FireBall(Monster monster, int num, float speed)
    {
        num = Mathf.Clamp(num, 1, Parameters.FIREBALL_MAX_NUM);

        await monster.LookAtEnemy();

        //魔法を詠唱
        await Chant(monster, Parameters.FIREBALL_CHANT_TIME);

        monster.ActionBar.SendText("Magic-FireBall");

        await monster.LookAtEnemy();

        for (int i = 0; i < num; i++)
        {
            if (monster.IsDead)
            {
                canceler.Dispose();
                break;
            }
          
            GameObject obj = Instantiate(fireBallPrefab, monster.transform.position, Quaternion.identity);
            FireBall fireBall = obj.GetComponent<FireBall>();
            fireBall.Owner = monster;
            fireBall.Direction = monster.EnemyDirection;
            // 方向ベクトルから角度を計算
            float angle = Mathf.Atan2(fireBall.Direction.y, fireBall.Direction.x) * Mathf.Rad2Deg;
            fireBall.transform.rotation = Quaternion.Euler(0, 0, angle);

            fireBall.Speed = speed;

            AudioManager.Instance.PlaySE(Parameters.SE_MAGIC_FIRE);

            await Wait(Parameters.FIREBALL_GEN_INTERVAL);
        }

        await Wait(Parameters.FIREBALL_END_INTERBAL);
    }

    async public Task IceNeedle(Monster owner, float dirX,  float speed) 
    {
        await owner.LookAtEnemy();

        //魔法を詠唱
        await Chant(owner, Parameters.FIREBALL_CHANT_TIME);

        owner.ActionBar.SendText("Magic-IceNeedle");

        await owner.LookAtEnemy();

        for (int i = 0; i < Parameters.ICE_MAX_NUM; i++)
        {
            if (owner.IsDead)
            {
                canceler.Dispose();
                break;
            }

            GameObject obj = Instantiate(icePrefab, owner.transform.position, Quaternion.identity);
            IceNeedle ice = obj.GetComponent<IceNeedle>();
            ice.Owner = owner;

            Vector2 dir = Vector2.zero;
            dirX = Mathf.Clamp(dirX, -1, 1);
            dir.x = dirX+ UnityEngine.Random.Range(-0.5f,0.5f);
            dir.y = 1 - dir.x;
            ice.Direction = dir;
            ice.Speed = speed;

            AudioManager.Instance.PlaySE(Parameters.SE_ICE_SHOT);
            await Wait(Parameters.ICE_GEN_INTERVAL);
        }

        await Wait(Parameters.ICE_END_INTERBAL);
    }

    async public Task Thunder(Monster monster)
    {
        if (monster.IsDead)
        {
            canceler.Dispose();
            return;
        }

        //魔法を詠唱
        await Chant(monster, Parameters.THUNDER_CHANT_TIME);

        monster.ActionBar.SendText("Magic-Thunder");

        await monster.LookAtEnemy();

        GameObject obj = Instantiate(thunderPrefab, monster.transform.position, Quaternion.identity);
        Thunder thunder = obj.GetComponent<Thunder>();
        thunder.Owner = monster;
        thunder.Direction = monster.EnemyDirection;

        AudioManager.Instance.PlaySE(Parameters.SE_MAGIC_THUNDER);

        if (monster.IsDead)
        {
            canceler.Dispose();
            return;
        }

        await Wait(Parameters.THUNDER_END_INTERBAL);
    }

    //魔法を詠唱する処理
    private async Task Chant(Monster owner, float chantTime) 
    {
        //詠唱中にする
        owner.SetChant(true);

        //魔法陣を生成して取り付ける
        var magicCircle = Instantiate(magicCirclePrefab);
        magicCircle.transform.position = owner.Position;
        magicCircle.transform.SetParent(owner.transform);
        magicCircle.SetActive(true);

        //詠唱の音を鳴らす
        AudioManager.Instance.PlaySE(Parameters.SE_MAGIC_CHANT);

        if (owner.IsDead) canceler.Dispose();

        //詠唱時間だけ待つ
        await Wait(chantTime);

        //魔法陣を外す
        magicCircle.transform.SetParent(null);
        magicCircle.SetActive(false);
        Destroy(magicCircle);

        //詠唱中を解除
        owner.SetChant(false);
    }

    //指定秒数待つ
    protected async Task Wait(float sec)
    {
        try
        {
            await Task.Delay((int)(sec * 1000), canceler.Token);
        }
        catch (OperationCanceledException)
        {
            //タスクがキャンセルされた時の処理
        }
    }

    void Update()
    {
        if (isActiveRotating) 
        {
            Rotate();
        }

        if (isActiveUpDowning)
        {
            UpDown();
        }
    }

    void UpDown() 
    {
        // 上下に滑らかに動かす
        float newY = startPosition.y + Mathf.Sin(Time.time * upDownSpeed) * amplitude;
        transform.position = new Vector3(startPosition.x, newY, startPosition.z);
    }
    
    void Rotate() 
    {
        // 現在の回転を取得
        Quaternion currentRotation = transform.localRotation;

        // Y軸の回転を増加
        float newYRotation = currentRotation.eulerAngles.y + rotationSpeed * Time.deltaTime;

        // 新しい回転を設定
        transform.localRotation = Quaternion.Euler(currentRotation.eulerAngles.x, newYRotation, currentRotation.eulerAngles.z);
    }
}


