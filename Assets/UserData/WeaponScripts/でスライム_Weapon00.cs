using UnityEngine;
using System.Threading.Tasks;

public class でスライム_Weapon00: Weapon
{
    async protected override Task Attack(int number)
    {
        await Drawing();

        await Shot(0, 100);
    }
}