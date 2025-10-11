using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;
using UnityEngine.UI;

public class RingMinigameController : MonoBehaviour
{

    [SerializeField] private RawImage imageSword;
    [SerializeField] private GameObject canvas;
    private ringstate currentState = ringstate.notStarted;
    private spindir spin;
    private float swordAngle = 0f;
    private float spinSpeed = 130f;
    private float nearestAngleToSword;
    private float distanceFromNearestAngle;
    [SerializeField] public float goodAngle = 30f;
    [SerializeField] public float perfectAngle = 10f; //todo: procedurally draw angles
    [SerializeField] public bool stopByTapping = false;

    public static float north { get; private set; } = 0;
    public static float east { get; private set; } = 90;
    public static float south { get; private set; } = 180;
    public static float west { get; private set; } = 270;

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

    public void SetStartDirection (float _direction)
    {
        swordAngle = _direction;
    }


    // Start is called before the first frame update
    void Start ()
    {
        SetStartDirection (north);
        canvas.SetActive (false);
    }

    // Update is called once per frame
    void Update ()
    {
        switch (currentState)
        {
            case ringstate.notStarted:
                {
                    if (Input.GetKeyDown (KeyCode.Z))
                    {
                        currentState = ringstate.spinning;
                        spin = spindir.left;
                    }
                    if (Input.GetKeyDown (KeyCode.X))
                    {
                        currentState = ringstate.spinning;
                        spin = spindir.right;
                    }
                    break;
                }
            case ringstate.spinning:
                {
                    canvas.SetActive (true);
                    DoSpinState ();
                    break;
                }
            case ringstate.finish:
                {
                    //todo: hide minigame after like 0.2 seconds, and swing actual sword.
                    break;
                }
        }

        imageSword.transform.localRotation = Quaternion.Euler (new Vector3 (0, 0f, swordAngle));
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
            if (spin == spindir.left && Input.GetKeyDown(KeyCode.Z))
            {
                FinishSpin ();
                return;
            }
            if (spin == spindir.right && Input.GetKeyDown (KeyCode.X))
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
            print ("Perfect!");
        }
        else if (distanceFromNearestAngle < goodAngle) {
            print ("Good.");
        }
        else
        {
            print ("Miss!");
        }
        StartCoroutine (ResetRoutine());
    }

    private IEnumerator ResetRoutine ()
    {
        currentState = ringstate.finish;
        yield return new WaitForSeconds (0.3f);
        swordAngle = nearestAngleToSword;
        currentState = ringstate.notStarted;
        yield return new WaitForSeconds (0.1f);
        canvas.SetActive (false);
    }
}
