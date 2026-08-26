using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField, Header("移動速度")]private float _moveSpeed;
    [SerializeField, Header("ジャンプ速度")]private float _jumpSpeed;
    [SerializeField, Header("攻撃力")]private int _attackPower;
    private Rigidbody2D _enemyRigidbody2D;
    private SpriteRenderer _spriteRenderer;
    private Animator _enemyAnimator;
    private Vector2 _moveDirection;
    private bool _bFloor;
    // Start is called before the first frame update
    void Start()
    {
        _enemyRigidbody2D = GetComponent<Rigidbody2D>();
        _enemyAnimator = GetComponent<Animator>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _moveDirection = Vector2.left;
        _bFloor = true;
    }

    // Update is called once per frame
    void Update()
    {
        _Move();
        _ChangeMoveDirection();
        _LookMoveDirection();
        _HitFloor();
    }

    private void _Move()
    {
        if(!_bFloor)return;
        _enemyRigidbody2D.velocity = new Vector2(_moveDirection.x * _moveSpeed, _enemyRigidbody2D.velocity.y);
    }
    private void _ChangeMoveDirection()
    {
        Vector2 halfSize = transform.lossyScale / 2.0f;
        int layerMask = LayerMask.GetMask("Floor");
        RaycastHit2D ray = Physics2D.Raycast(transform.position, -transform.right, halfSize.x + 0.1f, layerMask);
        if(ray.transform == null)return;
        if(ray.transform.tag == "Floor")
        {
            _moveDirection = -_moveDirection;
        }
    }
    private void _LookMoveDirection()
    {
        if(_moveDirection.x < 0.0f)
        {
            transform.eulerAngles = Vector3.zero;
        }
        else if(_moveDirection.x > 0.0f)
        {
            transform.eulerAngles = new Vector3(0.0f, 180.0f, 0.0f);
        }
    }
    private void _HitFloor()
    {
        int layerMask = LayerMask.GetMask("Floor");
        Vector3 rayPos = transform.position - new Vector3(0.0f, _spriteRenderer.bounds.size.y / 2.0f);
        Vector3 raySize = new Vector3(_spriteRenderer.bounds.size.x - 0.1f, 0.1f);
        RaycastHit2D rayHit = Physics2D.BoxCast(rayPos, raySize, 0.0f, Vector2.zero, 0.0f, layerMask);
        // Debug.Log("エネミー横サイズ："+_spriteRenderer.bounds.size.x);
        // Debug.Log("エネミー縦サイズ："+_spriteRenderer.bounds.size.y);
        // Debug.Log("プレイヤーの位置："+"("+transform.position.x+","+transform.position.y+")");
        // Debug.Log("レイを飛ばす位置："+"("+rayPos.x+","+rayPos.y+")");
        // Debug.Log("レイのサイズ："+"("+raySize.x+","+raySize.y+")");
        if(rayHit.transform == null)
        {
            _bFloor = false;
            _enemyAnimator.SetBool("IsIdle", true);
            Debug.Log("空中にいます");
            return;
        }
        if(rayHit.transform.tag == "Floor" && !_bFloor)
        {
            _bFloor = true;
            _enemyAnimator.SetBool("IsIdle", false);
        }
        
    }

    public void PlayerDamage(PlayerController player)
    {
        player.Damage(_attackPower);
    }
}
