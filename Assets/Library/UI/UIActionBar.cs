using UnityEngine;
using UnityEngine.TextCore.Text;
using TMPro;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;
using System.Threading.Tasks;
using System.Threading;

public class UIActionBar : MonoBehaviour
{
    public Monster Owner { get; set; }       // キャラクターの参照
    public Transform Character { get; set; } // キャラクターのTransform
    public Vector3 Offset { get; set; }      // キャラクターからのオフセット
    TextMeshProUGUI text;                    // ゲージの画像
    
    Canceler canceler = new Canceler();

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

        canceler.Cancel();
        canceler.Reset();

        Hide();
    }

    private async void Hide()
    {
        try
        {
            // 1秒待機 (キャンセル可能)
            await Task.Delay(1000, canceler.Token);

            if (Application.isPlaying) 
            {
                // キャンセルされていない場合に非表示にする
                gameObject.SetActive(false);
            }
            
        }
        catch (TaskCanceledException)
        {
            // タスクがキャンセルされた場合の処理（特に何も行わない）
        }
    }

    private void OnDestroy()
    {
        // オブジェクトが破棄される際にリソースを解放
        if (canceler != null)
        {
            canceler.Cancel();
            canceler.Dispose();
        }
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
