using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NpcScript : MonoBehaviour {

    public GameObject questionmark;
    public Script dlg;
    public string script;

	void Start () {
		
	}

    void Update()
    {

    }

    GameObject CreateButton()
    {
        GameObject ok = new GameObject("ok_button");
        ok.transform.SetParent(dlg.transform);
        Image img = ok.AddComponent<Image>();
        RectTransform rt = img.rectTransform;
        rt.localPosition = new Vector3(570, -320, 0);
        rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 128);
        rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 50);
        Button btn = ok.AddComponent<Button>();
        btn.targetGraphic = img;
        

        GameObject txt = new GameObject("btntxt");
        txt.transform.SetParent(ok.transform);
        Text t = txt.AddComponent<Text>();
        t.text = "OK";
        t.font = Resources.Load<Font>("Arial");
        t.rectTransform.localPosition = Vector3.zero;
        t.fontSize = 20;
        t.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 128);
        t.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 50);
        t.color = Color.black;
        t.alignment = TextAnchor.MiddleCenter;

        return ok;
    }

    public bool talking = false;
    public void Talk()
    {
        dlg.Open(script);
        dlg.onend = () => {
            dlg.gameObject.SetActive(true);

            GameObject okb = CreateButton();
            GameObject cancelb = CreateButton();
            okb.GetComponent<Button>().onClick.AddListener(delegate { Destroy(okb); Destroy(cancelb); dlg.Open("Accepted.txt"); dlg.onend = delegate { Invoke("TalkFree", 0.1f); }; });
            cancelb.GetComponent<Button>().onClick.AddListener(delegate { Destroy(okb); Destroy(cancelb); dlg.Open("Rejected.txt"); dlg.onend = delegate { Invoke("TalkFree", 0.1f); }; });

            cancelb.GetComponentInChildren<Text>().text = "Cancel";
            RectTransform rt = cancelb.GetComponent<RectTransform>();
            rt.localPosition = new Vector3(rt.localPosition.x, rt.localPosition.y + 65, rt.localPosition.z);
        };
        dlg.gameObject.SetActive(true);
        talking = true;
    }

    public void TalkFree()
    {
        talking = false;
    }
}
