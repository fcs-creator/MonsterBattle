using UnityEngine;
using System.Threading.Tasks;

public class Yoda : Monster
{
    bool atk;
    protected override async Task ActionLoop()
    {
        // if (Distance > 10)
        // {
        //     await Forward(80);
        //     //await Guard();
        // }
        // else if(Distance <= 5) {
        //    // await Jump(100);

        //    // await Attack();
        // } 
        // else 
        // {
        //     //await BackStep(20);

        //     await Attack();
        //     await Guard();

        // }
        //Forward(1);
        //Attack();

        //await Forward(Distance * 50);
        if (atk)
        {
            await Attack();
        }
        else
        {
            //await Forward(Distance * 10);
        }
        atk = !atk;
    }
}
