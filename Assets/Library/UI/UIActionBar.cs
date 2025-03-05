using UnityEngine;
using UnityEngine.TextCore.Text;
using TMPro;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;
using System.Threading.Tasks;

public class UIActionBar : MonoBehaviour
{
    public Monster Owner { get; set; }       // �L�����N�^�[�̎Q��
    public Transform Character { get; set; } // �L�����N�^�[��Transform
    public Vector3 Offset { get; set; }      // �L�����N�^�[����̃I�t�Z�b�g
    TextMeshProUGUI text;                    // �Q�[�W�̉摜
    
    void Awake()
    {
        text = transform.Find("Text").GetComponent<TextMeshProUGUI>();
        gameObject.SetActive(false);
    }

    void Start()
    {
        _ = UpdateActionBar();
    }
    
    async Task UpdateActionBar()
    {
        while (!Owner.IsDead)
        {
            UpdatePosition();

            await Task.Yield();
        }

        Hide();
    }

    // �e�L�X�g��\�����郁�\�b�h
    public void SendText(string value)
    {
        UpdatePosition();

        text.text = value;

        gameObject.SetActive(true);

        Invoke("Hide", 1);
    }

    void Hide() 
    {
        gameObject.SetActive(false);
    }

    void UpdatePosition() 
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

                //�o�[�̈ʒu���X�V
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
}
