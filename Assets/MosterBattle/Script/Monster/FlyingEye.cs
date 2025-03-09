using UnityEngine;
using System.Threading.Tasks;

public class FlyingEye : Monster
{
    protected override async Task ActionLoop()
    {
        await Forward(75);

        await BackStep(25);
    }

}
