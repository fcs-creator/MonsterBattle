using UnityEngine;
using System.Threading.Tasks;

public class Elder : Monster
{
    protected override async Task ActionLoop()
    {
        await Forward(70);
        
        await Attack();
        
        await BackStep(75);
    }

}
