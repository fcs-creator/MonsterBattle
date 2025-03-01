using UnityEngine;
using System.Threading.Tasks;

public class Braze : Monster
{
    protected override async Task ActionLoop()
    {
        await Dash(30.0f);
        
        await Wait(0.75f);
        
        await Attack();
        
        await BackStep(10.0f);
        
        await Wait(0.5f);
        
        await MagicBook.FireBall(this,3,20f);

        await Jump(30);

        await Wait(1);
    }

}
