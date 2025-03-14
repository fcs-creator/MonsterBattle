using UnityEngine;
using System.Threading.Tasks;

public class Braze : Monster
{
    protected override async Task ActionLoop()
    {
        await Guard();
        
        //await Forward(100);
        //
        //await Attack();
        //
        //await BackStep(50);
        //
        //await JumpForward(30);
    }

}
