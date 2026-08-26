using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakableBlock : MonoBehaviour
{
    private Animator _boxAnimator;
    private bool isBreking = false;

    // Start is called before the first frame update
    void Start()
    {
        _boxAnimator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Player" && !isBreking)
        {
            Vector2 contactNormal = collision.GetContact(0).normal;
            if (contactNormal.y > 0.5f)
            {
                isBreking = true;
                _boxAnimator.SetTrigger("IsBoxTouched");
                Debug.Log("ブロックに触れた");            
            }
        }
    }

    public void OnBreakAnimationEnd()
    {
        Destroy(gameObject);
    }
}
