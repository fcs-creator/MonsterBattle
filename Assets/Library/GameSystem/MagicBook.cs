using System.Security.Cryptography;
using System.Threading.Tasks;
using Unity.Cinemachine;
using UnityEngine;

public class MagicBook : MonoBehaviour
{
    [SerializeField] GameObject fireBallPrefab;     //ファイアーボールのプレハブ
    [SerializeField] GameObject thunderPrefab;      //サンダーのプレハブ
    [SerializeField] bool isActiveRotating = false; //回転するかどうか
    [SerializeField] bool isActiveUpDowning = false; //上下するかどうか

    float rotationSpeed = 120.0f; //回転速度

    //上下移動
    float upDownSpeed = 1f;     //上下する速度
    public float amplitude = 1.0f; // 振幅: 上下の移動量
    Vector3 startPosition;

    void Start()
    {
        startPosition = transform.position;
    }

    async public Task FireBall(Monster monster, int num, float speed) 
    {
        monster.LookAtEnemy();

        for (int i = 0; i < num; i++)
        {
            if (monster.IsDead) break;
          
            GameObject obj = Instantiate(fireBallPrefab, monster.transform.position, Quaternion.identity);
            FireBall fireBall = obj.GetComponent<FireBall>();
            fireBall.Owner = monster;
            fireBall.Direction = monster.Direction;
            fireBall.Speed = speed;

            await Task.Delay(200);
        }
    }

    async public Task Thunder(Monster monster)
    {
        monster.LookAtEnemy();

        GameObject obj = Instantiate(thunderPrefab, monster.transform.position, Quaternion.identity);
        Thunder thunder = obj.GetComponent<Thunder>();
        thunder.Owner = monster;
        thunder.Direction = monster.Direction;

        await Task.Delay(500);
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


