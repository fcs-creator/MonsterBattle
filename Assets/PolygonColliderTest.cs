using UnityEngine;

public class PolygonColliderTest : MonoBehaviour
{
    // �U���i�ړ��͈͂̔����j
    public float amplitude = 1.0f;

    // �����i�ړ����x�j
    public float period = 1.0f;

    // �����ʒu
    private Vector3 initialPosition;

    Rigidbody2D rb;

    void Start()
    {
        // �I�u�W�F�N�g�̏����ʒu��ۑ�
        initialPosition = transform.position;


        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // ���Ԍo�߂ɉ�����X���W���v�Z
        float x = Mathf.Sin(Time.time * Mathf.PI * 2.0f / period) * amplitude;

        // �V�����ʒu��ݒ�
        //transform.position = new Vector3(initialPosition.x + x, initialPosition.y, initialPosition.z);

        rb.position = new Vector3(initialPosition.x + x, initialPosition.y, initialPosition.z);
    }
}