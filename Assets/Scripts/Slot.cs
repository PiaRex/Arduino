using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Slot : MonoBehaviour, IDropHandler
{
    public RectTransform currentTransform;
    private GameObject mainContent;
    private Vector3 currentPossition;

    private int totalChild;
    public List<BaseElementClass> items = new List<BaseElementClass>();

    public void OnDrop(PointerEventData eventData)
    {
        var otherItemTransform = eventData.pointerDrag.transform;
        var otherItem = otherItemTransform.GetComponent<DragDrop>();

        if (otherItem.dropParent == Window.instance.WorkSpaceGrid)
        {
            print("РОДИТЕЛЬ СПЭЙСВОРК" + otherItem.dropParent);
        }
        else
        {
            var newItem = Instantiate(otherItemTransform.gameObject, Window.instance.WorkSpaceGrid);
            newItem.transform.localPosition = Vector3.zero;



            // Add new item to the list
            items.Add(newItem.GetComponent<BaseElementClass>());
            newItem.GetComponentInChildren<Image>().raycastTarget = true;

            print("Item added");
        }
    }
}
