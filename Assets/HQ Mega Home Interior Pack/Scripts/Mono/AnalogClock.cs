using UnityEngine;

public class AnalogClock : Clock
{
    public enum Axis { X, Y, Z }

    [Header("References")]
    
    [SerializeField] private Transform      hour            = null;
    [SerializeField] private Transform      minute          = null;
    [SerializeField] private Transform      second          = null;
   
    [SerializeField] private Axis           axis            = Axis.Z;

    protected override void Start()
    {
        base.Start();

        Rotate(hour, LocalEulerValue(hour) + (30f * (float)current.hour.value));
        Rotate(hour, LocalEulerValue(hour) + (30f * ((float)current.minute.value / 60f)));

        Rotate(minute, LocalEulerValue(minute) + (6f  * (float)current.minute.value));
        Rotate(second, LocalEulerValue(second) + (6f  * (float)current.second.value));
        
        if (_startOnPlay)
        {
            Begin();
        }
    }

    private void Update()
    {
        if (current.second.changed)
        {
            Rotate(second, LocalEulerValue(second) + 6);
            current.second.changed = false;
        }
        if (current.minute.changed)
        {
            Rotate(minute, LocalEulerValue(minute) + 6);
            current.minute.changed = false;

            Rotate(hour, LocalEulerValue(hour) + (30f * (1f / 60f)));
        }
    }

    private void Rotate(Transform arrow, float rot)
    {
        var euler = arrow.localEulerAngles;
        if (rot >= 360)
        {
            rot = 0;
        }
        switch (axis)
        {
            case Axis.X:
                euler.x = rot;
                break;
            case Axis.Y:
                euler.y = rot;
                break;
            case Axis.Z:
                euler.z = rot;
                break;
        }
        arrow.localEulerAngles = euler;
    }

    float LocalEulerValue(Transform arrow)
    {
        switch (axis)
        {
            case Axis.X:
                return arrow.localEulerAngles.x;
            case Axis.Y:
                return arrow.localEulerAngles.y;
            case Axis.Z:
                return arrow.localEulerAngles.z;
            default:
                return 0;
        }
    }
}