﻿using UnityEngine;
using System.Threading.Tasks;

public class Nail: Weapon
{
    async protected override Task Attack() 
    {
        await Drawing();

        await Spin(360, 1.0f);

        await Spin(360, 2f);

    }
}