using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrogAnimationQueuer : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    [SerializeField] private Rigidbody _rigid;

    private void Start()
    {
        _animator = GetComponent<Animator>();
        _rigid = GetComponent<Rigidbody>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        _animator.Play("benc");
    }

    public void MoveFrog(int spd)
    {
       
        _rigid.AddForce(new Vector3(0, spd, 0), ForceMode.Impulse);

    }

    public void SetAnimation(string name)
    {
        _animator.Play(name);
    }
}
