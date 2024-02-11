using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] Transform car;
    [SerializeField] Vector3 offset;
    [SerializeField] float translateSpeed;
    [SerializeField] float rotationSpeed;

    void Start()
    {

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        HandleTranslation();
        HandleRotation();
    }

    void HandleTranslation()
    {
        var targetPosition = car.TransformPoint(offset);
        transform.position = Vector3.Lerp(transform.position, targetPosition, translateSpeed * Time.deltaTime);
    }

    void HandleRotation()
    {
        var direction = car.position - transform.position;
        var rotation = Quaternion.LookRotation(direction, Vector3.up);
        transform.rotation = Quaternion.Lerp(transform.rotation, rotation, translateSpeed * Time.deltaTime);

    }
}
