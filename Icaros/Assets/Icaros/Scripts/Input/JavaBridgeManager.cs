using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System;

namespace Icaros.Input {
    internal enum JavaBridgeStatus {
        UNKNOWN,
        INITIALISED,
        INITIALISED_WITH_SAVED_PERIPHERAL,
        ERROR_NO_BLUETOOTH,
        SCANNING,
        PERIPHERAL_FOUND,
        PERIPHERAL_CONNECTING_TO_SAVED,
        PERIPHERAL_CONNECTING,
        PERIPHERAL_CONNECTED,
        PERIPHERAL_CONNECT_ERROR,
        PERIPHERAL_DISCONNECTED
    }

    internal class JavaBridgeManager : MonoBehaviour{

        internal static JavaBridgeManager Instance = null;

        public JavaBridgeStatus status = JavaBridgeStatus.UNKNOWN;

        public string statusInfo = "";

        public bool enableTraces = false;

        [HideInInspector]
        public bool savedPeripheralFound = false;

        public delegate void JavaBridgeStatusChange(JavaBridgeStatus newStatus, string newStatusInfo);
        public event JavaBridgeStatusChange OnStatusChange = delegate { };

        public delegate void JavaBridgeNewBytes(byte[] bytes);
        public event JavaBridgeNewBytes OnNewBytes = delegate { };

        public delegate void JavaBridgeControllerFound(string newControllerName);
        public event JavaBridgeControllerFound OnControllerFound = delegate { };

        public Dictionary<string, string> peripherals;

        private string lastPeripheralName;
        

        void Awake() {
            if (Instance != null) {
                Destroy(this.gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }

        public void ConfigureCamera() {
#if UNITY_ANDROID && !UNITY_EDITOR
		SetupCrazyHack();
#endif
        }

        public void StartScanning() {
#if UNITY_ANDROID && !UNITY_EDITOR
		StartScan();
#endif
        }

        public void StopScanning() {
#if UNITY_ANDROID && !UNITY_EDITOR
		StopScan();
#endif
        }

        public void Disconnect() {
#if UNITY_ANDROID && !UNITY_EDITOR
		DisconnectPeripheral();
#endif

#if UNITY_EDITOR
            UpdateStatus(JavaBridgeStatus.PERIPHERAL_DISCONNECTED, "");
#endif
        }

        public void ConnectToController(string name) {
#if UNITY_ANDROID && !UNITY_EDITOR
		ConnectToPeripheral(name);
#endif

#if UNITY_EDITOR
            UpdateStatus(JavaBridgeStatus.PERIPHERAL_CONNECTED, name);
#endif
        }

        internal void UpdateStatus(JavaBridgeStatus newStatus, string newStatusInfo) {
            status = newStatus;
            statusInfo = newStatusInfo;
            OnStatusChange(newStatus, newStatusInfo);
        }

        public void ReconnectToLastController() {
#if UNITY_ANDROID && !UNITY_EDITOR
		ConnectToPeripheral(lastPeripheralName);
#endif
        }
        
        public void Initialize() {
#if UNITY_ANDROID && !UNITY_EDITOR
		initialize();
#endif
        }

#if UNITY_ANDROID && !UNITY_EDITOR

        AndroidJavaObject javaBridge;

        private void initialize(){
            AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        
            AndroidJavaObject currentActivity = jc.GetStatic<AndroidJavaObject>("currentActivity");

            javaBridge = new AndroidJavaObject("com.lumacode.icaros.JavaBridge", currentActivity);

            InitialiseTIO();
        }

        void Update(){
            if (status == JavaBridgeStatus.PERIPHERAL_CONNECTED) {
                byte[] bytes = javaBridge.Get<byte[]>("lastBytes");
                if (bytes.Length >= 20) {
                    OnNewBytes(bytes);
                }
            }
        }

        public int StartBackCamera() {
            int result = javaBridge.Call<int>("initBackCamera");

            return result;
        }

        public void UpdateVideoTexture() {
            //javaBridge.Call("updateVideoTexture");
        }

        public void SetupCrazyHack() {

            // Get activity instance (standard way, solid)
            var pl_class = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            var currentActivity = pl_class.GetStatic<AndroidJavaObject>("currentActivity");

            // Get instance of UnityPlayer (hacky but will live)
            var pl_inst = currentActivity.Get<AndroidJavaObject>("mUnityPlayer");

            // Get list of camera wrapper
            var list = pl_inst.Get<AndroidJavaObject>("y");
            int x = list.Call<int>("size");

            if (x == 0) return;

            // Get the first element of list (active camera wrapper)
            var cam_holder = list.Call<AndroidJavaObject>("get", 0);

            // get camera (this is totally insane, again if "a" becomes not-"a" one day )
            var cam = cam_holder.Get<AndroidJavaObject>("a");

            // Call my setup camera routine in Android plugin  (will set params and call autoFocus)

            javaBridge.Call("setupCamera", new[] { cam });

        }

        public void InitialiseTIO() {

            peripherals = new Dictionary<string, string>();


            String result = javaBridge.Call<String>("initialiseTIO", enableTraces);

            if (result == "Initialised") {
                UpdateStatus(JavaBridgeStatus.INITIALISED, "");
                savedPeripheralFound = false;
            }
            else if (result == "InitialisedWithSavedPeripherals") {
                UpdateStatus(JavaBridgeStatus.INITIALISED_WITH_SAVED_PERIPHERAL, "");
                savedPeripheralFound = true;
            }

            StartScan();
        }

        private void StartScan() {
            peripherals.Clear();
            String result = javaBridge.Call<String>("startScan");

            if (result == "BluetoothNotEnabled") {
                UpdateStatus(JavaBridgeStatus.ERROR_NO_BLUETOOTH, "");
            }
            else if (result == "Scanning") {
                UpdateStatus(JavaBridgeStatus.SCANNING, "");
            }
        }

        private void StopScan() {
            javaBridge.Call("stopScan");
            UpdateStatus(JavaBridgeStatus.INITIALISED, "");
        }

        public void PeripheralFound(String peripheralNameAndAddress) {
            string[] parts = peripheralNameAndAddress.Split('|');
            string name = parts[0];
            string address = parts[1];
            peripherals.Add(name, address);

            OnControllerFound(name);
        }

        private void ConnectToPeripheral(string name) {
            if (peripherals.ContainsKey(name)) {
                javaBridge.Call("connectToPeripheral", peripherals[name]);
            }
            else {
                Debug.LogError("Peripheral address not found for " + name);
            }
        }

        private void DisconnectPeripheral() {
            savedPeripheralFound = false;
            javaBridge.Call("disconnectFromPeripheral");
        }

        public void PeripheralConnecting(String peripheral) {
            UpdateStatus(JavaBridgeStatus.PERIPHERAL_CONNECTING, peripheral);
        }

        public void PeripheralConnectingToSaved(String peripheral) {
            UpdateStatus(JavaBridgeStatus.PERIPHERAL_CONNECTING_TO_SAVED, peripheral);
        }

        public void PeripheralConnected(String peripheral) {
            savedPeripheralFound = true;
            lastPeripheralName = peripheral;
            UpdateStatus(JavaBridgeStatus.PERIPHERAL_CONNECTED, peripheral);
        }

        public void PeripheralConnectError(String error) {
            UpdateStatus(JavaBridgeStatus.PERIPHERAL_CONNECT_ERROR, error);
        }

        public void PeripheralDisconnected(String error) {
            UpdateStatus(JavaBridgeStatus.PERIPHERAL_DISCONNECTED, error);
        }

#endif
    }
}