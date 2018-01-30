using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;

public class HackMotion: MonoBehaviour
{
    public enum SensorID { None, Wrist, LowerArm, UpperArm, Shoulder }

    private Dictionary<SensorID, UnityEngine.Vector3> m_positions;

    /// <summary>
    /// Current UnityEngine.Vector3 positions indexed by SensorID
    /// </summary>
    public Dictionary<SensorID, UnityEngine.Vector3> Positions
    {
        get
        {
            if (!m_initialized)
            {
                Debug.LogError("Device not initialized. Wait for device connection");
                return null;
            }

            return m_positions;
        }
    }

    private Dictionary<SensorID, UnityEngine.Quaternion> m_rotations;

    /// <summary>
    /// Current UnityEngine.Quaternion rotations by SensorID
    /// </summary>
    public Dictionary<SensorID, UnityEngine.Quaternion> Rotations
    {
        get
        {
            if (!m_initialized)
            {
                Debug.LogError("Device not initialized. Wait for device connection");
                return null;
            }

            return m_rotations;
        }
    }

    public delegate void DeviceStateHandler();

    /// <summary>
    /// Sent when device starts streaming and position/rotation data structures are initialized
    /// </summary>
    public event DeviceStateHandler onDeviceStartStream;

    private const int SENSOR_COUNT = 4;

    [StructLayout(LayoutKind.Sequential)]
    struct Vector
    {
        public float x;
        public float y;
        public float z;
    }

    [StructLayout(LayoutKind.Sequential)]
    struct Quaternion
    {
        public float x;
        public float y;
        public float z;
        public float w;
    }

    [StructLayout(LayoutKind.Sequential)]
    unsafe struct Event
    {
        public void* sensor;
        public Quaternion rotation;
        public Vector acceleration;
        public byte calibration_status;
    }

    [DllImport("HackMotion")] unsafe private static extern void* hm_get_device_iterator();
    [DllImport("HackMotion")] unsafe private static extern void hm_device_iterator_free(void* iterator);
    [DllImport("HackMotion")] unsafe private static extern void* hm_get_next_device(void* iterator);
    [DllImport("HackMotion")] unsafe private static extern int hm_device_open(void* device);
    [DllImport("HackMotion")] unsafe private static extern int hm_device_free(void* device);
    [DllImport("HackMotion")] unsafe private static extern int hm_device_capture_pose(void* device, uint pose);
    [DllImport("HackMotion")] unsafe private static extern int hm_device_start_streaming(void* device);
    [DllImport("HackMotion")] unsafe private static extern int hm_device_stop_streaming(void* device);
    [DllImport("HackMotion")] unsafe private static extern uint hm_device_get_sensor_count(void* device);
    [DllImport("HackMotion")] unsafe private static extern void* hm_device_get_sensor(void* device, uint index);
    [DllImport("HackMotion")] unsafe private static extern int hm_device_next_event(void* device, Event* e);

    [DllImport("HackMotion")] unsafe private static extern int hm_sensor_get_location(void* sensor);
    [DllImport("HackMotion")] unsafe private static extern Vector* hm_sensor_get_offset(void* sensor);
    [DllImport("HackMotion")] unsafe private static extern Quaternion* hm_sensor_get_rotation(void* sensor);
    [DllImport("HackMotion")] unsafe private static extern Vector* hm_sensor_get_position(void* sensor);

    unsafe void* device;

    private bool m_initialized;

    unsafe void Awake()
    {
        //Debug.Log("Start");

        void* iterator = hm_get_device_iterator();

        if (iterator == null) return;

        //Debug.Log("Got iterator");

        device = hm_get_next_device(iterator);

        hm_device_iterator_free(iterator);

        if (device == null) return;

        //Debug.Log("Got device");

        if (hm_device_open(device) == 0) return;

        //Debug.Log("Device opened");

        if (hm_device_start_streaming(device) == 0) return;

        Debug.Log("Started streaming");

        m_positions = new Dictionary<SensorID, Vector3>();
        m_rotations = new Dictionary<SensorID, UnityEngine.Quaternion>();

        m_positions.Add(SensorID.None, Vector3.zero);
        m_rotations.Add(SensorID.None, UnityEngine.Quaternion.identity);

        for (int i = 1; i <= SENSOR_COUNT; i++)
        {
            m_positions.Add((SensorID)i, Vector3.zero);

            m_rotations.Add((SensorID)i, UnityEngine.Quaternion.identity);
        }

        m_initialized = true;
        OnDeviceStartStream();
    }

    unsafe void Update()
    {
        Event e;

        while (hm_device_next_event(device, &e) != 0)
        {
            //Debug.Log("Got event");

            void* sensor = e.sensor;
            //Debug.Log(hm_sensor_get_location(sensor));
            int sensorID = hm_sensor_get_location(sensor);

            Quaternion* rotation = hm_sensor_get_rotation(sensor);
            m_rotations[(SensorID)sensorID] = new UnityEngine.Quaternion(rotation->x, rotation->z, rotation->y, rotation->w);

            Vector* position = hm_sensor_get_position(sensor);
            m_positions[(SensorID)sensorID] = new Vector3(position->x, position->z, position->y);
        }
    }

    unsafe void OnDestroy()
    {
        if (device != null)
        {
            hm_device_free(device);
        }
    }

    void OnDeviceStartStream()
    {
        if (onDeviceStartStream != null)
        {
            onDeviceStartStream();
        }
    }

    /// <summary>
    /// Capture current pose (for calibration)
    /// </summary>
    /// <param name="id">Sensor id to capture</param>
    public void CapturePose(int id)
    {
        unsafe
        {
            hm_device_capture_pose(device, (uint)id);
        }
    }

    /// <summary>
    /// Capture current pose (for calibration)
    /// </summary>
    /// <param name="id">Sensor to capture</param>
    public void CapturePose(SensorID id)
    {
        unsafe
        {
            hm_device_capture_pose(device, (uint)id);
        }
    }
}
