using System;
using UnityEngine;

public class InitScript : MonoBehaviour
{
    public Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        RandomStart();
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void RandomStart()
    {
        var dice = GameObject.FindGameObjectsWithTag("Dice");
        Debug.Log($"Found {dice.Length} dice");
        transform.rotation = new Quaternion(0,0,0,1);
        var random = new System.Random();

        transform.position = new Vector3(0, 10, 0);

        rb.velocity = new Vector3((float)(60), 0, (float)(60));
        rb.angularVelocity = new Vector3((float)(10), (float)(10), (float)(10));
    }
}
