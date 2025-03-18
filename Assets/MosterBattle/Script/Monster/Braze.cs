using UnityEngine;
using System.Threading.Tasks;

public class Braze : Monster
{
    protected override async Task ActionLoop()
    {
        await Forward(50);

        await Attack();


        //await Guard();



        //
        //await Attack();
        //
        //await BackStep(50);
        //
        //await JumpForward(30);
    }

}
