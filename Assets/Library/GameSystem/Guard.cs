using UnityEngine;

public class Guard : MonoBehaviour
{
    public Monster Owner { get; private set; }          // ����̏��L��(�����X�^�[)

    void Awake()
    {
        // 1��̊K�w�ɂ��郂���X�^�[�̃I�u�W�F�N�g��T���ăZ�b�g
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
                    //�X�^����Ԃ�L���ɂ���
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
