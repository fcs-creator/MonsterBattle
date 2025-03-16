using UnityEngine;
using System.Threading.Tasks;

public class Wind: Weapon
{
    async protected override Task Attack(int number)
    {
        await Drawing();

        await Shot(0.2f, 70f);
    }
}