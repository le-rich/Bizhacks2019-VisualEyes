using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopperManager : MonoBehaviour
{
    public bool bobbing;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (bobbing)
        {
            transform.localScale = new Vector3(1f + (Mathf.Sin(Time.time * 2) / 4), 1f + (Mathf.Sin(Time.time * 2) / 4), 1f);
        }
    }

    public void setBob(bool state)
    {
        bobbing = state;
    }
    
}
