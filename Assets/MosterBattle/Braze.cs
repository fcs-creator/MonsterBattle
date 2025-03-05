using UnityEngine;
using System.Threading.Tasks;

public class Braze : Monster
{
    protected override async Task ActionLoop()
    {
        await Dash(5);
        
        await Wait(0.75f);
        
        await Attack();
        
        await BackStep(10.0f);
        
        await Wait(0.5f);
        
        await MagicBook.FireBall(this,5,30f);

        await Jump(5);

        await Wait(1);
    }

}
