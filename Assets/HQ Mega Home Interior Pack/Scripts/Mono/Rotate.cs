using UnityEngine;

public class Rotate : MonoBehaviour
{
    [SerializeField] Transform  target         = null;

    [Header("Parameters")]
    [SerializeField] bool       startOnPlay    = false;
    [SerializeField] float      speed          = 2.0f;
    [SerializeField] Vector3    euler          = Vector3.zero;

    private bool rotate = false;

    private void Start()
    {
        rotate = startOnPlay;
    }

    private void Update()
    {
        if (!rotate) { return; }
        
        target.Rotate(euler * (Time.deltaTime * (speed * 100)));
    }

    public void Set(bool state)
    {
        rotate = state;
    }
}