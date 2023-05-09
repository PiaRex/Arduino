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

            // Check if the position is already occupied
            for (int i = 0; i < items.Count; i++)
            {
                if (items[i].transform.localPosition == newItem.transform.localPosition)
                {
                    // If occupied, remove existing item and shift others
                    var itemToRemove = items[i];
                    items.RemoveAt(i);
                    Destroy(itemToRemove.gameObject);
                    for (int j = i; j < items.Count; j++)
                    {
                        // Shift subsequent items to fill the gap
                        items[j].transform.localPosition = new Vector3(items[j].transform.localPosition.x, items[j].transform.localPosition.y - 50f, items[j].transform.localPosition.z);
                    }
                    break;
                }
            }

            // Add new item to the list
            items.Add(newItem.GetComponent<BaseElementClass>());
            newItem.GetComponent<Image>().raycastTarget = true;

            print("Item added");
        }
    }
}
