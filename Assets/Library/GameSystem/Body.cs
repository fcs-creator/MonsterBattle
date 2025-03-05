using UnityEngine;
using static UnityEditor.Searcher.SearcherWindow.Alignment;

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
            float area = CalculateScaledArea();
            rbParent.mass = area * Parameters.MASS_MAGNIFICATION;  // 面積に比例して質量を設定
            Debug.Log(transform.parent.name + " : " + rbParent.mass + "kg");
        }
    }

    float CalculateScaledArea()
    {
        // ローカル座標での面積を計算
        Vector2[] points = bodyCollider.points;
        float localArea = 0;
        for (int i = 0; i < points.Length; i++)
        {
            Vector2 current = points[i];
            Vector2 next = points[(i + 1) % points.Length];
            localArea += current.x * next.y - current.y * next.x;
        }
        localArea = Mathf.Abs(localArea) * 0.5f;

        // 親のスケールも含めてスケールを適用
        Vector3 lossyScale = bodyCollider.transform.lossyScale;
        float totalScaledArea = localArea * Mathf.Abs(lossyScale.x) * Mathf.Abs(lossyScale.y);

        return totalScaledArea;
    }
}
