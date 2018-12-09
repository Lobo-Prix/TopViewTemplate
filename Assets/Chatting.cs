using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class Chatting : MonoBehaviour{
    public Text t;
    List<string> chats = new List<string>();

    public void AddChat(string s)
    {
        if (chats.Count >= 5)
            chats.RemoveAt(0);
        chats.Add(s);

        t.text = "";
        foreach (var i in chats)
            t.text += i + "\n";
    }
}
