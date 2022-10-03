namespace TurnTheGameOn.IKAvatarDriver
{
    using System;
    using System.IO;
    using System.Collections;
    using System.Collections.Generic;
    //using System.Runtime.InteropServices;
    using UnityEngine;
    using System.Text.Json;
    using System.Text.Json.Serialization;
    using Newtonsoft.Json.Linq;
    using Random = UnityEngine.Random;

    public class CameraController : MonoBehaviour
    {
        public Camera cam;
        private int count = 1;
        public AvatarDriver avatarDriver;

        private bool newline = false;

        private bool[] newline_list = {false, false, false};

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

        public Transform handTarget;
        public Transform phoneTarget;
        public Transform cigaretteTarget;
        public Transform beerTarget;
        public Transform gestureTarget;

        public GameObject index_1;
        public GameObject middle_1;
        public GameObject ring_1;
        public GameObject pinky_1;
        public GameObject thumb_1;

        public GameObject index_2;
        public GameObject middle_2;
        public GameObject ring_2;
        public GameObject pinky_2;
        public GameObject thumb_2;

        public Transform index_3;
        public Transform middle_3;
        public Transform ring_3;
        public Transform pinky_3;
        public Transform thumb_3;

        public GameObject index_end;
        public GameObject middle_end;
        public GameObject ring_end;
        public GameObject pinky_end;
        public GameObject thumb_end;

        public GameObject seatbelt_1;
        public GameObject seatbelt_2;

        static int Object_length = 46;

        private GameObject[] Object_Arr;

        private string folderPath = Directory.GetCurrentDirectory();

        private string imgfolderPath;
        private string metafolderPath;

        private string item = "/Body/";

        private string time = DateTime.Now.ToString("yyyyMMdd-HHmm");

        private string imgfolderName = "/ImageData/";
        private string metafolderName = "/MetaData/";

        public int Max_sets = 10;

        private int[] visible;
        private int[] drop_visible;

        private float[,] metadata;
        private float[,] drop_metadata;

        //Check v2e visible
        private bool check_v2e = false;
        private float[,] current_meta;
        private float threthold = 0.0001f;

        private int[,] pixel_size = {{0,0},{640,480}};
        private int[,] offset_bound = {{160,0},{640,480}};

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
        public bool divide = true;
        private bool over_cam = false;
        private int index = 0;
        private int shift;
        private float[] Dataset_Size = {0.64f, 0.16f, 0.2f}; //train64% val16% test20%
        private int[] dataset_size = new int[4];

        private bool[] behaviorState = new bool[4]; //beer, cigarette, phone, gesture

        private bool[] gestureState = new bool[5];
        private Transform[] gestureList = new Transform[5];
        private Transform[] gestureKeypointList = new Transform[5];
        private int gesture;
        
        //private int[] reserve_list = {1, 3, 5, 6, 7, 8, 9, 10, 17, 18, 19};
        private int[] reserve_list = {5, 6, 7, 8, 9, 10, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40, 41, 42, 43, 44, 45};
        
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
                bounding[0,0] = offset_bound[0,0];
                bounding[0,1] = offset_bound[0,1];
                bounding[1,0] = offset_bound[1,0];
                bounding[1,1] = offset_bound[1,1];
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

        
        public void CheckBehavior(){
            for (int i = 0; i < behaviorState.Length; i++){
                if (AvatarDriver.behaviorState[i]){
                    behaviorState[i] = true;
                }
                else{
                    behaviorState[i] = false;
                }
            }
        }
        
        public void CheckGesture(){
            gesture = 0;
            for(int i = 0; i < gestureList.Length; i++){
                if (Math.Abs(gestureList[i].localEulerAngles.z) < threthold){
                    gestureState[i] = true;
                }
                else{
                    gestureState[i] = false;
                }
            }
            for(int i = 0; i < gestureList.Length; i++){
                if (gestureState[i]){
                    gesture++;
                }
            }
            print(gesture);
        }

        public void CheckStation(){
            RaycastHit hit;
            for(int i = 0; i < visible.Length; i++) {
                
                var ray_pose = new Vector3(metadata[i,0] , metadata[i,1] , 0);
                Ray ray = cam.ScreenPointToRay(ray_pose);
                
                //Pixel Transfer
                metadata[i,1] = pixel_size[1,1] - metadata[i,1];
                
                //Check Bounding
                if (!offset_boundary){
                    if (pixel_size[0,0] <= metadata[i,0] && metadata[i,0] < pixel_size[1,0]){
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
                    
                    if (pixel_size[0,1] <= metadata[i,1] && metadata[i,1] < pixel_size[1,1]){
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
                    print("bbox: [" + offset_bound[0,0].ToString() + "," + offset_bound[0,1].ToString() + "][" + offset_bound[1,0].ToString() + "," + offset_bound[1,1].ToString() + "]");
                }
                else{
                    if (offset_bound[0,0] > metadata[i,0] || metadata[i,0] > offset_bound[1,0]){
                        over_cam = true;
                    }
                    else{
                        if (offset_bound[0,1] > metadata[i,1] || metadata[i,1] > offset_bound[1,1]){
                            over_cam = true; 
                        }
                    }
                    //print("bbox: [" + offset_bound[0,0].ToString() + "," + offset_bound[0,1].ToString() + "][" + offset_bound[1,0].ToString() + "," + offset_bound[1,1].ToString() + "]");
                }
                //Check visible
                if(i == reserve_list[index]){
                    if (Physics.Raycast(ray, out hit, ray_dist, _layerMask) && !over_cam)
                    {   
                        print("Detect:["+ index + "]: " + hit.collider.gameObject.name);
                        if (i == reserve_list[2]){
                            string[] hand_list = {Object_Arr[7].name, Object_Arr[39].name, Object_Arr[40].name, Object_Arr[41].name, Object_Arr[42].name, Object_Arr[43].name,
                                                    index_1.name, index_2.name, middle_1.name, middle_2.name, ring_1.name, ring_2.name, pinky_1.name, pinky_2.name, thumb_1.name, thumb_2.name};
                            if (Array.IndexOf(hand_list, hit.collider.gameObject.name) > -1)
                            {
                                visible[i] = 1;
                            }
                            else{
                                visible[i] = 0;
                            }
                        }
                        /*
                        else if(i >= 39 && i <= 43){
                            if (!behaviorState[3]){
                                visible[i] = 0;
                            }
                            else{
                                if (hit.collider.gameObject.name == Object_Arr[i].name)
                                {
                                    visible[i] = 1;
                                }
                                else{
                                    visible[i] = 0;
                                }
                            }
                        }
                        */
                        else if (i == reserve_list[3]){
                            if (hit.collider.gameObject.name == Object_Arr[i].name || hit.collider.gameObject.name == "Seatbelt_belt")
                            {
                                visible[i] = 1;
                            }
                            else{
                                visible[i] = 0;
                            }
                        }

                        else{
                            if (hit.collider.gameObject.name == Object_Arr[i].name)
                            {
                                visible[i] = 1;
                            }
                            else{
                                visible[i] = 0;
                            }
                        }
                    }
                    else{
                        print("Detect:["+ index + "]: " + "null");
                        visible[i] = 0;
                        over_cam = false;
                    }

                    index ++;
                    if (index == reserve_list.Length){
                        index = 0;
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
            if (divide){
                int i = 0;
                if (name == "Val.json"){
                    i = 1;
                }
                else if (name == "Test.json"){
                    i = 2;
                }

                if (newline_list[i]){
                    json = ","+ json;//"\r\n" + json;
                }
                else{
                    json = "["+ json;
                    newline_list[i] = true;
                }
                if(shift == max){
                    json = json + "]";
                    newline_list[i] = false;
                }
            }
            else{
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
                            mouth_18, mouth_19, mouth_20, mouth_21, mouth_22, mouth_23, nose_24,
                            thumb_end, index_end, middle_end, ring_end, pinky_end, seatbelt_1, seatbelt_2};

            Object_Arr = Arr;

            bounding = pixel_size;
            min_x = pixel_size[1,0];
            max_x = pixel_size[0,0];
            min_y = pixel_size[1,1];
            max_y = pixel_size[0,1];
            
            for(int i = 0; i < behaviorState.Length; i++) {
                behaviorState[i] = false;
            }
            
            for(int i = 0; i < gestureState.Length; i++) {
                gestureState[i] = false;
            }

            Transform[] g_Arr = {thumb_3, index_3, middle_3, ring_3, pinky_3};
            gestureList = g_Arr; 

            int temp = 0;
            for(int i = 0; i < Dataset_Size.Length; i++) {
                dataset_size[i] = (Max_sets * Dataset_Size[i])>(int)(Max_sets * Dataset_Size[i])?(int)(Max_sets * Dataset_Size[i])+1:(int)(Max_sets * Dataset_Size[i]);
                temp += dataset_size[i];
                print(dataset_size[i]);
            }
            if (temp > Max_sets){
                dataset_size[2] += (Max_sets - temp);
                print(dataset_size[2]);
            }
            else if (temp < Max_sets){
                dataset_size[0] += (Max_sets - temp);
                print(dataset_size[0]);
            }
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
                    CheckBehavior();
                    CheckStation();
                    //CheckGesture();
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
                        //Random Divide
                        int type = 0;
                        var rnd = Random.Range(0f, 1f);
                        if (rnd < 0.333f){
                            if (dataset_size[0] == 0){
                                if (dataset_size[1] > dataset_size[2]){
                                    type = 2;
                                }
                                else{
                                    type = 3;
                                }      
                            }
                            else{
                                type = 1;
                            }  
                        }
                        else if (rnd > 0.333f && rnd < 0.667f){
                            if (dataset_size[1] == 0){
                                if (dataset_size[0] > dataset_size[2]){
                                    type = 1;
                                }
                                else{
                                    type = 3;
                                }
                            }   
                            else{
                                type = 2;
                            }
                        } 
                        else {
                            if (dataset_size[2] == 0){
                                if (dataset_size[0] > dataset_size[1]){
                                    type = 1;
                                }
                                else{
                                    type = 2;
                                }
                            }   
                            else{
                                type = 3;
                            }
                        }

                        switch (type) {
                            case 1:
                                if (dataset_size[0] == 1){
                                    SaveJsonData("Train.json", shift);
                                    dataset_size[0] --;
                                }
                                else{
                                    SaveJsonData("Train.json", Max_sets);
                                    dataset_size[0] --;
                                }
                                break;

                            case 2:
                                if (dataset_size[1] == 1){
                                    SaveJsonData("Val.json", shift);
                                    dataset_size[1] --;
                                }
                                else{
                                    SaveJsonData("Val.json", Max_sets);
                                    dataset_size[1] --;
                                }
                                break;

                            case 3:
                                if (dataset_size[2] == 1){
                                    SaveJsonData("Test.json", shift);
                                    dataset_size[2] --;
                                }
                                else{
                                    SaveJsonData("Test.json", Max_sets);
                                    dataset_size[2] --;
                                }
                                break;
                    }
                        /*
                        //Sequential Data
                        if (shift <= train){
                            SaveJsonData("Train.json", train, false);
                        }
                        else if (shift > train && shift <= val){
                            SaveJsonData("Val.json", val, false);
                        }
                        else if (shift > val && shift <= Max_sets){
                            SaveJsonData("Test.json", Max_sets, false);
                        }*/
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
}
