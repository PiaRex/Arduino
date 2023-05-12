using UnityEngine;

public class BluetoothDeviceList : MonoBehaviour
{
    AndroidJavaObject bluetoothDevices;

    public void Refresh()
    {
        using (var javaUnityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        {
            using (var currentActivity = javaUnityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
            {
                bluetoothDevices = new AndroidJavaObject("com.microwave.mylibrary.BluetoothDevices");
            }
        }

        string deviceList = bluetoothDevices.Call<string>("getPairedDevices");
        Debug.Log("девайсы");
        Debug.Log(deviceList);
    }
}