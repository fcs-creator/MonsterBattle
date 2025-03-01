using UnityEngine;
using System.Threading.Tasks;

public class MonsterBlue : Monster
{
    protected override async Task ActionLoop()
    {
        await Attack();

        await Dash(50);
        
        await Wait(1);
        
        await Jump(50);
        
        await Wait(1);
        
        await BackStep(6);
        
        await MagicBook.FireBall(this, 3, 10);
        
        await MagicBook.Thunder(this);
        
        await Wait(1);
        
        await Jump(15);
        
        await Wait(1);
    }
}
