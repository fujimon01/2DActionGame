using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField, Header("振動時間")]private float _shakeTime;
    [SerializeField, Header("振動の大きさ")]private float _shakeMagnitude;
    private Vector3 iniPos;
    private PlayerController _playerController;
    private float _shakeCount;
    private int _currentPlayerHP;
    // Start is called before the first frame update
    void Start()
    {
        _playerController = FindObjectOfType<PlayerController>();
        _currentPlayerHP = _playerController.GetHp();
        iniPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        _ShakeCheck();
        _FollowPlayer();
    }
    private void _ShakeCheck()
    {
        if(_currentPlayerHP > _playerController.GetHp())
        {
            Debug.Log("ダメージを受けたので振動開始");
            _currentPlayerHP = _playerController.GetHp();
            _shakeCount = 0.0f;
            StartCoroutine(_Shake());
        }
    }

    IEnumerator _Shake()
    {
        Vector3 iniPos = transform.position;
        while(_shakeCount < _shakeTime)
        {
            float x = iniPos.x + Random.Range(-_shakeMagnitude, _shakeMagnitude);
            float y = iniPos.y + Random.Range(-_shakeMagnitude, _shakeMagnitude);
            transform.position = new Vector3(x, y, iniPos.z);
            _shakeCount += Time.deltaTime;
            yield return null;
        }
        transform.position = iniPos;
    }
    private void _FollowPlayer()
    {
        if(_playerController == null)return;
        float x = _playerController.transform.position.x;
        x = Mathf.Clamp(x, iniPos.x, Mathf.Infinity);
        transform.position = new Vector3(x, transform.position.y, transform.position.z);
    }
}
