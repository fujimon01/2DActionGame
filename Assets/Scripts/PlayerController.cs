using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField, Header("移動速度")] private float _moveSpeed;
    [SerializeField, Header("ジャンプ速度")] private float _jumpSpeed;
    [SerializeField, Header("体力")] private int _hp;
    [SerializeField, Header("無敵時間")] private float _damageTime;
    [SerializeField, Header("点滅時間")] private float _flashTime;
    private SpriteRenderer _spriteRenderer;
    private Rigidbody2D _playerRigidbody2D;
    private Animator _playerAnimator;
    private Vector2 _inputDirection;
    private int _maxJumpCount = 2;
    private int _jumpCount;
    private bool _bJump;
    // Start is called before the first frame update
    void Start()
    {
        _playerRigidbody2D = GetComponent<Rigidbody2D>();
        _playerAnimator = GetComponent<Animator>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _bJump = false;
        _jumpCount = 0;
    }

    // Update is called once per frame
    void Update()
    {
        _Move();
        _LookMoveDirection();
    }

    private void _Move()
    {
        _playerRigidbody2D.velocity = new Vector2(_inputDirection.x * _moveSpeed, _playerRigidbody2D.velocity.y);
        _playerAnimator.SetBool("Walk", _inputDirection.x != 0.0f);
    }

    private void _LookMoveDirection()
    {
        if (_inputDirection.x > 0.0f)
        {
            transform.eulerAngles = Vector3.zero;
        }
        else if (_inputDirection.x < 0.0f)
        {
            transform.eulerAngles = new Vector3(0.0f, 180.0f, 0.0f);
        }
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        _inputDirection = context.ReadValue<Vector2>();

    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (!context.performed || _jumpCount >= _maxJumpCount) return;
        _bJump = true;
        _jumpCount++;
        // _playerAnimator.SetBool("Jump", _jumpCount==1);
        // _playerAnimator.SetBool("DoubleJump", _jumpCount==2);
        if (_jumpCount == _maxJumpCount) _playerRigidbody2D.velocity = new Vector2(_playerRigidbody2D.velocity.x, 0.0f);
        _playerRigidbody2D.AddForce(Vector2.up * _jumpSpeed, ForceMode2D.Impulse);
    }

    private void _HitEnemy(GameObject enemy)
    {
        float halfScaleY = transform.lossyScale.y / 2.0f;
        float enemyHalfScaleY = enemy.transform.lossyScale.y / 2.0f;

        if (transform.position.y - (halfScaleY - 0.1f) >= enemy.transform.position.y + (enemyHalfScaleY - 0.1f))
        {
            Debug.Log("敵を踏んだ");
            Destroy(enemy);
            _jumpCount = 1;
            _playerRigidbody2D.velocity = new Vector2(_playerRigidbody2D.velocity.x, 0.0f);
            _playerRigidbody2D.AddForce(Vector2.up * _jumpSpeed, ForceMode2D.Impulse);
            _bJump = true;
        }
        else
        {
            enemy.GetComponent<EnemyController>().PlayerDamage(this);
            gameObject.layer = LayerMask.NameToLayer("PlayerDamage");
            StartCoroutine(_Damage());
        }
    }

    IEnumerator _Damage()
    {
        Color color = _spriteRenderer.color;
        for (int i = 0; i < _damageTime; i++)
        {
            yield return new WaitForSeconds(_flashTime);
            _spriteRenderer.color = new Color(color.r, color.g, color.b, 0.0f);
            yield return new WaitForSeconds(_flashTime);
            _spriteRenderer.color = new Color(color.r, color.g, color.b, 1.0f);
        }
        _spriteRenderer.color = color;
        gameObject.layer = LayerMask.NameToLayer("Default");
    }
    private void _Dead()
    {
        if (_hp <= 0)
        {
            Destroy(gameObject);
        }
    }
    private void OnBecameInvisible()
    {
        Camera camera = Camera.main;
        if (camera != null && camera.name == "Main Camera" && camera.transform.position.y > transform.position.y)
        {
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            Debug.Log("敵にあたった");
            _HitEnemy(collision.gameObject);
        }
        else if (collision.gameObject.tag == "Goal")
        {
            FindObjectOfType<MainManager>()._ShowGameClearUI();
            enabled = false;
            GetComponent<PlayerInput>().enabled = false;
        }
        else if (collision.gameObject.tag == "Floor")
        {
            _HitFloor();
        }
    }

    private void _HitFloor()
    {
        int layerMask = LayerMask.GetMask("Floor");
        Vector3 rayPos = transform.position - new Vector3(0.0f, _spriteRenderer.bounds.size.y / 2.0f);
        Vector3 raySize = new Vector3(_spriteRenderer.bounds.size.x - 0.1f, 0.2f);
        RaycastHit2D rayHit = Physics2D.BoxCast(rayPos, raySize, 0.0f, Vector2.zero, 0.0f, layerMask);
        if (rayHit.transform == null)
        {
            // _playerAnimator.SetBool("Jump", _jumpCount==1);
            // _playerAnimator.SetBool("DoubleJump", _jumpCount==2);
            // _playerAnimator.SetBool("Fall", _playerRigidbody2D.velocity.y < 0);
            // _bJump = true;
            // _playerAnimator.SetBool("Jump", _jumpCount==1);
            // _playerAnimator.SetBool("DoubleJump", _jumpCount==2);
            // // _playerAnimator.SetBool("Jump", _bJump);
            // _playerAnimator.SetBool("Fall", _playerRigidbody2D.velocity.y < 0);
            Debug.Log("床にあたっていない");
            return;
        }
        if (rayHit.transform.tag == "Floor"  /*&& _jumpCount > 0 _bJump*/)
        {
            _bJump = false;
            _jumpCount = 0;
            // _playerAnimator.SetBool("DoubleJump", false);
            // _playerAnimator.SetBool("Jump", false);
            // _playerAnimator.SetBool("Jump", _bJump);
            //_playerAnimator.SetBool("Fall", false);
            Debug.Log("床にあたった");
        }
    }

    public void Damage(int damage)
    {
        _hp = Math.Max(_hp - damage, 0);
        _Dead();
    }

    public int GetHp()
    {
        return _hp;
    }
}

