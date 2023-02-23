using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

[System.Serializable]
public class NodeElement
{
    public string elementType;
    public SoundObject sound;
    public CharacterObject character;
}

[System.Serializable]
public class SoundObject
{
    public string objectName;
    public string objectId;
    public string audioType;
    public string audioUrl;
    public bool loop;
    public float volume;
    public float pitch;
    public string spatialMode;
    public float minDistance;
    public float maxDistance;
    public float startTime;
    public float endTime;

    public override string ToString()
    {
        return $"SoundObject{{\n\tobjectName:{objectName} \n\tobjectId:{objectId} \n\t" +
            $"audioType:{audioType} \n\taudioUrl:{audioUrl} \n\t" +
            $"loop:{loop} \n\tvolume:{volume}\n\tpitch:{pitch} \n\tspatialMode:{spatialMode} \n\tminDistance:{minDistance}\n\t" +
            $"maxDistance:{maxDistance} \n\tstartTime:{startTime}\n\tendTime: {endTime}\n}}";
    }
}

[System.Serializable]
public class CharacterObject
{
    public string id;
    public string name;
    public float startTime;
    public float endTime;
    public string model;
    public Vector3 origin;
    public List<ElementObject> elements;

    public override string ToString()
    {
        return $"CharacterObject {{ \n\tid: {id}\n\tname: {name}\n\tstartTime: {startTime}\n\tendTime: {endTime}\n\tmodel: {model}\n\torigin: {origin} \n}}";
    }
}

[System.Serializable]
public class ElementObject
{
    public string elementType;
    public Animation animation;
    public string id;
}

[System.Serializable]
public class Animation
{
    public string animationId;
    public Vector3 destination;
    public float endTime;
    public int loopCount;
    public string name;
    public float startTime;
    public override string ToString()
    {
        return $"Animation {{\n\tanimationId: {animationId},\n\tname: {name},\n\tstartTime: {startTime},\n\tendTime: {endTime},\n\tloopCount: {loopCount},\n\tdestination: {destination}\n}}";
    }
}

[System.Serializable]
public class ConditionObject
{
    public string conditionType;
    public EventObject eventt;
}

[System.Serializable]
public class EventObject
{
    public int desiredEventValue;
    public string eventType;
}

[System.Serializable]
public class SceneObject
{
    public string mainName;
    public MainObject main;
}

[System.Serializable]
public class MainObject
{
    public List<NodeObject> nodes;
}

[System.Serializable]
public class NodeObject
{
    public string name;
    public int nodeId;
    public string nodeType;
    public int nextNodeID;
    public float startTime;
    public float endTime;
    public Vector3 origin;
    public List<NodeElement> nodeElements;
    public List<ConditionObject> conditions;

    public override string ToString()
    {
        return $"NodeObject {{\n\tname: {name},\n\tnodeId: {nodeId},\n\tnodeType: {nodeType},\n\tnextNodeID: {nextNodeID},\n\tstartTime: {startTime},\n\tendTime: {endTime},\n\torigin: {origin}\n}}";
    }
}

public class JSONParser : MonoBehaviour
{
    public GameObject jsonDataHolder;
    public GameObject button;

    public string url = "https://furniyar.com/hh.json";
    SceneObject sceneObject;
    public Dictionary<string, object> MyDict = new Dictionary<string, object>();
    public Dictionary<string, GameObject> ButtonsDict = new Dictionary<string, GameObject>();

    IEnumerator Start()
    {
        UnityWebRequest www = UnityWebRequest.Get(url);
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError(www.error);
        }
        else
        {
            sceneObject = JsonUtility.FromJson<SceneObject>(www.downloadHandler.text);
            fillMyDict(sceneObject);
        }
    }

    void fillMyDict(SceneObject sceneObj)
    {
        AddButtonForObject("Choose an element.", 0, sceneObj.mainName, "SceneObject", "sceneId");
        foreach(NodeObject node in sceneObject.main.nodes)
        {
            MyDict.Add(node.nodeId.ToString(), node);
            AddButtonForObject(node.ToString(), 1, node.name, node.nodeType, node.nodeId.ToString());
            extractNodeObject(node);
        }
    }

    void extractNodeObject(NodeObject node)
    {
        foreach (NodeElement nElm in node.nodeElements)
        {
            if(nElm.elementType == "Sound")
            {
                MyDict.Add(nElm.sound.objectId, nElm.sound);
                AddButtonForObject(nElm.sound.ToString(), 2, nElm.sound.objectName, nElm.elementType, nElm.sound.objectId);
            }else if(nElm.elementType == "Character")
            {
                MyDict.Add(nElm.character.id, nElm.character);
                AddButtonForObject(nElm.character.ToString(), 2, nElm.character.name, nElm.elementType, nElm.character.id);
                extractCharacterElements(nElm.character);
            }
        }

        //foreach (ConditionObject nCond in node.conditions)
        //{
           
        //}
    }

    void extractCharacterElements(CharacterObject CharObject)
    {
        foreach (ElementObject elm in CharObject.elements)
        {
            if (elm.elementType == "Animation")
            {
                
                MyDict.Add(elm.id, elm.animation);
                AddButtonForObject(elm.animation.ToString(), 3, elm.animation.name, elm.elementType, elm.id);
            }
        }
    }



    #region ui
    public Text TextHolder;
    void AddButtonForObject(string Oprops, int level, string name, string Btype, string id)
    {
        //print("types: "+ Btype);
        GameObject newButton = Instantiate(button, jsonDataHolder.transform);
        newButton.GetComponent<ButtonObject>().myObjectProps = Oprops;
        ButtonsDict.Add(id, newButton);

        string buttonText = "";
        for(int x=0; x < level; x++) { buttonText += "      "; }
        buttonText += "-> " + name + $" ({Btype})";
        newButton.GetComponentInChildren<Text>().text = buttonText;
    }

    public Text InputTextField;
    public void ClickFindById()
    {
        try
        {
            GameObject b = ButtonsDict[InputTextField.text];
            ClearAllSelection();
            b.GetComponent<ButtonObject>().OnClick();
        }
        catch
        {
            TextHolder.text = "object not found, please check your id string.";
        }
    }

    public void ClearAllSelection()
    {
        foreach(GameObject b in ButtonsDict.Values)
        {
            b.GetComponent<Image>().color = Color.white;
        }
        TextHolder.text = "";
    }
    #endregion
}
