using System;
using UnityEngine;

public class TransitionPoint:MonoBehaviour
{
    public enum TransitionType
    {
        SameScene,DifferentScene
    }
    [Header("Transition Info")] 
    public string sceneName;

    public TransitionType transitionType;
    public TransitionDestination.DestinationTag destinationTag;

    private bool canTrans;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && canTrans)
        {
            SceneController.Instance.TransitionToDestination(this);
            //TODO:SceneController 传送
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.layer.Equals(LayerUtils.Player))
        {
            canTrans = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer.Equals(LayerUtils.Player))
        {
            canTrans = false;
        }
    }
}