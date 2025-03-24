using UnityEngine;
using System.Threading.Tasks;
using Unity.VisualScripting.YamlDotNet.Core;

public class ぽっちゃろう_Weapon_66ccc7eee7a74f0fac44a0401f78115c : Weapon
{
	async protected override Task Attack(int number)
	{
        await Drawing();
        await Spin (360,5);
    }
}
