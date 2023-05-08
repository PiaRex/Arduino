using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.UI;


public class StartCode : MonoBehaviour
{
    private GameObject workPanel;

    public void Start()
    {
        workPanel = GameObject.Find("WorkSpaceGrid");
    }

    // public void ButtonStartStopClick()
    // {
    //     // if (!DataManager.Instance.isProgramStarted)
    //     {
    //         int childCount = workPanel.transform.childCount;
    //         Debug.Log("Number of children: " + childCount);

    //         // Пробегаем по всем дочерним элементам объекта "WorkPanel"
    //         float selectedScale = 1.2f;

    //         foreach (Transform child in workPanel.transform)
    //         {
    //             child.transform.localScale = new Vector3(selectedScale, selectedScale, selectedScale);
    //         }
    //         // DataManager.Instance.ChangeButtonLabel("STOP");
    //         // DataManager.Instance.isProgramStarted = true;
    //     }
    //     else
    //     {
    //         // DataManager.Instance.ChangeButtonLabel("START");
    //     }


}
//     public void ButtonResetClick()
//     {
//         foreach (Transform child in workPanel.transform)
//         {
//             Destroy(child.gameObject);
//         }

//     }
// }