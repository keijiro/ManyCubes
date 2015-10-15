using UnityEngine;
using System.Collections;

public class SceneController : MonoBehaviour
{
    [SerializeField]
    Transform[] _cameraTargets;

    [SerializeField]
    float _cameraChangeInterval = 15.0f;

    [SerializeField]
    float _transition;

    public float transition {
        get { return _transition; }
        set { _transition = value; }
    }

    [SerializeField]
    SmoothFollow _cameraRoot;

    [SerializeField]
    CubeCluster _cubeCluster;

    [SerializeField]
    RingArray _ringArray;

    [SerializeField]
    Kvant.Spray _spray;

    IEnumerator Start()
    {
        while (true)
        {
            foreach (var t in _cameraTargets)
            {
                _cameraRoot.target = t;
                yield return new WaitForSeconds(_cameraChangeInterval);
            }
        }
    }

    void Update()
    {
        _cubeCluster.transition = _transition;
        _ringArray.scale = 1.0f - _transition;
        _spray.throttle = 1.0f - _transition;
    }
}
