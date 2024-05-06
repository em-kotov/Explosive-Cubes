using UnityEngine;
using UnityEngine.Events;

public class Spawner : MonoBehaviour
{
    [SerializeField] private GameObject _prefab;
    [SerializeField] private int _numberOfCubes;
    [SerializeField] private float _startRadius;
    [SerializeField] private float _explosionForce;
    [SerializeField] private float _explosionRadius;

    private float _scaleMultiplier = 0.5f;
    private float _startThreshold = 100f;
    private float _spawnThresholdDivider = 2f;
    private int _minNumberOfScaledCubes = 2;
    private int _maxNumberOfScaledCubes = 6;

    private event UnityAction<GameObject> SpawnNewCubes;

    private void Start()
    {
        SpawnNewCubes = SpawnScaledCubes;
        AddStartCubes();
    }

    private void Subscribe(GameObject cube)
    {
        cube.GetComponent<CubeController>().Clicked += SpawnNewCubes;
    }

    private void SetThreshold(GameObject cube, float threshold)
    {
        cube.GetComponent<CubeController>().SetThreshhold(threshold);
    }

    private float GetNewThreshold(GameObject cube)
    {
        return cube.GetComponent<CubeController>().SpawnThreshold / _spawnThresholdDivider;
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

    private void Explode(GameObject cube, Vector3 position)
    {
        Rigidbody cubeRigidbody = cube.GetComponent<Rigidbody>();
        cubeRigidbody.AddExplosionForce(_explosionForce, position, _explosionRadius);
    }

    private void SpawnScaledCubes(GameObject clickedCube)
    {
        var cubeController = clickedCube.GetComponent<CubeController>();

        if (Random.Range(0f, _startThreshold) <= cubeController.SpawnThreshold)
            AddScaledCubes(clickedCube);

        Destroy(clickedCube);
    }

    private void AddScaledCubes(GameObject clickedCube)
    {
        int numberOfScaledCubes = Random.Range(_minNumberOfScaledCubes, _maxNumberOfScaledCubes);
        float newThreshold = GetNewThreshold(clickedCube);

        for (int i = 0; i < numberOfScaledCubes; i++)
        {
            GameObject newCube = Instantiate(_prefab, clickedCube.transform.position, GetRandomRotation());
            newCube.transform.localScale = clickedCube.transform.localScale * _scaleMultiplier;

            newCube.AddComponent<CubeController>();

            Explode(newCube, clickedCube.transform.position);
            SetThreshold(newCube, newThreshold);
            Subscribe(newCube);
        }
    }

    private void AddStartCubes()
    {
        for (int i = 0; i < _numberOfCubes; i++)
        {
            GameObject cube = Instantiate(_prefab, GetRandomPositionXZ(), GetRandomRotation());

            SetThreshold(cube, _startThreshold);
            Subscribe(cube);
        }
    }
}
