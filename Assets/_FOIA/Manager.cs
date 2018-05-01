using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.IO;

public class Manager : MonoBehaviour {

    public GameObject poros;
    public bool clockwise = true;
    public bool jalanRotate = false;
    public float speed;
    public GameObject processing;

    public static Manager Instance { get; private set; }

    [Header("panelInfo")]
    public GameObject panelDetail;
    public GameObject detail;
    public Text txtJudulDetail;
    public Image img_detail;
    public Sprite sprite_akomodasi;
    public Sprite sprite_festival;
    public Sprite sprite_kuliner;
    public Sprite sprite_souvenir;
    public GameObject bt_closePoros;

    public GameObject bt_capture;
    public NativeShare ShareManager;
    private string pathImageTemporary;
    public GameObject bt_share;
    public GameObject bt_closeinfoDetail;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            if (Instance == null)
            {
                Debug.Log("error when trying to create singleton");
            }
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
    GameObject minion = null;
    void Start()
    {
        minion = GameObject.Find("InstantTrackable").transform.Find("Minion").gameObject;
        
    }

    void Update()
    {
        if (jalanRotate)
        {
            if (clockwise)
            {
                poros.transform.Rotate(Vector3.up * Time.deltaTime *speed);
            }
            else
            {
                poros.transform.Rotate(Vector3.down * Time.deltaTime * speed);
            }
        }
    }

    public void anim(bool set)
    {
        jalanRotate = true;
        clockwise = set;
    }

    public void stopAnim()
    {
        jalanRotate = false;
    }
   
    public void showPanelPoros(bool show)
    {
        if (show)
        {
            poros.SetActive(true);
            poros.transform.DOScale(2, 0.5f);
            jalanRotate = true;
            bt_closePoros.SetActive(true);
            minion.transform.localScale = Vector3.zero;
        }
        else
        {
            poros.transform.DOScale(new Vector3(2, 0, 2), 0.5f).OnComplete(() =>
            {
                poros.SetActive(false);
                jalanRotate = false;
                bt_closePoros.SetActive(false);
                bt_capture.transform.localScale = Vector3.one;
            });
            minion.transform.localScale = new Vector3(5, 5, 5);
        }
    }

    public void showContent(string konten)
    {
        //print(konten);
        txtJudulDetail.text = konten;
        if (konten == "kuliner")
        {
            img_detail.sprite = sprite_kuliner;
        }
        else if (konten == "souvenir")
        {
            img_detail.sprite = sprite_souvenir;
        }
        else if (konten == "festival")
        {
            img_detail.sprite = sprite_festival;
        }
        else if (konten == "akomodasi")
        {
            img_detail.sprite = sprite_akomodasi;
        }

        panelDetail.SetActive(true);
        detail.transform.DOScale(1, 0.5f);
        jalanRotate = false;


    }

    public void hideKonten()
    {
        detail.transform.DOScale(new Vector3(0, 1, 1), 0.5f).OnComplete(() =>
        {
            panelDetail.SetActive(false);
            jalanRotate = true;
        });
    }

    //share
    public void foto()
    {
        bt_capture.SetActive(false);
        bt_share.SetActive(false);
        bt_closeinfoDetail.SetActive(false);
        StartCoroutine(TakePicture());
    }
    IEnumerator TakePicture()
    {
        Texture2D tex = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
        yield return new WaitForEndOfFrame();
        tex.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        tex.Apply();

        byte[] screenShootByte = tex.EncodeToPNG();

        //cetak
        //INITIAL SETUP
        pathImageTemporary = Application.persistentDataPath + "Screenshoot.png";

        //jika ada nama file yang sama di delete dahulu
        if (File.Exists(pathImageTemporary)) File.Delete(pathImageTemporary);
        System.IO.File.WriteAllBytes(pathImageTemporary, screenShootByte);

        Share();
    }

    //share foto
    public void Share()
    {
        bt_capture.SetActive(true);
        bt_share.SetActive(true);
        bt_closeinfoDetail.SetActive(true);
        ShareManager.shareScreenShoot("information", pathImageTemporary);
        
    }
}
