using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonObject : MonoBehaviour
{
    public string myObjectProps;
    public Text TextHolder;
    public JSONParser js;

    public void OnClick()
    {
        print(myObjectProps);
        js.ClearAllSelection();
        GetComponent<Image>().color = Color.blue;
        TextHolder.text = myObjectProps;
    }

}
