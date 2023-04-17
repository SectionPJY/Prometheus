using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using JsonSetting;


public class objManager : MonoBehaviour
{
    public GameObject point;                                // �ϳ��� �� ������ ������ ��ü
/*    public GameObject middle;    // */

    private GameObject instance;                            // ������ ����Ʈ���� ������ ���� ��ü

    private Vector3 playerPos;
    private Vector3 playerDir;
    private Vector3 spawnPos;

    private ScanDataArray myData;

    private float deltaTime = 0.0f;
    private float msec;
    private float fps;

    public float limitTime = 60;
    private float currTime;
    private float nowTime = 0;

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
        deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
        fps = 1.0f / deltaTime;
        msec = deltaTime * 1000.0f;

        if (currTime > limitTime)        // ���� �ð��� ���� �ð����� ū ���
        {
            /*currTime = 0;*/
        }
        else
        {
            currTime += Time.deltaTime;

            if (fps < 15)                                                                   // �������� 15���Ϸ� ������ ���
            {           
                if (Mathf.Round(currTime) > nowTime)                                        // 1�ʸ��� ����                    
                {
                    nowTime = Mathf.Round(currTime);
                    createPoint();
                }

/*            Debug.Log($"{Mathf.Round(currTime)} {currTime}");*/
            }
            else
            {
                createPoint();
            }

        }

    }


    /// <summary>
    /// ����Ʈ ������Ʈ�� �����ϴ� ������ �Լ��� ��� ���
    /// </summary>
    private void createPoint()
    {
        for (int i = 0; i < myData.data.Length; i += 2)
        {
            this.transform.rotation = Quaternion.Euler(0, float.Parse(myData.data[i].angle), 0);                                  // �߽� ��ġ ��ü�� rotaion�� ����
            this.transform.position = new Vector3((float)this.transform.position.x, y, (float)this.transform.position.z);     // y�� �� �߰��� ���� ��ü�� position�� ����
            playerPos = this.transform.position;                                                                                  // ���� �÷��̾� ��ġ
            playerDir = this.transform.forward;                                                                                   // ���� �÷��̾ �ٶ󺸴� ����
            playerDir *= float.Parse(myData.data[i].distance) * 0.01f;
            spawnPos = playerPos + playerDir;
            instance = Instantiate(point);
            instance.transform.position = spawnPos;
        }
        y = (float)this.transform.position.y + .1f;
    }
}
