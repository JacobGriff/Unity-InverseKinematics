using UnityEngine;

public class armControl : MonoBehaviour
{
    GameObject startObj;
    GameObject endObj;
    GameObject arm;
    GameObject arm2;
    GameObject foot;
    GameObject nextFootPos;

    //Calculation vars
    public float y = 5;
    public float z = 5;
    public float length;
    public float iDistance;
    public float distance;
    public float angle;
    public Vector2 jointPos;
    public Vector3 jointPos3;

    Vector3 footPos;
    Vector3 localFootResetPos;

    bool move = false;

    public float moveSpeed = 5;

    // Start is called before the first frame update
    void Start()
    {

        //Get objs
        startObj = transform.Find("Start").gameObject;
        endObj = transform.Find("End").gameObject;
        arm = transform.Find("Arm1").gameObject;
        arm2 = transform.Find("Arm2").gameObject;
        foot = transform.Find("Foot").gameObject;
        nextFootPos = transform.Find("NextFootPos").gameObject;

        footPos = foot.transform.position;
        localFootResetPos = nextFootPos.transform.localPosition;

        if (arm.transform.lossyScale.z != length)
        {
            float scaleDif = length - arm.transform.lossyScale.z;
            Vector3 scale = new Vector3(0, 0, scaleDif);
            arm.transform.localScale += scale;
            arm2.transform.localScale += scale;
        }

        SetFoot(true);
    }

    // Update is called once per frame
    void Update()
    {
        AutoMove();
    }

    void SetFoot(bool instant)
    {
        RaycastHit hit;
        LayerMask layerMask = LayerMask.GetMask("Ground");
        if (Physics.Raycast(nextFootPos.transform.position, Vector3.down, out hit, Mathf.Infinity, layerMask))
        {
            localFootResetPos = nextFootPos.transform.localPosition;
            localFootResetPos.y -= hit.distance;
            Vector3 pos = nextFootPos.transform.position;
            pos.y += localFootResetPos.y;
        }

        if (!instant) foot.transform.localPosition = Vector3.MoveTowards(foot.transform.localPosition, localFootResetPos, Time.deltaTime * moveSpeed);
        else foot.transform.localPosition = localFootResetPos;
        footPos = foot.transform.position;
    }

    void AutoMove()
    {
        if (arm.transform.lossyScale.z != length)
        {
            float scaleDif = length - arm.transform.lossyScale.z;
            Vector3 scale = new Vector3(0, 0, scaleDif);
            arm.transform.localScale += scale;
            arm2.transform.localScale += scale;
        }

        float maxDist = length * 2 - 0.0001f;
        float fSize = foot.transform.lossyScale.z;
        Vector3 jointP = footPos;
        jointP.y += fSize / 2;

        //Get distance between the joint placement and the start
        float dist = Vector3.Distance(startObj.transform.position, jointP);

        if (dist > maxDist)
        {
            move = true;
        }

        if (move)
        {
            SetFoot(false);
            if (Vector3.Distance(foot.transform.localPosition, localFootResetPos) < 0.01) move = false;
        }
        else
        {
            foot.transform.position = footPos;
        }

        endObj.transform.position = jointP;

        IkTo(startObj.transform.position, endObj.transform.position);
    }

    void IkTo(Vector3 start, Vector3 target)
    {

        jointPos3 = MidPointCalc(start, target, length);
        AlignPoints(jointPos3);
    }

    void AlignPoints(Vector3 joint)
    {
        arm.transform.position = startObj.transform.position;
        arm.transform.LookAt(joint);
        arm.transform.position += arm.transform.forward * length / 2;

        arm2.transform.position = joint;
        arm2.transform.LookAt(endObj.transform);
        arm2.transform.position += arm2.transform.forward * (length / 2);
    }

    Vector3 MidPointCalc(Vector3 start, Vector3 end, float length)
    {
        Vector3 groundedPos = new Vector3(end.x, start.y, end.z);
        float groundDist = Vector3.Distance(start, groundedPos);
        y = end.y - start.y;

        iDistance = Vector3.Distance(start, end);

        if (iDistance > length * 2)
        {
            startObj.transform.LookAt(endObj.transform);
            end = startObj.transform.position + startObj.transform.forward * (length * 2 - 0.0001f);
            endObj.transform.position = end;
            Debug.Log("Too far: " + end + Vector3.Distance(start, end));

            groundedPos = new Vector3(end.x, start.y, end.z);
            groundDist = Vector3.Distance(start, groundedPos);
            y = end.y - start.y;
        }

        distance = Vector3.Distance(start, end);

        //Get normalized direction
        float xDir = groundDist / distance;
        float yDir = y / distance;

        //Get the angle in degrees to turn
        float math = (-distance * distance) / (-2 * distance * length);
        angle = Mathf.Acos(math);

        //Apply angle on direciton
        float xPoint = length * (xDir * Mathf.Cos(angle) - yDir * Mathf.Sin(angle));
        float yPoint = length * (xDir * Mathf.Sin(angle) + yDir * Mathf.Cos(angle));

        startObj.transform.LookAt(groundedPos);
        jointPos3 = start + startObj.transform.forward * xPoint;
        jointPos3.y = start.y + yPoint;

        return jointPos3;
    }
}
