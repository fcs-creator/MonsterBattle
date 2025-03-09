using UnityEngine;
using System.Threading.Tasks;

public class Braze : Monster
{
    protected override async Task ActionLoop()
    {
        await Guard();

        await Forward(100);
        
        await Attack();
        
        await BackStep(50);
        
        //await MagicBook.FireBall(this,5,30f);
        
        await JumpForward(70);

        //await JumpBackward(100);

        //await Jump();
    }

}
