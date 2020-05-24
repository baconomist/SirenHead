
using System;
using UnityEngine;
using UnityEngine.UI;

public class InfoText : MonoBehaviour
{
    private const float FadeDuration = 1;

    public string msgText = "Set the 'msgText' attribute of InfoText";
    public float duration = 5;
    
    private Text _text;
    private float _timer;
    private int _state = 0;

    private void Start()
    {
        _text = GetComponent<Text>();
        _text.color = new Color(_text.color.r, _text.color.b, _text.color.g, 0);
        _text.text = msgText;
    }

    private void Update()
    {
        if (_state == 0 && _timer < FadeDuration)
        {
            _text.color = new Color(_text.color.r, _text.color.b, _text.color.g, _timer / FadeDuration);
        }
        else if (_state == 0)
        {
            _state = 1;
            _timer = 0;
        }

        if (_state == 1 && _timer > duration)
        {
            _state = 2;
            _timer = 0;
        }

        if (_state == 2 && _timer < 1)
        {
            _text.color = new Color(_text.color.r, _text.color.b, _text.color.g, (FadeDuration - _timer) / FadeDuration);
        }
        else if (_state == 2 && _timer > 1)
        {
            Destroy(gameObject);
        }

        _timer += Time.deltaTime;
    }
}
