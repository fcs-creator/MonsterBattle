using UnityEngine;
using System.Threading.Tasks;

public class Nishikun : Monster
{
    protected override async Task ActionLoop()
    {
        await Guard();

        await Forward(50);

        await Attack();

        await Backward(50);

        await JumpForward(30);
    }

}
