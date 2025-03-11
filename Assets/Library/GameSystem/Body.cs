using System;
using System.Threading.Tasks;
using UnityEngine;
using static UnityEditor.Searcher.SearcherWindow.Alignment;
using static UnityEngine.InputSystem.LowLevel.InputStateHistory;

public class Body : MonoBehaviour
{
    PolygonCollider2D bodyCollider;
    Rigidbody2D rbParent;

    SpriteRenderer sr;
    Color originalColor;
    float flashDuration = 0.1f;
    float hitStopDuration = 0.05f;
    float slowMotionScale = 0.1f; // �X���[�̓x�����𒲐�����p�����[�^

    //�^�X�N���L�����Z��
    readonly Canceler canceler = new Canceler();

    void Awake()
    {
        //�e�̃��W�b�h�{�f�B���擾
        rbParent = transform.parent.gameObject.GetComponent<Rigidbody2D>();
        rbParent.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        rbParent.gravityScale = Parameters.GRAVITY_SCALE;
        rbParent.freezeRotation = true;

        //�����}�e���A���̐ݒ�
        rbParent.sharedMaterial = Resources.Load<PhysicsMaterial2D>("MonsterPhysicsMaterial");

        //�^�O�̐ݒ�
        gameObject.tag = Tags.Body;

        //�X�v���C�g�̃\�[�g���C���[��ݒ�
        sr = gameObject.GetComponent<SpriteRenderer>();
        sr.sortingLayerName = SortLayer.Body;

        //�X�v���C�g�̐F���擾
        originalColor = sr.color;

        //�R���C�_�[��ݒ�
        bodyCollider = gameObject.AddComponent<PolygonCollider2D>();
        bodyCollider.autoTiling = true;

        UpdateMassBasedOnArea();
    }

    void UpdateMassBasedOnArea()
    {
        if (bodyCollider != null)
        {
            float area = CalculateScaledArea();
            float massMag= Parameters.MASS_MAGNIFICATION;
            float massMax = Parameters.MASS_MAX;
            float massMin = Parameters.MASS_MIN;
            rbParent.mass = Mathf.Clamp(Mathf.Sqrt(area) * massMag + massMin, massMin, massMax);  // ���ʂ�ݒ�
            Debug.Log(transform.parent.name + " : " + rbParent.mass + "kg");
        }
    }

    float CalculateScaledArea()
    {
        // ���[�J�����W�ł̖ʐς��v�Z
        Vector2[] points = bodyCollider.points;
        float localArea = 0;
        for (int i = 0; i < points.Length; i++)
        {
            Vector2 current = points[i];
            Vector2 next = points[(i + 1) % points.Length];
            localArea += current.x * next.y - current.y * next.x;
        }
        localArea = Mathf.Abs(localArea) * 0.5f;

        // �e�̃X�P�[�����܂߂ăX�P�[����K�p
        Vector3 lossyScale = bodyCollider.transform.lossyScale;
        float totalScaledArea = localArea * Mathf.Abs(lossyScale.x) * Mathf.Abs(lossyScale.y);

        return totalScaledArea;
    }

    public async Task Stun() 
    {
        await FlashAndHitStopTask();
    }

    private async Task FlashAndHitStopTask()
    {
        float elapsedTime = 0f;

        while (elapsedTime < Parameters.GUARD_STUN_DURATION && canceler.IsNotCancel)
        {
            // �X�v���C�g�̐F��ύX
            sr.color = new Color(originalColor.r * 0.5f, originalColor.g * 0.5f, originalColor.b * 0.5f, 0.5f); // �������ɐݒ�
            await Wait(flashDuration);
            elapsedTime += flashDuration;

            // �X�v���C�g�̐F�����ɖ߂�
            sr.color = originalColor;
            await Wait(flashDuration);
            elapsedTime += flashDuration;

            elapsedTime += Time.deltaTime;
            AudioManager.Instance.PlaySE(Parameters.SE_STAN);
            await Task.Yield();
        }
    }

    async protected Task Wait(float sec)
    {
        if (canceler.IsCancel) return;

        await Task.Delay((int)(sec * 1000), canceler.Token);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == Tags.Body) 
        {
            // �{�f�B���m���Փ˂����ۂ�SE�Đ�
            AudioManager.Instance.PlaySE(Parameters.SE_COLLIDE_BODY);
        }   
    }
}
