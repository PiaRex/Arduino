using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using TechTweaking.Bluetooth;
using UnityEngine.UI;
using System.Threading.Tasks;

public class TerminalController : MonoBehaviour
{
    private const string UUID = "0acc9c7c-48e1-41d2-acaa-610d1a7b085e";
    public TMP_Text statusText;
    public ScrollTerminalUI readDataText;//ScrollTerminalUI is a script used to control the ScrollView text

    public GameObject BluetoothPanel, ConnectionButton;
    public GameObject DataCanvas;
    public BluetoothDevice device;
    public Text dataToSend;
    private int count;

    void Awake()
    {
        BluetoothAdapter.askEnableBluetooth();//Ask user to enable Bluetooth

        BluetoothAdapter.OnDeviceOFF += HandleOnDeviceOff;
        BluetoothAdapter.OnDevicePicked += HandleOnDevicePicked; //To get what device the user picked out of the devices list

    }
    void HandleOnDeviceOff(BluetoothDevice dev)
    {
        if (!string.IsNullOrEmpty(dev.Name))
            statusText.text = "Couldn't connect to " + dev.Name + ", device might be OFF";
        else if (!string.IsNullOrEmpty(dev.MacAddress))
        {
            statusText.text = "Couldn't connect to " + dev.MacAddress + ", device might be OFF";
        }
    }

    void HandleOnDevicePicked(BluetoothDevice device)//Called when device is Picked by user
    {
        this.device = device;//save a global reference to the device

        this.device.UUID = UUID; //This is not required for HC-05/06 devices and many other electronic bluetooth modules.

        statusText.text = "Remote Device : " + device.Name;

    }


    //############### UI BUTTONS RELATED METHODS #####################
    public void showDevices()
    {
        BluetoothAdapter.showDevices();//show a list of all devices//any picked device will be sent to this.HandleOnDevicePicked()
    }

    public void connect()//Connect to the public global variable "device" if it's not null.
    {
        if (device != null)
        {
            device.connect();

            statusText.text = "Remote Device : " + device.Name + ". Trying to connect...";
        }
        else
            statusText.text = "Remote Device not selected";
    }

    public void disconnect()//Disconnect the public global variable "device" if it's not null.
    {
        if (device != null)
            device.close();
    }

    public void send(string message)
    {
        Debug.Log("TerminalController: попытка отправки сообщения: " + message + " на девайс - ");
        if (device != null)
        {
            Debug.Log("Bluetooth Sending : " + message + "!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
            device.send(System.Text.Encoding.ASCII.GetBytes(message + (char)10));//10 is our seperator Byte (sepration between packets)
        }
    }

    public void sendHello()
    {
        if (device != null)
        {
            /*
			 * Send and Read works only with bytes. You need to convert everything to bytes.
			 * Different devices with different encoding is the reason for this. You should know what encoding you're using.
			 * In the method call below I'm using the ASCII encoding to send "Hello" + a new line.
			 */
            device.send(System.Text.Encoding.ASCII.GetBytes("Hello\n"));
        }
    }







    public async Task<string> ReadBTMessageAsync()
    {
        string content = null;
        while (content == null || count == 40)
        {
            await Task.Delay(100);
            byte[] msg = device.read();
            if (msg != null)
            {
                content = System.Text.ASCIIEncoding.ASCII.GetString(msg);
            }
            count++;
        }

        return content;
    }


    //############### UnRegister Events  #####################
    void OnDestroy()
    {
        BluetoothAdapter.OnDevicePicked -= HandleOnDevicePicked;
        BluetoothAdapter.OnDeviceOFF -= HandleOnDeviceOff;
    }
    public void OnSwitchOn()
    {
        BluetoothPanel.SetActive(true);

    }
    public void OnSwitchOff()
    {
        BluetoothPanel.SetActive(false);
        disconnect();
    }
}
