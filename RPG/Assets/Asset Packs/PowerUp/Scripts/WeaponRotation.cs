using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponRotation : MonoBehaviour
{
    public float rotationSpeed = 99.0f;
    public bool reverse = false;
    void Update ()
    {
        if(this.reverse)
            //transform.Rotate(Vector3.back * Time.deltaTime * this.rotationSpeed);
            transform.Rotate(new Vector3(0f,1f,0f) * Time.deltaTime * this.rotationSpeed);
        else
            //transform.Rotate(Vector3.forward * Time.deltaTime * this.rotationSpeed);
            transform.Rotate(new Vector3(0f,1f,0f) * Time.deltaTime * this.rotationSpeed);
    }

    public void SetRotationSpeed(float speed)
    {
        this.rotationSpeed = speed;
    }

    public void SetReverse(bool reverse)
    {
        this.reverse = reverse;
    }
}
