using UnityEngine;
using System.Threading.Tasks;

public class Jackhammer: Weapon
{
    async protected override Task Attack() 
    {
        await Drawing();

        await Spin(360,0.2f);

        await Move(5,0,0.25f);

        await Move(-5,0,0.25f);

        await Move(5,0,0.25f);

        await Move(-5,0,0.25f);

        await Shot(0.7f, 50);

        await Shot(-0.2f, 60);
    }
}