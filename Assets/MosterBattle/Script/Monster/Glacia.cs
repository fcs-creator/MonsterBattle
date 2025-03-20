using UnityEngine;
using System.Threading.Tasks;

public class Glacia : Monster
{
    protected override async Task ActionLoop()
    {
        if (EnemyDistance > 10)
        {
            await Forward(80);
        }
        else 
        {
            await Backward(80);
        
            await Attack();
        
            await Guard();
        }
    }
}
