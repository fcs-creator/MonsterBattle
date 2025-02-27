using UnityEngine;

public class Body : MonoBehaviour
{
    Rigidbody2D rbParent;               //親のRigidbody
    PolygonCollider2D bodyCollider;     
    
    void Awake()
    {
        //質量をセットするため親のリジッドボディを取得
        rbParent = transform.parent.GetComponent<Rigidbody2D>();

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
