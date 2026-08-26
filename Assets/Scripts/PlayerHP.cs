using System.Collections;
using System.Collections.Generic;
using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHP : MonoBehaviour
{
    [SerializeField]private GameObject _HPIcon;
    private PlayerController _playerController;
    private int _beforeHP;
    // Start is called before the first frame update
    void Start()
    {
        _playerController = FindObjectOfType<PlayerController>();
        _beforeHP = _playerController.GetHp();
        _CreateHPIcon();
    }

    // Update is called once per frame
    void Update()
    {
        _ShowHPIcon();
    }

    private void _CreateHPIcon()
    {
        for(int i = 0; i < _playerController.GetHp(); i++)
        {
            GameObject _HPObj = Instantiate(_HPIcon);
            _HPObj.transform.SetParent(transform, false);
        }
    }

    private void _ShowHPIcon()
    {
        if(_beforeHP == _playerController.GetHp())return;

        UnityEngine.UI.Image[] icon = transform.GetComponentsInChildren<UnityEngine.UI.Image>();
        for(int i = 0; i < icon.Length; i++)
        {
            icon[i].gameObject.SetActive(i < _playerController.GetHp());
        }
        _beforeHP = _playerController.GetHp();
    }
}
