using UnityEngine;
using System.Threading.Tasks;

public class GoblinWeapon: Weapon
{
    async protected override Task Attack(int number) 
    {
        await Drawing();

        await ShotDirection(Owner.EnemyDirection, 45);
    }
}
