using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class GrabUI : MonoBehaviour
{
    [Tooltip("Sprite that will always be active")]
    public Sprite DefaultAim;  //normal state
    [Tooltip("Sprite that activates when it can pick up an object")]
    public Sprite passiveAim;  //normal state
    [Tooltip("Sprite that activates when holding an object")]
    public Sprite grabAim;  //grab state

    private Image img;
    private GrabObjects grabSys;

    void Start()
    {
        img = GetComponent<Image>();
        img.sprite = null;
        grabSys = Camera.main.GetComponent<GrabObjects>();
    }
    
    void Update()
    {

        if (grabSys.grabedObj != null)
            img.sprite = grabAim;
        else
        {
            if (grabSys.possibleGrab)
                img.sprite = passiveAim;
            else
                img.sprite = DefaultAim;
        }
    }
}
