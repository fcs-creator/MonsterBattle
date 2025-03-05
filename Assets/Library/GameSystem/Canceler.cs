using System;
using System.Threading;

public interface ICancellable
{
    void Cancel(); // �L�����Z�������s
    CancellationToken Token { get; } // �g�[�N�����擾
}

public class Canceler : ICancellable
{
    CancellationTokenSource cts;

    public Canceler()
    {
        cts = new CancellationTokenSource();
    }

    // �g�[�N�������J
    public CancellationToken Token => cts.Token;

    // �L�����Z������Ă��Ȃ��ꍇ
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
