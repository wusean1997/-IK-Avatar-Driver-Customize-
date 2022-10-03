namespace TurnTheGameOn.IKAvatarDriver
{
    using UnityEngine;
    using UnityEngine.UI;
    using System.Text.Json;
    using System.Text.Json.Serialization;
    using System.Collections.Generic;

    [System.Serializable, RequireComponent(typeof(Animator))]
    public class AvatarDriver : MonoBehaviour
    {
        public float horizontalInput;   // controls steering and hand positions
        public float verticalInput;     // controls accelerate/brake foot
        public string gearString;       // changes trigger shift
        public bool shift;              //set this bool to true to trigger a shift
        public AvatarDriverSettings avatarSettings;
        public Animator animator; // animator for calling IK functions
        public ConfigurableJoint bodyJoint;
        public Transform steeringWheel;
        public Transform readOnlySteeringWheel;
        public Rigidbody vehicleRigidbody; // for calculating steering wheel shake value
        public Text gearText; // when this text changes a shift will be triggered
        public Transform Hip, Jaw;
        public Transform Look_01, Look_02, Look_03, Look_04, Look_05, Look_06, Look_07, Look_08;
        public Transform Object_pA, Object_pB, Object_pC, Object_pD, handTemp, handTempIK, Head, HeadSubstitude, GlassesSubstitude;   //Object Parents
        public Transform Object_A, Object_B, Object_C, Object_D, Mouth, Beer_Mouth, Face, Hand, phoneHand,  Finger, Center;          //Pick IK Target A:Beer B:Phone C:Box D:Cigarette
        public Transform  Gesture, Gesture_1, Gesture_2, Gesture_3, Gesture_4, Gesture_5, Gesture_6, Gesture_7;
        public Transform  Gesture_8, Gesture_9, Gesture_10, Gesture_11, Gesture_12, Gesture_13, Gesture_14, Gesture_15;
        public GameObject Beer, Glasses, Cigarette, Phone;                                                           //Pick Objects
        public Transform Index_1, Index_2, Index_3, Middle_1, Middle_2, Middle_3, Ring_1, Ring_2, Ring_3, Pinky_1, Pinky_2, Pinky_3, Thumb_1, Thumb_2, Thumb_3;
        public Transform headLookIKCP, targetLeftHandIK, targetRightHandIK, targetRightFootIK, targetLeftFootIK; // Avatar IK control points
        public Transform leftHandObj, rightHandObj, leftFootObj, rightFootObj;							 // current IK targets
        public Transform lhswt_W, lhswt_NW, lhswt_N, lhswt_NE, lhswt_E, lhswt_SE, lhswt_S, lhswt_SW;             // left hand steering wheel IK targets
        public Transform rhswt_W, rhswt_NW, rhswt_N, rhswt_NE, rhswt_E, rhswt_SE, rhswt_S, rhswt_SW;             // right hand steering wheel IK targets
        public Transform leftFootIdle, rightFootIdle, footClutch, footBrake, footGas, handShift, returnShiftTarget;
        private Transform cachedTransform;
        private Transform wheelCP;
        private AvatarDriver_STSReference stsReference;
        private AvatarDriver_RCCReference rccReference;
        private AvatarDriver_EVPReference evpReference;
        private AvatarDriver_NWHReference nwhReference;
        private CarDriveSystem carDriveSystem;
        private bool shifting;                                                                      //used to determine when the right hand should target a steering wheel target or shift target
        private bool targetingWheel;
        private string lastGearString;
        private float lookObjMoveSpeed;                                                                                 //the speed at which the look target object will move
        private int gearInput, lastGearInput;
        private float tempShake;
        private float targetY;
        private float zAngle;
        private float yVelocity;
        private float rotationLimit;
        private float torsoRotation = 0.0f;
        private float torsoLean = 0.0f;
        private float targetRotInput = 0.0f;
        private float targetTorsoLeanSideway = 0.0f;
        private float targetTorsoRotation = 0.0f;
        private float shiftLerp;
        private float holdShiftTimer;
        private float shiftBackTimer;
        private float currentAngleNormalized = 0.0f;
        private float rotateSpeeds = 1.0f;
        private float lookTargetPosX; //the look target transform position.x value
        private Vector3 lookPosition;
        private Quaternion shiftStartRotation;
        private float prevZAngle;
        private float speedometerMultiplier;
        private float deltaTime;

        //Pick Parameter
        private int step = 0;
        private int run = 0;
        private int look = 0;
        private int look_max = 9;
        //private bool take = true;
        private bool over = false;
        private bool get = false;
        private bool init = false;
        private float speed = 0.5f;
        private float timeCount = 0.0f;
        private float[] holdingTime = {1.0f, 3.0f, 5.0f, 10.0f}; //sec
        private float angle, dist;
        private float threthold = 0.00001f;
        private Vector3 objOriginalPosition, buffer_OriginalPosition, buffer_HeadPosition;
        private Quaternion objOriginalRotation, buffer_OriginalRotation, buffer_HeadRotation;
        public Vector3 original_hip;

        public class Parameter
        {
            public float hor { get; set; }
            public float acc { get; set; }
            public float foo { get; set; }
            public float eme { get; set; }
        }
        private string folderPath = System.IO.Directory.GetCurrentDirectory();
        private string filename = "/AutoDriver/Inputkey.json";
        private string filepath;
        private string contents;
        private int index;
        private float[,] auto_param;
        private int count = 0;
        public bool use = false;
        private float Timedelate;
        private float time = 0;
        private int finger = 0;

        private Vector3[,] finger_rotation = new Vector3[5,3];
        private Transform[,] finger_list = new Transform[5,3];
        private bool[] gate = {false, false, false};
        private Transform[] lookTargetList = new Transform[9];
        private Transform[] gestureTargetList = new Transform[16];
        private int gestureIndex = 0;
        private int gestureIndex_max = 16;
        public static bool[] behaviorState = new bool[4];

        #region Register callbacks and variables
        void OnEnable()
        {
            bodyJoint.connectedBody = vehicleRigidbody;
            switch (avatarSettings.avatarInputType)
            {
                case AvatarInputType.Player:
                    if (!carDriveSystem) carDriveSystem = GetComponentInParent<CarDriveSystem>();
                    carDriveSystem.OnUpdateInput += OnGetInput;
                    break;
                case AvatarInputType.STS:
                    if (!stsReference) stsReference = GetComponentInParent<AvatarDriver_STSReference>();
                    stsReference.OnUpdateInput_STS += OnGetInput_STS;
                    break;
                case AvatarInputType.RCC:
                    if (!rccReference) rccReference = GetComponent<AvatarDriver_RCCReference>();
                    rccReference.OnUpdateInput_RCC += OnGetInput_RCC;
                    break;
                case AvatarInputType.EVP:
                    if (!evpReference) evpReference = GetComponent<AvatarDriver_EVPReference>();
                    evpReference.OnUpdateInput_EVP += OnGetInput_EVP;
                    break;
                case AvatarInputType.NWH:
                    if (!nwhReference) nwhReference = GetComponent<AvatarDriver_NWHReference>();
                    nwhReference.OnUpdateInput_NWH += OnGetInput_NWH;
                    break;
                case AvatarInputType.Custom:
                    break;
            }
        }

        void OnDisable()
        {
            switch (avatarSettings.avatarInputType)
            {
                case AvatarInputType.Player:
                    carDriveSystem.OnUpdateInput -= OnGetInput;
                    break;
                case AvatarInputType.STS:
                    stsReference.OnUpdateInput_STS -= OnGetInput_STS;
                    break;
                case AvatarInputType.RCC:
                    rccReference.OnUpdateInput_RCC -= OnGetInput_RCC;
                    break;
                case AvatarInputType.EVP:
                    evpReference.OnUpdateInput_EVP -= OnGetInput_EVP;
                    break;
                case AvatarInputType.NWH:
                    nwhReference.OnUpdateInput_NWH -= OnGetInput_NWH;
                    break;
                case AvatarInputType.Custom:
                    break;
            }
        }
        #endregion

        #region Event callbacks
        void OnGetInput()
        {   if (use){
                if (count == auto_param.GetLength(0)){
                    count = 0;
                }
                horizontalInput = auto_param[count,0];
                verticalInput = auto_param[count,1];
                count++;
            }
            else{
                horizontalInput = avatarSettings.mobile ? MobileInputManager.GetAxis(avatarSettings.steeringAxis) : Input.GetAxis(avatarSettings.steeringAxis);
                verticalInput = avatarSettings.mobile ? MobileInputManager.GetAxis(avatarSettings.throttleAxis) : Input.GetAxis(avatarSettings.throttleAxis);
            }
        }

        void OnGetInput_STS()
        {
            horizontalInput = Mathf.Clamp(stsReference.sts_Horizontal * avatarSettings.aISteerMultiplier, -1.0f, 1.0f);
            //gearText.text = carDriveSystem.currentGear == 0 ? "N" : carDriveSystem.currentGear == -1 ? "R" : carDriveSystem.currentGear.ToString();
            verticalInput = stsReference.sts_Vertical;
        }

        void OnGetInput_RCC()
        {
            horizontalInput = Mathf.Clamp(rccReference.rcc_Horizontal, -1.0f, 1.0f);
            verticalInput = rccReference.rcc_Vertical;
        }

        void OnGetInput_EVP()
        {
            OnGetInput();
            gearInput = evpReference.evp_Gear;
        }

        void OnGetInput_NWH()
        {
            verticalInput = nwhReference.nwh_Vertical;
            horizontalInput = nwhReference.nwh_Horizontal;
            gearString = nwhReference.nwh_gear;
        }
        #endregion

        #region PickObj
        void OnPickUp(GameObject obj, Transform parent){
            obj.transform.SetParent(parent);
            obj.transform.localPosition = parent.localPosition;
            obj.transform.localRotation = parent.localRotation;
        }

        void OnDrop(GameObject obj, Transform parent){
            obj.transform.SetParent(parent);
            obj.transform.localPosition = objOriginalPosition;
            obj.transform.localRotation = objOriginalRotation;
        }

        void Pack(Transform obj, Transform parent){
            obj.transform.SetParent(parent);
        }

        void UnPack(Transform obj, Transform parent){
            obj.transform.SetParent(parent);
        }
        #endregion


        void Start()
        {
            original_hip = Hip.localEulerAngles;
            cachedTransform = transform;
            transform.localPosition = avatarSettings.avatarPosition;
            lookTargetPosX = avatarSettings.defaultLookXPos;
            TargetShifter();
            rotationLimit = avatarSettings.steeringTargets == SteeringTargets.Two ? avatarSettings.steeringWheelRotationTwoTargets : avatarSettings.steeringWheelRotation;
            wheelCP = avatarSettings.shiftHand == TargetSide.Left ? targetLeftHandIK : avatarSettings.shiftHand == TargetSide.Right ? targetRightHandIK : null;

            if (use){
                filepath = folderPath + filename;
                var param = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Parameter>>(System.IO.File.ReadAllText(filepath));
                auto_param = new float[param.Count, 4];
                for(int i = 0; i < auto_param.GetLength(0); i++) {
                    auto_param[i, 0] = param[i].hor;
                    auto_param[i, 1] = param[i].acc;
                    auto_param[i, 2] = param[i].foo;
                    auto_param[i, 3] = param[i].eme;
                }
            }
            Transform[,] Finger_list = {{Index_1, Index_2, Index_3}, 
                                        {Middle_1, Middle_2, Middle_3}, 
                                        {Ring_1, Ring_2, Ring_3}, 
                                        {Pinky_1, Pinky_2, Pinky_3}, 
                                        {Thumb_1, Thumb_2, Thumb_3}};
            finger_list = Finger_list;

            Transform[] Look_list = {headLookIKCP, Look_01, Look_02, 
                                        Look_03, Look_04, Look_05,
                                        Look_06, Look_07, Look_08};
            
            lookTargetList = Look_list;

            Transform[] Gesture_List = {Gesture, Gesture_1, Gesture_2, Gesture_3,
                                        Gesture_4, Gesture_5, Gesture_6, Gesture_7,
                                        Gesture_8, Gesture_9, Gesture_10, Gesture_11,
                                        Gesture_12, Gesture_13, Gesture_14, Gesture_15};

            gestureTargetList = Gesture_List;
        }

        void Update(){
            Object_C.position = GlassesSubstitude.position;
            Object_C.rotation = GlassesSubstitude.rotation;
            HeadSubstitude.position = Head.position;
            HeadSubstitude.rotation = Head.rotation;
        }

        void OnAnimatorIK()
        {
            deltaTime = Time.deltaTime;
            if (animator && avatarSettings.ikActive && rightHandObj != null)
            {   

                SetShiftState();
                SetIKValues(AvatarIKGoal.LeftHand, 1, 1, targetLeftHandIK.position, targetLeftHandIK.rotation);
                SetIKValues(AvatarIKGoal.RightHand, 1, 1, targetRightHandIK.position, targetRightHandIK.rotation);
                SetIKValues(AvatarIKGoal.LeftFoot, 1, 1, targetLeftFootIK.position, targetLeftFootIK.rotation);
                SetIKValues(AvatarIKGoal.RightFoot, 1, 1, targetRightFootIK.position, targetRightFootIK.rotation);
                SetSteeringWheelRotation();
                SetTorsoRotation();
                UpdateHandTransforms();
                UpdateFootTransforms();
                Realistic();
                
                switch (run) {
                    /*
                    case 0:
                        if(!get){
                            UpdateIKTargetTransforms("TkoGlasses");
                            UnPack(targetRightHandIK, handTemp);
                        }
                        else{
                            get = false;
                            Pack(targetRightHandIK, steeringWheel);
                            run = 2;
                        }  
                        break;        

                    case 1:
                        if(!get){
                            UpdateIKTargetTransforms("WearGlasses");
                            UnPack(targetRightHandIK, handTemp);
                        }
                        else{
                            get = false;
                            Pack(targetRightHandIK, steeringWheel);
                            run = 2;
                        }  
                        break;
                    */   
                    
                    case 0:
                        timeCount += Time.deltaTime;
                        UpdateIKTargetTransforms("shifting");
                        UpdateIKLook("normal", 1);

                        if (timeCount > 5f){
                            run++;
                            timeCount = 0;
                        }
                        break;

                    case 1:
                        if (!get){
                            UpdateIKTargetTransforms("PickBeer");
                            UnPack(targetRightHandIK, handTemp);
                        }
                        else{
                            get = false;
                            Pack(targetRightHandIK, steeringWheel);
                            run++;
                        }
                        break;

                    case 2:
                        if(!get){
                            UpdateIKTargetTransforms("PickCigarette");
                            UnPack(targetRightHandIK, handTemp);
                        }
                        else{
                            get = false;
                            Pack(targetRightHandIK, steeringWheel);
                            run++;
                        }
                        break;

                    case 3:
                        if(!get){
                            UpdateIKTargetTransforms("PickPhone");
                            UnPack(targetRightHandIK, handTemp);
                        }
                        else{
                            get = false;
                            Pack(targetRightHandIK, steeringWheel);
                            run++;
                        }  
                        break;
                    
                    case 4:
                        if(!get){
                            UpdateIKTargetTransforms("Gesture");
                            UnPack(targetRightHandIK, handTemp);
                        }
                        else{
                            get = false;
                            Pack(targetRightHandIK, steeringWheel);
                            over = true;
                            run = 0;
                        }  
                        break;              
                }        
            }
            else
            {
                ZeroIKWeight(AvatarIKGoal.LeftHand);
                ZeroIKWeight(AvatarIKGoal.RightHand);
                ZeroIKWeight(AvatarIKGoal.LeftFoot);
                ZeroIKWeight(AvatarIKGoal.RightFoot);
                animator.SetLookAtWeight(0);
            }
        }

        void SetShiftState()
        {
            if (!avatarSettings.enableShifting)
            {
                return;
            }
            if (avatarSettings.avatarInputType == AvatarInputType.EVP)
            {
                if (gearInput != lastGearInput)
                {
                    lastGearInput = gearInput;
                    TargetShifter();
                }
            }
            else if (avatarSettings.avatarInputType == AvatarInputType.STS)
            {
                if (gearInput != lastGearInput)
                {
                    lastGearInput = gearInput;
                    TargetShifter();
                }
            }
            else if (avatarSettings.avatarInputType == AvatarInputType.NWH)
            {
                if (gearString != lastGearString)
                {
                    lastGearString = gearString;
                    TargetShifter();
                }
            }
            else if (gearText.text != gearString)
            {
                gearString = gearText.text;
                TargetShifter();
            }
            if (shift)
            {
                shift = false;
                TargetShifter();
            }
        }

        private static float WrapAngle(float angle)
        {
            angle %= 360;
            if (angle > 180)
                return angle - 360;

            return angle;
        }

        void SetSteeringWheelRotation()
        {
            if (avatarSettings.controlSteeringWheel)
            {
                if (steeringWheel != null)
                {
                    rotationLimit = avatarSettings.steeringTargets == SteeringTargets.Two ? avatarSettings.steeringWheelRotationTwoTargets : avatarSettings.steeringWheelRotation;
                    tempShake = Random.Range(1.0f, 2.0f);
                    speedometerMultiplier = avatarSettings.speedType == SpeedType.MPH ? 2.23693629f : avatarSettings.speedType == SpeedType.KPH ? 3.6f : 0f;
                    tempShake = tempShake * avatarSettings.wheelShake * 50 * LinearDistance(0, 150, vehicleRigidbody.velocity.magnitude * speedometerMultiplier);
                    tempShake = Random.Range(-tempShake, tempShake);
                    if (horizontalInput == 0) targetY = Mathf.SmoothDamp(targetY, (-(horizontalInput * rotationLimit) - tempShake), ref yVelocity, avatarSettings.snapBackRotationSpeed);
                    else targetY = Mathf.SmoothDamp(targetY, (-(horizontalInput * rotationLimit) - tempShake), ref yVelocity, avatarSettings.rotationSpeed);
                    if (avatarSettings.avatarInputType == AvatarInputType.STS) targetY = Mathf.Clamp(targetY, -rotationLimit, rotationLimit);
                    zAngle = Mathf.SmoothDampAngle(steeringWheel.localEulerAngles.z, targetY, ref yVelocity, avatarSettings.steeringRotationSpeed);
                    if (float.IsNaN(zAngle) == false)
                    {
                        steeringWheel.localEulerAngles = new Vector3(avatarSettings.defaultSteeringWheelRotation.x, avatarSettings.defaultSteeringWheelRotation.y, zAngle);
                        prevZAngle = zAngle;
                    }
                    else
                    {
                        zAngle = prevZAngle;
                    }
                }
            }
            else
            {
                if (readOnlySteeringWheel != null)
                {
                    targetY = WrapAngle(readOnlySteeringWheel.localEulerAngles.z);
                }
            }
        }

        void SetTorsoRotation()
        {
            if (horizontalInput == 0)
            {
                targetRotInput = 0;
            }
            else if (targetY > 0) // turning left ++
            {
                currentAngleNormalized = LinearDistance(0, rotationLimit, zAngle);
                if (steeringWheel.localEulerAngles.z > 0)
                {
                    targetRotInput = avatarSettings.torsoCurve.Evaluate(currentAngleNormalized);
                    targetRotInput = Mathf.Clamp(targetRotInput, -1, 1);
                }
                // targeting left ++
                else
                {
                    targetRotInput = -1 * avatarSettings.torsoCurve.Evaluate(currentAngleNormalized);
                    targetRotInput = Mathf.Clamp(targetRotInput, -1, 1);
                }
            }
            else if (targetY < 0)// turning right --
            {
                currentAngleNormalized = LinearDistance(0, rotationLimit, 360 - zAngle);
                if (steeringWheel.localEulerAngles.z < 0)
                {
                    targetRotInput = avatarSettings.torsoCurve.Evaluate(currentAngleNormalized);
                    targetRotInput = Mathf.Clamp(targetRotInput, -1, 1);
                }  // targeting right --
                else
                {
                    targetRotInput = -1 * avatarSettings.torsoCurve.Evaluate(currentAngleNormalized);
                    targetRotInput = Mathf.Clamp(targetRotInput, -1, 1);
                }
            }
            targetRotInput = targetRotInput > 0 ? avatarSettings.torsoCurve.Evaluate(Mathf.Abs(targetRotInput)) : targetRotInput < 0 ? -1 * avatarSettings.torsoCurve.Evaluate(Mathf.Abs(targetRotInput)) : 0;
            targetTorsoLeanSideway = targetRotInput > 0 ? targetRotInput * avatarSettings.maxLeanLeft : targetRotInput * avatarSettings.maxLeanRight * -1;
            targetTorsoRotation = targetRotInput > 0 ? targetRotInput * avatarSettings.maxRotateLeft : targetRotInput * avatarSettings.maxRotateRight * -1;
            torsoLean = Mathf.SmoothStep(torsoLean, targetTorsoLeanSideway, rotateSpeeds);
            torsoRotation = Mathf.SmoothStep(torsoRotation, targetTorsoRotation, rotateSpeeds);
            cachedTransform.localEulerAngles = new Vector3(avatarSettings.defaultTorsoLeanIn, torsoRotation, torsoLean);
        }

        void SetIKValues(AvatarIKGoal goal, float positionWeight, float rotationWeight, Vector3 position, Quaternion rotation)
        {
            animator.SetIKPositionWeight(goal, positionWeight);
            animator.SetIKRotationWeight(goal, rotationWeight);
            animator.SetIKPosition(goal, position);
            animator.SetIKRotation(goal, rotation);
        }

        void ZeroIKWeight(AvatarIKGoal goal)
        {
            animator.SetIKPositionWeight(goal, 0);
            animator.SetIKRotationWeight(goal, 0);
        }

        void UpdateIKLook(string type, float rate)
        {
            if (headLookIKCP != null)
            {   
                switch (type) {
                    case "PickBeer":
                        animator.SetLookAtWeight(rate);
                        animator.SetLookAtPosition(Beer.transform.position);
                        break;

                    case "PickPhone":
                        animator.SetLookAtWeight(rate);
                        animator.SetLookAtPosition(Phone.transform.position);
                        break;

                    case "Glasses":
                        animator.SetLookAtWeight(rate);
                        animator.SetLookAtPosition(Center.position);
                        break;

                    case "PickCigarette":
                        animator.SetLookAtWeight(rate);
                        animator.SetLookAtPosition(Cigarette.transform.position);
                        break;
                    /*
                    case "Hat":
                        animator.SetLookAtWeight(rate);
                        animator.SetLookAtPosition(Object_E.position);
                        break;
                    */
                    
                    case "normal" :
                        animator.SetLookAtWeight(rate);
                        animator.SetLookAtPosition(lookTargetList[look].position);
                        if (over){
                            look++;
                            over = false;
                            if (look == look_max){
                                look = 0;
                            }
                        }
                        break;
                } 
            }
        }

        void UpdateHandTransforms()
        {
            if (avatarSettings.steeringTargets == SteeringTargets.Two)
            {
                rightHandObj = rhswt_E;
                leftHandObj = lhswt_W;
            }
            else if (avatarSettings.steeringTargets == SteeringTargets.All)
            {
                /*
                if (targetY >= 0.0f && targetY <= 90.0f || targetY >= 360f && targetY <= 450f)
                {
                    leftHandObj = lhswt_W;
                    rightHandObj = rhswt_E;
                }
                else if (targetY >= 90.0f && targetY <= 180.0f || targetY >= 450 && targetY <= 540f)
                {
                    leftHandObj = lhswt_N;
                    rightHandObj = rhswt_SW;
                }
                else if (targetY >= 180.0f && targetY <= 270.0f || targetY >= 540f && targetY <= 630f)
                {
                    leftHandObj = lhswt_E;
                    rightHandObj = rhswt_NW;
                }
                else if (targetY >= 270f && targetY <= 360f || targetY >= 630f && targetY <= 730f)
                {
                    leftHandObj = lhswt_S;
                    rightHandObj = rhswt_N;
                }

                else if (targetY >= -90.0f && targetY <= 0.0f || targetY >= -450.0f && targetY <= -360.0f)
                {
                    leftHandObj = lhswt_W;
                    rightHandObj = rhswt_E;
                }
                else if (targetY >= -180.0f && targetY <= -90.0f || targetY >= -540.0f && targetY <= -450.0f)
                {
                    leftHandObj = lhswt_S;
                    rightHandObj = rhswt_N;
                }
                else if (targetY >= -270.0f && targetY <= -180.0f || targetY >= -630.0f && targetY <= -540.0f)
                {
                    leftHandObj = lhswt_E;
                    rightHandObj = rhswt_W;
                }
                else if (targetY >= -360.0f && targetY <= -270.0f || targetY >= -730.0f && targetY <= -630.0f)
                {
                    leftHandObj = lhswt_N;
                    rightHandObj = rhswt_S;
                }*/
                leftHandObj = lhswt_W;
                rightHandObj = rhswt_E;
            }
            

            lookTargetPosX = horizontalInput > 0 ? avatarSettings.defaultLookXPos + avatarSettings.maxLookRight :
                horizontalInput < 0 ? avatarSettings.defaultLookXPos + avatarSettings.maxLookLeft :
                avatarSettings.defaultLookXPos;
            lookObjMoveSpeed = horizontalInput > 0 || horizontalInput < 0 ? avatarSettings.minLookSpeed :
                Mathf.Approximately(lookPosition.x, lookTargetPosX) ? avatarSettings.minLookSpeed :
                Mathf.Lerp(lookObjMoveSpeed, avatarSettings.maxLookSpeed, deltaTime);
            rotateSpeeds = horizontalInput > 0 || horizontalInput < 0 ? 0.1f :
                Mathf.Lerp(rotateSpeeds, avatarSettings.maxRotateSpeeds, deltaTime);
        }

        void UpdateFootTransforms()
        {
            if (verticalInput > 0)
            {
                if (avatarSettings.gasFoot == TargetSide.Right)
                {
                    rightFootObj = footGas;
                }
                else if (avatarSettings.gasFoot == TargetSide.Left)
                {
                    rightFootObj = rightFootIdle;
                    leftFootObj = footGas;
                }
            }
            else if (verticalInput < 0)
            {
                if (gearString == "R")
                {   //Reversing	//Set Foot Gas					
                    if (avatarSettings.gasFoot == TargetSide.Right)
                    {
                        rightFootObj = footGas;
                    }
                    else if (avatarSettings.gasFoot == TargetSide.Left)
                    {
                        rightFootObj = rightFootIdle;
                        leftFootObj = footGas;
                    }
                }
                else
                {   //Braking	//Set Foot Brake					
                    if (avatarSettings.brakeFoot == TargetSide.Right)
                    {
                        rightFootObj = footBrake;
                    }
                    else if (avatarSettings.brakeFoot == TargetSide.Left)
                    {
                        leftFootObj = footBrake;
                    }
                }
            }
            else
            {
                rightFootObj = rightFootIdle;
            }
        }

        void UpdateIKTargetTransforms(string type)
        {
            targetRightFootIK.localPosition = Vector3.Lerp(targetRightFootIK.localPosition, rightFootObj.localPosition, 8 * deltaTime);
            targetRightFootIK.localRotation = Quaternion.Lerp(targetRightFootIK.localRotation, rightFootObj.localRotation, 8 * deltaTime);
            targetLeftFootIK.localPosition = Vector3.Lerp(targetLeftFootIK.localPosition, leftFootObj.localPosition, 8 * deltaTime);
            targetLeftFootIK.localRotation = Quaternion.Lerp(targetLeftFootIK.localRotation, leftFootObj.localRotation, 8 * deltaTime);

            switch(type){
                case "shifting":
                    if (avatarSettings.shiftHand == TargetSide.Right)
                    {
                        if (shifting)
                        {
                            shiftLerp += avatarSettings.shiftAnimSpeed * deltaTime;
                            if (shiftLerp <= 0.4f)
                            {
                                targetRightHandIK.SetPositionAndRotation(
                                    Vector3.Lerp(targetRightHandIK.position, returnShiftTarget.position, LinearDistance(0.0f, 0.4f, shiftLerp)),
                                    Quaternion.Lerp(shiftStartRotation, returnShiftTarget.rotation, LinearDistance(0.0f, 0.4f, shiftLerp))
                                    );
                            }
                            else if (shiftLerp >= 0.85f)
                            {
                                targetRightHandIK = handShift;
                                holdShiftTimer += deltaTime;
                                if (holdShiftTimer >= avatarSettings.holdShiftTime)
                                {
                                    shifting = false;
                                    shiftBackTimer = 0.0f;
                                    shiftLerp = 0.0f;
                                }
                            }
                            else
                            {
                                targetRightHandIK.SetPositionAndRotation(
                                    Vector3.Lerp(returnShiftTarget.position, handShift.position, avatarSettings.shiftCurve.Evaluate(shiftLerp)),
                                    Quaternion.Lerp(shiftStartRotation, handShift.rotation, avatarSettings.shiftCurve.Evaluate(shiftLerp))
                                    );
                            }
                        }
                        else
                        {
                            if (shiftBackTimer <= avatarSettings.shiftBackTime)
                            {
                                targetRightHandIK = wheelCP;
                                shiftLerp += avatarSettings.shiftAnimSpeed * deltaTime;
                                shiftBackTimer += deltaTime;
                                targetRightHandIK.SetPositionAndRotation(Vector3.Lerp(
                                    handShift.position, returnShiftTarget.position, LinearDistance(0.0f, avatarSettings.shiftBackTime, shiftBackTimer)),
                                    Quaternion.Lerp(handShift.rotation, returnShiftTarget.rotation, LinearDistance(0.0f, avatarSettings.shiftBackTime, shiftBackTimer))
                                    );
                            }
                            else
                            {
                                targetingWheel = true;
                                targetRightHandIK.localPosition = Vector3.Lerp(targetRightHandIK.localPosition, rightHandObj.localPosition, avatarSettings.IKSpeed);
                                targetRightHandIK.localRotation = Quaternion.Lerp(targetRightHandIK.localRotation, rightHandObj.localRotation, 1);
                            }
                        }
                        targetLeftHandIK.localPosition = Vector3.Slerp(targetLeftHandIK.localPosition, leftHandObj.localPosition, avatarSettings.IKSpeed);
                        targetLeftHandIK.localRotation = Quaternion.Lerp(targetLeftHandIK.localRotation, leftHandObj.localRotation, 1);
                    }
                    else if (avatarSettings.shiftHand == TargetSide.Left)
                    {
                        if (shifting)
                        {
                            shiftLerp += avatarSettings.shiftAnimSpeed * deltaTime;
                            if (shiftLerp <= 0.4f)
                            {
                                targetLeftHandIK.SetPositionAndRotation(
                                    Vector3.Lerp(targetLeftHandIK.position, returnShiftTarget.position, LinearDistance(0.0f, 0.4f, shiftLerp)),
                                    Quaternion.Lerp(shiftStartRotation, returnShiftTarget.rotation, LinearDistance(0.0f, 0.4f, shiftLerp))
                                    );
                            }
                            else if (shiftLerp >= 0.85f)
                            {
                                targetLeftHandIK = handShift;
                                holdShiftTimer += deltaTime;
                                if (holdShiftTimer >= avatarSettings.holdShiftTime)
                                {
                                    shifting = false;
                                    shiftBackTimer = 0.0f;
                                    shiftLerp = 0.0f;
                                }
                            }
                            else
                            {
                                targetLeftHandIK.SetPositionAndRotation(
                                    Vector3.Lerp(returnShiftTarget.position, handShift.position, avatarSettings.shiftCurve.Evaluate(shiftLerp)),
                                    Quaternion.Lerp(shiftStartRotation, handShift.rotation, avatarSettings.shiftCurve.Evaluate(shiftLerp))
                                    );
                            }
                        }
                        else
                        {
                            if (shiftBackTimer <= avatarSettings.shiftBackTime)
                            {
                                targetLeftHandIK = wheelCP;
                                shiftLerp += avatarSettings.shiftAnimSpeed * deltaTime;
                                shiftBackTimer += deltaTime;
                                targetLeftHandIK.SetPositionAndRotation(
                                    Vector3.Lerp(handShift.position, returnShiftTarget.position, LinearDistance(0.0f, avatarSettings.shiftBackTime, shiftBackTimer)),
                                    Quaternion.Lerp(handShift.rotation, returnShiftTarget.rotation, LinearDistance(0.0f, avatarSettings.shiftBackTime, shiftBackTimer))
                                    );
                            }
                            else
                            {
                                targetingWheel = true;
                                targetLeftHandIK.localPosition = Vector3.Lerp(targetLeftHandIK.localPosition, leftHandObj.localPosition, avatarSettings.IKSpeed);
                                targetLeftHandIK.localRotation = Quaternion.Lerp(targetLeftHandIK.localRotation, leftHandObj.localRotation, 1);
                            }
                        }
                        targetRightHandIK.localPosition = Vector3.Slerp(targetRightHandIK.localPosition, rightHandObj.localPosition, avatarSettings.IKSpeed);
                        targetRightHandIK.localRotation = Quaternion.Lerp(targetRightHandIK.localRotation, rightHandObj.localRotation, 1);
                    }
                    break;

                case "PickBeer":
                    targetLeftHandIK.localPosition = Vector3.Slerp(targetLeftHandIK.localPosition, leftHandObj.localPosition, avatarSettings.IKSpeed);
                    targetLeftHandIK.localRotation = Quaternion.Lerp(targetLeftHandIK.localRotation, leftHandObj.localRotation, 1);

                    switch (step) {
                        case 0:
                            if(!init){
                                objOriginalPosition = Beer.transform.localPosition;
                                objOriginalRotation = Beer.transform.localRotation;
                                init = true;
                            }
                            angle = Quaternion.Angle(targetRightHandIK.localRotation, Object_A.localRotation);
                            dist = Vector3.Distance(targetRightHandIK.localPosition, Object_A.localPosition);
                            timeCount += Time.deltaTime;
                            UpdateIKLook("PickBeer", timeCount);

                            if (angle > threthold && dist > threthold){
                                targetRightHandIK.localPosition = Vector3.Lerp(targetRightHandIK.localPosition, Object_A.localPosition, timeCount * speed);
                                targetRightHandIK.localRotation = Quaternion.Lerp(targetRightHandIK.localRotation, Object_A.localRotation, timeCount * speed);
                            }
                            else{
                                OnPickUp(Beer, Hand);
                                UpdateIKLook("PickBeer", 0.5f);
                                step++;
                                init = false;
                                timeCount = 0;
                            } 

                            break;

                        case 1: 
                            angle = Quaternion.Angle(targetRightHandIK.rotation, Beer_Mouth.rotation);
                            dist = Vector3.Distance(targetRightHandIK.position, Beer_Mouth.position);
                            timeCount += Time.deltaTime;
                            UpdateIKLook("normal", 1);

                            if (angle > threthold && dist > threthold){
                                targetRightHandIK.position = Vector3.Lerp(targetRightHandIK.position, Beer_Mouth.position, timeCount * speed);
                                targetRightHandIK.rotation = Quaternion.Lerp(targetRightHandIK.rotation, Beer_Mouth.rotation, timeCount * speed);   
                            }
                            else{
                                time += Time.deltaTime;
                                behaviorState[0] = true;
                                if (time > holdingTime[0]){
                                    step++;
                                    time = 0;
                                    timeCount = 0;
                                }
                            }
                            break;

                        case 2:
                            angle = Quaternion.Angle(targetRightHandIK.localRotation, Object_A.localRotation);
                            dist = Vector3.Distance(targetRightHandIK.localPosition, Object_A.localPosition);
                            UpdateIKLook("normal", 1);
                            timeCount += Time.deltaTime;
                            behaviorState[0] = false;
                            if (angle > threthold && dist > threthold){
                                targetRightHandIK.localPosition = Vector3.Lerp(targetRightHandIK.localPosition, Object_A.localPosition, timeCount * speed);
                                targetRightHandIK.localRotation = Quaternion.Lerp(targetRightHandIK.localRotation, Object_A.localRotation, timeCount * speed);
                            }
                            else{
                                OnDrop(Beer, Object_pA);
                                UpdateIKLook("normal", 1);
                                step++;
                                timeCount = 0;
                            }
                            break;

                        case 3:
                            angle = Quaternion.Angle(targetRightHandIK.localRotation, handTempIK.localRotation);
                            dist = Vector3.Distance(targetRightHandIK.localPosition, handTempIK.localPosition);
                            UpdateIKLook("normal", 1);
                            timeCount += Time.deltaTime;

                            if (angle > threthold && dist > threthold){
                                targetRightHandIK.localPosition = Vector3.Lerp(targetRightHandIK.localPosition, handTempIK.localPosition, timeCount * speed);
                                targetRightHandIK.localRotation = Quaternion.Lerp(targetRightHandIK.localRotation, handTempIK.localRotation, timeCount * speed);
                            }
                            else{ 
                                Pack(targetRightHandIK, steeringWheel);
                                step = 0;
                                timeCount = 0;
                                get = true;
                            }
                            break;
                    }
                    break;

                case "PickCigarette":
                    targetLeftHandIK.localPosition = Vector3.Slerp(targetLeftHandIK.localPosition, leftHandObj.localPosition, avatarSettings.IKSpeed);
                    targetLeftHandIK.localRotation = Quaternion.Lerp(targetLeftHandIK.localRotation, leftHandObj.localRotation, 1);

                    switch (step) {
                        case 0:
                            if(!init){
                                objOriginalPosition = Cigarette.transform.localPosition;
                                objOriginalRotation = Cigarette.transform.localRotation;
                                init = true;
                            }
                            angle = Quaternion.Angle(targetRightHandIK.localRotation, Object_D.localRotation);
                            dist = Vector3.Distance(targetRightHandIK.localPosition, Object_D.localPosition);
                            timeCount += Time.deltaTime;
                            UpdateIKLook("PickCigarette", timeCount);
                            
                            if (angle > threthold && dist > threthold){
                                targetRightHandIK.localPosition = Vector3.Lerp(targetRightHandIK.localPosition, Object_D.localPosition, timeCount * speed);
                                targetRightHandIK.localRotation = Quaternion.Lerp(targetRightHandIK.localRotation, Object_D.localRotation, timeCount * speed);  
                            }
                            else{
                                OnPickUp(Cigarette, Finger); 
                                UpdateIKLook("PickCigarette", 0.5f);
                                init = false;
                                step++;
                                timeCount = 0;
                            }
                            break;

                        case 1: 
                            angle = Quaternion.Angle(targetRightHandIK.rotation, Mouth.rotation);
                            dist = Vector3.Distance(targetRightHandIK.position, Mouth.position);
                            UpdateIKLook("normal", 1);
                            timeCount += Time.deltaTime;

                            if (angle > threthold && dist > threthold){
                                targetRightHandIK.position = Vector3.Lerp(targetRightHandIK.position, Mouth.position, timeCount * speed);
                                targetRightHandIK.rotation = Quaternion.Lerp(targetRightHandIK.rotation, Mouth.rotation, timeCount * speed); 
                            }
                            else{
                                behaviorState[1] = true;
                                time += Time.deltaTime;
                                if(time > holdingTime[1]){
                                    step++;
                                    time = 0;
                                    timeCount = 0;
                                } 
                            }
                            break;

                        case 2:
                            angle = Quaternion.Angle(targetRightHandIK.localRotation, Object_D.localRotation);
                            dist = Vector3.Distance(targetRightHandIK.localPosition, Object_D.localPosition);
                            timeCount += Time.deltaTime;
                            UpdateIKLook("normal", 1);
                            behaviorState[1] = false;

                            if (angle > threthold && dist > threthold){
                                targetRightHandIK.localPosition = Vector3.Lerp(targetRightHandIK.localPosition, Object_D.localPosition, timeCount * speed);
                                targetRightHandIK.localRotation = Quaternion.Lerp(targetRightHandIK.localRotation, Object_D.localRotation, timeCount * speed); 
                            }
                            else{
                                OnDrop(Cigarette, Object_pD);
                                UpdateIKLook("normal", 1);
                                step++;
                                timeCount = 0;
                            }
                            break;

                        case 3:
                            angle = Quaternion.Angle(targetRightHandIK.localRotation, handTempIK.localRotation);
                            dist = Vector3.Distance(targetRightHandIK.localPosition, handTempIK.localPosition);
                            timeCount += Time.deltaTime;
                            UpdateIKLook("normal", 1);

                            if (angle > threthold && dist > threthold){
                                targetRightHandIK.localPosition = Vector3.Lerp(targetRightHandIK.localPosition, handTempIK.localPosition, timeCount * speed);
                                targetRightHandIK.localRotation = Quaternion.Lerp(targetRightHandIK.localRotation, handTempIK.localRotation, timeCount * speed);
                            }
                            else{ 
                                step = 0;
                                timeCount = 0;
                                get = true;
                            }
                            break;
                    }
                    break;

                case "PickPhone":
                    targetLeftHandIK.localPosition = Vector3.Slerp(targetLeftHandIK.localPosition, leftHandObj.localPosition, avatarSettings.IKSpeed);
                    targetLeftHandIK.localRotation = Quaternion.Lerp(targetLeftHandIK.localRotation, leftHandObj.localRotation, 1);

                    switch (step) {
                        case 0:
                            if(!init){
                                objOriginalPosition = Phone.transform.localPosition;
                                objOriginalRotation = Phone.transform.localRotation;
                                init = true;
                            }
                            angle = Quaternion.Angle(targetRightHandIK.localRotation, Object_B.localRotation);
                            dist = Vector3.Distance(targetRightHandIK.localPosition, Object_B.localPosition);
                            timeCount += Time.deltaTime;
                            UpdateIKLook("PickPhone", timeCount);

                            if (angle > threthold && dist > threthold){
                                targetRightHandIK.localPosition = Vector3.Lerp(targetRightHandIK.localPosition, Object_B.localPosition, timeCount * speed);
                                targetRightHandIK.localRotation = Quaternion.Lerp(targetRightHandIK.localRotation, Object_B.localRotation, timeCount * speed);                  
                            }
                            else{
                                OnPickUp(Phone, phoneHand);
                                UpdateIKLook("PickPhone", 0.5f);
                                init = false;
                                step++;
                                timeCount = 0;
                            }

                            break;

                        case 1: 
                            angle = Quaternion.Angle(targetRightHandIK.rotation, Face.rotation);
                            dist = Vector3.Distance(targetRightHandIK.position, Face.position);
                            timeCount += Time.deltaTime;
                            UpdateIKLook("normal", 1);

                            if (angle > threthold && dist > threthold){
                                targetRightHandIK.position = Vector3.Lerp(targetRightHandIK.position, Face.position, timeCount * speed);
                                targetRightHandIK.rotation = Quaternion.Lerp(targetRightHandIK.rotation, Face.rotation, timeCount * speed);
                            }
                            else{
                                behaviorState[2] = true;
                                time += Time.deltaTime;
                                if (time > holdingTime[2]){
                                    step++;
                                    time = 0;
                                    timeCount = 0;
                                }
                            }
                            break;

                        case 2:
                            angle = Quaternion.Angle(targetRightHandIK.localRotation, Object_B.localRotation);
                            dist = Vector3.Distance(targetRightHandIK.localPosition, Object_B.localPosition);
                            timeCount += Time.deltaTime;
                            UpdateIKLook("normal", 1);
                            behaviorState[2] = false;

                            if (angle > threthold && dist > threthold){
                                targetRightHandIK.localPosition = Vector3.Lerp(targetRightHandIK.localPosition, Object_B.localPosition, timeCount * speed);
                                targetRightHandIK.localRotation = Quaternion.Lerp(targetRightHandIK.localRotation, Object_B.localRotation, timeCount * speed);    
                            }
                            else{
                                OnDrop(Phone, Object_pB);
                                UpdateIKLook("normal", 1);
                                step++;
                                timeCount = 0;
                            }
                            break;

                        case 3:
                            angle = Quaternion.Angle(targetRightHandIK.localRotation, handTempIK.localRotation);
                            dist = Vector3.Distance(targetRightHandIK.localPosition, handTempIK.localPosition);
                            timeCount += Time.deltaTime;
                            UpdateIKLook("normal", 1);

                            if (angle > threthold && dist > threthold){
                                targetRightHandIK.localPosition = Vector3.Lerp(targetRightHandIK.localPosition, handTempIK.localPosition, timeCount * speed);
                                targetRightHandIK.localRotation = Quaternion.Lerp(targetRightHandIK.localRotation, handTempIK.localRotation, timeCount * speed);   
                            }
                            else{ 
                                step = 0;
                                timeCount = 0;
                                get = true;
                            }
                            break;
                    }
                    break;
                
                case "WearGlasses":
                    targetLeftHandIK.localPosition = Vector3.Slerp(targetLeftHandIK.localPosition, leftHandObj.localPosition, avatarSettings.IKSpeed);
                    targetLeftHandIK.localRotation = Quaternion.Lerp(targetLeftHandIK.localRotation, leftHandObj.localRotation, 1);

                    switch (step) {
                        case 0:
                            angle = Quaternion.Angle(targetRightHandIK.localRotation, Center.localRotation);
                            dist = Vector3.Distance(targetRightHandIK.localPosition, Center.localPosition);
                            timeCount += Time.deltaTime;

                            if (angle > threthold && dist > threthold){
                                targetRightHandIK.localPosition = Vector3.Lerp(targetRightHandIK.localPosition, Center.localPosition, timeCount * speed);
                                targetRightHandIK.localRotation = Quaternion.Lerp(targetRightHandIK.localRotation, Center.localRotation, timeCount * speed);
                                if (timeCount < 0.5f){
                                    UpdateIKLook("Glasses", 2*timeCount);
                                }
                                else{
                                    UpdateIKLook("normal", 1);
                                }
                            }
                            else{
                                step++;
                                timeCount = 0;
                            } 
                            break;
                        case 1: 
                            OnPickUp(Glasses, Finger);
                            UpdateIKLook("normal", 1);
                            step++;
                            break;

                        case 2: 
                            angle = Quaternion.Angle(targetRightHandIK.localRotation, buffer_HeadRotation);
                            dist = Vector3.Distance(targetRightHandIK.localPosition, buffer_HeadPosition);
                            timeCount += Time.deltaTime;
                            UpdateIKLook("normal", 1);

                            if (angle > threthold && dist > threthold){
                                targetRightHandIK.localPosition = Vector3.Lerp(targetRightHandIK.localPosition, buffer_HeadPosition, timeCount * speed);
                                targetRightHandIK.localRotation = Quaternion.Lerp(targetRightHandIK.localRotation, buffer_HeadRotation, timeCount * speed);    
                            }
                            else{
                                step++;
                                timeCount = 0;
                            }
                            break;

                        case 3:
                            Glasses.transform.SetParent(Head);
                            Glasses.transform.localPosition = buffer_OriginalPosition;
                            Glasses.transform.localRotation = buffer_OriginalRotation;
                            UpdateIKLook("normal", 1);
                            step++;
                            break;

                        case 4:
                            angle = Quaternion.Angle(targetRightHandIK.localRotation, handTempIK.localRotation);
                            dist = Vector3.Distance(targetRightHandIK.localPosition, handTempIK.localPosition);
                            timeCount += Time.deltaTime;
                            UpdateIKLook("normal", 1);

                            if (angle > threthold && dist > threthold){
                                targetRightHandIK.localPosition = Vector3.Lerp(targetRightHandIK.localPosition, handTempIK.localPosition, timeCount * speed);
                                targetRightHandIK.localRotation = Quaternion.Lerp(targetRightHandIK.localRotation, handTempIK.localRotation, timeCount * speed); 
                            }
                            else{ 
                                step = 0;
                                timeCount = 0;
                                get = true;
                            }
                            break;
                    }
                    break;

                case "TkoGlasses":
                    targetLeftHandIK.localPosition = Vector3.Slerp(targetLeftHandIK.localPosition, leftHandObj.localPosition, avatarSettings.IKSpeed);
                    targetLeftHandIK.localRotation = Quaternion.Lerp(targetLeftHandIK.localRotation, leftHandObj.localRotation, 1);

                    switch (step) {
                        case 0:
                            if(!init){
                                buffer_OriginalPosition = Glasses.transform.localPosition;
                                buffer_OriginalRotation = Glasses.transform.localRotation;
                                buffer_HeadPosition = Object_C.localPosition;
                                buffer_HeadRotation = Object_C.localRotation;
                                init = true;
                            }
                            angle = Quaternion.Angle(targetRightHandIK.localRotation, Object_C.localRotation);
                            dist = Vector3.Distance(targetRightHandIK.localPosition, Object_C.localPosition);
                            timeCount += Time.deltaTime;
                            UpdateIKLook("normal", 1);

                            if (angle > threthold && dist > threthold){
                                targetRightHandIK.localPosition = Vector3.Lerp(targetRightHandIK.localPosition, Object_C.localPosition, timeCount * speed);
                                targetRightHandIK.localRotation = Quaternion.Lerp(targetRightHandIK.localRotation, Object_C.localRotation, timeCount * speed);
                            }
                            else{
                                step++;
                                init = false;
                                timeCount = 0;
                            } 
                            break;
                        case 1: 
                            OnPickUp(Glasses, Finger);
                            UpdateIKLook("normal", 1);
                            step++;
                            break;

                        case 2: 
                            angle = Quaternion.Angle(targetRightHandIK.localRotation, Center.localRotation);
                            dist = Vector3.Distance(targetRightHandIK.localPosition, Center.localPosition);
                            timeCount += Time.deltaTime;

                            if (angle > threthold && dist > threthold){
                                targetRightHandIK.localPosition = Vector3.Lerp(targetRightHandIK.localPosition, Center.localPosition, timeCount * speed);
                                targetRightHandIK.localRotation = Quaternion.Lerp(targetRightHandIK.localRotation, Center.localRotation, timeCount * speed);
                                if (timeCount < 0.5f){
                                    UpdateIKLook("Glasses", 2*timeCount);
                                }
                                else{
                                    UpdateIKLook("normal", 1);
                                }
                            }
                            else{
                                step++;
                                timeCount = 0;
                            }
                            break;

                        case 3:
                            Glasses.transform.SetParent(Object_pC);
                            UpdateIKLook("normal", 1);
                            step++;
                            break;

                        case 4:
                            angle = Quaternion.Angle(targetRightHandIK.localRotation, handTempIK.localRotation);
                            dist = Vector3.Distance(targetRightHandIK.localPosition, handTempIK.localPosition);
                            timeCount += Time.deltaTime;
                            UpdateIKLook("normal", 1);

                            if (angle > threthold && dist > threthold){
                                targetRightHandIK.localPosition = Vector3.Lerp(targetRightHandIK.localPosition, handTempIK.localPosition, timeCount * speed);
                                targetRightHandIK.localRotation = Quaternion.Lerp(targetRightHandIK.localRotation, handTempIK.localRotation, timeCount * speed);  
                            }
                            else{ 
                                step = 0;
                                timeCount = 0;
                                get = true;
                            }
                            break;
                    }
                    break;
                    
                case "Gesture":
                    if (!init){
                        for (int i = 0; i < 5; i++) {
                            for(int j = 0; j < 3; j++){
                                finger_rotation[i,j] = finger_list[i,j].localEulerAngles;
                            } 
                        }
                        init = true;
                    }
                    //print("Avatar: " + Gesture.localPosition.ToString() + "," + Gesture.localRotation.ToString());
                    float angular = 5.0f;
                    switch (step) {
                        case 0:
                            angle = Quaternion.Angle(targetRightHandIK.localRotation, gestureTargetList[gestureIndex].localRotation);
                            dist = Vector3.Distance(targetRightHandIK.localPosition, gestureTargetList[gestureIndex].localPosition);
                            timeCount += Time.deltaTime;
                            UpdateIKLook("normal", 1);

                            if (angle > threthold && dist > threthold){
                                targetRightHandIK.localPosition = Vector3.Lerp(targetRightHandIK.localPosition, gestureTargetList[gestureIndex].localPosition, timeCount * speed);
                                targetRightHandIK.localRotation = Quaternion.Lerp(targetRightHandIK.localRotation, gestureTargetList[gestureIndex].localRotation, timeCount * speed); 
                            }
                            else{
                                behaviorState[3] = true;
                                for (int i = 1; i < 3; i++) {
                                    if (!gate[i]){
                                        float angle_dist = tuneAngle(finger_list[finger, i].localEulerAngles.z);
                                        if (angle_dist > 0){
                                            finger_list[finger, i].localEulerAngles += new Vector3(0, 0, angular);
                                            if (finger_list[finger, i].localEulerAngles.z < angular && finger_list[finger, i].localEulerAngles.z > -angular){
                                                gate[i] = true;
                                            }
                                        }
                                        else if (angle_dist < 0){
                                            finger_list[finger, i].localEulerAngles -= new Vector3(0, 0, angular);
                                            if (finger_list[finger, i].localEulerAngles.z < angular && finger_list[finger, i].localEulerAngles.z > -angular){
                                                gate[i] = true;
                                            }
                                        }
                                    }    
                                }
                                if (gate[1] && gate[2]){
                                    finger ++ ;
                                    gate[1] = gate[2] = false;
                                }
                                if (finger > 4){
                                    gestureIndex++;
                                    if (gestureIndex == gestureIndex_max){
                                        gestureIndex = 0;
                                    }
                                    step++;
                                    finger = 4;
                                    timeCount = 0;
                                }
                            }
                            break;

                        case 1:
                            angle = Quaternion.Angle(targetRightHandIK.localRotation, gestureTargetList[gestureIndex].localRotation);
                            dist = Vector3.Distance(targetRightHandIK.localPosition, gestureTargetList[gestureIndex].localPosition);
                            timeCount += Time.deltaTime;
                            UpdateIKLook("normal", 1);
                            behaviorState[3] = true;

                            if (angle > threthold && dist > threthold){
                                targetRightHandIK.localPosition = Vector3.Lerp(targetRightHandIK.localPosition, gestureTargetList[gestureIndex].localPosition, timeCount * speed);
                                targetRightHandIK.localRotation = Quaternion.Lerp(targetRightHandIK.localRotation, gestureTargetList[gestureIndex].localRotation, timeCount * speed); 
                            }
                            else{
                                for (int i = 1; i < 3; i++) {
                                    if (!gate[i]){
                                        float angle_dist = finger_list[finger, i].localEulerAngles.z - finger_rotation[finger, i].z;
                                        angle_dist = tuneAngle(angle_dist);
                                        if (angle_dist > 0){
                                            finger_list[finger, i].localEulerAngles -= new Vector3(0, 0, angular);
                                            if (finger_list[finger, i].localEulerAngles.z < (finger_rotation[finger, i].z + angular) && finger_list[finger, i].localEulerAngles.z > (finger_rotation[finger, i].z - angular)){
                                                gate[i] = true;
                                            }
                                        }
                                        else if (angle_dist < 0){
                                            finger_list[finger, i].localEulerAngles += new Vector3(0, 0, angular);
                                            if ( finger_list[finger, i].localEulerAngles.z < (finger_rotation[finger, i].z + angular) && finger_list[finger, i].localEulerAngles.z > (finger_rotation[finger, i].z -angular)){
                                                gate[i] = true;
                                            }
                                        }
                                    }
                                }
                                if (gate[1] && gate[2]){
                                        finger -- ;
                                        gate[1] = gate[2] = false;
                                    }
                                if (finger < 0){
                                    gestureIndex ++ ;
                                    if(gestureIndex == gestureIndex_max){
                                        gestureIndex = 0;
                                    }
                                    step++;
                                    finger = 0;
                                    timeCount = 0;
                                }
                            }
                            
                            break;

                        case 2:
                            angle = Quaternion.Angle(targetRightHandIK.localRotation, handTempIK.localRotation);
                            dist = Vector3.Distance(targetRightHandIK.localPosition, handTempIK.localPosition);
                            timeCount += Time.deltaTime;
                            UpdateIKLook("normal", 1);
                            behaviorState[3] = false;

                            if (angle > threthold && dist > threthold){
                                targetRightHandIK.localPosition = Vector3.Lerp(targetRightHandIK.localPosition, handTempIK.localPosition, timeCount * speed);
                                targetRightHandIK.localRotation = Quaternion.Lerp(targetRightHandIK.localRotation, handTempIK.localRotation, timeCount * speed); 
                            }
                            else{
                                step = 0;
                                timeCount = 0;
                                get = true;
                                init = false;
                            }
                            break;
                    }
                    break;
            }
            lookPosition = headLookIKCP.localPosition;
            lookPosition.x = Mathf.Lerp(lookPosition.x, lookTargetPosX, lookObjMoveSpeed * deltaTime);
            headLookIKCP.localPosition = lookPosition;
        }

        public void Realistic(){
            Hip.localEulerAngles = original_hip + new Vector3(Random.Range(-1,1) * 0.1f, 0, Random.Range(-1,1) * 0.1f);
        }

        public float tuneAngle(float angle){
            while(true){
                if (angle > 360){
                    angle -= 360;
                }
                else if(angle < -360){
                    angle += 360;
                }
                else{
                    break;
                }
            }
            if (angle > 180 && angle < 360){
                angle = 360 - angle;
            }
            else if(angle < -180 && angle > -360){
                angle += 360;
            }

            return angle;  
        }

        public void TargetShifter()
        {
            if (!shifting && targetingWheel)
            {
                targetingWheel = false;
                shifting = true;
                shiftLerp = 0.0f;
                holdShiftTimer = 0.0f;
                if (avatarSettings.shiftHand == TargetSide.Left)
                {
                    shiftStartRotation = targetLeftHandIK.rotation;
                }
                else if (avatarSettings.shiftHand == TargetSide.Right)
                {
                    shiftStartRotation = targetRightHandIK.rotation;
                }
                if (avatarSettings.clutchFoot == TargetSide.Left)
                {
                    leftFootObj = footClutch;
                }
                else if (avatarSettings.clutchFoot == TargetSide.Right)
                {
                    rightFootObj = footClutch;
                }
                Invoke(nameof(SetClutchFootIdle), 0.5f);
            }
        }

        public void SetClutchFootIdle()
        {
            if (avatarSettings.clutchFoot == TargetSide.Left)
            {
                leftFootObj = leftFootIdle;
            }
            else if (avatarSettings.clutchFoot == TargetSide.Right)
            {
                rightFootObj = rightFootIdle;
            }
        }

        float LinearDistance(float _start, float _end, float _position)
        {
            return Mathf.InverseLerp(_start, _end, _position);
        }

    }
}