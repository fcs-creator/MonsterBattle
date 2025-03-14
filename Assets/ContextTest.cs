using UnityEngine;

public class ContextTest : MonoBehaviour
{
    public void ShowAppearance()
    {
        // 見た目を確認するための処理
        Debug.Log("外観を確認します！");
    }

    [ContextMenu("外観を確認")]
    private void ShowAppearanceFromContextMenu()
    {
        ShowAppearance(); // ボタン機能を実行
    }
}
