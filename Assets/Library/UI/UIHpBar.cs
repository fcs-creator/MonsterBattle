using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;

public class UIHPBar : MonoBehaviour
{
    
    public Transform Character { get; set; } // キャラクターのTransform
    public Vector3 Offset { get; set; }      // キャラクターからのオフセット
    public float Hp { get; private set; }    // 現在のHP

    Image gauge;                    // ゲージの画像
    const float MAX_HP = 100.0f;    // 最大HP

    void Awake()
    {
        Hp = MAX_HP;
    }

    void Start()
    {
        gauge = transform.Find("Gauge").GetComponent<Image>();
    }

    void Update()
    {
        if (Character != null)
        {
            // カメラの参照が正しいかチェック
            if (Camera.main != null)
            {
                // キャラクターのワールド座標をスクリーン座標に変換
                Vector3 screenPosition = Camera.main.WorldToScreenPoint(Character.position + Offset);

                // スクリーン座標をキャンバスの座標に変換
                Vector2 localPoint;
                Canvas canvas = GameObject.Find("UIPlay").GetComponent<Canvas>();
                RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    canvas.transform as RectTransform,
                    screenPosition,
                    Camera.main,
                    out localPoint
                );

                // HPバーの位置を更新
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

    // ダメージを受けるメソッド
    public void TakeDamage(float damage)
    {
        SetHp(Hp-damage);
    }

    // HPを更新するメソッド
    public void SetHp(float value)
    {
        Hp = Mathf.Clamp(value, 0, MAX_HP);
        gauge.fillAmount = Hp / MAX_HP;
    }

    public bool IsEmpty() 
    {
        return Hp <= 0;
    }
}
