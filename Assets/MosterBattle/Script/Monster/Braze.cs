using UnityEngine;
using System.Threading.Tasks;

public class Braze : Monster
{
    protected override async Task ActionLoop()
    {
        //ここにプログラムを書く

        await Task.Yield();
    }
}