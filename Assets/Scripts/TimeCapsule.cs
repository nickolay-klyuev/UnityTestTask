using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeCapsule : MonoBehaviour
{
    private List<Vector3> positions = new List<Vector3>();
    private List<Quaternion> rotations = new List<Quaternion>();
    private Rigidbody rigidBody;

    private bool isRewindOver = false;
    public bool GetIsRewindOver()
    {
        return isRewindOver;
    }

    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        if (isRewindOver && positions.Count > 0 && rotations.Count > 0)
        {
            isRewindOver = false;
        }

        if (GameManager.rewindTime && !isRewindOver)
        {
            rigidBody.isKinematic = true;

            if (positions.Count > 0)
            {
                transform.position = positions[0];
                positions.RemoveAt(0);
            }

            if (rotations.Count > 0)
            {
                transform.rotation = rotations[0];
                rotations.RemoveAt(0);
            }

            if (positions.Count == 0 && rotations.Count == 0)
            {
                isRewindOver = true;
                rigidBody.isKinematic = false;
            }
        }
        else if (!GameManager.gameOver && GameManager.isBallLaunched)
        {
            positions.Insert(0, transform.position);
            rotations.Insert(0, transform.rotation);
        }
    }
}
