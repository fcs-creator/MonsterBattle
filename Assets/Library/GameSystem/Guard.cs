using UnityEngine;

public class Guard : MonoBehaviour
{
    public Monster Owner { get; private set; }          // ����̏��L��(�����X�^�[)

    void Awake()
    {
        // 1��̊K�w�ɂ��郂���X�^�[�̃I�u�W�F�N�g��T���ăZ�b�g
        Owner = transform.parent.GetComponent<Monster>();
        gameObject.tag = Tags.Guard;
        var collider = gameObject.AddComponent<PolygonCollider2D>();
        collider.autoTiling = true;
        collider.isTrigger = true;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        GameObject obj = other.gameObject;

        if(obj.CompareTag(Tags.Weapon))
        {
            if (HasComponent<Weapon>(obj))
            {
                Weapon weapon = obj.GetComponent<Weapon>();

                if (weapon.Owner != Owner)
                {
                    weapon.Owner.IsStunned = true;

                    //�K�[�h�G�t�F�N�g�̍Đ�
                    PlayGuardVFX(obj, other);

                    //����̏��L�҂𐁂���΂��������v�Z
                    Vector2 direction = (weapon.Owner.transform.position - transform.position).normalized;
                    weapon.Owner.GetComponent<Rigidbody2D>().AddForce(direction * weapon.Damage * Parameters.GUARD_FORCE_SCALE, ForceMode2D.Impulse);

                    //�p���B�����Đ�
                    AudioManager.Instance.PlaySE(Parameters.SE_PARRY);

                    //�X�^����Ԃ�L���ɂ���
                    weapon.Owner.IsStunned = true;

                    Debug.Log("Guard Hit->Stun Flag On !!!!");
                }
            }
        }
    }

    //�K�[�h�q�b�g�G�t�F�N�g�̍Đ�
    void PlayGuardVFX(GameObject weapon, Collider2D weaponCollider) 
    {
        // �Փ˓_���擾
        Vector3 collisionPoint = weaponCollider.ClosestPoint(transform.position);

        // �����̈ʒu����ɂ��ďՓ˂̖@���x�N�g�����v�Z
        Vector3 hitNormal = (weapon.transform.position - transform.position).normalized;
        Quaternion rotation = Quaternion.LookRotation(hitNormal);

        // �K�[�h�G�t�F�N�g�̍Đ�
        VFXManager.Instance.Play(VFX.Guard, collisionPoint, rotation);
    }

    bool HasComponent<T>(GameObject obj) where T : Component
    {
        return obj.GetComponent<T>() != null;
    }
}
