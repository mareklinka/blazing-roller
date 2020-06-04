using BlazingRoller.Unity;
using UnityEngine;

public class DieScript : MonoBehaviour
{
    private int _dieId;
    private int _valueMultiplier;
    private Rigidbody _rb;
    private Vector3? _targetPosition;
    private Quaternion? _targetRotation;
    private Vector3? _sourcePosition;
    private Quaternion? _sourceRotation;
    private float? _targetConfigurationStartTime;

    void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_targetPosition == null)
        {
            return;
        }

        const int repositionDuration = 30; // frames

        var frameDifference = Time.frameCount - _targetConfigurationStartTime.Value;
        var t = frameDifference / repositionDuration;
        transform.position = Vector3.Lerp(_sourcePosition.Value, _targetPosition.Value, t);
        transform.rotation = Quaternion.Lerp(_sourceRotation.Value, _targetRotation.Value, t);

        if (frameDifference == 30)
        {
            // we got into the
            _targetPosition = null;
            _targetRotation = null;
            _sourcePosition = null;
            _sourceRotation = null;
            _targetConfigurationStartTime = null;

            _rb.velocity = Vector3.zero;
            _rb.angularVelocity = Vector3.zero;
        }
    }

    public void RandomStart(int seed)
    {
        transform.rotation = new Quaternion(0, 0, 0, 1);
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

    public int GetId()
    {
        return _dieId;
    }

    public void SetId(int id)
    {
        _dieId = id;
    }

    public void RepositionTo(DieFinalConfiguration config)
    {
        _targetConfigurationStartTime = Time.frameCount;
        _targetPosition = new Vector3(config.X, config.Y, config.Z);
        _targetRotation = new Quaternion(config.RX, config.RY, config.RZ, config.RW);
        _sourcePosition = transform.position;
        _sourceRotation = transform.rotation;
    }

    private float Randomize(float origin, float range, System.Random random)
    {
        return (float)(origin + range * random.NextDouble());
    }
}
