using System;
using System.Text;
using TMPro;
using UnityEngine;

public class DigitalClock : Clock
{
    public enum DigitalClockFormat { H_M = 1, H_M_S = 2, HH_MM = 3, HH_MM_SS = 4 }

    //EXPOSED

    [Header("References")]
    [SerializeField] TextMeshProUGUI _timeText    = null;
    [SerializeField] TextMeshProUGUI _overlayText = null;

    [Header("Digital Settings")]
    [SerializeField] DigitalClockFormat _format = DigitalClockFormat.HH_MM;
    [Space]
    [SerializeField] char _separator = ':';
    [SerializeField] bool _blinkSeparator = true;
    [SerializeField] float _blinkRate = 1;
    [Tooltip("Text word spacing value when the separator is removed. Depending on the separator character, this value might need to be tweaked manually to get the right result.")]
    [SerializeField] float _wordSpaceAdjust = -31.5f;
    [Tooltip("Should the tick sound be played when the separator blinks?")]
    [SerializeField] bool _playSoundWhenBlink = true;
    [Space]
    [SerializeField] bool _use24hClock = true;
    [Tooltip("Adds 'PM' or 'AM'. Applicable only for 12h format.")]
    [SerializeField] bool _useEnding = false;
    
    //PRIVATE

    private bool _showSeparator = true;

    protected override void Start()
    {
        base.Start();
        SetTimeText(GetFormattedTime());

        playSoundOnTick = !_playSoundWhenBlink;
        if (_startOnPlay)
        {
            Begin();
            InvokeRepeating(nameof(CheckTime), 0, 1);
            if (_blinkSeparator)
            {
                InvokeRepeating(nameof(BlinkSeparator), 0, _blinkRate);
            }
        }
    }

    void CheckTime()
    {
        if ((int)_format % 2 == 0)
        {
            if (current.second.changed)
            {
                SetTimeText(GetFormattedTime());
            }
        }
        if (current.minute.changed)
        {
            SetTimeText(GetFormattedTime());
        }
        if (current.hour.changed)
        {
            SetTimeText(GetFormattedTime());
        }
    }
    void BlinkSeparator()
    {
        _showSeparator = !_showSeparator;

        if (_playSound && _playSoundWhenBlink)
        {
            PlaySound();
        }

        Char c = _showSeparator ? ' ' : _separator;
        Char d = _showSeparator ? _separator : ' ';

        SetTimeText(_timeText.text.Replace(c, d));
    }

    public String GetFormattedTime()
    {
        var hour = _use24hClock ? current.hour.value : (current.hour.value > 12 ? current.hour.value - 12 : current.hour.value);

        //Take four steps backwards.. and do couple of actions that would take you forward.

        StringBuilder timeBuilder = new StringBuilder();
        if (hour < 10 && (int)_format > 2)
        {
            timeBuilder.Append(0);
        }
        timeBuilder.Append(hour.ToString());

        timeBuilder.Append(_showSeparator ? _separator : ' ');
        if (current.minute.value < 10 && (int)_format > 2)
        {
            timeBuilder.Append(0);
        }
        timeBuilder.Append(current.minute.value);

        if ((int)_format % 2 == 0)
        {
            timeBuilder.Append(_separator);
            if (current.second.value < 10 && (int)_format > 2)
            {
                timeBuilder.Append(0);
            }
            timeBuilder.Append(current.second.value);
        }

        if (!_use24hClock && _useEnding)
        {
            timeBuilder.Append(" ");
            timeBuilder.Append(current.hour.value < 12 ? "PM" : "AM");
        }

        return timeBuilder.ToString();
    }

    void SetTimeText(string text)
    {
        _timeText.text = text;
        _timeText.wordSpacing = _showSeparator ? 0 : _wordSpaceAdjust;
        if (_overlayText)
        {
            _overlayText.text = text;
            _overlayText.wordSpacing = _showSeparator ? 0 : _wordSpaceAdjust;
        }
    }
}