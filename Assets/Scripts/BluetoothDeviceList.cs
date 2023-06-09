using UnityEngine;

public class BluetoothDeviceList : MonoBehaviour
{
    AndroidJavaObject bluetoothDevices;

    public void StartSearch()
    {
        using (var javaUnityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        {
            using (var currentActivity = javaUnityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
            {
                bluetoothDevices = new AndroidJavaObject("com.microwave.bluetootheunity.BluetoothDevices");
            }
        }

        string deviceList = bluetoothDevices.Call<string>("getPairedDevices");
        Debug.Log(deviceList);
    }
}