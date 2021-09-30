using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using ZXing;

/// <summary>
/// Reads QRCode from standard camera (It does not use the ARFoundation ARKit camera)
/// </summary>
public class ReadQRCode : MonoBehaviour
{
    public Camera ARCamera;
    public Camera QRCamera;
    public GameObject scanner;
    public Text txtQRCodeResult;
    public GameObject infoPanel;

    private bool startScan;
    
    /// <summary>
    /// Match the Unity camera used for QR Code scanning with the one used by ARFoundation 
    /// </summary>
    public void OnPreRender()
    {
        QRCamera.projectionMatrix = ARCamera.projectionMatrix;
        QRCamera.fieldOfView = ARCamera.fieldOfView;
        QRCamera.transform.localPosition = Vector3.zero;
        QRCamera.transform.localRotation = Quaternion.Euler(Vector3.zero);
    }
    public void Start()
    {
        QRCamera.enabled = false;
        Application.runInBackground = true;
        startScan = false;
    }
    private void Update()
    {
        // Scan QRCode 
        if (startScan)
        {
            QRCamera.enabled = true;
            scanner.GetComponent<Animator>().enabled = true;
        }
    }
    /// <summary>
    /// Scan QRCode using ZXing
    /// </summary>
    private void OnPostRender()
    {
        if (startScan)
        {
            //Create a new texture with the width and height of the screen
            Texture2D QRTexture = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
            //Read the pixels in the Rect starting at 0,0 and ending at the screen's width and height
            QRTexture.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0, false);
            QRTexture.Apply();

            // Scan Barcode using ZXing 
            try
            {
                IBarcodeReader barcodeReader = new BarcodeReader();
                // decode the current frame from QRCamera
                var result = barcodeReader.Decode(QRTexture.GetPixels32(), QRTexture.width, QRTexture.height);
                if (result != null)
                {
                    infoPanel.SetActive(true);
                    txtQRCodeResult.text = result.Text;
                    Debug.Log("QR Text:" + result.Text);
                    scanner.GetComponent<Animator>().enabled = false;
                    startScan = false;
                }
            }
            catch (Exception e)
            {
                Debug.Log("Error when capturing camera texture: " + e);
            }
            // No need for the QR camera
            QRCamera.enabled = false;
        }
    }

    public void StartScan() {
        if(!startScan){
            startScan = true;
            txtQRCodeResult.text = "";
            infoPanel.SetActive(false);
        }
    }
}
