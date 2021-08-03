using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyGenerator : MonoBehaviour
{

    [SerializeField] private GameObject unit;
    [SerializeField] private bool isInfiniteGen;
    [SerializeField] private int numOfGen;
    [SerializeField] private float countTime;
    private float lastTime;
    private int lastGen;
    private GameObject tempUnit;

    // Start is called before the first frame update
    void Start()
    {
        lastTime = Time.deltaTime;
        lastGen = numOfGen;
    }

    // Update is called once per frame
    void Update()
    {
        if (isInfiniteGen)
        {
            StartCoroutine(CreateUnit(countTime));
            isInfiniteGen = false;
        }
    }

    IEnumerator CreateUnit(float time)
    {
        yield return new WaitForSeconds(time);
        tempUnit = Instantiate(unit, gameObject.transform);
        float xoffset = Random.Range(-2.5f, 2.5f);
        float yoffset = Random.Range(-0.3f, 0.4f);
        tempUnit.transform.position += new Vector3(xoffset, yoffset, 0);
        isInfiniteGen = true;
    }

    public void TimeToGen(string val)
    {
        countTime = float.Parse(val);
    }
}
