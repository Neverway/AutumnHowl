using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RingMinigameController : MonoBehaviour
{

    [SerializeField] private RawImage imageSword;
    [SerializeField] private GameObject canvas;
    [SerializeField] private TMP_Text text;
    private ringstate currentState = ringstate.notStarted;
    private spindir spin;
    private float swordAngle = 0f;
    private float spinSpeed = 130f;
    private float nearestAngleToSword;
    private float distanceFromNearestAngle;
    [SerializeField] public float goodAngle = 30f;
    [SerializeField] public float perfectAngle = 10f; //todo: procedurally draw angles
    [SerializeField] public bool stopByTapping = false;
    //the 4 blue images for the "Good" ring fill
    [SerializeField] public Image[] blueFills;
    //the 4 green images for the "Perfect" ring fill
    [SerializeField] public Image[] greenFills; 

    public static float north { get; private set; } = 0;
    public static float east { get; private set; } = 90;
    public static float south { get; private set; } = 180;
    public static float west { get; private set; } = 270;

    private Coroutine textCoroutine;
    private float timeToShowText = .75f;

    private bool bufferLeft = false;
    private bool bufferRight = false;

    public enum direction
    {
        north, south, west, east
    }

    private enum ringstate
    {
        notStarted, spinning, finish
    }
    private enum spindir
    {
        left, right
    }

    #region Monobehaviour

    // Start is called before the first frame update
    void Start ()
    {
        SetStartDirection (north);
        canvas.SetActive (false);
        text.SetText ("");
        SetupRingColors ();
    }

    // Update is called once per frame
    void Update ()
    {
        switch (currentState)
        {
            case ringstate.notStarted:
                {
                    if (bufferLeft || Input.GetKeyDown (KeyCode.Z))
                    {
                        currentState = ringstate.spinning;
                        spin = spindir.left;
                    }
                    if (bufferRight || Input.GetKeyDown (KeyCode.X))
                    {
                        currentState = ringstate.spinning;
                        spin = spindir.right;
                    }
                    break;
                }
            case ringstate.spinning:
                {
                    bufferRight = false;
                    bufferLeft = false;
                    canvas.SetActive (true);
                    DoSpinState ();
                    break;
                }
            case ringstate.finish:
                {
                    if (Input.GetKeyDown(KeyCode.Z))
                    {
                        bufferLeft = true;
                    }
                    if (Input.GetKeyDown (KeyCode.X))
                    {
                        bufferRight = true;
                    }
                    break;
                }
        }

        imageSword.transform.localRotation = Quaternion.Euler (new Vector3 (0, 0f, -swordAngle));
    }

    #endregion

    public void SetStartDirection (float _direction)
    {
        swordAngle = _direction;
    }

    private void DoSpinState ()
    {
        if (stopByTapping == false)
        {
            if (spin == spindir.left && Input.GetKey (KeyCode.Z) == false)
            {
                FinishSpin ();
                return;
            }
            if (spin == spindir.right && Input.GetKey (KeyCode.X) == false)
            {
                FinishSpin ();
                return;
            }
        }
        if (stopByTapping == true)
        {
            if (Input.GetKeyDown (KeyCode.X) || Input.GetKeyDown(KeyCode.Z))
            {
                FinishSpin ();
                return;
            }
        }
        if (spin == spindir.left)
        {
            swordAngle -= spinSpeed * Time.deltaTime;
        }
        if (spin == spindir.right)
        {
            swordAngle += spinSpeed * Time.deltaTime;
        }
        while (swordAngle > 360f)
        {
            swordAngle -= 360f;
        }
        while (swordAngle < 0f)
        {
            swordAngle += 360f;
        }
    }

    /// <summary>
    /// Ends sword spinning and calculates the direction it was pointing.
    /// </summary>
    private void FinishSpin ()
    {
        nearestAngleToSword = north;
        float test = Mathf.Abs (Mathf.DeltaAngle (swordAngle, north));
        distanceFromNearestAngle = test;
        test = Mathf.Abs (Mathf.DeltaAngle (swordAngle, east));
        if (test < distanceFromNearestAngle)
        {
            distanceFromNearestAngle = test;
            nearestAngleToSword = east;
        }
        test = Mathf.Abs (Mathf.DeltaAngle (swordAngle, south));
        if (test < distanceFromNearestAngle)
        {
            distanceFromNearestAngle = test;
            nearestAngleToSword = south;
        }
        test = Mathf.Abs(Mathf.DeltaAngle (swordAngle, west));
        if (test < distanceFromNearestAngle)
        {
            distanceFromNearestAngle = test;
            nearestAngleToSword = west;
        }

        if (distanceFromNearestAngle < perfectAngle)
        {
            ShowText ("Perfect!");
        }
        else if (distanceFromNearestAngle < goodAngle) {
            ShowText ("Good");
        }
        else
        {
            ShowText ("Miss!");
        }
        StartCoroutine (ResetRoutine());
    }

    private IEnumerator ResetRoutine ()
    {
        currentState = ringstate.finish;
        yield return new WaitForSeconds (0.3f);
        swordAngle = nearestAngleToSword;
        yield return new WaitForSeconds (0.1f);
        if (!bufferLeft && !bufferRight)
        {
            canvas.SetActive (false);
        }
        currentState = ringstate.notStarted;
    }

    private void ShowText(string _text)
    {
        if (textCoroutine != null) { StopCoroutine (textCoroutine); }
        textCoroutine = StartCoroutine (TextRoutine (_text));
    }

    private IEnumerator TextRoutine (string _text)
    {
        text.SetText (_text);
        yield return new WaitForSeconds (timeToShowText);
        text.SetText ("");
    }

    private void SetupRingColors ()
    {
        for (int i = 0; i < 4; i++)
        {
            blueFills[i].gameObject.transform.localRotation = Quaternion.Euler (0, 0, goodAngle + (90 * i));
            blueFills[i].fillAmount = ((goodAngle * 2f)) / 360;
            greenFills[i].gameObject.transform.localRotation = Quaternion.Euler (0, 0, perfectAngle + (90 * i));
            greenFills[i].fillAmount = ((perfectAngle * 2f)) / 360;
        }
    }
}
