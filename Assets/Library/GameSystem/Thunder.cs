using UnityEngine;

public class Thunder : Magic
{
    public Vector2 Direction { get; set; }

    void Start()
    {
        Type = MagicType.Thunder;
        Damage = Parameters.THUNDER_DAMAGE;

        Destroy(gameObject, Parameters.THUNDER_DESTOROY_WAIT_TIME);

        if (Direction.x >= 0) 
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
        else
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
    }
}
