using UnityEngine;
using System.Threading.Tasks;

public class WeaponTest: Weapon
{
    async protected override Task Attack() 
    {
        await Move(10, 0, 0.5f);

        await Spin(360, 0.2f);

        await Move(-10, 0, 0.5f);

        //await Rotate(0, 120, 0.5f);

        //await Default();

        //await Wait(1.0f);
        //
        //await Move(-10, 0, 0.2f);
        //


    }
}
