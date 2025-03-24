using UnityEngine;
using System.Threading.Tasks;

public class Braze : Monster
{
    protected override async Task ActionLoop()
    {
        await MagicBook.FireBall(this, 3, 30f);

        await Forward(100);

        await Attack();

        await Guard();

        await MagicBook.FireBall(this, 3, 30f);

        await JumpForward(30);
    }
}