using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GUID_Generator : MonoBehaviour
{
    public static GUID_Generator _instance;

    public static GUID_Generator GetInstance() => _instance;

    [Header("User Settings")]
    [SerializeField] string m_userGUID;

    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    private void Start()
    {
        CreateNewUserGUID();
    }

    public string GetUserGUID()
    {
        return m_userGUID;
    }

    public string CreateNewUserGUID()
    {
        m_userGUID = Guid.NewGuid().ToString();
        return m_userGUID;
       // Debug.Log($"Created neW user GUID : {m_userGUID}");
    }
}
