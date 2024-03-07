using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Utilities;

public struct SensorState : IInputStateTypeInfo
{
    public FourCC format => new FourCC('S', 'Y', 'N', 'C');

    [InputControl(name = "acceleration", layout = "Vector3")]
    public Vector3 acceleration;
    [InputControl(name = "angularVelocity", layout = "Vector3")]
    public Vector3 angularVelocity;
    // [InputControl(name = "sensorRotation", layout = "Axis")]
    // public float sensorRotation;

}



#if UNITY_EDITOR
[InitializeOnLoad]
#endif
[InputControlLayout(displayName = "Syncsense Sensor", stateType = typeof(SensorState))]
public class SyncsenseSensor : InputDevice
{

    public static Dictionary<string, SyncsenseSensor> sensorInputDict = new Dictionary<string, SyncsenseSensor>();
    
    public string deviceId  { get; set; }
    public string controllerName  { get; set; }

    public Vector3Control acceleration { get; private set; }
    public Vector3Control angularVelocity { get; private set; }
    // public AxisControl sensorRotation { get; private set; }
    
    protected override void FinishSetup()
    {
        base.FinishSetup();

        // NOTE: The Input System creates the Controls automatically.
        //       This is why don't do `new` here but rather just look
        //       the Controls up.
        acceleration = GetChildControl<Vector3Control>("acceleration");
        angularVelocity = GetChildControl<Vector3Control>("angularVelocity");
        // sensorRotation = GetChildControl<AxisControl>("sensorRotation");

    }

    static SyncsenseSensor()
    {
        InputSystem.RegisterLayout<SyncsenseSensor>();
    }

    private void OnUpdate()
    {
        // create new state from data
        SensorState sensorState = new SensorState();
        // sensorState.acceleration = GameDataManager.Instance.GetData(controllerName).acceleration;
        // sensorState.angularVelocity = GameDataManager.Instance.GetData(controllerName).angularVelocity;
        
        // set state to device
        InputSystem.QueueStateEvent<SensorState>(this, sensorState);
    }
    
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void InitializeInPlayer() {}
}