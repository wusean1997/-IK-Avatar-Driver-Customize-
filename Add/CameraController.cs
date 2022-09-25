using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
//using System.Runtime.InteropServices;
using UnityEngine;
using System.Text.Json;
using System.Text.Json.Serialization;
using Newtonsoft.Json.Linq;

public class CameraController : MonoBehaviour
{
    Camera cam;
    private int count = 1;

    private bool newline = false;

    public class data
    {
        public string image { get; set; }
        public float[,] points { get; set; }
        public int[] visibility { get; set;}
        public int[,] bbox{get; set;}
    }

    public GameObject R_UpArm_pos;
    public GameObject R_LowArm_pos;
    public GameObject R_Hand_pos;

    public GameObject L_UpArm_pos;
    public GameObject L_LowArm_pos;
    public GameObject L_Hand_pos;

    public GameObject R_UpLeg_pos;
    public GameObject R_LowLeg_pos;
    public GameObject R_Foot_pos;

    public GameObject L_UpLeg_pos;
    public GameObject L_LowLeg_pos;
    public GameObject L_Foot_pos;

    public GameObject Head_pos;
    public GameObject Neck_pos;
    public GameObject UpChest_pos;
    public GameObject Chest_pos;
    public GameObject Spine_pos;

    public GameObject L_Eye_pos;
    public GameObject R_Eye_pos;
    public GameObject Jaw_pos;

    public GameObject eye_06;
    public GameObject eye_07;
    public GameObject eye_08;
    public GameObject eye_09;
    public GameObject eye_10;
    public GameObject eye_11;
    public GameObject eye_12;
    public GameObject eye_13;
    public GameObject eye_14;
    public GameObject eye_15;
    public GameObject eye_16;
    public GameObject eye_17;

    public GameObject mouth_18;
    public GameObject mouth_19;
    public GameObject mouth_20;
    public GameObject mouth_21;
    public GameObject mouth_22;
    public GameObject mouth_23;

    public GameObject nose_24;

    static int Object_length = 39;

    private GameObject[] Object_Arr;

    private string folderPath = Directory.GetCurrentDirectory();

    private string imgfolderPath;
    private string metafolderPath;

    private string item = "/Body/";

    private string time = DateTime.Now.ToString("yyyyMMdd-HHmm");

    private string imgfolderName = "/ImageData/";
    private string metafolderName = "/MetaData/";

    public int Max_sets = 100;

    private int[] visible;
    private int[] drop_visible;

    private float[,] metadata;
    private float[,] drop_metadata;

    //Check v2e visible
    private bool check_v2e = false;
    private float[,] current_meta;
    private float threthold = 0.00001f;

    private int[,] pixel_size = {{0,0},{240,180}};

    private int[,] bounding;
    private int min_x;
    private int max_x;
    private int min_y;
    private int max_y;
    private int boundary = 20;
    public bool offset_boundary = false;
    
    private int fps = 30;
    private float videoTime = 0.0f;

    private int train;
    private int val;

    private int _layerMask = 1<<3;
    private float ray_dist = 5.0f;

    public bool _drop = true;
    public bool save = true;
    private bool capture = false;
    private bool divide = false;
    private bool over_cam = false;
    private int index = 0;
    private int shift;

    //private int[] reserve_list = {1, 3, 5, 6, 7, 8, 9, 10, 17, 18, 19};
    private int[] reserve_list = {5, 6, 7, 8, 9, 10, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38};
    
    
    public void saveBound(){
        if (!offset_boundary){
            bounding = pixel_size;

            if (min_x > boundary){
                bounding[0,0] = min_x - boundary;
            }
            if (min_y > boundary){
                bounding[0,1] = min_y - boundary;
            }
            if (max_x < (bounding[1,0] - boundary)){
                bounding[1,0] = max_x + boundary;
            }
            if (max_y < (bounding[1,1] - boundary)){
                bounding[1,1] = max_y + boundary;
            }

            min_x = pixel_size[1,0];
            max_x = pixel_size[0,0]; 
            min_y = pixel_size[1,1];
            max_y = pixel_size[0,1];
        }
        else{
            bounding[0,0] = 80;
            bounding[0,1] = 0;
            bounding[1,0] = 240;
            bounding[1,1] = 180;
        }
            
    }

    public void Drop(){
        int j = 0;
        foreach (int i in reserve_list)
        {
            drop_visible[j] = visible[i];
            drop_metadata[j, 0] = metadata[i, 0];
            drop_metadata[j, 1] = metadata[i, 1];
            j++;
        }
    }

    public void CheckStation(){
        RaycastHit hit;
        for(int i = 0; i < visible.Length; i++) {
            
            var ray_pose = new Vector3(metadata[i,0] , metadata[i,1] , 0);
            Ray ray = cam.ScreenPointToRay(ray_pose);
            
            //Pixel Transfer
            metadata[i,1] = 180.0f - metadata[i,1];

            //Check Bounding
            if (!offset_boundary){
                if (pixel_size[0,0] <= metadata[i,0] || metadata[i,0] < pixel_size[1,0]){
                    if (metadata[i,0] < min_x){
                        min_x = (int)metadata[i,0];
                    }
                    if (metadata[i,0] > max_x){
                        max_x = (int)metadata[i,0];
                    }
                }
                else{
                    over_cam = true;
                }
                
                if (pixel_size[0,1] <= metadata[i,1] || metadata[i,1] < pixel_size[1,1]){
                    if (metadata[i,1] < min_y){
                        min_y = (int)metadata[i,1];
                    }
                    if (metadata[i,1] > max_y){
                        max_y = (int)metadata[i,1];
                    }
                }
                else{
                    over_cam = true;
                }
            }
            else{
                if (80 > metadata[i,0] || metadata[i,0] > 240){
                    over_cam = true;
                }
                else{
                    if (0 > metadata[i,1] || metadata[i,1] > 180){
                    over_cam = true; 
                    }
                }
            }
            
            //Check visible
            if (Physics.Raycast(ray, out hit, ray_dist, _layerMask) && !over_cam)
            {   
                /*
                if(i == reserve_list[index]){
                    print("Detect:["+ index + "]: " + hit.collider.gameObject.name);
                    if (index == (reserve_list.Length - 1)){
                        index = 0;
                    }
                    else{
                        index ++;
                    }
                }*/
                
                if (hit.collider.gameObject.name == Object_Arr[i].name)
                {
                    visible[i] = 1;
                }
                else{
                    visible[i] = 0;
                }
            }
            else{
                //print (" Nothing ");
                visible[i] = 0;
                over_cam = false;
            }

            //Check v2e visible
            if (check_v2e){
                var distance = Math.Sqrt((Math.Pow(current_meta[i,0] - metadata[i,0], 2) + Math.Pow(current_meta[i,1] - metadata[i,1], 2)));

                if (count != 1 && distance < threthold){
                    visible[i] = 0;
                }
            }
        }
    }

    public void SaveJsonData(string name, float max){
        string json;
        if(!_drop){
            var raw_data = new data{
                image = shift.ToString() + ".png",
                points = metadata,
                visibility = visible,
                bbox = bounding
            };
            json = Newtonsoft.Json.JsonConvert.SerializeObject(raw_data);
        }
        else{
            Drop();
            var raw_data = new data{
                image = shift.ToString() + ".png",
                points = drop_metadata,
                visibility = drop_visible,
                bbox = bounding
            };
            json = Newtonsoft.Json.JsonConvert.SerializeObject(raw_data);
        }     

        if (newline){
            json = ","+ json;//"\r\n" + json;
        }
        else{
            json = "["+ json;
            newline = true;
        }
        if(shift == max){
            json = json + "]";
            newline = false;
        }

        string filename = Path.Combine(metafolderPath, name);

        File.AppendAllText(filename, json);

    }

    public void TurnOff(){
        this.enabled = false;
    }

    // Start is called before the first frame update
    void Start()
    {
        cam = GetComponent<Camera>();
        if(save){
            imgfolderPath = folderPath + item + time + imgfolderName;
            metafolderPath = folderPath + item + time + metafolderName;

            if(!Directory.Exists(imgfolderPath)) {
                Directory.CreateDirectory(imgfolderPath);
            }
            if(!Directory.Exists(metafolderPath)) {
                Directory.CreateDirectory(metafolderPath);
            }

            train = (int)(Max_sets * 0.975f); //39:40
            val = train + (int)(Max_sets * 0.0225); //9:400
        }

        visible = new int[Object_length];
        metadata = new float[Object_length, 2];

        if (_drop){
            drop_visible = new int[reserve_list.Length];
            drop_metadata = new float[(reserve_list.Length), 2];
        }
        if (check_v2e){
            current_meta = new float[Object_length, 2];
        }

        GameObject[] Arr = { Head_pos, Neck_pos, UpChest_pos, Chest_pos, Spine_pos,
                        R_UpArm_pos, R_LowArm_pos, R_Hand_pos,
                        L_UpArm_pos, L_LowArm_pos, L_Hand_pos,
                        R_UpLeg_pos, R_LowLeg_pos, R_Foot_pos,
                        L_UpLeg_pos, L_LowLeg_pos, L_Foot_pos,
                        R_Eye_pos, L_Eye_pos, Jaw_pos, 
                        eye_06, eye_07, eye_08, eye_09, eye_10, eye_11,
                        eye_12, eye_13, eye_14, eye_15, eye_16, eye_17,
                        mouth_18, mouth_19, mouth_20, mouth_21, mouth_22, mouth_23, nose_24};

        Object_Arr = Arr;

        bounding = pixel_size;
        min_x = pixel_size[1,0];
        max_x = pixel_size[0,0];
        min_y = pixel_size[1,1];
        max_y = pixel_size[0,1];
    }

    // Update is called once per frame
    void Update()
    {
        if (save){
            videoTime += Time.deltaTime;
            if (videoTime >= (1.0f/ (float)fps)){
                if (count <= Max_sets){
                    ScreenCapture.CaptureScreenshot(Path.Combine(imgfolderPath, (count.ToString()+ ".png")));
                }
                for(int i = 0; i < Object_length; i++){
                    Vector2 temp = RectTransformUtility.WorldToScreenPoint(cam, Object_Arr[i].transform.position);
                    metadata[i,0] = temp[0];
                    metadata[i,1] = temp[1];
                }
                CheckStation();
                saveBound();
                capture = true;
                shift = count - 1;
            }
        }

        if (check_v2e){
            current_meta = metadata;
        }
        

        if (capture){
            if(shift > 0){
                if (divide){
                    if (shift <= train){
                        SaveJsonData("Train.json", train);
                    }
                    else if (shift > train && shift <= val){
                        SaveJsonData("Val.json", val);
                    }
                    else if (shift > val && shift <= Max_sets){
                        SaveJsonData("Test.json", Max_sets);
                    }
                    //print(videoTime);
                }
                else{
                    SaveJsonData("MetaData.json", Max_sets);
                }
            }
            
            videoTime = 0;
            capture = false;
            count++;
        }

        if (shift == Max_sets){
            TurnOff();
        }
    }
}