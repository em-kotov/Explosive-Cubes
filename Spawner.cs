using UnityEngine;
using UnityEngine.Events;

public class Spawner : MonoBehaviour
{
    [SerializeField] private CubeClickInvoker _prefab;
    [SerializeField] private int _numberOfCubes;
    [SerializeField] private float _startRadius;
    [SerializeField] private float _explosionForce;
    [SerializeField] private float _explosionRadius;

    private float _scaleMultiplier = 0.5f;
    private float _startThreshold = 100f;
    private float _spawnThresholdDivider = 2f;
    private int _minNumberOfScaledCubes = 2;
    private int _maxNumberOfScaledCubes = 6;

    private event UnityAction<CubeClickInvoker> CubeClicked;

    private void Start()
    {
        CubeClicked = DestroyCube;
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

    private void Explode(CubeClickInvoker cube, Vector3 position)
    {
        Rigidbody cubeRigidbody = cube.GetComponent<Rigidbody>();
        cubeRigidbody.AddExplosionForce(_explosionForce, position, _explosionRadius);
    }

    private void DestroyCube(CubeClickInvoker clickedCube)
    {
        if (Random.Range(0f, _startThreshold) <= clickedCube.SpawnThreshold)
            AddScaledCubes(clickedCube);

        Destroy(clickedCube.gameObject);
    }

    private void AddScaledCubes(CubeClickInvoker clickedCube)
    {
        int numberOfScaledCubes = Random.Range(_minNumberOfScaledCubes, _maxNumberOfScaledCubes);
        float newThreshold = clickedCube.SpawnThreshold / _spawnThresholdDivider;

        for (int i = 0; i < numberOfScaledCubes; i++)
        {
            CubeClickInvoker newCube = Instantiate(_prefab, clickedCube.transform.position, GetRandomRotation());
            newCube.transform.localScale = clickedCube.transform.localScale * _scaleMultiplier;

            Explode(newCube, clickedCube.transform.position);
            newCube.SetThreshhold(newThreshold);
            newCube.Clicked += CubeClicked;
        }
    }

    private void AddStartCubes()
    {
        for (int i = 0; i < _numberOfCubes; i++)
        {
            CubeClickInvoker cube = Instantiate(_prefab, GetRandomPositionXZ(), GetRandomRotation());

            cube.SetThreshhold(_startThreshold);
            cube.Clicked += CubeClicked;
        }
    }
}
