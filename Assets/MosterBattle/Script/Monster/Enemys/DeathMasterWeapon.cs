using UnityEngine;
using System.Threading.Tasks;

public class DeathMasterWeapon: Weapon
{
    //1
    async protected override Task Attack(int number) 
    {
        
    }

    async protected Task CloneShotAround() 
    {
        var clones = await Clone(10);

        for (int i = 0; i < 10; i++)
        {
            var dirction = (clones[i].transform.position - Owner.transform.position).normalized;
            await clones[i].ShotDirection(dirction, 30);
        }
    }

    async protected Task CloneAround() 
    {
        var clones = await Clone(10);

        Vector2 center = Owner.Enemy.transform.position;
        float radius = 10;

        for (int i = 0; i < 10; i++)
        {
            // クローンの位置を円周上に配置
            float angle = 2 * Mathf.PI / 10 * i; // ラジアン単位で計算
            float x = center.x + radius * Mathf.Cos(angle);
            float y = center.y + radius * Mathf.Sin(angle);
            clones[i].transform.position = new Vector3(x, y, 0);
        }
    }
}
