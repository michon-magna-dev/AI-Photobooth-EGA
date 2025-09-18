//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.IO.Ports;
//using System.Text;
//using System.Threading;
//using UnityEngine;
//using Random = System.Random;

//[RequireComponent(typeof(AudioSource))]
//public class ScaleFromPump : MonoBehaviour
//{
//    public GameObject BalloonGameObject;

//    public List<Texture2D> m_Sprite = new List<Texture2D>();
//    private bool play = false;

//    [Header("Inflate Sound ")]
//    [SerializeField] AudioSource m_AudioSource;
//    [SerializeField] AudioClip[] m_inflateSounds;
//    [SerializeField] AudioClip m_popSound;

//    [Header("Balloon ")]
//    [SerializeField] GameObject balloonPrefab;
//    [SerializeField] GameObject balloonParent;
//    [SerializeField] float scaleLimit = 100f;
//    [SerializeField] float timeToReleaseMS = 1f;
//    [Header("Color ")]
//    [SerializeField] private Color[] m_colorsAvailable = { Color.green, Color.blue, Color.red, Color.yellow };
//    [SerializeField] private Color m_currentColor;


//    [Header("Speed ")]
//    [SerializeField] float scaleUpSpeed = 5;
//    [SerializeField] float scaleDownSpeed = 0.2f;

//    [Header("Port ")]
//    [SerializeField] SerialPort mySerialPort;
//    [SerializeField] Thread dataReceiveThread;
//    [SerializeField] bool canReceive = true;
//    [SerializeField] byte[] datasBytes;
//    [SerializeField] bool receive = false;

//    private bool startChar = false;
//    private Vector3 m_initScale;

//    private bool small = true;
//    private bool imagePlay = false;

//    public ShakeCamera m_cameraShake;

//    void Start()
//    {
//        m_initScale = BalloonGameObject.transform.localScale;
//        canReceive = true;
//        StartCoroutine(ResetStartend());
//        InitSerialPort();
//        ResetColor();
//    }

//    void LateUpdate()
//    {
//        ScaleByPump();
//        ResetToZero();


//#if UNITY_EDITOR

//    if (Input.GetKeyDown(KeyCode.Space))
//    {
//        receive = true;
//    }
//#endif
//    }

//    void ScaleByPump()
//    {
//        if (canReceive && !play && receive)
//        {
//            StopAllCoroutines();
//            receive = false;
//            BalloonGameObject.transform.localScale += new Vector3(scaleUpSpeed, scaleUpSpeed, scaleUpSpeed);
//            PlayInflateSound();
//            if (BalloonGameObject.transform.localScale.x >= scaleLimit)
//            {
//                PlayPopSound();
//                BalloonGameObject.transform.localScale = m_initScale;
//                m_cameraShake.enabled = true;
//                canReceive = false;
//                small = true;
//            }
//            StartCoroutine(StartReleaseTime());
//        }
//    }

//    void InitSerialPort()
//    {
//        try
//        {
//            mySerialPort = new SerialPort(ToolReadInConfig.ReadSerialCount().ToString(),
//                9600, Parity.None, 8);
//            mySerialPort.Open();
//            if (!mySerialPort.IsOpen)
//            {
//                Debug.Log("Port opening failed");
//                return;
//            }
//            else
//            {
//                Debug.Log($"Port Name:{mySerialPort.PortName}");
//                Debug.Log($"Port SerialCount:{ToolReadInConfig.ReadSerialCount().ToString()}");
//                Debug.Log("Port opened successfully");
//                dataReceiveThread = new Thread(ReceiveData);//该线程用于接收串口数据 
//                dataReceiveThread.Start();
//            }
//        }
//        catch (Exception e)
//        {
//            mySerialPort.Dispose();
//            Debug.LogError(e);
//            throw;
//        }
//    }

//    void ResetToZero()
//    {
//        if (small)
//        {
//            Vector3 v3 = BalloonGameObject.transform.localScale - new Vector3(scaleDownSpeed, scaleDownSpeed, scaleDownSpeed);

//            if (v3.x <= 0)
//            {
//                BalloonGameObject.transform.localScale = m_initScale;
//            }

//            if (v3.x > 0.2)
//            {
//                BalloonGameObject.transform.localScale = v3;
//            }
//        }
//        canReceive = true;
//    }

//    void PlayInflateSound()
//    {
//        if (m_AudioSource.isPlaying)
//            return;
//        System.Random random = new System.Random();
//        m_AudioSource.clip = m_inflateSounds[random.Next(m_inflateSounds.Length)];
//        m_AudioSource.Play();
//    }

//    void PlayPopSound()
//    {
//        m_AudioSource.clip = m_popSound;
//        m_AudioSource.Play();
//    }

//    private IEnumerator StartReleaseTime()
//    {
//        yield return new WaitForSeconds(timeToReleaseMS);
//        if (BalloonGameObject.transform.localScale.x < 0.1)
//        {
//            StopAllCoroutines();
//            yield break;
//        }
//        else
//        {
//            SpawnBalloon();
//            ResetToZero();
//        }
//    }

//    private void SpawnBalloon()
//    {
//        GameObject ins = Instantiate(balloonPrefab, this.transform.position, this.transform.rotation, balloonParent.transform);
//        ins.transform.localScale = BalloonGameObject.transform.localScale;
//        ins.GetComponent<Renderer>().material.SetColor("_Color", m_currentColor);
//        ResetColor();
//        BalloonGameObject.transform.localScale = m_initScale;

//    }

//    void ResetColor()
//    {
//        System.Random rand = new System.Random();
//        m_currentColor = m_colorsAvailable[rand.Next(0, m_colorsAvailable.Length)];
//        BalloonGameObject.transform.GetChild(0).GetComponent<Renderer>().material.SetColor("_Color", m_currentColor);
//    }

//    private IEnumerator ResetStartend()
//    {
//        yield return new WaitForSeconds(3f);
//        startChar = true;
//    }

//    void ReceiveData()
//    {
//        int bytesToRead = 0;
//        while (true)
//        {
//            if (mySerialPort != null && mySerialPort.IsOpen)
//            {
//                try
//                {
//                    datasBytes = new byte[1024];
//                    bytesToRead = mySerialPort.Read(datasBytes, 0, datasBytes.Length);
//                    if (bytesToRead == 0)
//                    {
//                        continue;
//                    }
//                    else
//                    {
//                        if (startChar && canReceive)
//                        {
//                            receive = true;
//                        }
//                        else
//                        {
//                            receive = false;
//                        }

//                        string strbytes = Encoding.Default.GetString(datasBytes);
//                        Debug.Log($"Received Data:{strbytes}");
//                    }
//                }
//                catch (Exception e)
//                {
//                    Debug.Log(e.Message);
//                }
//            }
//            Thread.Sleep(100);
//        }
//    }

//    void ResetReceive()
//    {
//        canReceive = true;
//    }

//    void ClosePort()
//    {
//        if (mySerialPort != null && mySerialPort.IsOpen)
//        {
//            mySerialPort.Close();//Close the serial port
//            mySerialPort.Dispose();//Release the serial port from the memory
//        }
//    }

//    void OnDestroy()
//    {
//        ClosePort();
//    }

//    void OnApplicationPause()
//    {
//        ClosePort();
//    }

//    void OnDisable()
//    {
//        ClosePort();
//    }

//}