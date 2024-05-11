using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Renderer))]
[RequireComponent(typeof(Rigidbody))]

public class ClickHandler : MonoBehaviour
{
    private float _spawnThreshold;

    public event UnityAction<ClickHandler> Clicked;

    public float SpawnThreshold => _spawnThreshold;
    
    private void Start()
    {
        GetComponent<Renderer>().material.color = Random.ColorHSV(0f, 1f, 0.5f, 0.5f, 0.85f, 0.85f);
    }

    private void OnMouseUpAsButton()
    {
        Clicked?.Invoke(this);
    }

    public void SetThreshhold(float threshold)
    {
        float max = 100f;

        if (threshold <= max && threshold > 0)
            _spawnThreshold = threshold;
    }
}
