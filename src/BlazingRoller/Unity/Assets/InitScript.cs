using UnityEngine;

public class InitScript : MonoBehaviour
{
    public Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void RandomStart(int seed)
    {
        transform.rotation = new Quaternion(0,0,0,1);
        var r = new System.Random(seed);

        transform.position = new Vector3(0, 10, 0);

        rb.velocity = new Vector3((float)(60 + 60 * r.NextDouble()), 0, (float)(60 + 60 * r.NextDouble()));
        rb.angularVelocity = new Vector3((float)(10 + 7 * r.NextDouble()), (float)(10 + 7 * r.NextDouble()), (float)(10 + 7 * r.NextDouble()));
    }
}
