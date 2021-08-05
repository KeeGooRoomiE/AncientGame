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
        if (collision.collider.tag == "arrow")
        {
            _animator.Play("benc");
            collision.collider.gameObject.GetComponent<MainLauncherController>().AbleChangeAlpha(true);
        }
        //if (collision.collider.tag == "axe")
        //{
        //    _animator.Play("benc");
        //    collision.collider.gameObject.GetComponent<MainLauncherController>().AbleChangeAlpha(true);
        //}

    }

    public void MoveFrog(float time, float spd)
    {

        //yield return new WaitForSeconds(1.6f);
        //_rigid.AddForce(new Vector3(0, spd, 0), ForceMode.Impulse);
        StartCoroutine(Await(time, spd));

    }

    IEnumerator Await(float time, float spd)
    {
        yield return new WaitForSeconds(time);
        _rigid.AddForce(new Vector3(0, spd, 0), ForceMode.Impulse);
    }

    public void SetAnimation(string name)
    {
        _animator.Play(name);
    }
}
