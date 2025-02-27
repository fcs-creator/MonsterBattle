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
        // �e�I�u�W�F�N�g�̂��ׂĂ�2D�R���C�_�[���擾����isTrigger��ݒ�
        parentColliders2D = GetComponents<Collider2D>();

        // �e�I�u�W�F�N�g�̂��ׂĂ̎q�I�u�W�F�N�g�����[�v
        foreach (Transform child in transform)
        {     
            // �q�I�u�W�F�N�g�̂��ׂĂ�2D�R���C�_�[���擾����isTrigger��ݒ�
            colliders2D = child.GetComponents<Collider2D>();
        }

        SetCollisionEnable(false);
    }

    public void SetCollisionEnable(bool value) 
    {
        // �e�I�u�W�F�N�g�̂��ׂĂ�2D�R���C�_�[���擾����isTrigger��ݒ�
        foreach (Collider2D collider in parentColliders2D)
        {
            collider.isTrigger = !value;
        }

        // �e�I�u�W�F�N�g�̂��ׂĂ̎q�I�u�W�F�N�g�����[�v
        foreach (Transform child in transform)
        {   
            foreach (Collider2D collider in colliders2D)
            {
                collider.isTrigger = !value;
            }
        }
    }
}
