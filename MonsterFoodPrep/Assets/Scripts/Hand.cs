﻿using UnityEngine;
using System.Collections;

public class Hand : MonoBehaviour
{
    public float moveSpeed = 10.0f;
    public float chopSpeed = 40.0f;
    public float rotationSpeed = 1.0f;
    public float releaseTime = 0.1f;

    [HideInInspector]
    public bool chopping;

    Vector3 choppingPoint;

    float y;
    float distance;

    float rotation;

    void Awake()
    {
        y = transform.position.y;
        distance = Vector3.Distance(Camera.main.transform.position, transform.position);
    }

    void Release()
    {
        chopping = false;
        HingeJoint hingeJoint = GetComponent<HingeJoint>();
        hingeJoint.connectedBody.constraints = RigidbodyConstraints.None;
    }

    void Update()
    {
        Rigidbody rigidbody = GetComponent<Rigidbody>();
        HingeJoint hingeJoint = GetComponent<HingeJoint>();

        if(Input.GetMouseButtonDown(0))
        {
            RaycastHit raycastHit;
            if(Physics.Raycast(new Ray(transform.position, Vector3.down), out raycastHit))
            {
                chopping = true;
                hingeJoint.connectedBody.constraints = RigidbodyConstraints.FreezeRotation;
                choppingPoint = raycastHit.point - raycastHit.normal;
            }
        }

        if(chopping)
        {
            rigidbody.MovePosition(Vector3.Lerp(transform.position, choppingPoint, Time.deltaTime * chopSpeed));
        }
        else
        {
            Vector3 screenPosition = Input.mousePosition;
            screenPosition.z = distance;
            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(screenPosition);
            worldPosition.y = y;
            rigidbody.MovePosition(Vector3.Lerp(transform.position, worldPosition, Time.deltaTime * moveSpeed));

            float scroll = Input.GetAxis("Mouse ScrollWheel");
            scroll = (scroll > 0.0f) ? Mathf.Ceil(scroll) : Mathf.Floor(scroll);
            rotation = Mathf.Clamp(rotation + scroll, -2, 2);
            rigidbody.MoveRotation(Quaternion.AngleAxis((90 / 2) * Mathf.Round(rotation), new Vector3(0, 1, 0)));
        }
    }
}