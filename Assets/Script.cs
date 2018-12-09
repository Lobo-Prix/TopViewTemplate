using System;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class Script : MonoBehaviour {

    public Image player_img;
    public Image npc_img;
    public Text player_name;
    public Text npc_name;
    public Text text;
    public Image player_nameimg;
    public Image npc_nameimg;

    public delegate void EndCallback();
    public EndCallback onend;

    StreamReader f = null;
    int line_cnt = 1;

    Sprite LoadSprite(string path)
    {
        return Resources.Load<Sprite>(path.Split(new char[] { '.' })[0]);
    }

    public void Open(string path)
    {
        if (f != null)
            f.Close();
        f = new StreamReader(path);
        RunLine();
    }

    public void RunLine()
    {
        while (true)
        {
            string s = f.ReadLine();
            if (s == null)
                return;
            string[] toks = s.Split(new char[] { ' ' }, 2);
            switch (toks[0])
            {
                case "#":
                    continue;
                case "player_img":
                    if (toks.Length < 2)
                        throw new Exception("Error: Too few argument in player_img operator(Line: " + line_cnt + ")");
                    player_img.overrideSprite = LoadSprite(toks[1]);
                    break;
                case "npc_img":
                    if (toks.Length < 2)
                        throw new Exception("Error: Too few argument in npc_img operator(Line: " + line_cnt + ")");
                    npc_img.overrideSprite = LoadSprite(toks[1]);
                    break;
                case "npc_name":
                    if (toks.Length < 2)
                        throw new Exception("Error: Too few argument in npc_name operator(Line: " + line_cnt + ")");
                    npc_name.text = toks[1];
                    break;
                case "text":
                    if (toks.Length < 2)
                        throw new Exception("Error: Too few argument in text operator(Line: " + line_cnt + ")");
                    text.text = toks[1];
                    break;
                case "textb":
                    if (toks.Length < 2)
                        throw new Exception("Error: Too few argument in text operator(Line: " + line_cnt + ")");
                    text.text = toks[1];
                    return;
                case "whose":
                    if (toks.Length < 2)
                        throw new Exception("Error: Too few argument in whose operator(Line: " + line_cnt + ")");
                    (toks[1] == "npc" ? player_img : npc_img).color = new Color(0.3f, 0.3f, 0.3f, 0.3f);
                    (toks[1] == "npc" ? npc_img : player_img).color = Color.white;
                    break;
                //handle branch in game code
                //case "branch":
                //    break;
                case "next":
                    if (toks.Length < 2)
                        throw new Exception("Error: Too few argument in next operator(Line: " + line_cnt + ")");
                    f.Close();
                    f = new StreamReader(toks[1]);
                    continue;
                case "end":
                    gameObject.SetActive(false);
                    onend();
                    return;
                case "":
                    return;
                default:
                    throw new Exception("Error: Invalid operator. (Line: " + line_cnt + ")");
            }
            line_cnt++;
        }
    }

	void Update ()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            try
            {
                RunLine();
            }
            catch (Exception e)
            {
                text.text = e.Message;
            }
        }
		
	}

    private void OnDestroy()
    {
        f.Close();
    }
}
