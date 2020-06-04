using UnityEngine;

public class DieScript : MonoBehaviour
{
    private int _valueMultiplier;
    private Rigidbody _rb;

    void Awake()
    {
        _rb = GetComponent<Rigidbody>();
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

        _rb.velocity = new Vector3(Randomize(-40, 80, r), Randomize(8, 6, r), Randomize(-40, 80, r));
        _rb.angularVelocity = new Vector3(Randomize(-10, 20, r), Randomize(-10, 20, r), Randomize(-10, 20, r));
    }

    public void SetMultiplier(int multiplier)
    {
        _valueMultiplier = multiplier;
    }

    public int GetValue()
    {
        return GetComponent<DiceStats>().side * _valueMultiplier;
    }

    private float Randomize(float origin, float range, System.Random random)
    {
        return (float)(origin + range * random.NextDouble());
    }
}
