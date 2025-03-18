using UnityEngine;
using System.Threading.Tasks;

public class Nail: Weapon
{
    async protected override Task Attack(int number) 
    {
        await Drawing();

        await Shot(0.2f, 25);

        await Shot(0.2f, 50);

        await Shot(0.2f, 100);

        await Shot(0.2f, 250);

        await Shot(0.2f, 500);

        await Shot(0.2f, 1000);

        await Shot(0.2f, 10000);

        //var clones = await Clone(10);
        //
        //foreach (var clone in clones) 
        //{
        //    if (clone != null) 
        //    {
        //        await clone.Move(3, 0, 0.1f);
        //
        //        await clone.Move(-3, 0, 0.1f);
        //
        //        await clone.Move(3, 0, 0.1f);
        //
        //        await clone.Move(-3, 0, 0.1f);
        //
        //        await clone.Rotate(0, 360, 0.1f);
        //
        //        await clone.Rotate(0, 360, 0.1f);
        //
        //        await clone.Rotate(0, 360, 0.1f);
        //
        //        await clone.Shot(Random.Range(-0.1f, 0.1f), 50);
        //    }
        //}


        //await Spin(360,1);

    }
}