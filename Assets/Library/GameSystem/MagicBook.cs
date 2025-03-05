using System.Threading.Tasks;
using Unity.Cinemachine;
using UnityEngine;

public class MagicBook : MonoBehaviour
{
    [SerializeField] GameObject fireBallPrefab;     //�t�@�C�A�[�{�[���̃v���n�u
    [SerializeField] GameObject thunderPrefab;      //�T���_�[�̃v���n�u
    [SerializeField] bool isActiveRotating = false; //��]���邩�ǂ���

    float rotationSpeed = 120.0f; //��]���x

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
        // ���݂̉�]���擾
        Quaternion currentRotation = transform.localRotation;

        // Y���̉�]�𑝉�
        float newYRotation = currentRotation.eulerAngles.y + rotationSpeed * Time.deltaTime;

        // �V������]��ݒ�
        transform.localRotation = Quaternion.Euler(currentRotation.eulerAngles.x, newYRotation, currentRotation.eulerAngles.z);
    }
}


