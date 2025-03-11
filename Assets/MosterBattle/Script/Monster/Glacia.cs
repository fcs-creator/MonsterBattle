using UnityEngine;
using System.Threading.Tasks;

public class Glacia : Monster
{
    protected override async Task ActionLoop()
    {
        await Forward(80);

        await BackStep(60);

        await Attack();

        await Guard();
    }

}
