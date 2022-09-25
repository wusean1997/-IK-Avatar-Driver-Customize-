using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InnerCam : MonoBehaviour
{   
    private Vector3 originalPosition;
    private float shadowStrength;
    private float timecount = 0f;

    //public GameObject lightGameObject;
    public Light lightComp;

    [Header("Param")]
    [SerializeField]private float min_vib;
    [SerializeField]private float max_vib;
    [SerializeField]private float min_light;
    [SerializeField]private float max_light;
    [SerializeField]private int sec;

    // Start is called before the first frame update
    void Start()
    {
        //Light lightComp = lightGameObject.AddComponent<Light>();
        originalPosition = transform.localPosition;
        shadowStrength = lightComp.shadowStrength;
    }

    // Update is called once per frame
    void Update()
    {   
        timecount += Time.deltaTime;
        transform.localPosition = originalPosition + new Vector3(Random.Range(min_vib, max_vib), Random.Range(min_vib, max_vib), Random.Range(min_vib, max_vib));
        if (((int)timecount % sec) == 0 && ((int)timecount / sec) == 1){
            shadowStrength = Random.Range(min_light, max_light);
            timecount = 0;
        }
        if (lightComp.shadowStrength < shadowStrength){
            lightComp.shadowStrength = Random.Range(lightComp.shadowStrength, shadowStrength);
        }
        else if (lightComp.shadowStrength > shadowStrength){
            lightComp.shadowStrength = Random.Range(shadowStrength, lightComp.shadowStrength);
        }
        else{
            lightComp.shadowStrength = shadowStrength;
        }
    }
}