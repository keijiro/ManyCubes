using UnityEngine;
using System.Collections;
using Reaktion;

[ExecuteInEditMode]
public class CubeCluster : MonoBehaviour
{
    [SerializeField]
    int _cubeCount = 100;

    [SerializeField, Range(0, 1)]
    float _transition = 0.0f;

    [Space(10)]
    [SerializeField]
    float _thetaRange = 75;

    [SerializeField]
    float _minAngularSpeed = 8;

    [SerializeField]
    float _maxAngularSpeed = 90;

    [Space(10)]
    [SerializeField]
    float _minDistance = 0.1f;

    [SerializeField]
    float _maxDistance = 10;

    [Space(10)]
    [SerializeField]
    float _scatterRange = 20;

    [Space(10)]
    [SerializeField]
    float _noiseFrequency = 1;

    [SerializeField]
    float _noiseSpeed = 1;

    [Space(10)]
    [SerializeField]
    float _minScale = 0.2f;

    [SerializeField]
    float _maxScale = 1;

    [Space(10)]
    [SerializeField]
    Mesh _mesh;

    [SerializeField]
    Material _material;

    XXHash _hashDistance = new XXHash(0);
    XXHash _hashPosition1 = new XXHash(830);
    XXHash _hashPosition2 = new XXHash(803);
    XXHash _hashPosition3 = new XXHash(329);
    XXHash _hashTheta = new XXHash(2842);
    XXHash _hashPhi = new XXHash(1394);
    XXHash _hashVPhi = new XXHash(894);
    XXHash _hashSelect = new XXHash(4384);

    MaterialPropertyBlock _materialOption;

    float GetDistance(int index)
    {
        return _hashDistance.Range(_minDistance, _maxDistance, index);
    }

    Vector3 GetScatterPosition(int index)
    {
        return new Vector3(
            _hashPosition1.Range(-_scatterRange, _scatterRange, index),
            _hashPosition2.Range(-_scatterRange, _scatterRange, index),
            _hashPosition3.Range(-_scatterRange, _scatterRange, index)
        );
    }

    Quaternion GetRotation(int index)
    {
        var theta = _hashTheta.Range(-_thetaRange, _thetaRange, index);
        var phi = _hashPhi.Range(-180.0f, 180.0f, index);
        var vphi = _hashVPhi.Range(_minAngularSpeed, _maxAngularSpeed, index);
        return Quaternion.Euler(theta, phi + vphi * Time.time, 0);
    }

    float GetScale(Vector3 position)
    {
        var noffs = Vector3.up * _noiseSpeed * Time.time;
        var s = Perlin.Noise(position * _noiseFrequency + noffs);
        return Mathf.Lerp(_minScale, _maxScale, (s + 1) / 2);
    }

    void Update()
    {
        if (_materialOption == null)
            _materialOption  = new MaterialPropertyBlock();

        for (var i = 0; i < _cubeCount; i++)
        {
            var distance = GetDistance(i);
            var rotation = GetRotation(i);
            var position = rotation * new Vector3(0, 0, distance);
            var position2 = GetScatterPosition(i);
            var scale = GetScale(position);

            position = Vector3.Lerp(position, position2, _transition);

            var matrix = Matrix4x4.TRS(position, rotation, Vector3.one * scale);

            var select = _hashSelect.Range(0, 16, i);
            _materialOption.SetVector("_select", new Vector2(
                (select / 4) * 0.25f,
                (select % 4) * 0.25f
            ));

            Graphics.DrawMesh(
                _mesh, matrix, _material,
                0, null, 0, _materialOption);
        }
    }
}
