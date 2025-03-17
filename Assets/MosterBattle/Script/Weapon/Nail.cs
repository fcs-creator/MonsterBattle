using UnityEngine;
using System.Threading.Tasks;

public class Nail: Weapon
{
    async protected override Task Attack(int number) 
    {
        await Drawing();

        //var clones = await Clone(10);
        //
        //foreach (var clone in clones) 
        //{
        //    await clone.Move(3,0, 0.1f);
        //
        //    await clone.Move(-3, 0, 0.1f);
        //
        //    await clone.Move(3, 0, 0.1f);
        //
        //    await clone.Move(-3, 0, 0.1f);
        //
        //    await clone.Rotate(0,360, 0.1f);
        //
        //    await clone.Rotate(0, 360, 0.1f);
        //
        //    await clone.Rotate(0, 360, 0.1f);
        //
        //    await clone.Shot(Random.Range(-0.1f,0.1f), 50);
        //}
        //
        //
        //await Spin(360,1);

    }
}