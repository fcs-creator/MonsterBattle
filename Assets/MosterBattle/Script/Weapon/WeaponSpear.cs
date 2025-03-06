using UnityEngine;
using System.Threading.Tasks;

public class WeaponSpear: Weapon
{
    async protected override Task Attack() 
    {
        await Spin(360, 0.2f);

        await Move(5, 0, 0.25f);

        await Move(-5, 0, 0.25f);

        await Move(5, 0, 0.25f);

        await Move(-5, 0, 0.25f);
    }
}
