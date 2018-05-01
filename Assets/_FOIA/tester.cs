using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour {

    public GameObject poros;
    public bool clockwise = true;
    public bool jalanRotate = false;
    public float speed;
    public void tes(string id)
    {
        print(id);
    }

    void Update()
    {
        if (jalanRotate)
        {
            if (clockwise)
            {
                poros.transform.Rotate(Vector3.up * Time.deltaTime *speed);
            }
            else
            {
                poros.transform.Rotate(Vector3.down * Time.deltaTime * speed);
            }
        }
    }

    public void anim(bool set)
    {
        jalanRotate = true;
        clockwise = set;
    }

    public void stopAnim()
    {
        print("stop");
        jalanRotate = false;
    }
}
