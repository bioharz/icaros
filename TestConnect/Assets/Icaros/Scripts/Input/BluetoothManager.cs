using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Icaros.Input {
    public class BluetoothManager : MonoBehaviour {

        public static BluetoothManager Instance = null;

        void Awake() {
            if (Instance != null) {
                Destroy(this.gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(this.gameObject);

#if UNITY_STANDALONE_WIN
            Bluetooth.Manager.OnDebugMessage += debug;
#endif
        }

        private string[] newControllers = new string[0];
        private bool isPaired = false;

        public void startScan() {
#if UNITY_STANDALONE_WIN
            Bluetooth.Manager.StartScan();
#endif
        }

        public string[] GetNewControllers() {
            string[] result = newControllers;
            newControllers = new string[0];
            return result;
        }

        public void ConnectToController(string name) {
#if UNITY_STANDALONE_WIN
            Bluetooth.Manager.PairController(name);
#endif
        }

        public void Initialize() {
#if UNITY_STANDALONE_WIN
            Bluetooth.Manager.OnInitialized += bluetoothInitialized;
            Bluetooth.Manager.OnInitializeFailed += bluetoothInitFailed;
            Bluetooth.Manager.Initialize();
#endif
        }

        internal void bluetoothInitialized() {
            Debug.Log("Bluetooth init");
#if UNITY_STANDALONE_WIN
            Bluetooth.Manager.OnScanFinished += bluetoothScanFinished;
            Bluetooth.Manager.OnPaired += bluetoothPaired;
#endif
            startScan();
        }

        internal void bluetoothInitFailed() {
#if UNITY_STANDALONE_WIN
            Bluetooth.Manager.Initialize();
#endif
        }

        internal void bluetoothScanFinished(string[] bluetoothDevices) {
            newControllers = bluetoothDevices;
            if (!isPaired) {
                startScan();
            }
        }

        internal void bluetoothPaired() {
            Debug.Log("Bluetooth paired");
            isPaired = true;
        }

        internal void debug(string debugMessage) {
            Debug.Log(debugMessage);
        }

        private void OnDestroy() {
#if UNITY_STANDALONE_WIN
            Bluetooth.Manager.CloseConnection();
#endif
        }
    }
}