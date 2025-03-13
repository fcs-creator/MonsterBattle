using UnityEngine;
using System.Threading.Tasks;

public class Square : Monster
{
    protected override async Task ActionLoop()
    {
        if (Enemy.IsStunned)
        {
            await Attack();
        }
        else 
        {
            await Guard();
        }

    }

}
