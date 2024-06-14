using System;
using System.Collections;
using UnityEngine;

[Serializable()]
public struct TimeValue
{
    public int value;

    [HideInInspector]
    public bool changed;

    public TimeValue(int value) : this()
    {
        this.value  = value;
        changed     = false;
    }
}

[Serializable()]
public struct ClockTime
{
    //00-24
    public TimeValue hour;
    //00-60
    public TimeValue minute;
    //00-60
    public TimeValue second;

    public ClockTime(int hour, int minute, int second) : this()
    {
        SetTime(hour, minute, second);
    }

    public ClockTime(ClockTime other) : this()
    {
        SetTime(other.hour.value, other.minute.value, other.second.value);
    }

    /// <summary>
    /// Set all the references
    /// </summary>
    /// <param name="hour"></param>
    /// <param name="minute"></param>
    /// <param name="second"></param>
    private void SetTime(int hour, int minute, int second)
    {
        hour = Math.Min(hour, 24);
        this.hour   = new TimeValue(hour <= 12 ? hour : hour - 12);

        minute = Math.Min(minute, 60);
        this.minute = new TimeValue(minute);

        second = Math.Min(second, 60);
        this.second = new TimeValue(second);
    }
}

[RequireComponent(typeof(AudioSource))]
public abstract class Clock : MonoBehaviour
{
    [Header("Audio")]
    [SerializeField] protected bool           _playSound      = true;
    [SerializeField] protected AudioClip[]    _tickSounds     = new AudioClip[0];
    [SerializeField] protected bool           _randomizeTick  = true;

    [Header("Settings")]
    [SerializeField] protected bool           _useSystemTime  = true;
    [SerializeField] protected bool           _useUtcTime     = false;
    [SerializeField] protected bool           _startOnPlay    = true;
    [SerializeField] protected ClockTime      _startTime      = new ClockTime();
    [Space][Range(0.01f, 100f)]
    [SerializeField] protected float          _speed          = 1;

    private float Speed
    {
        get
        {
            return 1f / _speed;
        }
    }

    #region PRIVATE

    protected bool tick = true;

    protected ClockTime current = new ClockTime(0, 0, 0);

    protected bool reEnable = false;
    protected IEnumerator IE_Tick = null;

    protected AudioSource source = null;
    int tickIndex = -1;

    protected bool playSoundOnTick = true;

    #endregion

    protected virtual void Start()
    {
        source = GetComponent<AudioSource>();
        if (!source)
        {
            source = gameObject.AddComponent<AudioSource>();
        }

        if (!_useSystemTime)
        {
            current = new ClockTime(_startTime);
        }
        else
        {
            var dateTime = _useUtcTime ? DateTime.UtcNow : DateTime.Now;
            current = new ClockTime(dateTime.Hour, dateTime.Minute, dateTime.Second);
        }
    }

    protected virtual void OnEnable()
    {
        if (reEnable)
        {
            Begin();
            reEnable = false;
        }
    }

    protected virtual void OnDisable()
    {
        Stop();
        reEnable = true;
    }

    protected void PlaySound()
    {
        if (_tickSounds == null || _tickSounds.Length <= 0) { return; }

        if (_randomizeTick)
        {
            tickIndex = UnityEngine.Random.Range(0, _tickSounds.Length);
        }
        else
        {
            tickIndex++;
            if (tickIndex >= _tickSounds.Length)
            {
                tickIndex = 0;
            }
        }

        source.clip = _tickSounds[tickIndex];
        source.Play();
    }

    public void Begin()
    {
        Stop();

        IE_Tick = Tick();
        StartCoroutine(IE_Tick);
    }

    public void Stop()
    {
        if (IE_Tick != null)
        {
            StopCoroutine(IE_Tick);
            IE_Tick = null;
        }
    }

    protected IEnumerator Tick()
    {
        while (tick)
        {
            current.second.value++;
            current.second.changed = true;

            if (_playSound && playSoundOnTick)
            { PlaySound(); }

            if (current.second.value >= 60)
            {
                current.second.value = 0;

                current.minute.value++;
                current.minute.changed = true;

                if (_playSound && playSoundOnTick)
                { PlaySound(); }

                if (current.minute.value >= 60)
                {
                    current.minute.value = 0;

                    current.hour.value++;
                    current.hour.changed = true;
                }
            }
            yield return new WaitForSecondsRealtime(Speed / Time.timeScale);
        }
    }
}