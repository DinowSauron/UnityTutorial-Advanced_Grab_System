using UnityEngine;

public class GrabObjects : MonoBehaviour
{
    public string[] objectTags;
    [Tooltip("Force to apply in object")]
    public float forceGrab = 5;
    public float maxDist;
    [Tooltip("Put all layers, the player layer not!")]
    public LayerMask acceptLayers = 0;


    [HideInInspector]
    public GameObject grabedObj = null;
    private bool holding;
    [HideInInspector]
    public bool possibleGrab = false;
    private Vector2 rigSaveGrabed;
    private float objDistance;

    //in next videos i'll improve this script!

    void Update()
    {
        Transform cam = Camera.main.transform;
        RaycastHit hit = new RaycastHit();

        if (Physics.Raycast(cam.position, cam.forward, out hit, maxDist, acceptLayers, QueryTriggerInteraction.Ignore))
        {
            Debug.DrawLine(cam.position, hit.point, Color.blue);

            foreach (string tag in objectTags)
            {
                possibleGrab = false;
                if (hit.transform.tag == tag)
                {
                    if (Input.GetMouseButtonDown(0))
                    {
                        holding = true;
                    }
                    possibleGrab = true;
                }
            }
        }
        else
        {
            possibleGrab = false;
        }



        if (grabedObj != null)
        {
            if (!grabedObj.GetComponent<Rigidbody>())
            {
                Debug.LogError("Your object NEED RigidBody Component! | Coloque um Rigidbody no objeto!");
                return;
            }
            
            Rigidbody objRig = grabedObj.GetComponent<Rigidbody>();
            Vector3 posGrab = cam.position + cam.forward * objDistance;
            float dist = Vector3.Distance(grabedObj.transform.position, posGrab);
            float calc = forceGrab * dist * 6 * Time.deltaTime;

            if (rigSaveGrabed == Vector2.zero)
                rigSaveGrabed = new Vector2(objRig.drag, objRig.angularDrag);
            objRig.drag = 2.5f;
            objRig.angularDrag = 2.5f;

            objRig.AddForce(-(grabedObj.transform.position - posGrab).normalized * calc, ForceMode.Impulse);
            RotateObject(cam, objRig);
            ChangeDistance(cam, objRig);

            if (Input.GetMouseButtonDown(0))
                holding = false;

            if (objRig.velocity.magnitude >= 20)
                UngrabObject();

            if (dist >= 10)
                UngrabObject();
        }


        if(holding && grabedObj == null){
            grabedObj = hit.transform.gameObject;
            objDistance = Vector3.Distance(cam.position, hit.transform.gameObject.transform.position);
        }
        if(!holding && grabedObj != null){
            UngrabObject();
        }
    }

    void ChangeDistance(Transform cam, Rigidbody objRig)
    {
        float distChange = Input.GetAxis("Mouse ScrollWheel");

        if (objDistance + distChange > maxDist)
        {
            objDistance = maxDist;
            return;
        }
        if (objDistance + distChange < .5f) {
            objDistance = .5f;
            return;
        }
        objDistance += distChange;
    }

    void RotateObject(Transform cam, Rigidbody objRig)
    {
        KeyCode rotKey = KeyCode.R; // R key to rotate Object

        if (Input.GetKey(rotKey)) 
        {
            Vector3 axis = -cam.forward * Input.GetAxis("Mouse X") + cam.right * Input.GetAxis("Mouse Y");
            objRig.AddTorque(axis * forceGrab * Time.deltaTime, ForceMode.Impulse);
        }
        /*
            Pt-Br:
            -Aqui é por sua conta, como este tutorial é para qualquer tipo de jogador, não da para eu fazer um sistema que funcione bem em todos os jogadores, então como você é um programador, PROGRAME!
            -Basicamente nesta area precisa de alguma forma interromper os movimentos do seu jogador de olhar para os lados, boa sorte :D

            En-Us:
            -Here it is up to you, as this tutorial is for any type of player, I cannot make a system that works well for all players, so as you are a programmer, PROGRAM!
            -Basically in this area you need to somehow stop your player's movements from looking sideways, good luck: D
        */


        // Se você não estiver usando o character controller do standard assets, isto não funcionará!
        // If you are not using the standard assets character controller, this will not work!
        /*
        UnityStandardAssets.Characters.FirstPerson.FirstPersonController scriptPlayer = null;

        scriptPlayer = GameObject.FindGameObjectWithTag("Player").GetComponent<UnityStandardAssets.Characters.FirstPerson.FirstPersonController>();
        if (Input.GetKey(rotKey) )//ainda há um bug por aqui...
            scriptPlayer.m_MouseLook.XSensitivity = scriptPlayer.m_MouseLook.YSensitivity = 0;
        else
            scriptPlayer.m_MouseLook.XSensitivity = scriptPlayer.m_MouseLook.YSensitivity = 2;
        */
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
