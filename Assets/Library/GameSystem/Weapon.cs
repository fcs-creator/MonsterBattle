using UnityEngine;
using System.Threading.Tasks;
using System.Collections.Generic;

public class Weapon : MonoBehaviour
{
    public Monster Owner { get; private set; }          // ����̏��L��(�����X�^�[)
    public bool IsHitableOwner { get; private set; }    // ���킪���L�҂ɓ����邩
    public float Damage { get; private set; }           // ����̃_���[�W
    public float StrikeForce { get; private set; }      // ����̐�����΂���

    Vector3 defaultLocalPosition;   // ����̏������W
    Vector3 defaultLocalScale;      // ����̏����X�P�[��
    Quaternion defaultRotation;     // ����̏�����]

    Rigidbody2D rb;
    List<GameObject> weapons;


    void Awake()
    {
        // 1��̊K�w�ɂ��郂���X�^�[�̃I�u�W�F�N�g��T���ăZ�b�g
        Owner = transform.parent.GetComponent<Monster>();

        //����̃_���[�W
        Damage = Parameters.WEAPON_DAMAGE;

        //����̐�����΂���
        StrikeForce = Parameters.WEAPON_STRIKE_FORCE;

        // �����������g����悤�ɂ��Ă���
        rb = gameObject.AddComponent<Rigidbody2D>();
        rb.constraints = RigidbodyConstraints2D.FreezeAll;

        // �����̈ʒu�A��]�A�X�P�[����ۑ�
        defaultLocalPosition = transform.localPosition;
        defaultLocalScale = transform.localScale;
        defaultRotation = transform.rotation;

        //���툵���̃I�u�W�F�N�g�����ׂĎ擾
        weapons = new List<GameObject>();
        weapons.Add(gameObject);//�e��ǉ�
        foreach (Transform child in transform)
        {
            weapons.Add(child.gameObject);// �q�I�u�W�F�N�g�����X�g�ɒǉ�
        }

        foreach (GameObject obj in weapons)
        {
            // �^�O��ݒ�
            gameObject.tag = Tags.Weapon;

            if (HasComponent<SpriteRenderer>(obj))
            {
                //�X�v���C�g�̃\�[�g���C���[��ݒ�
                SpriteRenderer sr = gameObject.GetComponent<SpriteRenderer>();
                sr.sortingLayerName = SortLayer.Weapon;

                // �摜�̌`��ɍ��킹�ăR���C�_��ݒ�
                var weaponCollider = gameObject.AddComponent<PolygonCollider2D>();
                weaponCollider.autoTiling = true;
                weaponCollider.isTrigger = true;
            }
        }

        // �ŏ��͕�����B���Ă���
        SetActive(false);
    }

    void Start()
    {
        _ = ExcecuteActionLoop();   //�Ăяo��&Task��j��
    }

    async Task ExcecuteActionLoop()
    {
        if (Owner == null)
        {
            Debug.Log(gameObject.name);
            Debug.LogError("Error: Weapon Owner is null");
            
            return;
        }

        while (!Owner.IsDead)
        {
            await ActionLoop();
        }
    }

    async public Task ExecuteAttack()
    {
        WarpDefault();

        IsHitableOwner = false;

        SetActive(true);

        await Attack();

        SetActive(false);
    }

    //��{�͂����炪�Ăяo�����
    async virtual protected Task Attack()
    {
        await Task.Yield();
    }

    //�������䂷�镐�킪����(���܂��v�f)
    async virtual protected Task ActionLoop()
    {
        await Task.Yield();
    }

    //�w��b���҂�
    async protected Task Wait(float sec)
    {
        await Task.Delay((int)(sec * 1000));
    }

    //�����ʒu�Ƀ��[�v
    public void WarpDefault()
    {
        transform.SetParent(Owner.transform);

        transform.localPosition = defaultLocalPosition;
        transform.rotation = defaultRotation;
        transform.localScale = defaultLocalScale;
    }

    //�����̌q�����⊮����֐�
    async Task Lerp(Vector3 startPosition, Quaternion startRotation, Vector3 startScale, Vector3 endPosition, Quaternion endRotation, Vector3 endScale, float second)
    {
        float elapsedTime = 0f;

        while (elapsedTime < second)
        {
            float t = elapsedTime / second;

            // ��Ԃ��s��
            transform.position = Vector3.Lerp(startPosition, endPosition, t);
            transform.rotation = Quaternion.Slerp(startRotation, endRotation, t);
            transform.localScale = Vector3.Lerp(startScale, endScale, t);

            elapsedTime += Time.deltaTime;
            await Task.Yield();
        }
    }

    //����������ʒu�Ƀ��Z�b�g����
    async protected Task Default()
    {
        //���݂̏��
        Vector3 currentPosition = transform.position;
        Quaternion currentRotation = transform.rotation;
        Vector3 currentScale = transform.localScale;

        //���Z�b�g��̏��(���L���郂���X�^�[�̉�]�Ɗg�k�͔��f���Ȃ�)
        Vector3 targetPosition;
        Quaternion targetRotation = defaultRotation;
        Vector3 targetScale = defaultLocalScale;

        //�����ɂ���Ė߂��ʒu��ς���
        if (Owner.IsFacingRight)
            targetPosition = Owner.transform.position + defaultLocalPosition;
        else
            targetPosition = Owner.transform.position - defaultLocalPosition;

        //�⊮����
        await Lerp(currentPosition, currentRotation, currentScale, targetPosition, targetRotation, targetScale, Parameters.DEFAULT_RETURN_TIME);

        //�Ō�ɐe�ɖ߂�
        transform.SetParent(Owner.transform);

        // �Ō�Ɋm���Ɍ��̏�Ԃɐݒ�
        transform.localPosition = defaultLocalPosition;
        transform.rotation = targetRotation;
        transform.localScale = targetScale;

        await Wait(Parameters.DEFAULT_RETURN_WAIT_TIME);
    }

    //������w�肳�ꂽ(x, y)�ʒu��s�b�ňړ�������
    async protected Task Move(float x, float y, float s)
    {
        Vector2 start = transform.localPosition;
        Vector2 target = start + new Vector2(x, y);
        float elapsedTime = 0f;

        while (elapsedTime < s)
        {
            // �o�ߎ��Ԃ̊������v�Z
            float t = elapsedTime / s;
            // ���`��ԁiLerp�j�ňʒu���X�V
            transform.localPosition = Vector2.Lerp(start, target, t);
            // �o�ߎ��Ԃ��X�V
            elapsedTime += Time.deltaTime;
            // 1�t���[���ҋ@
            await Task.Yield();
        }

        // �ŏI�ʒu��ݒ�i�덷��␳�j
        transform.localPosition = target;
    }

    //�����angle�xs�b�ł��̏��]������
    async protected Task Spin(float angle, float s)
    {
        float elapsed = 0f;
        float initialRotation = transform.rotation.eulerAngles.z;
        float targetRotation = initialRotation + angle;

        while (elapsed < s)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / s;
            float zRotation = Mathf.Lerp(initialRotation, targetRotation, t);
            // Z�����ŉ�]������
            transform.rotation = Quaternion.Euler(0, 0, zRotation);
            await Task.Yield();
        }

        // �Ō�Ɋm���ɖڕW�p�x�܂ŉ�]������
        transform.rotation = Quaternion.Euler(0, 0, targetRotation);
    }

    //��������͂ŉ�]������ startAngle:��]�J�n�p�x(���_��0�x), ��]�ʁFrotAngle, ��]�ɂ�����b���Fsecond   
    async protected Task Rotate(float startAngle, float rotAngle, float second)
    {
        //���v��肩�ǂ�������
        bool clockwise = true;
        if ((Owner.IsFacingRight && rotAngle < 0) || (!Owner.IsFacingRight && rotAngle >= 0))
            clockwise = false;

        //�J�n�p�x�̒���
        float addStartAngle = startAngle;

        if ((clockwise && startAngle < 0) || (!clockwise && startAngle > 0))
            addStartAngle *= -1;

        float startAngleToward = addStartAngle + 90.0f + transform.localEulerAngles.z;

        //����̏����ʒu���L�����N�^�[�̐^��ɐݒu
        float radius = Vector2.Distance(Owner.transform.position, transform.position);
        transform.position = Owner.transform.position + new Vector3(radius, 0, 0);
        transform.rotation = Quaternion.Euler(0, 0, 0);
        transform.RotateAround(Owner.transform.position, Vector3.forward, startAngleToward);

        bool stop = false;
        float currentRotAngle = 0;
        float step;

        while (!stop)
        {
            //�t���[�����̉�]�ʂ��v�Z
            step = (rotAngle / second) * Time.deltaTime;

            //�����ɉ����ċt��]
            if ((clockwise && rotAngle > 0) || (!clockwise && rotAngle < 0))
                step *= -1;

            //���݂܂ł̉�]���ʂ̉��Z
            currentRotAngle += Mathf.Abs(step);

            //��]�ʂ��w��ʂɒB������I��
            if (currentRotAngle >= Mathf.Abs(rotAngle))
            {
                currentRotAngle = rotAngle;
                stop = true;
            }
            else
            {
                transform.RotateAround(Owner.transform.position, Vector3.forward, step);
            }

            await Task.Yield();
        }

        WarpDefault();
    }

    //������w�肳�ꂽ�����ɔ�΂�
    async protected Task Shot(Vector2 direction, float power)
    {


        await Task.Yield();
    }

    //����������X�^�[�ɂ�鐧�䂩��؂藣��
    async protected Task Purge()
    {
        // ���[���h���W��ۑ�
        Vector3 worldPosition = transform.position;

        // �e�I�u�W�F�N�g����؂藣��
        transform.SetParent(null);

        // ���̈ʒu�ɖ߂�
        transform.position = worldPosition;

        await Task.Yield();
    }


    static (Vector2, Quaternion) RotateAround(Vector2 point, Vector2 pivot, float angle)
    {
        // �p�x�����W�A���ɕϊ�
        float radians = angle * Mathf.Deg2Rad;

        // ��]�s��̌v�Z
        float cos = Mathf.Cos(radians);
        float sin = Mathf.Sin(radians);

        // ���̓_�𒆐S�_�̌��_�Ɉړ�
        Vector2 translatedPoint = point - pivot;

        // �V�������W���v�Z
        float newX = translatedPoint.x * cos - translatedPoint.y * sin;
        float newY = translatedPoint.x * sin + translatedPoint.y * cos;

        // �V�������W���쐬
        Vector2 rotatedPoint = new Vector2(newX, newY) + pivot;

        // ��]���N�H�[�^�j�I���ŕ\��
        Quaternion rotation = Quaternion.Euler(0, 0, angle);

        return (rotatedPoint, rotation);
    }


    private bool HasComponent<T>(GameObject obj) where T : Component
    {
        return obj.GetComponent<T>() != null;
    }

    //������Body�̓����蔻�肩��1�x�o������͓�����悤�ɂȂ�
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag(Tags.Body))
        {
            GameObject monsterObj = other.transform.parent.gameObject;

            if (HasComponent<Monster>(monsterObj))
            {
                Monster monster = monsterObj.GetComponent<Monster>();

                if (Owner == monster)
                {
                    IsHitableOwner = true;
                }
            }
        }
    }

    void SetActive(bool value) 
    {
        foreach (GameObject weapon in weapons)  
        {
            weapon.SetActive(value);
        }
    }
}