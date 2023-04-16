using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using JsonSetting;


public class objManager : MonoBehaviour
{
    public GameObject point;    // �ϳ��� �� ������ ������ ��ü
    public GameObject middle;    // 

    private GameObject instance; // ������ ����Ʈ���� ������ ���� ��ü

    private Vector3 playerPos;
    private Vector3 playerDir;
    private Vector3 spawnPos;

    private ScanDataArray myData;

    public float limitTime = 60;
    private float currTime;

    float y;

    // Start is called before the first frame update
    void Start()
    {
        LoadJson ldJson = new LoadJson("json/data");        // json �����͸� �ε�
        myData = ldJson.Data;                               // json ������ ��������

    }

    // Update is called once per frame
    void Update()
    {

        if (currTime > limitTime)        // ���� �ð��� ���� �ð����� ū ���
        {
            currTime = 0;

        }
        else
        {
            currTime += Time.deltaTime;

            y = (float)middle.transform.position.y + .1f;
            for (int i = 0; i < myData.data.Length; i += 2)
            {
                middle.transform.rotation = Quaternion.Euler(0, float.Parse(myData.data[i].angle), 0);                                  // �߽� ��ġ ��ü�� rotaion�� ����
                middle.transform.position = new Vector3((float)middle.transform.position.x, y, (float)middle.transform.position.z);     // y�� �� �߰��� ���� ��ü�� position�� ����
                playerPos = middle.transform.position;                                                                                  // ���� �÷��̾� ��ġ
                playerDir = middle.transform.forward;                                                                                   // ���� �÷��̾ �ٶ󺸴� ����
                playerDir *= float.Parse(myData.data[i].distance) * 0.01f;
                spawnPos = playerPos + playerDir;
                instance = Instantiate(point);
                instance.transform.position = spawnPos;
            }
            Debug.Log($"{Mathf.Round(currTime)} {currTime}");
        }

    }
}
