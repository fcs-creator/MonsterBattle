using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotatingsword : MonoBehaviour
{
    public Transform centerObject; // ���S�I�u�W�F�N�g
    public float radius = 7f; // ���a
    public float rotationSpeed = 100.0f; // ��]���x

    void start()
    {
        radius = (transform.position - centerObject.position).magnitude;
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
