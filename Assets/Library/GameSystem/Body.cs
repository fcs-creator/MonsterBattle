using UnityEngine;

public class Body : MonoBehaviour
{
    PolygonCollider2D bodyCollider;
    Rigidbody2D rbParent;

    void Awake()
    {
        //親のリジッドボディを取得
        rbParent = transform.parent.gameObject.GetComponent<Rigidbody2D>();
        rbParent.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        rbParent.gravityScale = Parameters.GRAVITY_SCALE;
        rbParent.freezeRotation = true;

        //物理マテリアルの設定
        rbParent.sharedMaterial = Resources.Load<PhysicsMaterial2D>("MonsterPhysicsMaterial");

        // タグの設定
        gameObject.tag = Tags.Body;

        //スプライトのソートレイヤーを設定
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
            rbParent.mass = area* Parameters.MASS_MAGNIFICATION;  // 面積に比例して質量を設定
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
