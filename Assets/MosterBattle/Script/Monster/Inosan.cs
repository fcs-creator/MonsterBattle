using UnityEngine;
using System.Threading.Tasks;

public class Inosan : Monster
{
    protected override async Task ActionLoop()
    {

        await Guard();

        await Attack();

        await Guard();

        await Forward(100);

        await Attack();

        await JumpBackward(20);

        await JumpForward(30);

        await Attack();

        await Guard();
    }

}
