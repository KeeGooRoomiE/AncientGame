using System;
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

    [Header("Time between shots")]
    public bool isFixedTimeBetweenShots;
    public float shotRestartTime;
    public float minShotRestartTime;
    public float maxShotRestartTime;

    [Header("Bot pierce level")]
    public float pierceMultiplierAngle;
    public float fakeWindPower;
    public float targetAngle1;
    public float targetAngle2;

    [Header("Linked elements")]
    public GameObject botObject;
    public GameObject arrowObject;
    public GameObject botTarget;

    /*
     * 
     * Set edit functions to shot await time
     * 
     */
    public void SetIsFixedTimeBeforeShot(bool val)
    {
        isFixedTimeBeforeShot = val;
        UpdateTimetoShot();
    }

    public void SetShotAwaitTime(string val) {
        shotAwaitTime = Single.Parse(val);
        UpdateTimetoShot();
    }

    public void SetMinShotAwaitTime(string val)
    {
        minShotAwaitTime = Single.Parse(val);
        UpdateTimetoShot();
    }

    public void SetMaxShotAwaitTime(string val)
    {
        maxShotAwaitTime = Single.Parse(val);
        UpdateTimetoShot();
    }

    /*
     * 
     * Set edit functions to pierce await time
     * 
     */
    public void SetIsFixedTimeBeforePierce(bool val)
    {
        isFixedTimeBeforePierce = val;
        UpdateTimetoPierce();
    }

    public void SetPireceTime(string val)
    {
        pierceTime = Single.Parse(val);
        UpdateTimetoPierce();
    }

    public void SetMinPireceTime(string val)
    {
        minPiecreTime = Single.Parse(val);
        UpdateTimetoPierce();
    }

    public void SetMaxPireceTime(string val)
    {
        maxPierceTime = Single.Parse(val);
        UpdateTimetoPierce();
    }

    /*
     * 
     * Set edit functions to back animation rotation state
     * 
     */

    public void SetisFixedTimeToBackRot(bool val)
    {
        isFixedTimeToBackRot = val;
        UpdateTimetoBackRot();
    }

    public void SetbackRotateTime(string val)
    {
        backRotateTime = Single.Parse(val);
        UpdateTimetoBackRot();
    }

    public void SetMinBackRotateTime(string val)
    {
        minBackRotateTime = Single.Parse(val);
        UpdateTimetoBackRot();
    }

    public void SetMaxBackRotateTime(string val)
    {
        maxBackRotateTime = Single.Parse(val);
        UpdateTimetoBackRot();
    }

    /*
     * 
     * Set edit functions to update time to shot
     * 
     */
    public void UpdateTimetoShot()
    {
        if (isFixedTimeBeforeShot)
        {
            var t = shotAwaitTime;
            botObject.GetComponent<EnemyController>().timeToShot = t;
        } else
        {
            var t = UnityEngine.Random.Range(minShotAwaitTime, maxShotAwaitTime);
            botObject.GetComponent<EnemyController>().timeToShot = t;
        }
    }

    /*
     * 
     * Set edit functions to update time to shot
     * 
     */
    public void UpdateTimetoPierce()
    {
        if (isFixedTimeBeforePierce)
        {
            var t = pierceTime;
            botObject.GetComponent<EnemyController>().timeToPierce = t;
        }
        else
        {
            var t = UnityEngine.Random.Range(minPiecreTime, maxPierceTime);
            botObject.GetComponent<EnemyController>().timeToPierce = t;
        }
    }

    /*
     * 
     * Set edit functions to update time to back rotation state
     * 
     */

    public void UpdateTimetoBackRot()
    {
        if (isFixedTimeToBackRot)
        {
            var t = pierceTime;
            botObject.GetComponent<EnemyController>().timeToBackRot = t;
        }
        else
        {
            var t = UnityEngine.Random.Range(minBackRotateTime, maxBackRotateTime);
            botObject.GetComponent<EnemyController>().timeToBackRot = t;
        }
    }

    /*
     * 
     * Set edit functions to update time between shots
     * 
     */

    public void UpdateTimeToReload()
    {
        if (isFixedTimeBetweenShots)
        {
            var t = shotRestartTime;
            botObject.GetComponent<EnemyController>().timeToNextShot = t;
        }
        else
        {
            var t = UnityEngine.Random.Range(minShotRestartTime, maxShotRestartTime);
            botObject.GetComponent<EnemyController>().timeToNextShot = t;
        }
    }
}
