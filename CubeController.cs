using UnityEngine;
using UnityEngine.Events;

public class CubeController : MonoBehaviour
{
    public event UnityAction<GameObject> Clicked;

    private float _spawnThreshold;

    public float SpawnThreshold => _spawnThreshold;

    public void SetThreshhold(float threshold)
    {
        if (threshold <= 100f && threshold > 0f)
            _spawnThreshold = threshold;
    }

    private void Start()
    {
        GetComponent<Renderer>().material.color = Random.ColorHSV(0f, 1f, 0.5f, 0.5f, 0.85f, 0.85f);
    }

    private void OnMouseUpAsButton()
    {
        Clicked?.Invoke(gameObject);
    }
}
