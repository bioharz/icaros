using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Icaros.Input
{
    //später pro steuersystem in weitere interfaces aufteilen (zbsp icaros support, animal support etc)
    public interface IInputDevice
    {
        event System.Action FirstButtonPressed;
        event System.Action SecondButtonPressed;
        event System.Action ThirdButtonPressed;
        event System.Action FourthButtonPressed;

        bool FirstButtonDown();
        bool SecondButtonDown();
        bool ThirdButtonDown();
        bool FourthButtonDown();

        event System.Action FirstButtonReleased;
        event System.Action SecondButtonReleased;
        event System.Action ThirdButtonReleased;
        event System.Action FourthButtonReleased;

        event System.Action<float> xAxisRotated;
        /*event System.Action<float> yAxisRotated;
        event System.Action<float> zAxisRotated;*/
        event System.Action<Quaternion> RotationChanged;

        string GetDeviceTypeID();
        string GetDeviceName();
        bool IsInUse();

        void Update();
    }
}