using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] private ClickHandler _prefab;
    [SerializeField] private int _numberOfCubes;
    [SerializeField] private float _startRadius;
    [SerializeField] private float _explosionForce;
    [SerializeField] private float _explosionRadius;

    private float _scaleMultiplier = 0.5f;
    private float _startThreshold = 100f;
    private float _spawnThresholdDivider = 2f;
    private int _minNumberOfScaledCubes = 2;
    private int _maxNumberOfScaledCubes = 6;

    private void Start()
    {
        AddStartCubes();
    }

    private Quaternion GetRandomRotation()
    {
        return Quaternion.Euler(Random.Range(0f, 360f), Random.Range(0f, 360f), Random.Range(0f, 360f));
    }

    private Vector3 GetRandomPositionXZ()
    {
        Vector3 randomPositionXZ = Random.insideUnitCircle * _startRadius;
        return new Vector3(randomPositionXZ.x, transform.position.y, randomPositionXZ.z);
    }

    private void Explode(ClickHandler cube, Vector3 position)
    {
        Rigidbody cubeRigidbody = cube.GetComponent<Rigidbody>();
        cubeRigidbody.AddExplosionForce(_explosionForce, position, _explosionRadius);
    }

    private void DestroyCube(ClickHandler clickedCube)
    {
        if (Random.Range(0f, _startThreshold) <= clickedCube.SpawnThreshold)
            AddScaledCubes(clickedCube);

        Destroy(clickedCube.gameObject);
    }

    private void AddScaledCubes(ClickHandler clickedCube)
    {
        int numberOfScaledCubes = Random.Range(_minNumberOfScaledCubes, _maxNumberOfScaledCubes);
        float newThreshold = clickedCube.SpawnThreshold / _spawnThresholdDivider;

        for (int i = 0; i < numberOfScaledCubes; i++)
        {
            ClickHandler newCube = Instantiate(_prefab, clickedCube.transform.position, GetRandomRotation());
            newCube.transform.localScale = clickedCube.transform.localScale * _scaleMultiplier;

            Explode(newCube, clickedCube.transform.position);
            newCube.SetThreshhold(newThreshold);
            newCube.Clicked += DestroyCube;
        }
    }

    private void AddStartCubes()
    {
        for (int i = 0; i < _numberOfCubes; i++)
        {
            ClickHandler cube = Instantiate(_prefab, GetRandomPositionXZ(), GetRandomRotation());

            cube.SetThreshhold(_startThreshold);
            cube.Clicked += DestroyCube;
        }
    }
}
