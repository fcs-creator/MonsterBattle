using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;

public class UIHPBar : MonoBehaviour
{
    
    public Transform Character { get; set; } // �L�����N�^�[��Transform
    public Vector3 Offset { get; set; }      // �L�����N�^�[����̃I�t�Z�b�g
    public float Hp { get; private set; }    // ���݂�HP

    Image gauge;                    // �Q�[�W�̉摜
    const float MAX_HP = 100.0f;    // �ő�HP

    void Awake()
    {
        Hp = MAX_HP;
    }

    void Start()
    {
        gauge = transform.Find("Gauge").GetComponent<Image>();
    }

    void Update()
    {
        if (Character != null)
        {
            // �J�����̎Q�Ƃ����������`�F�b�N
            if (Camera.main != null)
            {
                // �L�����N�^�[�̃��[���h���W���X�N���[�����W�ɕϊ�
                Vector3 screenPosition = Camera.main.WorldToScreenPoint(Character.position + Offset);

                // �X�N���[�����W���L�����o�X�̍��W�ɕϊ�
                Vector2 localPoint;
                Canvas canvas = GameObject.Find("UIPlay").GetComponent<Canvas>();
                RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    canvas.transform as RectTransform,
                    screenPosition,
                    Camera.main,
                    out localPoint
                );

                // HP�o�[�̈ʒu���X�V
                transform.localPosition = localPoint;
            }
            else
            {
                Debug.LogError("Main Camera not found. Ensure your camera has the 'MainCamera' tag.");
            }
        }
        else
        {
            Debug.LogError("Character Transform is not assigned.");


        }
    }

    // �_���[�W���󂯂郁�\�b�h
    public void TakeDamage(float damage)
    {
        SetHp(Hp-damage);
    }

    // HP���X�V���郁�\�b�h
    public void SetHp(float value)
    {
        Hp = Mathf.Clamp(value, 0, MAX_HP);
        gauge.fillAmount = Hp / MAX_HP;
    }

    public bool IsEmpty() 
    {
        return Hp <= 0;
    }
}
