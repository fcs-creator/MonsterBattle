using UnityEngine;
using System.Threading.Tasks;

public class Braze : Monster
{
    protected override async Task ActionLoop()
    {
        await Forward(100);
    }
}
