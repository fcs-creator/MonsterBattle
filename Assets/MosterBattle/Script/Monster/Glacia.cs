using UnityEngine;
using System.Threading.Tasks;

public class Glacia : Monster
{
    protected override async Task ActionLoop()
    {
        await Forward(100);
        
        await Attack();
        
        await BackStep(100);
    }

}
