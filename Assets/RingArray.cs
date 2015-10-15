using UnityEngine;
using System.Collections;

public class RingArray : MonoBehaviour
{
    [SerializeField]
    Mesh _mesh;

    [SerializeField]
    Material _material;

    [SerializeField]
    float _scale = 1.0f;

    public float scale {
        get { return _scale; }
        set { _scale = value; }
    }

    XXHash _hashPosition = new XXHash(0);
    XXHash _hashRotation = new XXHash(3340);
    XXHash _hashScale = new XXHash(840);

    void Update()
    {
        for (var i = 0 ; i < 30; i++)
        {
            var pos = new Vector3(0, _hashPosition.Range(-5.0f, 5.0f, i), 0);
            var rot = Quaternion.AngleAxis(_hashRotation.Range(300.0f, 500.0f, i) * Time.time, Vector3.up);
            var scale = _hashScale.Range(0.3f, 5.0f, i) * _scale;

            var matrix = Matrix4x4.TRS(pos, rot, Vector3.one * scale);

            Graphics.DrawMesh(
                _mesh, matrix, _material,
                0, null, 0, null);
        }
    }
}
