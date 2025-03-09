using UnityEngine;
using System.Threading.Tasks;

public class Wind: Weapon
{
    async protected override Task Attack()
    {
        await Drawing();

        await Shot(new Vector2(0.8f, 0.2f), 50f);
    }
}