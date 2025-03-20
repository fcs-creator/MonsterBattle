using Unity.VisualScripting;
using UnityEngine;

public enum MagicType
{
    FireBall,
    Thunder,
    Ice,
}

public class Magic : MonoBehaviour
{
    public Monster Owner { get;  set; }
    public MagicType Type { get;  set; }

    public bool hasReflected = false;

    public float Damage { get; protected set; }

    void Awake()
    {
        hasReflected = false;

        gameObject.tag = Tags.Magic;
    }

    public void SetDamage(float value) 
    {
        Damage = value;    
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        GameObject obj = other.gameObject;

        if (obj.CompareTag(Tags.Guard)) 
        {
            var guard = obj.GetComponent<Guard>();

            
        }
    }
}
