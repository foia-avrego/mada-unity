using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mada : MonoBehaviour {
    public static string FLASK_SERVER = "http://156.67.217.18:5000/";
    public static string SERVER = "http://156.67.217.18/"; // 202.51.126.3
                                                           // Use this for initialization

    InstantTrackerSample iTracker = null;

    void Start () {

        iTracker = FindObjectOfType<InstantTrackerSample>() as InstantTrackerSample;
        StartCoroutine(InitInstantTracker());
        this.ShowObject(GameObject.Find("Message"), false);
        this.ShowObject(GameObject.Find("Capture"), false);
        /*
        WebCamTexture webcamTexture = new WebCamTexture();
        webcamTexture.requestedWidth = 850;
        webcamTexture.requestedHeight = 480;
        Renderer renderer = GetComponent<Renderer>();
        renderer.material.mainTexture = webcamTexture;
        webcamTexture.Play();
        */
        Debug.Log(SystemInfo.deviceUniqueIdentifier);
        StartCoroutine(AutoRegisterDevice());
    }

    IEnumerator InitInstantTracker()
    {
        yield return new WaitForSeconds(2);
        Debug.Log("Start Instant racker");
        iTracker.OnClickStart();
    }

    public void PutImage()
    {
        StartCoroutine(IEPutImage());
    }

    IEnumerator IEPutImage()
    {
        /*
        Rect rect = new Rect(0, 0, Screen.width, Screen.height);
        RenderTexture renderTexture = new RenderTexture(Screen.width, Screen.height, 24);
        Texture2D screenShot = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
        Camera camera = GameObject.Find("Main Camera").GetComponent<Camera>();
        camera.targetTexture = renderTexture;
        camera.Render();
        RenderTexture.active = renderTexture;
        screenShot.ReadPixels(rect, 0, 0);
        byte[] a = screenShot.EncodeToJPG(50);
        */

        Texture2D screenShot = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
        yield return new WaitForEndOfFrame();
        screenShot.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        screenShot.Apply();

        byte[] a = screenShot.EncodeToJPG(50);
        //var f = System.IO.File.Create("aa.jpg");
        //f.Write(a, 0, a.Length);
        //f.Close();

        //camera.targetTexture = null;
        //RenderTexture.active = null;
        //Texture2D tex = (Texture2D)GameObject.Find("CamTexture").GetComponent<MeshRenderer>().material.GetTexture();
        //Texture2D tex = new Texture2D(634, 310, TextureFormat.RGB24, true);
        //tex.ReadPixels(new Rect(0, 0, 634, 310), 0, 0);
        //tex.LoadRawTextureData(tex.GetRawTextureData());
        //tex.Apply();
        //GetComponent<Renderer>().material.mainTexture.
        if (screenShot != null)
        {
            WWWForm wf = new WWWForm();
            wf.AddField("sn", SystemInfo.deviceUniqueIdentifier);
            wf.AddBinaryData("userfile", a, "obj.jpg", "image/jpg");
            WWW www = new WWW(Mada.FLASK_SERVER + "obj_recog", wf);
            ShowObject(GameObject.Find("Capture"), false);

            this.ShowMessage("Recognizing...", 0, null);

            Manager.Instance.processing.SetActive(true);

            yield return www;
            this.ShowObject(GameObject.Find("Message"), false);
            Manager.Instance.processing.SetActive(false);

            if (string.IsNullOrEmpty(www.error))
            {
                SimpleJSON.JSONNode data = SimpleJSON.JSON.Parse(www.text);
                string name = data["result"].Value.ToString();
                Debug.Log(name);
                yield return StartCoroutine(this.ObjRecogActivity(name));
                this.ShowMessage(name, 3, GameObject.Find("Capture"));
                //this.ShowTextBox("Sending Image", www.text, 3f);
                //currentImageUploaded = SimpleJSON.JSON.Parse(www.text);
                //StartCoroutine(IERecognize());
            }
            else
            {
                Debug.Log(www.url);
                Debug.Log(www.error);
                Debug.Log(www.text);

                //demo
                Manager.Instance.showPanelPoros(true);
                
                //ShowObject(GameObject.Find("Loading"), false);
                //this.ShowTextBox("Sending Image", "Error :\n" + www.error, 3f);
                yield return new WaitForSeconds(3f);
            }
        }

        
    }

    void ShowMessage(string msg, int time, GameObject callBackObj)
    {
        GameObject.Find("Message").transform.Find("Text").GetComponent<UnityEngine.UI.Text>().text = msg;
        StartCoroutine(this._ShowMessage(time, callBackObj));
    }

    IEnumerator _ShowMessage(int time, GameObject callBackObj)
    {
        
        this.ShowObject(GameObject.Find("Message"), true);
        if (time > 0)
        {
            yield return new WaitForSeconds(time);
            this.ShowObject(GameObject.Find("Message"), false);
            this.ShowObject(callBackObj, true);
        }
    }

    void ShowObject(GameObject obj, bool state)
    {
        if (!state)
            obj.transform.localScale = Vector3.zero;
        else
            obj.transform.localScale = Vector3.one;
    }

    
    IEnumerator AutoRegisterDevice()
    {
        WWWForm wf = new WWWForm();
        wf.AddField("serial_number", SystemInfo.deviceUniqueIdentifier);
        WWW w = new WWW(Mada.SERVER + "aws/sn_check", wf);
        yield return w;
        if (string.IsNullOrEmpty(w.error))
        {
            Debug.Log(w.text);
            //this.ShowTextBox("Registering Device Check", w.text, 3f);
            SimpleJSON.JSONNode result = SimpleJSON.JSON.Parse(w.text);

            this.ShowObject(GameObject.Find("Capture"), true);
        }
        else
        {
            this.ShowMessage("Registering Device Check\nError :\n" + w.error, 3, null);
            yield return new WaitForSeconds(3f);
            this.ShowMessage("Registering Device Check\nRetrying...", 1, null);
            StartCoroutine(AutoRegisterDevice());
        }
    }

    IEnumerator ObjRecogActivity(string name)
    {
        WWWForm wf = new WWWForm();
        wf.AddField("serial_number", SystemInfo.deviceUniqueIdentifier);
        wf.AddField("name", name);
        WWW w = new WWW(Mada.SERVER + "aws/objrecog_activity", wf);
        yield return w;
        if (string.IsNullOrEmpty(w.error))
        {
            Debug.Log(w.text);
            
        }
        else
        {
            Debug.Log(w.error);
        }
    }
   
}
