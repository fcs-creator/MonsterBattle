using UnityEngine;

public class Guard : MonoBehaviour
{
    public Monster Owner { get; private set; }          // 武器の所有者(モンスター)

    void Awake()
    {
        // 1つ上の階層にいるモンスターのオブジェクトを探してセット
        Owner = transform.parent.GetComponent<Monster>();
        gameObject.tag = Tags.Guard;
        gameObject.AddComponent<PolygonCollider2D>().autoTiling = true;
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        GameObject obj = collider.gameObject;

        if(obj.CompareTag(Tags.Weapon))
        {
            if (HasComponent<Weapon>(obj))
            {
                Weapon weapon = obj.GetComponent<Weapon>();

                if (weapon.Owner != Owner)
                {
                    //スタン状態を有効にする
                    weapon.Owner.IsStunned = true;

                    Debug.Log("Guard Hit->Stun Flag On !!!!");
                }
            }
        }
    }

    bool HasComponent<T>(GameObject obj) where T : Component
    {
        return obj.GetComponent<T>() != null;
    }
}
