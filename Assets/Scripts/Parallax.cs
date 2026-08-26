using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Parallax : MonoBehaviour
{
    [SerializeField, Header("視差効果"), Range(0, 1)] private float _parallaxEffectMultiplier;
    private GameObject _camera;
    private float _length;
    private float _startPosX;
    // Start is called before the first frame update
    void Start()
    {
        _startPosX = transform.position.x;
        _length = GetComponent<SpriteRenderer>().bounds.size.x;
        //_length = GetComponentInChildren<TilemapRenderer>().bounds.size.x;
        _camera = Camera.main.gameObject;
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void FixedUpdate()
    {
        _Parallax();
    }

    private void _Parallax()
    {
        float temp = _camera.transform.position.x * (1 - _parallaxEffectMultiplier);
        float dist = _camera.transform.position.x * _parallaxEffectMultiplier;

        transform.position = new Vector3(_startPosX + dist, transform.position.y, transform.position.z);

        if (temp > _startPosX + _length)
        {
            _startPosX += _length;
        }
        else if (temp < _startPosX - _length)
        {
            _startPosX -= _length;
        }
    }
}
