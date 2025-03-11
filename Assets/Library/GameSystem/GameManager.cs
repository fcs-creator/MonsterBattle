using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;

public static class Parameters
{
    //�����X�^�[
    public const float GRAVITY_SCALE = 3;                                       //�d��
    public const float ACTION_FORCE_SCALE = 5;                                  //�A�N�V�����̗͂̕␳�{��
    public const float JUMP_FORCE_SCALE = 3f;                                   //�W�����v�̗͂̕␳�{��
    public const float KNOCKBACK_FORCE = 40;                                    //�m�b�N�o�b�N�̗�
    public const float KNOCKBACK_RANDOM_RANGE_X = 0.3f;                         //�m�b�N�o�b�N�̃����_��X�����͈̔�
    public const float MAX_VELOCITY_X = 50;                                     //�ō����x_X
    public const float MAX_VELOCITY_Y = 50;                                     //�ō����x_Y
    public const float DEAD_LINE_X = 40;                                        //���S���C��X
    public const float DEAD_LINE_Y_UP = 40;                                     //���S���C����Y
    public const float DEAD_LINE_Y_DOWN = -20;                                  //���S���C����Y
    public const int ENEMY_CHECK_FREAKENCE = 20;                                //���G�̕p�x
    public const int LAND_VELOCITY = 10;                                        //���n���ɑ��x��0�ɂ��邽�߂�臒l

    //�A�N�V����
    public const float START_INTERVAL = 2.0f;                                   //�A�N�V�����J�n�܂ł̑҂�����
    public const float ACTION_INTERVAL_DASH = 2.0f;                             //�_�b�V��
    public const float ACTION_INTERVAL_BACKSTEP = 1.0f;                         //�o�b�N�X�e�b�v
    public const float ACTION_INTERVAL_JUMP = 1.0f;                             //�W�����v
    public const float ACTION_INTERVAL_ATTACK = 1.0f;                           //����ōU��
    public const float ACTION_INTERVAL_SHOT = 3.0f;                             //����𓊂���
    public const float ACTION_INTERVAL_GUARD = 2.0f;                            //�K�[�h
    public const float ACTION_INTERVAL_MAGIC = 1.0f;                            //���@
    public const float ACTION_INTERVAL_FLOATING = 1.0f;                         //���V
    public const float BACKSTEP_CANCELATION_VELOCITY = 0.2f;                    //�o�b�N�X�e�b�v������������鑬�x

    public static readonly Vector2 FORWARD_JUMP_DIRECTION = new Vector2(0.3f, 0.7f);    //�O�W�����v�̕���
    public static readonly Vector2 BACKWARD_JUMP_DIRECTION = new Vector2(-0.3f, 0.7f);  //��W�����v�̕���

    //�{�f�B
    public const float MASS_MAGNIFICATION = 3;                                  //�ʐςɑ΂���d���̔{��
    public const float MASS_MAX = 100;                                          //�ő县��
    public const float MASS_MIN = 5;                                            //�ŏ�����

    //����
    public const float WEAPON_ONHIT_ADD_DIRECTION_Y = 0.25f;                    //���킪���������Ƃ��̏�����ւ̐�����΂��̉��Z�l
    public const float WEAPON_DAMAGE_REDUCATION_RATE_ON_GUARDING = 0.2f;        //�K�[�h���̕���̃_���[�W�̌y����
    public const float WEAPON_STRIKE_FORCE_REDUCATION_RATE_ON_GUARDING = 0.2f;  //�K�[�h���̕���ɂ�鐁����΂��̌y����
    public const float WEAPON_STRIKE_FORCE = 300;                               //���킪���������Ƃ��ɐ�����΂���
    public const float WEAPON_SHOT_FORCE_SCALE = 5;                             //����𓊂���͂̕␳�{��
    public const float WEAPON_DAMAGE = 15;                                      //����ɂ��_���[�W�l
    public const float WEAPON_DAMAGE_SCALE = 3.0f;                              //����̃_���[�W�{��
    public const float WEAPON_GRAVITY_SCALE = 3;                                //����ɂ�����d��
    public const float DEFAULT_RETURN_TIME= 0.5f;                               //�����ʒu�ɖ߂�̂ɂ�����b��
    public const float DEFAULT_RETURN_WAIT_TIME = 0.5f;                         //�����ʒu�ɖ߂�����̑҂�����
    public const float WEAPON_INTERVAL_DRAWING = 1;                             //������̎���
    public const float MASS_WEAPON_MAGNIFICATION = 1;                           //�ʐςɑ΂���d���̔{��
    public const float MASS_WEAPON_MAX = 30;                                    //�ő县��
    public const float MASS_WEAPON_MIN = 3;                                     //�ŏ�����
    

    //�K�[�h
    public const float GUARD_DURATION = 1.0f;                                   //�p������
    public const float GUARD_STUN_DURATION = 3f;                                //�K�[�h�����܂������̃X�^������
    public const float GUARD_FORCE_SCALE = 50;                                  //�K�[�h�����܂������ɐ�����΂���(�����͑���̋t�x�N�g��)

    //���@
    public const float FIREBALL_DAMAGE = 5;                                     //�t�@�C�A�[�{�[���_���[�W�l
    public const float FIREBALL_DESTOROY_WAIT_TIME = 0.75f;                     //������ɔj�������܂ł̑҂�����
    public const float FIREBALL_SHOT_GROUPING = 0.12f;                           //�W�e��(�������قǐ��m�ɑ_��)
    public const float FIREBALL_SHOT_ADJUST_Y = 0.1f;                           //���ˎ���Y���̒����l
    public const float THUNDER_DAMAGE = 20;                                     //�T���_�[�_���[�W�l
    public const float THUNDER_DESTOROY_WAIT_TIME = 0.5f;                       //������ɔj�������܂ł̑҂�����

    //UI
    public static readonly Vector2 HPBAR_OFFSET = new Vector2(0, 4f);            //HP�o�[�̕\���I�t�Z�b�g�ʒu
    public static readonly Vector2 ACTIONBAR_OFFSET = new Vector2(0,-5f);        //�A�N�V�����o�[�̕\���I�t�Z�b�g�ʒu

    //VFX
    public const VFX VFX_HIT_S = VFX.HitS;                                      //��q�b�g���Ɏg�p����VFX
    public const VFX VFX_HIT_M = VFX.HitM;                                      //���q�b�g���Ɏg�p����VFX
    public const VFX VFX_HIT_L = VFX.HitL;                                      //��q�b�g���Ɏg�p����VFX
    public const VFX VFX_GUARD = VFX.Guard;                                     //�K�[�h���Ɏg�p����VFX
    public const VFX VFX_DEAD = VFX.Dead;                                       //�K�[�h���Ɏg�p����VFX

    public static readonly Vector3 VFX_HIT_S_SCALE = new Vector3(10, 10, 1);    //��q�b�g����VFX�̃X�P�[��
    public static readonly Vector3 VFX_HIT_M_SCALE = new Vector3(10, 10, 1);    //���q�b�g����VFX�̃X�P�[��
    public static readonly Vector3 VFX_HIT_L_SCALE = new Vector3(10, 10, 1);    //��q�b�g����VFX�̃X�P�[��
    public static readonly Vector3 VFX_GUARD_SCALE = new Vector3(20, 20, 1);    //�K�[�h����VFX�̃X�P�[��
    public static readonly Vector3 VFX_DEAD_SCALE = new Vector3(10, 10, 1);     //���S����VFX�̃X�P�[��

    //BGM
    public const BGM BGM_BATTLE = BGM.Battle;                                   //�o�g��BGM
    public const BGM BGM_RESULT = BGM.Result;                                   //���U���gBGM

    //SE
    public const SE SE_HIT_STRIKE_S = SE.HitStrikeS;                            //��q�b�g����SE
    public const SE SE_HIT_STRIKE_M = SE.HitStrikeM;                            //���q�b�g����SE
    public const SE SE_HIT_STRIKW_L = SE.HitStrikeL;                            //��q�b�g����SE
    public const SE SE_PARRY = SE.Parry;                                        //�p���B����SE
    public const SE SE_STAN = SE.Stan;                                          //�X�^������SE
    public const SE SE_GUARD = SE.Guard;                                        //�K�[�h����SE
    public const SE SE_DEAD = SE.Dead;                                          //���S����SE
    public const SE SE_JUMP = SE.Jump;                                          //�W�����v����SE
    public const SE SE_LAND = SE.None;                                          //���n����SE
    public const SE SE_WEAPON_SHOT = SE.WeaponShot;                             //����𓊂�������SE
    public const SE SE_WEAPON_DRAWING = SE.WeaponDrawing;                       //����𔲓�����SE
    public const SE SE_COLLIDE_BODY = SE.CollideBody;                           //�{�f�B���m���Ԃ���������SE
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

        //UIResult�̂����I�u�W�F�N�g��S�ĒT���Đ擪���擾(��A�N�e�B�u�̃I�u�W�F�N�g���Ώۂ̂��߂��̋L�q)
        result = FindObjectsByType<UIRestult>(FindObjectsInactive.Include, FindObjectsSortMode.None)[0];�@
        result.gameObject.SetActive(false);

        //�S�Ẵ����X�^�[���擾(��A�N�e�B�u�̃����X�^�[�͏��O)
        allMonsters = FindObjectsByType<Monster>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);

        //�S�Ẵ����X�^�[�ɑ΂��ď������s��
        foreach (Monster monster in allMonsters)
        {
            //�G�̃��X�g�̍쐬
            List<Monster> otherMonsters = new List<Monster>(allMonsters.OrderBy(obj => Vector3.Distance(obj.transform.position, monster.transform.position)).ToList());
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
            hpBar.Offset = Parameters.HPBAR_OFFSET;
            monster.SetHpBar(hpBar);
            objHpBar.transform.SetParent(GameObject.Find("UIPlay").transform);
            objHpBar.transform.localScale = new Vector3(1, 1, 1);

            //�A�N�V�����e�L�X�g��ݒ�
            GameObject actionBarObj = Instantiate(actionBarPrefab, Vector3.zero, Quaternion.identity);
            UIActionBar actionBar = actionBarObj.GetComponent<UIActionBar>();
            actionBar.Character = monster.transform;
            actionBar.Offset = Parameters.ACTIONBAR_OFFSET;
            monster.SetActionBar(actionBar);
            actionBar.transform.SetParent(GameObject.Find("UIPlay").transform);
            actionBar.transform.localScale = new Vector3(1, 1, 1);

            //�J�����̒Ǐ]�̃^�[�Q�b�g��ݒ�
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
        //BGM�̍Đ�
        AudioManager.Instance.PlayBGM(Parameters.BGM_BATTLE);

        while (!gameSet) 
        {
            foreach (var monster in allMonsters)
            {
                _ = monster.Action(); // �e�����X�^�[��Action���Ăяo��
            }

            // �w�肳�ꂽ���ԁi�~���b�j�����ҋ@
            await Task.Delay(5000);
        }

        foreach (var monster in allMonsters)
        {
            monster.CancelActions();
        }

        //BGM�̒�~
        AudioManager.Instance.StopBGM();

        AudioManager.Instance.PlayBGM(Parameters.BGM_RESULT);
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
