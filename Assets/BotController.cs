using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotController : MonoBehaviour
{
    [Header("Shot force")]
    public bool isFixedForce;
    public float shotForce;
    public float minShotForce;
    public float maxShotForce;

    [Header("Wait time before shot")]
    public bool isFixedTimeBeforeShot;
    public float shotAwaitTime;
    public float minShotAwaitTime;
    public float maxShotAwaitTime;

    [Header("Wait time before pierce")]
    public bool isFixedTimeBeforePierce;
    public float pierceTime;
    public float minPiecreTime;
    public float maxPierceTime;

    [Header("Wait time to back rotation")]
    public bool isFixedTimeToBackRot;
    public float backRotateTime;
    public float minBackRotateTime;
    public float maxBackRotateTime;

    [Header("Bot pierce level")]
    public float pierceMultiplierAngle;
    public float fakeWindPower;
    public float targetAngle1;
    public float targetAngle2;

    [Header("Linked elements")]
    public GameObject botObject;
    public GameObject arrowObject;
    public GameObject botTarget;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
