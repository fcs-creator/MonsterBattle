using UnityEngine;
using UnityEngine.TextCore.Text;
using TMPro;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;
using System.Threading.Tasks;

public class UIActionBar : MonoBehaviour
{
    public Monster Owner { get; set; }       // キャラクターの参照
    public Transform Character { get; set; } // キャラクターのTransform
    public Vector3 Offset { get; set; }      // キャラクターからのオフセット
    TextMeshProUGUI text;                    // ゲージの画像
    
    void Awake()
    {
        text = transform.Find("Text").GetComponent<TextMeshProUGUI>();
        gameObject.SetActive(false);
    }

    void Start()
    {
        _ = UpdateActionBar();
    }
    
    async Task UpdateActionBar()
    {
        while (!Owner.IsDead)
        {
            UpdatePosition();

            await Task.Yield();
        }

        Hide();
    }

    // テキストを表示するメソッド
    public void SendText(string value)
    {
        UpdatePosition();

        text.text = value;

        gameObject.SetActive(true);

        Invoke("Hide", 1);
    }

    void Hide() 
    {
        gameObject.SetActive(false);
    }

    void UpdatePosition() 
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

                //バーの位置を更新
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
}
