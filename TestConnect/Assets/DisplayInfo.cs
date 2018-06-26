using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Icaros.Input;
using System.Text;

public class DisplayInfo : MonoBehaviour
{

    public Text InfoText;

    internal int button1Count = 0;
    internal int button2Count = 0;
    internal int button3Count = 0;
    internal int button4Count = 0;
    internal float x = 0;
    /*internal float y = 0;
	internal float z = 0;*/
    internal List<string> devices = new List<string>();
    internal IInputDevice activeDevice = null;

    // Use this for initialization
    void Start()
    {
        DeviceManager.Instance.NewDeviceRegistered += NewDeviceRegistered;
        DeviceManager.Instance.DeviceUsed += DeviceUsed;
    }

    private void DeviceUsed(IInputDevice obj)
    {
        Debug.Log(string.Format("!! DeviceUsed {0} - ({1})", obj.GetDeviceName(), obj.GetDeviceTypeID()));
        // Events registrieren, die die Button Clicks zählen
        obj.FirstButtonPressed += () => button1Count++;
        obj.SecondButtonPressed += () => button2Count++;
        obj.ThirdButtonPressed += () => button3Count++;
        obj.FourthButtonPressed += () => button4Count++;

        // Events für X, Y und Z registrieren
        obj.xAxisRotated += (value) => x = value;
        /*obj.yAxisRotated += (value) => y = value;
		obj.zAxisRotated += (value) => z = value;*/

        activeDevice = obj;
    }

    private void NewDeviceRegistered(IInputDevice obj)
    {
#if UNITY_ANDROID && !UNITY_EDITOR
		// wenn es ein Android Device ist, dann sicherstellen, dass nur der 
		// Icaros Controller als Device verwendet wird
		if (!(obj is IcarosController))
		return;
#endif
        Debug.Log(string.Format("!! NewDeviceRegistered {0} - ({1})", obj.GetDeviceName(), obj.GetDeviceTypeID()));
        DeviceManager.Instance.UseDevice(obj);

        devices.Add(obj.GetDeviceName());
    }

    // Update is called once per frame
    void Update()
    {
        var sb = new StringBuilder();
        sb.AppendFormat("Button Count: 1={0} / 2={1} / 3={2} / 4={3}", button1Count, button2Count, button3Count, button4Count).AppendLine();
        sb.AppendFormat("Axis: X={0}", x).AppendLine();
        sb.AppendFormat("Active Input: {0}", activeDevice != null ? activeDevice.GetDeviceName() : "").AppendLine();
        sb.AppendLine("Available Inputs:");
        foreach (var dev in devices)
            sb.Append("- ").AppendLine(dev);

        InfoText.text = sb.ToString();
    }
}