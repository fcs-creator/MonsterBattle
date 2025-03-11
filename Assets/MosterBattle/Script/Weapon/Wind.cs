using UnityEngine;
using System.Threading.Tasks;

public class Wind: Weapon
{
    async protected override Task Attack()
    {
        await Drawing();

        await Shot(new Vector2(0.7f, 0.3f), 70f);
    }
}