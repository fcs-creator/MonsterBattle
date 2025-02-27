using UnityEngine;

public class PolygonColliderTest : MonoBehaviour
{
    // 振幅（移動範囲の半分）
    public float amplitude = 1.0f;

    // 周期（移動速度）
    public float period = 1.0f;

    // 初期位置
    private Vector3 initialPosition;

    Rigidbody2D rb;

    void Start()
    {
        // オブジェクトの初期位置を保存
        initialPosition = transform.position;


        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // 時間経過に応じてX座標を計算
        float x = Mathf.Sin(Time.time * Mathf.PI * 2.0f / period) * amplitude;

        // 新しい位置を設定
        //transform.position = new Vector3(initialPosition.x + x, initialPosition.y, initialPosition.z);

        rb.position = new Vector3(initialPosition.x + x, initialPosition.y, initialPosition.z);
    }
}