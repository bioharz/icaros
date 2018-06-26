using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

namespace Icaros.Input {
    public class UnityInputSetup {

        [MenuItem("ICAROS/Input/Add Default Keyboard Controls")]
        private static void addKeyboard() {
            try {
                BindAxis(new Axis() { name = DeviceManager.INPUT_FIRST_BUTTON, positiveButton = "k", type = 0 });
                BindAxis(new Axis() { name = DeviceManager.INPUT_SECOND_BUTTON, positiveButton = "l", type = 0 });
                BindAxis(new Axis() { name = DeviceManager.INPUT_THIRD_BUTTON, positiveButton = "i", type = 0 });
                BindAxis(new Axis() { name = DeviceManager.INPUT_FOURTH_BUTTON, positiveButton = "o", type = 0 });
                BindAxis(new Axis() { name = DeviceManager.INPUT_X_AXIS, positiveButton = "w", altPositiveButton = "up", negativeButton = "s", altNegativeButton = "down", type = 0 });
                BindAxis(new Axis() { name = DeviceManager.INPUT_Y_AXIS, positiveButton = "e", altPositiveButton = "right ctrl", negativeButton = "q", altNegativeButton = "right shift", type = 0 });
                BindAxis(new Axis() { name = DeviceManager.INPUT_Z_AXIS, positiveButton ="a", altPositiveButton = "left", negativeButton = "d", altNegativeButton = "right", type = 0 });
            }
            catch {
                Debug.LogError("Failed to apply Keyboard input manager bindings.");
            }
        }


        /*
         * BindAxis(new Axis() { name = "Oculus_GearVR_LThumbstickX", axis = 0, });
                BindAxis(new Axis() { name = "Oculus_GearVR_LThumbstickY", axis = 1, invert = true });
                BindAxis(new Axis() { name = "Oculus_GearVR_RThumbstickX", axis = 2, });
                BindAxis(new Axis() { name = "Oculus_GearVR_RThumbstickY", axis = 3, invert = true });
                BindAxis(new Axis() { name = "Oculus_GearVR_DpadX", axis = 4, });
                BindAxis(new Axis() { name = "Oculus_GearVR_DpadY", axis = 5, invert = true });
                BindAxis(new Axis() { name = "Oculus_GearVR_LIndexTrigger", axis = 12, });
                BindAxis(new Axis() { name = "Oculus_GearVR_RIndexTrigger", axis = 11, });
         */

        //IMPORTANT! The GearVR Gamepad registers on unity android as keyboard instead of a joystick for some reason. 
        //[MenuItem("ICAROS/Input/Add Default GearVR Gamepad Controls")]
        private static void addGearVR() {
            try {
                BindAxis(new Axis() { name = DeviceManager.INPUT_FIRST_BUTTON, positiveButton = "joystick button 0", type = 0 });
                BindAxis(new Axis() { name = DeviceManager.INPUT_SECOND_BUTTON, positiveButton = "joystick button 1", type = 0 });
                BindAxis(new Axis() { name = DeviceManager.INPUT_THIRD_BUTTON, positiveButton = "joystick button 2", type = 0 });
                BindAxis(new Axis() { name = DeviceManager.INPUT_FOURTH_BUTTON, positiveButton = "joystick button 3", type = 0 });
                BindAxis(new Axis() { name = DeviceManager.INPUT_X_AXIS, axis = 1, invert = true });
                BindAxis(new Axis() { name = DeviceManager.INPUT_Y_AXIS, axis = 2 });
                BindAxis(new Axis() { name = DeviceManager.INPUT_Z_AXIS, axis = 0 });
            }
            catch {
                Debug.LogError("Failed to apply GearVR input manager bindings.");
            }
        }


        //A Button joystick button 0
        //B Button joystick button 1
        //X Button joystick button 2
        //Y Button joystick button 3
        //L Button joystick button 4
        //R Button joystick button 5
        //Back joystick button 6
        //Start joystick button 7
        //Left Analog pressed joystick button 8
        //Right Analog pressed joystick button 9
        //Left Analog X Axis Joystick Axis, X Axis
        //Left Analog Y Axis Joystick Axis, Y Axis
        //Right Analog X Axis Joystick Axis, 4th Axis
        //Right Analog Y Axis Joystick Axis, 5th Axis
        //Left/Right on D-Pad Joystick Axis, Axis 6
        //Up/Down on D-Pad Joystick Axis, Axis 7
        //Left Trigger and Right Trigger both correspond to joystick axis, axis 3

        [MenuItem("ICAROS/Input/Add Default XBox Gamepad Controls")]
        private static void addXBox() {
            BindAxis(new Axis() { name = DeviceManager.INPUT_FIRST_BUTTON, positiveButton = "joystick button 0", type = 0 });
            BindAxis(new Axis() { name = DeviceManager.INPUT_SECOND_BUTTON, positiveButton = "joystick button 1", type = 0 });
            BindAxis(new Axis() { name = DeviceManager.INPUT_THIRD_BUTTON, positiveButton = "joystick button 2", type = 0 });
            BindAxis(new Axis() { name = DeviceManager.INPUT_FOURTH_BUTTON, positiveButton = "joystick button 3", type = 0 });
            BindAxis(new Axis() { name = DeviceManager.INPUT_X_AXIS, axis = 4 });
            BindAxis(new Axis() { name = DeviceManager.INPUT_Y_AXIS, axis = 3, invert = true });
            BindAxis(new Axis() { name = DeviceManager.INPUT_Z_AXIS, axis = 0, invert = true });
        }

        private class Axis {
            public string name = String.Empty;
            public string descriptiveName = String.Empty;
            public string descriptiveNegativeName = String.Empty;
            public string negativeButton = String.Empty;
            public string positiveButton = String.Empty;
            public string altNegativeButton = String.Empty;
            public string altPositiveButton = String.Empty;
            public float gravity = 0.0f;
            public float dead = 0.001f;
            public float sensitivity = 1.0f;
            public bool snap = false;
            public bool invert = false;
            public int type = 2;
            public int axis = 0;
            public int joyNum = 0;
        }

        private static void BindAxis(Axis axis) {
            SerializedObject serializedObject = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/InputManager.asset")[0]);
            SerializedProperty axesProperty = serializedObject.FindProperty("m_Axes");

            /*SerializedProperty axisIter = axesProperty.Copy();
            axisIter.Next(true);
            axisIter.Next(true);
            while (axisIter.Next(false)) {
                if (axisIter.FindPropertyRelative("m_Name").stringValue == axis.name) {
                    // Axis already exists. Don't create binding.
                    return;
                }
            }*/

            axesProperty.arraySize++;
            serializedObject.ApplyModifiedProperties();

            //fill in new axis
            SerializedProperty axisProperty = axesProperty.GetArrayElementAtIndex(axesProperty.arraySize - 1);
            axisProperty.FindPropertyRelative("m_Name").stringValue = axis.name;
            axisProperty.FindPropertyRelative("descriptiveName").stringValue = axis.descriptiveName;
            axisProperty.FindPropertyRelative("descriptiveNegativeName").stringValue = axis.descriptiveNegativeName;
            axisProperty.FindPropertyRelative("negativeButton").stringValue = axis.negativeButton;
            axisProperty.FindPropertyRelative("positiveButton").stringValue = axis.positiveButton;
            axisProperty.FindPropertyRelative("altNegativeButton").stringValue = axis.altNegativeButton;
            axisProperty.FindPropertyRelative("altPositiveButton").stringValue = axis.altPositiveButton;
            axisProperty.FindPropertyRelative("gravity").floatValue = axis.gravity;
            axisProperty.FindPropertyRelative("dead").floatValue = axis.dead;
            axisProperty.FindPropertyRelative("sensitivity").floatValue = axis.sensitivity;
            axisProperty.FindPropertyRelative("snap").boolValue = axis.snap;
            axisProperty.FindPropertyRelative("invert").boolValue = axis.invert;
            axisProperty.FindPropertyRelative("type").intValue = axis.type;
            axisProperty.FindPropertyRelative("axis").intValue = axis.axis;
            axisProperty.FindPropertyRelative("joyNum").intValue = axis.joyNum;
            serializedObject.ApplyModifiedProperties();
        }
    }
}