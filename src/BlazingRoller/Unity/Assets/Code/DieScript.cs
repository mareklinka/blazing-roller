using UnityEngine;

public class DieScript : MonoBehaviour
{
    private Rigidbody rb;

    void Awake()
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

        transform.position = new Vector3(Randomize(-2.5F, 5, r), Randomize(10, 5, r), Randomize(-2.5F, 5, r));

        rb.velocity = new Vector3(Randomize(-30, 60, r), Randomize(3, 6, r), Randomize(-30, 60, r));
        rb.angularVelocity = new Vector3(Randomize(-10, 20, r), Randomize(-10, 20, r), Randomize(-10, 20, r));
    }

    private float Randomize(float origin, float range, System.Random random)
    {
        return (float)(origin + range * random.NextDouble());
    }
}
