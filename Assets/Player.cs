using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class Player : NetworkBehaviour {

    //애니메이션들 너무 하드코딩이다. Resources.Load로 이차원배열에 저장하고 dir을 인덱스로 사용하도록 개선필요
    public Sprite[] idle_d, idle_u, idle_r;
    public Sprite[] walk_d, walk_u, walk_r;

    Sprite[] cur_anim;
    int idx = 0;
    SpriteRenderer sr;
    Rigidbody2D rb;
    int dir = 0;//SWNE
    Chatting chat;
    NpcScript[] npcs;

    void Start () {
        sr = GetComponent<SpriteRenderer>();
        cur_anim = idle_d;
        InvokeRepeating("Animate", 0, 0.1f);

        rb = GetComponent<Rigidbody2D>();
        chat = FindObjectOfType<Chatting>();

        if (isLocalPlayer)
        {
            var inputfield = chat.GetComponentInChildren<InputField>();
            inputfield.onEndEdit.AddListener(delegate { AddChat(inputfield.textComponent.text); });
        }
        npcs = FindObjectsOfType<NpcScript>();
    }
    
	void Update () {
        if (!isLocalPlayer)
            return;

        if (npcs.Length > 0)
        {
            NpcScript closest_npc = npcs[0];
            foreach (var i in npcs)
            {
                i.questionmark.SetActive(false);
                if ((closest_npc.transform.position - transform.position).magnitude > (i.transform.position - transform.position).magnitude)
                    closest_npc = i;
            }
            if ((closest_npc.transform.position - transform.position).magnitude < 1)
            {
                closest_npc.questionmark.SetActive(true);
                if (Input.GetKeyDown(KeyCode.Space) && !closest_npc.talking)
                    closest_npc.Talk();
            }
        }

        float hbias = Input.GetAxis("Horizontal");
        float vbias = Input.GetAxis("Vertical");
        
        if (hbias != 0)
        {
            rb.velocity = new Vector2(hbias*5, 0);
            //transform.position = new Vector3(transform.position.x + hbias * 0.1f, transform.position.y, transform.position.z);
            CmdSetAnim(Anim2Int(walk_r), hbias < 0);
            dir = hbias < 0 ? 1 : 3;
        }
        else
        {
            rb.velocity = new Vector2(0, vbias * 5);
            //transform.position = new Vector3(transform.position.x, transform.position.y + vbias * 0.1f, transform.position.z);
            if (vbias > 0)
            {
                CmdSetAnim(Anim2Int(walk_u), false);
                dir = 2;
            }
            else if(vbias < 0)
            {
                CmdSetAnim(Anim2Int(walk_d), false);
                dir = 0;
            }
            else
            {
                switch (dir)
                {
                    case 0:
                        CmdSetAnim(Anim2Int(idle_d), false);
                        break;
                    case 1:
                        CmdSetAnim(Anim2Int(idle_r), true);
                        break;
                    case 2:
                        CmdSetAnim(Anim2Int(idle_u), false);
                        break;
                    case 3:
                        CmdSetAnim(Anim2Int(idle_r), false);
                        break;
                }
            }
        }
    }

    void Animate()
    {
        sr.sprite = cur_anim[idx];
        idx = (idx + 1) % cur_anim.Length;
    }

    int Anim2Int(Sprite[] anim)
    {
        Sprite[][] anims = { idle_d, idle_r, idle_u, walk_d, walk_r, walk_u };
        for (int i = 0; i < anims.Length; i++)
            if (anim == anims[i])
                return i;
        return -1;
    }

    Sprite[] Int2Anim(int i)
    {
        Sprite[][] anims = { idle_d, idle_r, idle_u, walk_d, walk_r, walk_u };
        return anims[i];
    }

    void SetAnim(int animi, bool flipX)
    {
        if (Int2Anim(animi) != cur_anim || sr.flipX != flipX)
            CmdSetAnim(animi, flipX);
    }

    [Command]
    void CmdSetAnim(int animi, bool flipX)
    {
        if (isServer)
            RpcSetAnim(animi, flipX);
    }

    [ClientRpc]
    void RpcSetAnim(int animi, bool flipX)
    {
        Sprite[] anim = Int2Anim(animi);
        sr.flipX = flipX;
        if (anim == cur_anim)
            return;
        cur_anim = anim;
        idx = 0;
    }
    
    public void AddChat(string s)
    {
        CmdAddChat(s);
    }

    [Command]
    public void CmdAddChat(string s)
    {
        if (isServer)
            RpcAddChat(s);
    }

    [ClientRpc]
    void RpcAddChat(string s)
    {
        chat.AddChat(s);
    }
}
