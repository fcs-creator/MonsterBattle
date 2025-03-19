using UnityEngine;
using System.Threading.Tasks;

public class Mushroom : Monster
{
    protected override async Task ActionLoop()
    {
        if (EnemyDistance < 5)
        {
            await Guard();

            await Attack();
        }
        else
        { 

        }

        
    }
}
