using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class IceNeedle : Magic
{
    Rigidbody2D rb;

    public bool isExplode = false;
    List<Transform> ices = new List<Transform> ();
    Transform iceCenter = null;

    Vector3 hitPosition;
    Quaternion hitRotation;

    public Vector2 Direction { get; set; }
    public float Speed { get; set; }

    void Start()
    {
        foreach (Transform child in transform) 
        {
            ices.Add(child);
            if (child.name == "IceCenter") 
            {
                iceCenter = child;
            }
            child.gameObject.SetActive(false);
        }

        Type = MagicType.Ice;
        Damage = Parameters.ICE_BALL_DAMAGE;
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 4.2f;
        rb.AddForce(Direction * Speed, ForceMode2D.Impulse);
    }

    void Explode() 
    {
        if (!isExplode) 
        {
            foreach (Transform ice in ices)
            {
                ice.gameObject.SetActive(true);
            }
            transform.rotation = hitRotation;
            Damage = Parameters.ICE_DAMAGE;
            rb.constraints = RigidbodyConstraints2D.FreezeAll;
            Destroy(gameObject, Parameters.ICE_DESTROY_WAIT_TIME);
            AudioManager.Instance.PlaySE(Parameters.SE_MAGIC_ICE);
            isExplode = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        bool hit = false;

        if (other.CompareTag(Tags.Platform))
        {
            hit = true;
            hitPosition = other.ClosestPoint(transform.position);
            hitRotation = Quaternion.Euler(0,0,0);
        }
        else if (other.CompareTag(Tags.StageWall))
        {
            hit = true;
            hitPosition = other.ClosestPoint(transform.position);

            if (hitPosition.x >= 0)
            {
                hitRotation = Quaternion.Euler(0, 0, 90);
            }
            else
            {
                hitRotation = Quaternion.Euler(0, 0, -90);
            }
            
        }
        else if (other.CompareTag(Tags.Loof)) 
        {
            hit = true;
            hitPosition = other.ClosestPoint(transform.position);
            hitRotation = Quaternion.Euler(0, 0, 180);
        }

        if (hit) 
        {
            Explode();
        }
        
    }
}
