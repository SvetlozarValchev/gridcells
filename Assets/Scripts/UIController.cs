using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIController : MonoBehaviour {
    public UITooltip tooltip;

    // Static singleton property
    public static UIController Instance { get; private set; }

    void Awake()
    {
        // Save a reference to the AudioHandler component as our singleton instance
        Instance = this;
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
