using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading.Tasks;
using UnityEngine;

public class Rotatingsword : MonoBehaviour
{
    public Transform centerObject; // 中心オブジェクト
    public float radius = 7f; // 半径
    public float rotationSpeed = 100.0f; // 回転速度

    Rigidbody2D rb;

    void Start()
    {
        rb = transform.parent.gameObject.GetComponent<Rigidbody2D>();

        radius = (transform.position - centerObject.position).magnitude;

        _ = Execute();
    }

    async protected Task Execute()
    {
        while (true)
        {
            await AddForce();
        }
    }

    async protected Task AddForce() 
    {
        float x = Random.Range(-1.0f, 1.0f);
        float y = Random.Range(-1.0f, 1.0f);

        rb.AddForce(new Vector2(x, y) * 50, ForceMode2D.Impulse);

        await Task.Delay(1000);
    }

    void Update()
    {
        if (centerObject != null)
        {
            float angle = rotationSpeed * Time.deltaTime;
            transform.RotateAround(centerObject.position, Vector3.forward, angle);
            Vector3 desiredPosition = (transform.position - centerObject.position).normalized * radius + centerObject.position;
            transform.position = desiredPosition;
        }
    }
}
