using UnityEngine;
using System.Threading.Tasks;

public class EvilSummoner : Monster
{
    protected override async Task ActionLoop()
    {
        //ワーウルフによる攻撃
        SwitchWeapon(0);

        await Attack();

        if (Mathf.Abs(Position.x) < 10)
        {
            await Backward(30);
        }
        else 
        {
            if (Enemy.Hp < Hp && EnemyDistance >15)
            {
                await Guard();
            }
            else 
            {
                await Forward(200);
            } 
        }
        
        //フライングアイによる攻撃
        SwitchWeapon(1);

        await Attack();
    }
}
