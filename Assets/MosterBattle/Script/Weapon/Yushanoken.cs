using UnityEngine;
using System.Threading.Tasks;

public class Yushanoken :Weapon
{
    async protected override Task Attack() 
    {
        await Drawing();

        //await Rotate(0, 120, 0.25f);

        //await Rotate(-120, 0, 0.25f);

        await Shot(0.3f, 50);
    }
}