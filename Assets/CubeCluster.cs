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

    public float transition {
        get { return _transition; }
        set { _transition = value; }
    }

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
    Material _material;

    XXHash _hashDistance = new XXHash(0);
    XXHash _hashPosition1 = new XXHash(830);
    XXHash _hashPosition2 = new XXHash(803);
    XXHash _hashPosition3 = new XXHash(329);
    XXHash _hashTheta = new XXHash(2842);
    XXHash _hashPhi = new XXHash(1394);
    XXHash _hashVPhi = new XXHash(894);
    XXHash _hashSelect = new XXHash(4384);

    Mesh _mesh;
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

    Quaternion GetScatterRotation(int index)
    {
        var theta = _hashTheta.Range(-_thetaRange, _thetaRange, index + 10000);
        var phi = _hashPhi.Range(-180.0f, 180.0f, index + 10000);
        return Quaternion.Euler(theta, phi, theta + phi);
    }

    float GetScale(Vector3 position)
    {
        var noffs = Vector3.up * _noiseSpeed * Time.time;
        var s = Perlin.Noise(position * _noiseFrequency + noffs);
        return Mathf.Lerp(_minScale, _maxScale, (s + 1) / 2);
    }

    void Update()
    {
        if (_mesh == null)
            _mesh = BuildMesh();

        if (_materialOption == null)
            _materialOption  = new MaterialPropertyBlock();

        //_transition = 0.5f - Mathf.Cos(Mathf.Min(Time.time % 10, Mathf.PI * 2)) * 0.5f;

        for (var i = 0; i < _cubeCount; i++)
        {
            var distance = GetDistance(i);
            var rotation = GetRotation(i);
            var rotation2 = GetScatterRotation(i);
            var position = rotation * new Vector3(0, 0, distance);
            var position2 = GetScatterPosition(i);
            var scale = GetScale(position);

            position = Vector3.Lerp(position, position2, _transition);
            rotation = Quaternion.Slerp(rotation, rotation2, _transition);

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

    Mesh BuildMesh()
    {
        Vector3[] vertices =
        {
            new Vector3(+0.5f, +0.5f, +0.5f),
            new Vector3(-0.5f, +0.5f, +0.5f),
            new Vector3(+0.5f, -0.5f, +0.5f),
            new Vector3(-0.5f, -0.5f, +0.5f),

            new Vector3(-0.5f, +0.5f, -0.5f),
            new Vector3(+0.5f, +0.5f, -0.5f),
            new Vector3(-0.5f, -0.5f, -0.5f),
            new Vector3(+0.5f, -0.5f, -0.5f),

            new Vector3(+0.5f, +0.5f, +0.5f),
            new Vector3(+0.5f, +0.5f, -0.5f),
            new Vector3(-0.5f, +0.5f, +0.5f),
            new Vector3(-0.5f, +0.5f, -0.5f),

            new Vector3(+0.5f, -0.5f, -0.5f),
            new Vector3(+0.5f, -0.5f, +0.5f),
            new Vector3(-0.5f, -0.5f, -0.5f),
            new Vector3(-0.5f, -0.5f, +0.5f),

            new Vector3(+0.5f, +0.5f, +0.5f),
            new Vector3(+0.5f, -0.5f, +0.5f),
            new Vector3(+0.5f, +0.5f, -0.5f),
            new Vector3(+0.5f, -0.5f, -0.5f),

            new Vector3(-0.5f, -0.5f, +0.5f),
            new Vector3(-0.5f, +0.5f, +0.5f),
            new Vector3(-0.5f, -0.5f, -0.5f),
            new Vector3(-0.5f, +0.5f, -0.5f)
        };

        Vector2[] uvs =
        {
            new Vector2(0, 1),
            new Vector2(1, 1),
            new Vector2(0, 0),
            new Vector2(1, 0),

            new Vector2(-1, -1),
            new Vector2(-1, -1),
            new Vector2(-1, -1),
            new Vector2(-1, -1),

            new Vector2(-1, -1),
            new Vector2(-1, -1),
            new Vector2(-1, -1),
            new Vector2(-1, -1),

            new Vector2(-1, -1),
            new Vector2(-1, -1),
            new Vector2(-1, -1),
            new Vector2(-1, -1),

            new Vector2(-1, -1),
            new Vector2(-1, -1),
            new Vector2(-1, -1),
            new Vector2(-1, -1),

            new Vector2(-1, -1),
            new Vector2(-1, -1),
            new Vector2(-1, -1),
            new Vector2(-1, -1)
        };

        int[] indices = new int[vertices.Length / 4 * 6];

        var vi = 0;
        for (var ii = 0; ii < indices.Length;)
        {
            indices[ii++] = vi;
            indices[ii++] = vi + 1;
            indices[ii++] = vi + 2;
            indices[ii++] = vi + 1;
            indices[ii++] = vi + 3;
            indices[ii++] = vi + 2;
            vi += 4;
        }

        var mesh = new Mesh();
        mesh.hideFlags = HideFlags.DontSave;
        mesh.vertices = vertices;
        mesh.uv = uvs;
        mesh.SetIndices(indices, MeshTopology.Triangles, 0);
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        return mesh;
    }
}
