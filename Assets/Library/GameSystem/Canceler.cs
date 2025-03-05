using System;
using System.Threading;

public interface ICancellable
{
    void Cancel(); // キャンセルを実行
    CancellationToken Token { get; } // トークンを取得
}

public class Canceler : ICancellable
{
    CancellationTokenSource cts;

    public Canceler()
    {
        cts = new CancellationTokenSource();
    }

    // トークンを公開
    public CancellationToken Token => cts.Token;

    // キャンセルされていない場合
    public bool IsNotCancel => !cts.IsCancellationRequested;

    public void Cancel()
    {
        if (!cts.IsCancellationRequested)
        {
            cts.Cancel();
        }
    }

    public void Reset()
    {
        cts?.Dispose();
        cts = new CancellationTokenSource();
    }
}
