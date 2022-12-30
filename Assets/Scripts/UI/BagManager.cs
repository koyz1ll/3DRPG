using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BagManager : Singleton<BagManager>
{
    public GameObject bagCanvasPrefab;

    private GameObject bag;
    protected override void Awake()
    {
        base.Awake();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            if (!bag)
            {
                bag = Instantiate(bagCanvasPrefab);
                bag.SetActive(false);
            }
            bag.SetActive(!bag.activeSelf);
        }
    }
}
