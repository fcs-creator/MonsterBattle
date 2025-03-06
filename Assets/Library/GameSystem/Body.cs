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
    float slowMotionScale = 0.1f; // スローの度合いを調整するパラメータ

    //タスクをキャンセル
    readonly Canceler canceler = new Canceler();

    void Awake()
    {
        //親のリジッドボディを取得
        rbParent = transform.parent.gameObject.GetComponent<Rigidbody2D>();
        rbParent.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        rbParent.gravityScale = Parameters.GRAVITY_SCALE;
        rbParent.freezeRotation = true;

        //物理マテリアルの設定
        rbParent.sharedMaterial = Resources.Load<PhysicsMaterial2D>("MonsterPhysicsMaterial");

        //タグの設定
        gameObject.tag = Tags.Body;

        //スプライトのソートレイヤーを設定
        sr = gameObject.GetComponent<SpriteRenderer>();
        sr.sortingLayerName = SortLayer.Body;

        //スプライトの色を取得
        originalColor = sr.color;

        //コライダーを設定
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

    public async Task Stun() 
    {
        await FlashAndHitStopTask();

        await Wait(Parameters.GUARD_STUN_DURATION);
    }

    private async Task FlashAndHitStopTask()
    {
        // スプライトの色を変更
        sr.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0.5f); // 半透明に設定
        await Wait(flashDuration);

        // ヒットストップとスローモーションの処理
        Time.timeScale = slowMotionScale;
        await Wait(hitStopDuration);
        Time.timeScale = 1f;

        // スプライトの色を元に戻す
        sr.color = originalColor;
    }

    async protected Task Wait(float sec)
    {
        if (canceler.IsCancel) return;

        await Task.Delay((int)(sec * 1000), canceler.Token);
    }
}
