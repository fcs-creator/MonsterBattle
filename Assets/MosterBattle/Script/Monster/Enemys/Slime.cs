using UnityEngine;
using System.Threading.Tasks;

public class Slime : Monster
{
    protected override async Task ActionLoop() 
    {
        await Forward(70);
    }
}