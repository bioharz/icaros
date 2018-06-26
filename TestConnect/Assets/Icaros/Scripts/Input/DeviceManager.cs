using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Icaros.Input
{
    public class DeviceManager : MonoBehaviour
    {

        #region deviceIDs
        public const string ICAROS_CONTROLLER_DEVICE_ID = "ICAROS_CONTROLLER";
        public const string KEYBOARD_DEVICE_ID = "KEYBOARD";
        public const string JOYSTICK_DEVICE_ID = "JOYSTICK";
        #endregion

        #region UnityInputManager Mapping
        public const string INPUT_FIRST_BUTTON = "ICAROS_Button1";
        public const string INPUT_SECOND_BUTTON = "ICAROS_Button2";
        public const string INPUT_THIRD_BUTTON = "ICAROS_Button3";
        public const string INPUT_FOURTH_BUTTON = "ICAROS_Button4";

        public const string INPUT_X_AXIS = "ICAROS_xAxis";
        public const string INPUT_Y_AXIS = "ICAROS_yAxis";
        public const string INPUT_Z_AXIS = "ICAROS_zAxis";
        #endregion

        public static DeviceManager Instance = null;

        public System.Action<IInputDevice> NewDeviceRegistered = delegate { };
        public System.Action<IInputDevice> DeviceLost = delegate { };
        public System.Action<IInputDevice> DeviceUsed = delegate { };

        List<IInputDevice> registeredInputDevices = new List<IInputDevice>();
        bool initialized = false;

        private IInputDevice deviceWaitingForJBM = null;

        void Awake()
        {
            if (Instance != null)
            {
                Destroy(this.gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }

        public void dbg(string dbg)
        {
            Debug.Log(dbg);
        }

        void Start()
        {
#if UNITY_ANDROID
            JavaBridgeManager.Instance.OnControllerFound += IcarosControllerFound;
            JavaBridgeManager.Instance.OnStatusChange += IcarosControllerStatusChange;
#endif
        }

        void init()
        {
            try
            {
                JavaBridgeManager.Instance.Initialize();
                //BluetoothManager.Instance.Initialize();

#if !UNITY_ANDROID || UNITY_EDITOR
                registerDevice(new Keyboard());
#endif
                //IMPORTANT! The GearVR Gamepad registers on unity android as keyboard instead of a joystick for some reason. 
#if UNITY_ANDROID && !UNITY_EDITOR
                registerDevice(new Keyboard() { deviceName = "Gear VR Gamepad" });
#endif
                foreach (string jname in UnityEngine.Input.GetJoystickNames())
                {
                    GenericJoystick joystick = new GenericJoystick();
                    joystick.deviceName = jname;
                    registerDevice(joystick);
                }

            }
            catch (System.Exception e)
            {
                Debug.Log(e.Message);
                //Test.addDebugMessage(e.Message);
            }
        }

        void Update()
        {
            if (!initialized)
            {
                init();
                initialized = true;
            }

            foreach (IInputDevice device in registeredInputDevices)
            {
                if (device.IsInUse())
                    device.Update();
            }

            //string[] potentialControllers = BluetoothManager.Instance.GetNewControllers();
            //foreach (string name in potentialControllers)
            //IcarosControllerFound(name);
        }

        public void UseDevice(IInputDevice device)
        {
            if (device.GetType() == typeof(IcarosController))
            {
                IcarosController con = device as IcarosController;
#if UNITY_ANDROID && !UNITY_EDITOR
                JavaBridgeManager.Instance.OnNewBytes += con.HandleNewBytes;
                JavaBridgeManager.Instance.ConnectToController(con.GetDeviceName());
                deviceWaitingForJBM = device;
#endif
#if UNITY_STANDALONE_WIN
                BluetoothManager.Instance.ConnectToController(con.GetDeviceName());
                DeviceUsed(device);
#endif
                con.used = true;
            }
            if (device.GetType() == typeof(Keyboard))
            {
                Keyboard key = device as Keyboard;
                key.used = true;
                DeviceUsed(device);
            }
            if (device.GetType() == typeof(GenericJoystick))
            {
                GenericJoystick joy = device as GenericJoystick;
                joy.used = true;
                DeviceUsed(device);
            }

        }

        public IInputDevice[] GetRegisteredDevices()
        {
            IInputDevice[] devices = new IInputDevice[registeredInputDevices.Count];
            registeredInputDevices.CopyTo(devices);
            return devices;
        }

        public List<IInputDevice> GetUsedDevices()
        {
            List<IInputDevice> devices = new List<IInputDevice>();
            foreach (IInputDevice device in registeredInputDevices)
            {
                if (device.IsInUse())
                    devices.Add(device);
            }
            return devices;
        }

        void registerDevice(IInputDevice device)
        {
            registeredInputDevices.Add(device);
            NewDeviceRegistered(device);
        }

        #region Icaros Controller
        internal void IcarosControllerFound(string name)
        {
            IcarosController con = new IcarosController();
            con.deviceName = name;
            registerDevice(con);
        }

        internal void IcarosControllerStatusChange(JavaBridgeStatus newStatus, string newStatusInfo)
        {
            if (newStatus.Equals(JavaBridgeStatus.PERIPHERAL_DISCONNECTED))
            {
                IcarosController con = null;
                foreach (IInputDevice device in registeredInputDevices)
                {
                    if (device.GetType() == typeof(IcarosController) && device.IsInUse())
                    {
                        con = device as IcarosController;
                    }
                }

                if (con != null)
                {
                    con.used = false;
                    registeredInputDevices.Remove(con);
                    DeviceLost(con);
                }
            }

            if (newStatus.Equals(JavaBridgeStatus.PERIPHERAL_CONNECTED))
            {
                DeviceUsed(deviceWaitingForJBM);
            }
        }
        #endregion

    }
}