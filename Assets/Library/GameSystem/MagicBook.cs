using System.Threading.Tasks;
using Unity.Cinemachine;
using UnityEngine;

public class MagicBook : MonoBehaviour
{
    [SerializeField] GameObject fireBallPrefab;     //ファイアーボールのプレハブ
    [SerializeField] GameObject thunderPrefab;      //サンダーのプレハブ
    [SerializeField] bool isActiveRotating = false; //回転するかどうか

    float rotationSpeed = 120.0f; //回転速度

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


