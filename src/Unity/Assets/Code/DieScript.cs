using System.Linq;
using System;
using BlazingRoller.Unity;
using UnityEngine;

public class DieScript : MonoBehaviour
{
    private int _sign;
    private Rigidbody _rb;
    private Vector3? _targetPosition;
    private Quaternion? _targetRotation;
    private Vector3? _sourcePosition;
    private Quaternion? _sourceRotation;
    private float? _targetConfigurationStartTime;

    public int Id { get; set; }
    public bool IsPrimary { get; set; }
    public GameObject[] SecondaryDice { get; set; }
    public bool IsPercentile { get; set; }

    void Awake() => _rb = GetComponent<Rigidbody>();

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
        transform.rotation = Quaternion.Slerp(_sourceRotation.Value, _targetRotation.Value, t);

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

        _rb.velocity = new Vector3(Randomize(-80, 160, r), Randomize(10, 10, r), Randomize(-80, 160, r));
        _rb.angularVelocity = new Vector3(Randomize(-10, 20, r), Randomize(-10, 20, r), Randomize(-10, 20, r));
    }

    public void SetSign(int sign) => _sign = sign < 0 ? -1 : 1;

    public int GetValue()
    {
        if (IsPercentile)
        {
            var secondarySum = SecondaryDice.Sum(_ => _.GetComponent<DieScript>().GetValue());
            return (GetComponent<DiceStats>().side % 10 * 10 * _sign) + secondarySum;
        }
        else
        {
            return GetComponent<DiceStats>().side * _sign;
        }
    }

    public void RepositionTo(DieFinalConfiguration config)
    {
        _targetConfigurationStartTime = Time.frameCount;
        _targetPosition = new Vector3(config.X, config.Y, config.Z);
        _targetRotation = new Quaternion(config.RX, config.RY, config.RZ, config.RW);
        _sourcePosition = transform.position;
        _sourceRotation = transform.rotation;
    }

    public bool IsRepositioning() => _targetPosition != null;

    private float Randomize(float origin,
                            float range,
                            System.Random random) =>
        (float)(origin + (range * random.NextDouble()));
}
