using UnityEngine;
using System.Threading.Tasks;

public class Mushroom : Monster
{
    protected override async Task ActionLoop()
    {
        await Attack();

        await Forward(120);
    }
}
