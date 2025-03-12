using UnityEngine;
using System.Threading.Tasks;

public class Glacia : Monster
{
    protected override async Task ActionLoop()
    {
        if (Distance > 10)
        {
            await Forward(80);
        }
        else 
        {
            await BackStep(80);

            await Attack();

            await Guard();
        }
    }
}
