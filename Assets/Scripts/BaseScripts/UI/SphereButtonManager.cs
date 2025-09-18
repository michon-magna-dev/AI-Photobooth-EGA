using UnityEngine;

public class SphereButtonManager : MonoBehaviour
{
    public static SphereButtonManager Instance => _instance;
    private static SphereButtonManager _instance;
    [SerializeField] RotateSphere sphereRotator;

    private void Awake()
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
        sphereRotator = GetComponent<RotateSphere>();
    }

    public void ButtonSelected(int p_selectedObjIndex)
    {
        EnableSphereRotation(false);
        //UDPSend.GetInstance().SendUDPMsg(p_selectedObjIndex.ToString());
        Debug.Log($"Selected: INDEX: {p_selectedObjIndex}");
    }

    public void EnableSphereRotation(bool p_enable)
    {
        sphereRotator.enabled = p_enable;
    }

    public void ButtonSelected(string p_buttonTopicName)
    {
        //do stuff here

    }

}