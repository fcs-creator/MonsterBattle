using UnityEngine;

public class FireBall : Magic
{
    public Vector2 Direction { get; set; }
    public float Speed { get; set; }

    public Rigidbody2D rb;

    void Start()
    {
        Type = MagicType.FireBall;
        Damage = Parameters.FIREBALL_DAMAGE;
        rb = GetComponent<Rigidbody2D>();
        Destroy(gameObject, Parameters.FIREBALL_DESTOROY_WAIT_TIME);
        
        //集弾率を計算して発射
        float grouping = Parameters.FIREBALL_SHOT_GROUPING;
        float dirX = Direction.x + Random.Range(-grouping,grouping);
        float dirY = Direction.y + Random.Range(-grouping, grouping);
        rb.AddForce(new Vector2(dirX, dirY).normalized * Speed, ForceMode2D.Impulse);
    }
}