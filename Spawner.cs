using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] private ClickHandler _prefab;
    [SerializeField] private ParticleSystem _effect;
    [SerializeField] private int _numberOfCubes;
    [SerializeField] private float _startRadius;
    [SerializeField] private float _explosionForce;
    [SerializeField] private float _explosionRadius;
    [SerializeField] private float _newCubesExplosionForce;
    [SerializeField] private float _newCubesExplosionRadius;

    private float _scaleMultiplier = 0.5f;
    private float _startThreshold = 100f;
    private float _spawnThresholdDivider = 0.5f;
    private int _minNumberOfScaledCubes = 2;
    private int _maxNumberOfScaledCubes = 6;

    private void Start()
    {
        AddStartCubes();
    }

    private Vector3 GetRandomPositionXZ()
    {
        Vector3 randomPositionXZ = Random.insideUnitCircle * _startRadius;
        return new Vector3(randomPositionXZ.x, transform.position.y, randomPositionXZ.z);
    }

    private List<Rigidbody> GetExplodableObjects(ClickHandler cube)
    {
        Collider[] hits = Physics.OverlapSphere(cube.transform.position, _explosionRadius);

        List<Rigidbody> cubes = new();

        foreach (Collider hit in hits)
            if (hit.attachedRigidbody != null)
                cubes.Add(hit.attachedRigidbody);

        return cubes;
    }

    private void AddForce(ClickHandler cube, Vector3 position)
    {
        Rigidbody cubeRigidbody = cube.GetComponent<Rigidbody>();
        cubeRigidbody.AddExplosionForce(_newCubesExplosionForce, position, _newCubesExplosionRadius);
    }

    private void DestroyCube(ClickHandler clickedCube)
    {
        if (Random.Range(0f, _startThreshold) <= clickedCube.SpawnThreshold)
            AddScaledCubes(clickedCube);
        else
            Explode(clickedCube);

        clickedCube.Clicked -= DestroyCube;
        Destroy(clickedCube.gameObject);
    }

    private void Explode(ClickHandler cube)
    {
        float multiplier = 50f;
        float currentForce = _explosionForce * multiplier / cube.SpawnThreshold;
        float currentRadius = _explosionRadius * multiplier / cube.SpawnThreshold;

        Instantiate(_effect, cube.transform.position, Quaternion.identity);

        foreach (Rigidbody explodableObject in GetExplodableObjects(cube))
            explodableObject.AddExplosionForce(currentForce, cube.transform.position, currentRadius);
    }

    private void AddScaledCubes(ClickHandler clickedCube)
    {
        int numberOfScaledCubes = Random.Range(_minNumberOfScaledCubes, _maxNumberOfScaledCubes);
        float newThreshold = clickedCube.SpawnThreshold * _spawnThresholdDivider;

        for (int i = 0; i < numberOfScaledCubes; i++)
        {
            ClickHandler newCube = Instantiate(_prefab, clickedCube.transform.position, Random.rotation);
            newCube.transform.localScale = clickedCube.transform.localScale * _scaleMultiplier;

            AddForce(newCube, clickedCube.transform.position);
            newCube.SetThreshhold(newThreshold);
            newCube.Clicked += DestroyCube;
        }
    }

    private void AddStartCubes()
    {
        for (int i = 0; i < _numberOfCubes; i++)
        {
            ClickHandler cube = Instantiate(_prefab, GetRandomPositionXZ(), Random.rotation);

            cube.SetThreshhold(_startThreshold);
            cube.Clicked += DestroyCube;
        }
    }
}
