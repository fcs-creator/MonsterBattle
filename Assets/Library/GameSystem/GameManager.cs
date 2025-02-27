using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEngine;

public static class Parameters
{
    //�����X�^�[
    public const float GRAVITY = -9.81f * 2.5f;                                 //�d��
    public const float KNOCKBACK_FORCE = 2;                                     //�m�b�N�o�b�N�̗�
    public const float MAX_VELOCITY_X = 50;                                     //�ō����x_X
    public const float MAX_VELOCITY_Y = 50;                                     //�ō����x_Y
    public const float DEAD_LINE_X = 25;                                        //���S���C��X
    public const float DEAD_LINE_Y_UP = 20;                                     //���S���C����Y
    public const float DEAD_LINE_Y_DOWN = -10;                                  //���S���C����Y
    public const int ENEMY_CHECK_FREAKENCE = 20;                                //���G�̕p�x
    
    //�A�N�V����
    public const float ACTION_INTERVAL_DASH = 1.0f;                             //�_�b�V��
    public const float ACTION_INTERVAL_BACKSTEP = 1.0f;                         //�o�b�N�X�e�b�v
    public const float ACTION_INTERVAL_JUMP = 1.0f;                             //�W�����v
    public const float ACTION_INTERVAL_ATTACK = 1.0f;                           //�U��
    public const float ACTION_INTERVAL_GUARD = 1.0f;                            //�K�[�h
    public const float ACTION_INTERVAL_MAGIC = 1.0f;                            //���@
    public const float BACKSTEP_CANCELATION_VELOCITY = 0.2f;                    //�o�b�N�X�e�b�v������������鑬�x

    //�{�f�B
    public const float MASS_MAGNIFICATION = 40.0f;                              //�ʐςɑ΂���d���̔{��

    //����
    public const float WEAPON_ONHIT_ADD_DIRECTION_Y = 0.7f;                     //���킪���������Ƃ��̏�����ւ̐�����΂��̉��Z�l
    public const float WEAPON_DAMAGE_REDUCATION_RATE_ON_GUARDING = 0.2f;        //�K�[�h���̕���̃_���[�W�̌y����
    public const float WEAPON_STRIKE_FORCE_REDUCATION_RATE_ON_GUARDING = 0.2f;  //�K�[�h���̕���ɂ�鐁����΂��̌y����
    public const float WEAPON_STRIKE_FORCE = 20;                                //���킪���������Ƃ��ɐ�����΂���
    public const float WEAPON_DAMAGE = 20;                                      //����ɂ��_���[�W�l
    public const float DEFAULT_RETURN_TIME= 0.5f;                               //�����ʒu�ɖ߂�̂ɂ�����b��
    public const float DEFAULT_RETURN_WAIT_TIME = 0.5f;                         //�����ʒu�ɖ߂�����̑҂�����

    //���@
    public const float FIREBALL_DAMAGE = 5;                                     //�_���[�W�l
    public const float FIREBALL_DESTOROY_WAIT_TIME = 0.5f;                      //������ɔj�������܂ł̑҂�����
    public const float FIREBALL_SHOT_GROUPING = 0.3f;                           //�W�e��(�������قǐ��m�ɑ_��)

    public const float THUNDER_DAMAGE = 20;                                     //�_���[�W�l
    public const float THUNDER_DESTOROY_WAIT_TIME = 0.5f;                       //������ɔj�������܂ł̑҂�����
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

        //UIResult�̂����I�u�W�F�N�g��S�ĒT���Đ擪���擾(��A�N�e�B�u�̃I�u�W�F�N�g���Ώۂ̂��߂��̋L�q)
        result = FindObjectsByType<UIRestult>(FindObjectsInactive.Include, FindObjectsSortMode.None)[0];�@
        result.gameObject.SetActive(false);

        //�S�Ẵ����X�^�[���擾(��A�N�e�B�u�̃����X�^�[�͏��O)
        allMonsters = FindObjectsByType<Monster>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);

        //�S�Ẵ����X�^�[�ɑ΂��ď������s��
        foreach (Monster monster in allMonsters)
        {
            //�G�̃��X�g�̍쐬
            List<Monster> otherMonsters = new List<Monster>(allMonsters.OrderBy(obj => Vector3.Distance(obj.transform.position, monster.transform.position)).ToList<Monster>());
            otherMonsters.Remove(monster);
            monster.Enemies = otherMonsters;
            if (otherMonsters.Count > 0)
                monster.Enemy = otherMonsters[0];

            //���@����ݒ�
            monster.MagicBook = magicBook;

            //HP�o�[��ݒ�
            GameObject objHpBar = Instantiate(hpBarPrefab, Vector3.zero, Quaternion.identity);
            UIHPBar hpBar = objHpBar.GetComponent<UIHPBar>();
            hpBar.Character = monster.transform;
            hpBar.Offset = new Vector3(0, 2.75f, 0);
            monster.SetHpBar(hpBar);
            objHpBar.transform.SetParent(GameObject.Find("UIPlay").transform);
            objHpBar.transform.localScale = new Vector3(1, 1, 1);

            //�J�����̒Ǐ]�̃^�[�Q�b�g��ݒ�
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

    //�Q�[���̏I������
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

            //�����c���������X�^�[���擾
            foreach (Monster monster in allMonsters)
            {
                if (!monster.IsDead)
                {
                    result.SetWinMonster(monster);
                    break;
                }
            }

            //���U���g��ʂ�\��
            result.gameObject.SetActive(true);
        }
    }
}
