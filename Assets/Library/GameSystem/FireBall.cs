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
        Destroy(gameObject, Parameters.FIREBALL_DESTOROY_WAIT_TIME);
        
        //èWíeó¶ÇåvéZÇµÇƒî≠éÀ
        float grouping = Parameters.FIREBALL_SHOT_GROUPING;
        float adjust_y = Parameters.FIREBALL_SHOT_ADJUST_Y;
        rb.AddForce(new Vector2(Direction.x, Random.Range(-grouping, grouping)+ adjust_y) * Speed, ForceMode2D.Impulse);
    }
}