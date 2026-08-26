using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;

public class CheckpointFlag : MonoBehaviour
{
    private Animator _flagAnimator;
    // Start is called before the first frame update
    void Start()
    {
        _flagAnimator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("何かに触れた");
        if(collision.gameObject.tag == "Player")
        {
            _flagAnimator.SetTrigger("IsFlagTouched");
            Debug.Log("チェックポイントに触れた");
            _flagAnimator.SetBool("HasAppeared",true);
            Debug.Log("旗が現れた");
        }
    }
}
