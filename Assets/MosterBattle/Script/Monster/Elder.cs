using UnityEngine;
using System.Threading.Tasks;

public class Elder : Monster
{
    protected override async Task ActionLoop()
    {
        await Attack();

        await Forward(70);
        
        await BackStep(75);
    }

}
