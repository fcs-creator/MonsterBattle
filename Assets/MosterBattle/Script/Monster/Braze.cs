using UnityEngine;
using System.Threading.Tasks;

public class Braze : Monster
{
    protected override async Task ActionLoop()
    {
        await Guard();

        await Dash(100);
        
        await Attack();
        
        await BackStep(100);
        
        await MagicBook.FireBall(this,5,30f);
        
        await Jump(100);
    }

}
