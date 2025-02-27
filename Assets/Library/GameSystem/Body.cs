using UnityEngine;

public class Body : MonoBehaviour
{
    Rigidbody2D rbParent;               //�e��Rigidbody
    PolygonCollider2D bodyCollider;     
    
    void Awake()
    {
        //���ʂ��Z�b�g���邽�ߐe�̃��W�b�h�{�f�B���擾
        rbParent = transform.parent.GetComponent<Rigidbody2D>();

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
            float area = CalculatePolygonArea(bodyCollider.points);
            rbParent.mass = area* Parameters.MASS_MAGNIFICATION;  // �ʐςɔ�Ⴕ�Ď��ʂ�ݒ�
            Debug.Log(transform.parent.name + " : " + rbParent.mass + "kg");
        }
    }

    float CalculatePolygonArea(Vector2[] vertices)
    {
        float area = 0f;
        int j = vertices.Length - 1;

        for (int i = 0; i < vertices.Length; i++)
        {
            area += (vertices[j].x + vertices[i].x) * (vertices[j].y - vertices[i].y);
            j = i;
        }

        return Mathf.Abs(area) / 2f;
    }
}
