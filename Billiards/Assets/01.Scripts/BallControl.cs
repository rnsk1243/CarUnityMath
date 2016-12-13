using UnityEngine;
using System.Collections;

public class BallControl : MonoBehaviour {

    Transform tr;
    // 공 클릭 했나 안했나 확인용 레이 정보
    RaycastHit rayInfo;
    // 공 클릭했으면 이제부터 공과 현재 마우스포인터와 거리를 구하기 위한 레이 정보
    RaycastHit rayPositionInfo;
    // 공 클릭 했나?
    bool isOnClick = false;
    Ray ray;
   
    // 방향&크기 화살표
    GameObject Arrow;
    Transform ArrowTr;

    // 힘의 크기(화살표의 크기)
     float power = 0.0f;
    // 힘의 방향(degrees 값)
     float angle = 0.0f;
    // 힘의 xz성분(화살표의 x or z성분) BallEnergy 스크립트에 보낼 것
    float vectorX;
    float vectorZ;
    //
    BallEnergy BallE;

    public float Power = 5.0f;
    // Use this for initialization
    void Start () {
        tr = GetComponent<Transform>();
        Arrow = GameObject.FindGameObjectWithTag("Arrow");
        Arrow.SetActive(false);
        ArrowTr = Arrow.GetComponent<Transform>();
        BallE = GetComponent<BallEnergy>();
        //ray = Camera.main.ScreenPointToRay(Input.mousePosition);
    }
	
	// Update is called once per frame
	void Update () {
        //oldTime += Time.deltaTime;
       
        //if(oldTime > 0.1f)
        //{
        //    oldTime = 0;
        //    oldPosition = newPosition;
        //    newPosition = tr.position;
        //}
        ////Debug.Log("=========" + newPosition.x);
        ////Debug.Log(oldPosition.x);
        //Vector3 deltaPosition = newPosition - oldPosition;
        ////Debug.Log(deltaPosition);

        //if ((deltaPosition.x < 0.08f && deltaPosition.x > -0.08f) && (deltaPosition.z < 0.08f && deltaPosition.z > -0.08f))
        //{
        //    //Debug.Log("정지");
        //    //rig.drag = 5f;
        //    //rig.angularDrag = 5.0f;
        //}else
        //{
        //    rig.drag = 0.1f;
        //    rig.angularDrag = 0.1f;
        //}

        // 공 클릭 했다면
        if (isOnClick)
        {
            ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            // 공과 마우스의 거리를 구하기 위함.
            Physics.Raycast(ray, out rayPositionInfo, Mathf.Infinity);

            // 공을 0,0(원점)으로 보내려면 x값과 z값이 얼마인지 구함
            float tempX = zeroPosition(tr.position.x);
            float tempZ = zeroPosition(tr.position.z);

            // 0,0으로 보냈을때 레이의 좌표를 구함
            Vector3 zeroRay = 
                new Vector3(rayPositionInfo.point.x + tempX,
                0.0f, rayPositionInfo.point.z + tempZ);
            // 이때 피타고라스 정리로 0,0에서 
            // 레이 좌표까지 길이를 구함(이것이 힘의 크기)
            float zeroTolength = Mathf.Sqrt(Mathf.Pow(zeroRay.x, 2) 
                + Mathf.Pow(zeroRay.z, 2));
            // 힘의 크기 저장
            power = zeroTolength;
            // 빗변 길이를 이용하여 sin값을 구함
            float sin = zeroRay.z / zeroTolength;
            // sin값(라디안)을 (도)로 변경
            float degrees = (Mathf.Asin(sin) * 180) / Mathf.PI;
            
            // 공을 기준으로 몇사분면인지 확인
            if(zeroRay.x >= 0 && zeroRay.z > 0)
            {
                // 1사 분면
                //Debug.Log("1사 분면");
            }else if (zeroRay.x < 0 && zeroRay.z >= 0)
            {
                // 2사 분면
                //Debug.Log("2사 분면");
                degrees = 180 - degrees;
            }
            else if (zeroRay.x <= 0 && zeroRay.z < 0)
            {
                // 3사 분면
                //Debug.Log("3사 분면");
                degrees = 180 - degrees;
            }
            else if (zeroRay.x > 0 && zeroRay.z <= 0)
            {
                // 4사 분면
                //Debug.Log("4사 분면");
                degrees = 360 + degrees;
            }
            else
            {
                // 원점
                //Debug.Log("원점");
                degrees = 0;
            }
            angle = degrees;
            Arrow.SetActive(true);

            // 각도에 맞게 회전
            ArrowTr.rotation = Quaternion.Euler(90.0f, 0.0f, degrees);
            // 크기 변경
            ArrowTr.localScale = new Vector3(zeroTolength * 2, zeroTolength * 0.5f, 1.0f);
        }
        Debug.DrawRay(ray.origin, ray.direction * 100.0f, Color.green);
        // 공 클릭 했는지 확인용
        if(Input.GetMouseButtonDown(0))
        {
            ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            
            if (Physics.Raycast(ray, out rayInfo, Mathf.Infinity))
            {
                
                if ((rayInfo.transform.tag == "Left"||
                    rayInfo.transform.tag == "Right"||
                    rayInfo.transform.tag == "Bottom"||
                    rayInfo.transform.tag == "Up"||
                    rayInfo.transform.tag == "Floow") && isOnClick)
                {
                    vectorX = power * Mathf.Cos((angle * Mathf.PI) / 180.0f);
                    vectorZ = power * Mathf.Sin((angle * Mathf.PI) / 180.0f);
                    //rig.AddForce(-(new Vector3(vectorX, 0.0f, vectorZ)) * 100);
                    //rig.velocity = -(new Vector3(vectorX, 0.0f, vectorZ));
                    //Debug.Log("출발!");
                    BallE.BallStart(-(new Vector3(vectorX, 0.0f, vectorZ) * Power));
                    Arrow.SetActive(false);
                }

                //Debug.Log(ray.origin);
                //Debug.Log(rayInfo.transform.tag);
                if(rayInfo.transform.tag == "Ball")
                {
                    isOnClick = true;
                }else
                {
                    isOnClick = false;
                }
            }
        }
        //Debug.Log(Mathf.Sqrt(16));
        //Debug.Log(isOnClick);
        // Debug.Log("rayInfo.point = " + rayInfo.point);
        // Debug.Log("rayPositionInfo.point = " + rayPositionInfo.point);
    }

    float zeroPosition(float position)
    {
        return -position;
    }


}

/*
 
    if( Input.GetKey( KeyCode.LeftArrow ) )
				trArrow.Rotate( Vector3.forward * -180f * Time.deltaTime );
			if( Input.GetKey( KeyCode.RightArrow ) )
				trArrow.Rotate( Vector3.forward * 180f * Time.deltaTime );
			if( Input.GetKey( KeyCode.UpArrow ) )
				fForce = fForce < 50f ? fForce + Time.deltaTime * 20f : 50f;
			if( Input.GetKey( KeyCode.DownArrow ) )
				fForce = fForce > 5f ? fForce - Time.deltaTime * 20f : 5f;
			trArrow.localScale = new Vector3( fForce * 0.1f, trArrow.localScale.y, trArrow.localScale.z );
     
     */


/*
   ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        // 공과 마우스의 거리를 구하기 위함.
        Physics.Raycast(ray, out rayPositionInfo, Mathf.Infinity);
        dis = rayPositionInfo.point - tr.position;
        //Quaternion q = Quaternion.FromToRotation(new Vector3(rayPositionInfo.point.x, 0.0f, rayPositionInfo.point.z), new Vector3(tr.position.x, 0.0f, tr.position.z));
        Quaternion q = Quaternion.Euler(dis.x, 0.0f, dis.z);
        float powdis = Mathf.Pow(dis.x, 2) + Mathf.Pow(dis.z, 2);
        resultDis = Mathf.Sqrt(powdis);
        float sin = resultDis / (rayPositionInfo.point.z - tr.position.z);
        //Debug.Log(resultDis);
        Arrow.SetActive(true);
       // Debug.Log("q = " + q);
        ArrowTr.transform.rotation = q;
        Debug.Log(rayPositionInfo.point);
        //ArrowTr.Rotate(Vector3.forward * 180f * Time.deltaTime);
        //Arrow.transform.localScale = new Vector3(resultDis, 1.0f, 1.0f);
        //Arrow.transform.rotation = Quaternion.Euler(new Vector3(90.0f, q.y * 100.0f, q.z * 100.0f));
 */
