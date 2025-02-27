using Unity.VisualScripting;
using UnityEngine;

public enum MagicType
{
    FireBall,
    Thunder,
}

public class Magic : MonoBehaviour
{
    public Monster Owner { get;  set; }
    public MagicType Type { get;  set; }

    Collider2D[] colliders2D;
    Collider2D[] parentColliders2D;

    public float Damage { get; protected set; }

    void Awake()
    {
        // 親オブジェクトのすべての2Dコライダーを取得してisTriggerを設定
        parentColliders2D = GetComponents<Collider2D>();

        // 親オブジェクトのすべての子オブジェクトをループ
        foreach (Transform child in transform)
        {     
            // 子オブジェクトのすべての2Dコライダーを取得してisTriggerを設定
            colliders2D = child.GetComponents<Collider2D>();
        }

        SetCollisionEnable(false);
    }

    public void SetCollisionEnable(bool value) 
    {
        // 親オブジェクトのすべての2Dコライダーを取得してisTriggerを設定
        foreach (Collider2D collider in parentColliders2D)
        {
            collider.isTrigger = !value;
        }

        // 親オブジェクトのすべての子オブジェクトをループ
        foreach (Transform child in transform)
        {   
            foreach (Collider2D collider in colliders2D)
            {
                collider.isTrigger = !value;
            }
        }
    }
}
