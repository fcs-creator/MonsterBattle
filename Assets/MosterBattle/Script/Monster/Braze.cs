using UnityEngine;
using System.Threading.Tasks;

public class Braze : Monster
{
    protected override async Task ActionLoop()
    {
        await Attack();

        await Forward(100);

        //await Guard();

       

        //
        //await Attack();
        //
        //await BackStep(50);
        //
        //await JumpForward(30);
    }

}
