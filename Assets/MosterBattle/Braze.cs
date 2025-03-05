using UnityEngine;
using System.Threading.Tasks;

public class Braze : Monster
{
    protected override async Task ActionLoop()
    {
        await Dash(100);
        
        await Wait(0.75f);
        
        await Attack();
        
        await BackStep(100);
        
        await Wait(0.5f);
        
        await MagicBook.FireBall(this,5,30f);

        await Jump(100);

        await Wait(1);
    }

}
