using UnityEngine;

public class FireBall : Magic
{
    public Vector2 Direction { get; set; }
    public float Speed { get; set; }

    Rigidbody2D rb;

    void Start()
    {
        Type = MagicType.FireBall;
        Damage = Parameters.FIREBALL_DAMAGE;
        rb = GetComponent<Rigidbody2D>();
        Destroy(gameObject, Parameters.FIREBALL_DAMAGE);
        
        //èWíeó¶ÇåvéZÇµÇƒî≠éÀ
        float grouping = Parameters.FIREBALL_SHOT_GROUPING;
        rb.AddForce(new Vector2(Direction.x, Random.Range(-grouping, grouping)) * Speed, ForceMode2D.Impulse);
    }
}