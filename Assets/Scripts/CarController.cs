using System;
using UnityEngine;
using UnityEngine.UI;
enum CarDriveType //驱动方式
{
    FrontWheelDrive,//前驱
    BackWheelDrive,//后驱
    FourWheelDrive//四驱
}

enum SpeedType
{
    MPH,//英里每小时
    KPH//公里每小时
}


public class CarController : MonoBehaviour
{
    public Text SpeedText;
    public  int caraSpeed;
    [SerializeField]
    private Transform Velocitypointer;//速度指针
    [SerializeField]
    private Transform RPMpointer;//油门指针
    [Header("不需要赋值")]
    public Camera currentCamera;//当前视角摄像机
    private int viewIndex = 0;
    [SerializeField]
    private Camera inCamera;//驾驶位视角
    [SerializeField]
    private Camera outCamera;//第三人称视角
    [SerializeField]
    private MeshRenderer steeringWheel;//方向盘
    [SerializeField]
    private float steeringWheelMaxAngle;//方向盘旋转最大角度
    [SerializeField]
    private CarDriveType m_CarDriveType = CarDriveType.FourWheelDrive;//默认驱动方式为四驱
    [SerializeField]
    private WheelCollider[] m_WheelColliders = new WheelCollider[4];//得到汽车的四个车轮碰撞体
    [SerializeField]
    private GameObject[] m_WheelMeshes = new GameObject[4];//得到汽车的四个车轮模型
    [SerializeField]
    private Vector3 m_CentreOfMassOffset;//汽车重心
    [SerializeField]
    private float m_MaximumSteerAngle;//车轮最大转向角(前轮)
    [Range(0, 1)]
    [SerializeField]
    //0是原始物理，1汽车将抓向它所面对的方向
    private float m_SteerHelper; // 0 is raw physics , 1 the car will grip in the direction it is facing
    [Range(0, 1)]
    [SerializeField]
    //0为无牵引控制，1为全干扰
    private float m_TractionControl; // 0 is no traction control, 1 is full interference
    [SerializeField]
    //前进总扭矩(4个轮子一起：最大扭矩)
    private float m_FullTorqueOverAllWheels;
    [SerializeField]
    //倒车扭矩
    private float m_ReverseTorque;
    [SerializeField]
    //最大刹车扭矩
    private float m_MaxHandbrakeTorque;
    [SerializeField]
    //向下的力
    private float m_Downforce = 100f;
    [SerializeField]
    private SpeedType m_SpeedType;//速度类型
    [SerializeField]
    private float m_Topspeed = 200;//最高速度
    [SerializeField]
    //最大五个档位
    private static int NoOfGears = 5;
    [SerializeField]
    //发动机转速？
    private float m_RevRangeBoundary = 1f;
    [SerializeField]
    //滑移限制
    private float m_SlipLimit;
    [SerializeField]
    //刹车扭矩
    private float m_BrakeTorque;
    //车轮模型自身的旋转
    private Quaternion[] m_WheelMeshLocalRotations;
    private Vector3 m_Prevpos, m_Pos;
    private float m_SteerAngle;//车轮旋转角度
    private int m_GearNum;//当前档位
    private float m_GearFactor;//齿轮系数？
    private float m_OldRotation;//原始角度
    private float m_CurrentTorque;//当前扭矩
    private Rigidbody m_Rigidbody;//刚体组件
    private const float k_ReversingThreshold = 0.01f;//反转阈值?

    //public bool Skidding { get; private set; }
    public float BrakeInput { get; private set; } //手刹输入
    //public float CurrentSteerAngle { get { return m_SteerAngle; } }
    public float CurrentSpeed { get { return m_Rigidbody.velocity.magnitude * 2.23693629f; } }//当前速度？
    public float MaxSpeed { get { return m_Topspeed; } }//最大速度
    public float Revs { get; protected set; }//转速
    public float AccelInput { get; private set; }//加速输入

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Tab))
        {
            ChangeView();
        }
        SpeedText.text = (caraSpeed*2f).ToString()+"m/s";
    }
    private void Start()
    {
        currentCamera = inCamera;
        outCamera.enabled = false;
    //    steeringWheelMaxAngle = 270f;
        //m_WheelMeshLocalRotations = new Quaternion[4];//四个轮子的数组
        //for (int i = 0; i < 4; i++)
        //{
        //遍历四个轮子的自身旋转
        //  m_WheelMeshLocalRotations[i] = m_WheelMeshes[i].transform.localRotation;
        // }
        //整个车的重心都在第一个轮子上？
        m_WheelColliders[0].attachedRigidbody.centerOfMass = m_CentreOfMassOffset;

        m_MaxHandbrakeTorque = float.MaxValue;//最大刹车扭矩为单精度浮点型数据的最大值

        m_Rigidbody = GetComponent<Rigidbody>();
        //当前的扭矩 = 最大的前进扭矩 - (最大前进扭矩 * 牵引力分配比值)？
        m_CurrentTorque = m_FullTorqueOverAllWheels - (m_TractionControl * m_FullTorqueOverAllWheels);
    }

    //档位变化
    private void GearChanging()
    {
        //得到当前速度与最大速度的比值
        float f = Mathf.Abs(CurrentSpeed / MaxSpeed);
        //升档
        float upgearlimit = (1 / (float)NoOfGears) * (m_GearNum + 1);
        //降档
        float downgearlimit = (1 / (float)NoOfGears) * m_GearNum;
        if (m_GearNum > 0 && f < downgearlimit)
        {
            m_GearNum--;
        }
        
        if (f > upgearlimit && (m_GearNum < (NoOfGears - 1)))
        {
            m_GearNum++;
        }
        
    }


    // simple function to add a curved bias towards 1 for a value in the 0-1 range
    //为0-1范围内的值向1添加曲线偏移的简单函数
    private static float CurveFactor(float factor)
    {
        return 1 - (1 - factor) * (1 - factor);
    }


    // unclamped version of Lerp, to allow value to exceed the from-to range
    private static float ULerp(float from, float to, float value)
    {
        return (1.0f - value) * from + value * to;
    }

    //计算齿轮系数
    private void CalculateGearFactor()
    {
        float f = (1 / (float)NoOfGears);
        // gear factor is a normalised representation of the current speed within the current gear's range of speeds.
        // We smooth towards the 'target' gear factor, so that revs don't instantly snap up or down when changing gear.
        //齿轮系数是在当前齿轮的速度范围内对当前速度的规范表示。我们向“目标”齿轮系数方向平滑，这样，
        //在换档时，Revs就不会立即上下抢购。
        //计算档位比值  也就是即将加速还是减速
        var targetGearFactor = Mathf.InverseLerp(f * m_GearNum, f * (m_GearNum + 1), Mathf.Abs(CurrentSpeed / MaxSpeed));
        
        m_GearFactor = Mathf.Lerp(m_GearFactor, targetGearFactor, Time.deltaTime * 5f);
    }

    //计算转速
    private void CalculateRevs()
    {
        // calculate engine revs (for display / sound)
        // (this is done in retrospect - revs are not used in force/power calculations)
        CalculateGearFactor();
        var gearNumFactor = m_GearNum / (float)NoOfGears;
        var revsRangeMin = ULerp(0f, m_RevRangeBoundary, CurveFactor(gearNumFactor));
        var revsRangeMax = ULerp(m_RevRangeBoundary, 1f, gearNumFactor);
        Revs = ULerp(revsRangeMin, revsRangeMax, m_GearFactor);
    }

    bool braketorque = false;
    public void Move(float steering, float accel, float footbrake, float handbrake)
    {
        for (int i = 0; i < 4; i++)
        {
            Quaternion quat;
            Vector3 position;
            //得到车轮碰撞器的位置和转动角度；
            m_WheelColliders[i].GetWorldPose(out position, out quat);
            m_WheelMeshes[i].transform.position = position;
            m_WheelMeshes[i].transform.rotation = quat;
        }

        //clamp input values
        steering = Mathf.Clamp(steering, -1, 1);
        AccelInput = accel = Mathf.Clamp(accel, 0, 1);
        BrakeInput = footbrake = -1 * Mathf.Clamp(footbrake, -1, 0);
        handbrake = Mathf.Clamp(handbrake, 0, 1);

        //Set the steer on the front wheels.
        //把方向盘放在前轮上。
        //Assuming that wheels 0 and 1 are the front wheels.
        //假设0和1是前轮。
        m_SteerAngle = steering * m_MaximumSteerAngle*0.3f;
        m_WheelColliders[0].steerAngle = m_SteerAngle;
        m_WheelColliders[1].steerAngle = m_SteerAngle;
        //转弯辅助
       // SteerHelper();
        //这个地方就是车子能动的关键
        ApplyDrive(accel, footbrake);
        //方向盘
        SteeringWheelRotate(-steering);

        CapSpeed();

        //Set the handbrake.
        //设置手刹。
        //Assuming that wheels 2 and 3 are the rear wheels.
        //假设2号和3号是后轮。
        if (handbrake > 0f)
        {
            var hbTorque = m_MaxHandbrakeTorque;
            m_WheelColliders[0].brakeTorque = hbTorque;
            m_WheelColliders[1].brakeTorque = hbTorque;
            m_WheelColliders[2].brakeTorque = hbTorque;
            m_WheelColliders[3].brakeTorque = hbTorque;

            braketorque = true;
        }
        else if (braketorque)
        {
            m_WheelColliders[0].brakeTorque = 0f;
            m_WheelColliders[1].brakeTorque = 0f;
            m_WheelColliders[2].brakeTorque = 0f;
            m_WheelColliders[3].brakeTorque = 0f;
            braketorque = false;
        }

        //计算转速
        CalculateRevs();//转速不影响车速，只影响声音
        //档位改变
        GearChanging();
        //增加抓地力
        AddDownForce();
        //牵引力控制
        TractionControl();
    }

    //汽车速度
    private void CapSpeed()
    {
        float speed = m_Rigidbody.velocity.magnitude;
        caraSpeed = (int)speed;
        switch (m_SpeedType)
        {
            case SpeedType.MPH:

                speed *= 2.23693629f;
               // speed *= 1.5f;
                if (speed > m_Topspeed)
                    m_Rigidbody.velocity = (m_Topspeed / 2.23693629f) * m_Rigidbody.velocity.normalized;
                break;

            case SpeedType.KPH:
                speed *= 3.6f;
                if (speed > m_Topspeed)
                    m_Rigidbody.velocity = (m_Topspeed / 3.6f) * m_Rigidbody.velocity.normalized;
                break;
        }
    }

    //牵引力具体分配
    private void ApplyDrive(float accel, float footbrake)
    {
        //动力扭矩
        float thrustTorque;
        switch (m_CarDriveType)
        {
            case CarDriveType.FourWheelDrive:
                thrustTorque = accel * (m_CurrentTorque / 4f);//每个车轮的动力
                for (int i = 0; i < 4; i++)
                {
                    m_WheelColliders[i].motorTorque = thrustTorque;
                }
                break;

            case CarDriveType.FrontWheelDrive:
                thrustTorque = accel * (m_CurrentTorque / 2f);
                m_WheelColliders[0].motorTorque = m_WheelColliders[1].motorTorque = thrustTorque;
                break;

            case CarDriveType.BackWheelDrive:
                thrustTorque = accel * (m_CurrentTorque / 2f);
                m_WheelColliders[2].motorTorque = m_WheelColliders[3].motorTorque = thrustTorque;
                break;

        }

        for (int i = 0; i < 4; i++)
        {
            if (CurrentSpeed > 5 && Vector3.Angle(transform.forward, m_Rigidbody.velocity) < 50f)
            {
                m_WheelColliders[i].brakeTorque = m_BrakeTorque * footbrake;
            }
            //footbrake:脚刹大小
            else if (footbrake > 0)
            {
                m_WheelColliders[i].brakeTorque = 0f;//制动扭矩
                                    //动力扭矩
                m_WheelColliders[i].motorTorque = -m_ReverseTorque * footbrake;
            }
        }
    }

    //转弯辅助？
    private void SteerHelper()
    {
        for (int i = 0; i < 4; i++)
        {
            WheelHit wheelhit;
            m_WheelColliders[i].GetGroundHit(out wheelhit);
            if (wheelhit.normal == Vector3.zero)
                return;
            // wheels arent on the ground so dont realign the rigidbody velocity
            //轮子不在地上，所以不要调整刚体的速度
        }

        // this if is needed to avoid gimbal lock problems that will make the car suddenly shift direction
        //这如果是为了避免出现使汽车突然转向的齿轮锁问题
        if (Mathf.Abs(m_OldRotation - transform.eulerAngles.y) < 10f)
        {
            var turnadjust = (transform.eulerAngles.y - m_OldRotation) * m_SteerHelper;
            Quaternion velRotation = Quaternion.AngleAxis(turnadjust, Vector3.up);
            m_Rigidbody.velocity = velRotation * m_Rigidbody.velocity;
        }
        m_OldRotation = transform.eulerAngles.y;
    }


    // this is used to add more grip in relation to speed
    //这是用来增加更多的抓地力相对于速度
    private void AddDownForce()
    {
        m_WheelColliders[0].attachedRigidbody.AddForce(-transform.up * m_Downforce *
                                                     m_WheelColliders[0].attachedRigidbody.velocity.magnitude);
    }

    // crude traction control that reduces the power to wheel if the car is wheel spinning too much
    //如果汽车的轮子旋转太多，就会降低车轮功率的粗牵引力控制
    private void TractionControl()//牵引力控制
    {
        WheelHit wheelHit;//车轮的碰撞点
        switch (m_CarDriveType)
        {
            case CarDriveType.FourWheelDrive:
                // loop through all wheels
                for (int i = 0; i < 4; i++)
                {
                    m_WheelColliders[i].GetGroundHit(out wheelHit);
                    //调整扭矩
                    AdjustTorque(wheelHit.forwardSlip);
                    //wheelHit.forwardSlip 轮胎在滚动的方向滑动。加速度滑动为负，制动滑动为正。
                }
                break;

            case CarDriveType.BackWheelDrive:
                m_WheelColliders[2].GetGroundHit(out wheelHit);
                AdjustTorque(wheelHit.forwardSlip);

                m_WheelColliders[3].GetGroundHit(out wheelHit);
                AdjustTorque(wheelHit.forwardSlip);
                break;

            case CarDriveType.FrontWheelDrive:
                m_WheelColliders[0].GetGroundHit(out wheelHit);
                AdjustTorque(wheelHit.forwardSlip);

                m_WheelColliders[1].GetGroundHit(out wheelHit);
                AdjustTorque(wheelHit.forwardSlip);
                break;
        }
    }

    //调整扭矩
    private void AdjustTorque(float forwardSlip)
    {
        if (forwardSlip >= m_SlipLimit && m_CurrentTorque >= 0)
        {
            m_CurrentTorque -= 10 * m_TractionControl;
        }
        else
        {
            m_CurrentTorque += 10 * m_TractionControl;
            if (m_CurrentTorque > m_FullTorqueOverAllWheels)
            {
                m_CurrentTorque = m_FullTorqueOverAllWheels;
            }
        }
    }

    private void SteeringWheelRotate(float h)//方向盘旋转
    {
        steeringWheel.transform.localRotation = Quaternion.Euler(0, 0, h * steeringWheelMaxAngle);
        Velocitypointer.transform.localRotation = Quaternion.Euler(0, 0, -(CurrentSpeed / MaxSpeed) * 90);
        RPMpointer.transform.localRotation = Quaternion.Euler(0, 0, -(Revs) * 30);
      
    }

    public void ChangeView()//切换视角
    {
      
            if(inCamera.enabled==true)
        {
            inCamera.enabled = false;
            outCamera.enabled = true;
            currentCamera = outCamera;
        }
            else
        {
            inCamera.enabled = true;
            outCamera.enabled = false;
            currentCamera = inCamera;
        }
            
       
           
      

       
    }
}