using System;
using System.Threading.Tasks;
using UnityEngine;
using static UnityEditor.Searcher.SearcherWindow.Alignment;

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
            rbParent.mass = area * Parameters.MASS_MAGNIFICATION;  // �ʐςɔ�Ⴕ�Ď��ʂ�ݒ�
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

        await Wait(Parameters.GUARD_STUN_DURATION);
    }

    private async Task FlashAndHitStopTask()
    {
        // �X�v���C�g�̐F��ύX
        sr.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0.5f); // �������ɐݒ�
        await Wait(flashDuration);

        // �q�b�g�X�g�b�v�ƃX���[���[�V�����̏���
        Time.timeScale = slowMotionScale;
        await Wait(hitStopDuration);
        Time.timeScale = 1f;

        // �X�v���C�g�̐F�����ɖ߂�
        sr.color = originalColor;
    }

    async protected Task Wait(float sec)
    {
        if (canceler.IsCancel) return;

        await Task.Delay((int)(sec * 1000), canceler.Token);
    }
}
