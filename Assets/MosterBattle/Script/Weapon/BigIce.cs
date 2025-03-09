using UnityEngine;
using System.Threading.Tasks;

public class BigIce: Weapon
{
    async protected override Task Attack() 
    {
        await Rotate(0,100,0.5f);

        await Rotate(100, -100, 0.5f);

        await Rotate(0, 100, 0.5f);

        await Rotate(100, -100, 0.5f);

        await Shot(new Vector2(0.5f, -0.5f), 50f);
    }
}