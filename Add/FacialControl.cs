using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FacialControl : MonoBehaviour
{
    public Transform eyelid_l; //z: 103.24~75
    public Transform eyelid_r; //z: 105.603~77
    public Transform eye_l; //x: -5 ~ 5, z: 70 ~ 100
    public Transform eye_r; //x: -5 ~ 5, z: 70 ~ 100
    public Transform jaw; //z: 122.669~150

    private float[] timeCount = new float[5];

    private Vector3 eyelid_l_org;
    private Vector3 eyelid_r_org;
    private Vector3 eyelid_l_end;
    private Vector3 eyelid_r_end;
    private Vector3 jaw_org;
    private Vector3 jaw_end;

    private float threthold;
    private bool[] get = new bool[5];

    // Start is called before the first frame update
    void Start()
    {
        eyelid_l_org = eyelid_l.localEulerAngles;
        eyelid_r_org = eyelid_l.localEulerAngles;
        eyelid_l_end = eyelid_l.localEulerAngles + new Vector3(0, 0, 28);
        eyelid_r_end = eyelid_r.localEulerAngles + new Vector3(0, 0, 28);

        jaw_org = jaw.localPosition;
        jaw_end = jaw.localPosition + new Vector3(0.072f, 0.045f, 0);
        

        for(int i = 0; i < get.Length; i++) {
            get[i] = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        timeCount[2] += Time.deltaTime;
        timeCount[3] += Time.deltaTime;
        if (timeCount[2] >= 3){
            wink(4);
        }
        if (get[0]){
            get[0] = false;
            timeCount[2] = 0;
        }
        if (timeCount[3] >= 5){
            yawn(3);
        }
        if (get[1]){
            get[1] = false;
            timeCount[3] = 0;
        }
    }

    public void wink(float speed)
    {
        timeCount[0] += Time.deltaTime;
        threthold = timeCount[0] * speed;
        if (threthold <= 1){
            eyelid_l.localEulerAngles = Vector3.Lerp(eyelid_l_org, eyelid_l_end, threthold);
            eyelid_r.localEulerAngles = Vector3.Lerp(eyelid_r_org, eyelid_r_end, threthold);
        }
        else if (threthold > 1 && threthold <= 2){
            eyelid_l.localEulerAngles = Vector3.Lerp(eyelid_l_end, eyelid_l_org, threthold - 1);
            eyelid_r.localEulerAngles = Vector3.Lerp(eyelid_r_end, eyelid_r_org, threthold - 1);
        }
        else{
            timeCount[0] = 0;
            get[0] = true;
        }
    }

    public void eyesight()
    {
        
    }

    public void yawn(int speed)
    {   
        timeCount[1] += Time.deltaTime;
        threthold = timeCount[1] * speed;
        if (threthold <= 1){
            jaw.localPosition = Vector3.Lerp(jaw_org, jaw_end, threthold);
        }
        else if (threthold > 1 && threthold <= 1.5){
            
        }
        else if (threthold > 1.5 && threthold <= 2.5){
            jaw.localPosition = Vector3.Lerp(jaw_end, jaw_org, threthold - 1.5f);
        }
        else{
            timeCount[1] = 0;
            get[1] = true;
        }
    }
}


