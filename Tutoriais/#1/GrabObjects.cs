﻿using UnityEngine;

public class GrabObjects : MonoBehaviour
{
    public string[] objectTags;
    [Tooltip("Forte to apply in object")]
    public float forceGrab = 5;
    public float maxDist;
    [Tooltip("Put all layers, the player layer not!")]
    public LayerMask acceptLayers = 0;


    private GameObject grabedObj;
    private Vector2 rigSaveGrabed;
    

    //in next videos i'll improve this script!

    void Update()
    {
        Transform cam = Camera.main.transform;



        RaycastHit hit = new RaycastHit();
        if (Physics.Raycast(cam.position, cam.forward, out hit, maxDist, acceptLayers, QueryTriggerInteraction.Ignore))
        {
            foreach (string tag in objectTags)
            {
                if (hit.transform.tag == tag)
                {
                    if (Input.GetMouseButtonDown(0))
                        grabedObj = hit.transform.gameObject;
                    
                }
            }
            Debug.DrawLine(cam.position, hit.point, Color.blue);
        }


        if (grabedObj != null)
        {
            if (!grabedObj.GetComponent<Rigidbody>())
            {
                Debug.LogError("Your object to grab NEEDS RigidBory");
                return;
            }
            Rigidbody objRig = grabedObj.GetComponent<Rigidbody>();
            Vector3 posGrab = cam.position + cam.forward * maxDist;
            float dist = Vector3.Distance(grabedObj.transform.position, posGrab);
            float calc = forceGrab * dist * 6 * Time.deltaTime;

            if (rigSaveGrabed == Vector2.zero)
                rigSaveGrabed = new Vector2(objRig.drag, objRig.angularDrag);
            objRig.drag = 2.5f;
            objRig.angularDrag = 2.5f;
            objRig.AddForce(-(grabedObj.transform.position - posGrab).normalized * calc, ForceMode.Impulse);

            if (Input.GetMouseButtonUp(0))
                UngrabObject();

            if (objRig.velocity.magnitude >= 20)
                UngrabObject();

            if (dist >= 10)
                UngrabObject();
        }


    }

    void UngrabObject()
    {
        Rigidbody objRig = grabedObj.GetComponent<Rigidbody>();
        objRig.drag = rigSaveGrabed.x;
        objRig.angularDrag = rigSaveGrabed.y;
        rigSaveGrabed = Vector2.zero;

        grabedObj = null;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Transform cam = Camera.main.transform;
        if (!Physics.Raycast(cam.position, cam.forward, maxDist))
        {
            Gizmos.DrawLine(cam.position, cam.position + cam.forward * maxDist);
        }
    }
}
