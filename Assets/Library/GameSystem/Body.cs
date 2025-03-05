using UnityEngine;
using static UnityEditor.Searcher.SearcherWindow.Alignment;

public class Body : MonoBehaviour
{
    PolygonCollider2D bodyCollider;
    Rigidbody2D rbParent;

    void Awake()
    {
        //�e�̃��W�b�h�{�f�B���擾
        rbParent = transform.parent.gameObject.GetComponent<Rigidbody2D>();
        rbParent.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        rbParent.gravityScale = Parameters.GRAVITY_SCALE;
        rbParent.freezeRotation = true;

        //�����}�e���A���̐ݒ�
        rbParent.sharedMaterial = Resources.Load<PhysicsMaterial2D>("MonsterPhysicsMaterial");

        // �^�O�̐ݒ�
        gameObject.tag = Tags.Body;

        //�X�v���C�g�̃\�[�g���C���[��ݒ�
        SpriteRenderer sr = gameObject.GetComponent<SpriteRenderer>();
        sr.sortingLayerName = SortLayer.Body;

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
}
