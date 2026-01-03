using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AiGate : MonoBehaviour
{
    public AiGate nextGate;
    public float maxSpeed;
    public float getRotation()
    {
        return gameObject.GetComponent<Transform>().rotation.y;
    }
    /*private void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.CompareTag("Player"))
        {
            float speed = collider.gameObject.GetComponent<Rigidbody>().velocity.magnitude * 3.6f;
            Debug.Log(gameObject.name + " " + speed + "km/h");
        }
    }*/

}
