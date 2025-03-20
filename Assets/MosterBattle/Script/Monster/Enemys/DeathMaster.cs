using UnityEngine;
using System.Threading.Tasks;

public class DeathMaster : Monster
{
    protected override async Task ActionLoop()
    {
        await Task.Yield();
    }
}
