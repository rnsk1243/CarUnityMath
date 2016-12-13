using UnityEngine;
using System.Collections;

public class BallEnergy : MonoBehaviour {
    public float CarRotationVisulalSpeed = 1000.0f;
    Rigidbody rig;
    Transform tr;
   // public float MOElasticity = 0.0000001f;
    public float MOElasticity = 0.999999999f;
    public bool isOK = false;
    public BallEnergy OtherCar;
    Vector3 v1_f = Vector3.zero;
    float angleAcceleration = 0.0f;
    bool isCrash = false;
    float newAngle = 0.0f;
    // 코루틴 회전 갱신 속도
    float rotationVisualSpeed = 0.0f;

    // Use this for initialization
    void Start () {
        // 전체 업데이트 속도 줄이기
        //Time.timeScale = 0.5f;
        rig = GetComponent<Rigidbody>();
        //rig.velocity = Vector3.zero;
        tr = GetComponent<Transform>();
        StartCoroutine(AngleDicrease());
    }
	
	// Update is called once per frame
	void Update () {
        // 출발 준비 됐나?(계산 완료 되었나?)
            if (isOK)
            {
               // 충돌 후 자신의 속도 
                BallStart(v1_f);
                isOK = false;
            }
        if(!isCrash)
        {
            //Debug.Log(Pythagoras_theorem(rig.velocity.x, rig.velocity.z));
            //tr.rotation = Quaternion.Euler(new Vector3(0.0f, Pythagoras_theorem(rig.velocity.x, rig.velocity.z), 0.0f));
            if(rig.velocity.x < 0 && rig.velocity.z > 0)
            {
                tr.rotation = Quaternion.Euler(new Vector3(0.0f, 360.0f - Vector3.Angle(rig.velocity, Vector3.forward), 0.0f));
            }
            else if(rig.velocity.x < 0 && rig.velocity.z < 0)
            {
                tr.rotation = Quaternion.Euler(new Vector3(0.0f, 360.0f - Vector3.Angle(rig.velocity, Vector3.forward), 0.0f));
            }
            else
            {
                tr.rotation = Quaternion.Euler(new Vector3(0.0f, Vector3.Angle(rig.velocity, Vector3.forward), 0.0f));
            }
            
           // Debug.Log("각 = " + Vector3.Angle(rig.velocity, Vector3.forward));
          //  Debug.Log("rig.velocity = " + rig.velocity);
        }

      //  Debug.Log(rig.velocity / Time.deltaTime);
      //  tr.rotation = Quaternion.Euler(new Vector3(rig.velocity.x, 0.0f, rig.velocity.z));
       
      //  Debug.Log(angleAcceleration);


    }


    void OnTriggerEnter(Collider coll)
    {
        // 축 비정렬 벡터 반사
        // 벽과 충돌하면 
        if (coll.tag == "Wall")
        {
            // 벽의 정보를 담을 구조체
            RaycastHit hit;
            // 레이가 벽과 충돌하면
            if (Physics.Raycast(transform.position, rig.velocity.normalized, 
                out hit, Mathf.Infinity, LayerMask.GetMask("Wall")))
            {
                // 벽의 노말정보를 가져옴 (내적에 이용)
                Vector3 n = hit.normal;
                // 공의 충돌전 속도 (내적에 이용)
                Vector3 vi = rig.velocity;
                //  충돌방향의 반대 방향과 벽의 노말정보와 내적하여 벡터의 길이를 구하고
                float pLength = Vector3.Dot(-vi, n);
                // 벡터의 길이와 노말정보를 곱하여 벡터를 구합니다.
                Vector3 p = pLength * n;
                // 평행사변형이므로 2배를 해주고 vi값을 더해주면 반사벡터가 나온다.
                Vector3 vf = 2 * p + vi;
                // 적용
                rig.velocity = vf;
            }
        }
        /* 당구대가 회전하면 쓸모가 없다... 축정렬
        if (coll.gameObject.tag == "Right")
        {
            // Debug.Log("right");
            //Debug.Log(rig.velocity);
            rig.velocity = new Vector3(-rig.velocity.x, 0.0f, rig.velocity.z);
        }
        else if (coll.gameObject.tag == "Left")
        {
          //  Debug.Log("Left");
            //Debug.Log("전 = " + rig.velocity);
            // rig.velocity = Vector3.zero;
            rig.velocity = new Vector3(-rig.velocity.x, 0.0f, rig.velocity.z);
            // Debug.Log("후 = " + rig.velocity);
        }
        else if (coll.gameObject.tag == "Up")
        {
            rig.velocity = new Vector3(rig.velocity.x, 0.0f, -rig.velocity.z);
           // Debug.Log("Up");
        }
        else if (coll.gameObject.tag == "Bottom")
        {
            rig.velocity = new Vector3(rig.velocity.x, 0.0f, -rig.velocity.z);
           // Debug.Log("Bottom");
        }
         */
        else if (coll.gameObject.tag == "Ball")
        {
            isCrash = true;
          //  Debug.Log("공과 충돌함");

            /*
            modulus of elasticity <= 탄성계수
             m1, m2 는 각각 공의 질량
             v1_i, v2_i 는 각각 공의 충돌전 속도
             v1_f, v2_f 는 각각 공의 충돌후 속도
            =================================================================================

             운동량 보존의 법칙에 따라 m1v1_i +m2v2_i = m1v1_f + m2v2_f 가 성립 함-- 1번 식
             (v1_f - v2_f) = -탄성계수(v1_i - v2_i)------------------------------ - 2번 식
             
            위 공식을 이용하여 충돌후 v1_f 와 v2_f를 구함
            
            우선 m1v1_i + m2v2_i = m1v1_f + m2v2_f 을 이용해 본다.
            (전제1) 당구공의 질량은 똑같다 즉, m1 == m2 가 성립함
            따라서 v1_i + v2_i = v1_f + v2_f 이다. ---------------------------------- 1번 식
            
            1번식과 2번식을 더하면 v1_f와 v2_f를 구할 수 있다.
             v1_f + v2_f = v1_i + v2_i  ------------------------------------------- 1번 식
           +)(v1_f - v2_f) = -탄성계수(v1_i - v2_i)-------------------------------- 2번 식
            ----------------------------------------
            2v1_f = (v1_i + v2_i) - 탄성계수(v1_i - v2_i)
            v1_f = ((v1_i + v2_i) - 탄성계수(v1_i - v2_i)) / 2 -------------------- v1_f값
            
            v2_f = v1_i + v2_i - v1_f
            v2_f = v1_i + v2_i - ((v1_i + v2_i) - 탄성계수(v1_i - v2_i)) / 2 ------ v2_f값
 
 *     */
            // 충돌하는 물체 처음속도 v1_i // 나중속도 v1_f  각각 x,z 성분 따로 구함.
            // 충돌당할 물체 처음속도 v2_i // 나중속도 v2_f
            //Debug.Log("나의 이름은 : " + this.name);
            //if (coll.gameObject.name == "Ball (3)")
            //{
            //    Debug.Log("");
            //    return;
            //}
            // 충돌당할 공의 속도를 가져오기 위해 스크립트를 가져옴.
            BallEnergy OtherBall = coll.gameObject.GetComponent<BallEnergy>();
            Vector3 OtherBallVelocity = OtherBall.getVelocity();
            // 충돌전 충돌당할 물체의 속도를 가져옴
            float v2_i_x = OtherBallVelocity.x;
            float v2_i_z = OtherBallVelocity.z;

            // 나의 속도
            float v1_i_x = this.rig.velocity.x;
            float v1_i_z = this.rig.velocity.z;

            // v1_f = ((v1_i + v2_i) - 탄성계수(v1_i - v2_i)) / 2 -------------------- v1_f값
            // 충돌체의 질량이 같을 때
            //float v1_f_x = get_V1_F(v1_i_x, v2_i_x);
            //float v1_f_z = get_V1_F(v1_i_z, v2_i_z);

            // 충돌체의 질량이 서로 다를 때
            float v1_f_x = get_V1_F(v1_i_x, rig.mass, v2_i_x, OtherBall.rig.mass);
            float v1_f_z = get_V1_F(v1_i_z, rig.mass, v2_i_z, OtherBall.rig.mass);

            // 충돌할때 Ball 끼리 충돌하므로 서로 자신의 OnTriggerEnter를 호출 한다.
            // 따라서 이곳에서 충돌하는 모든 Ball의 나중속도를 구해 버리면
            // 각각 Ball이 한번씩 모두 2번 나중속도를 구하는 꼴이 되버린다.
            // 이렇게 되면 충돌 당하는 Ball이 순간적으로 다시 충돌 하는 Ball에게 속도를 줘버려서
            // 마치 아무일도 없는냥 충돌 하는 Ball이 그냥 통과하는 것 처럼 보인다.

            // 따라서 나 외에 다른 Ball은 신경 쓰지 말고 각자 자신의 Ball의 속도를 구하기로 했다.
            // 그래서 v2_f를 구하는 곳은 모두 주석 처리 했다.

            // 하지만 각각 공의 속도를 구하려고 하니.. 문제가 있었다.
            // 충돌 하는 쪽과 충돌 당하는 쪽, 둘 중에 먼저 호출 된 OnTriggerEnter함수가 자신의 
            // 속도를 바꾸어 버리는 바람에 나중에 호출 된 Ball의 OnTriggerEnter함수는 
            // 먼저 호출되어 이미 속도가 바뀌어 버린 v2_i를 가져와 버린다...
            // 이 문제를 해결하기 위해 준비가 되었냐는 bool isOK변수를 만들었다.
            // 충돌하는 두 공이 모두 자신의 나중 속도를 구한 후에 isOK = true로 변경해주었다.
            // 그리고 Update함수에서 isOK가 true일때 자신의 나중 속도를 적용해 주었다.

            // 따라서 굳이 구할 필요가 없는 v2_f값 주석 처리(다른 공의 충돌 후 속도)
            // v2_f = v1_i + v2_i - v1_f -------------------------------- v2_f값
            //float v2_f_x = v1_i_x + v2_i_x - v1_f_x;
            //float v2_f_z = v1_i_z + v2_i_z - v1_f_z;

            // 속도 벡터 만듬
            v1_f = new Vector3(v1_f_x, 0.0f, v1_f_z);
            angleAcceleration = GetAngleacceleration(coll);
            rotationVisualSpeed = CarRotationVisulalSpeed / angleAcceleration;
            // 출발!
            isOK = true;
            return;
        }
    }

    public void BallStart(Vector3 vec3)
    {
       
        rig.velocity = vec3;
    }

    public float GetAngleacceleration(Collider coll)
    {
        float collisionAngle = Vector3.Angle(rig.velocity, coll.GetComponent<Rigidbody>().velocity);
        Debug.Log("angle = " + collisionAngle);
        float angleacceleration = 0.0f;
        if (178.0f <= collisionAngle && collisionAngle <= 182.0f)
        {

        }else if(-2.0f <= collisionAngle && collisionAngle <= 2.0f)
        {

        }
        else
        {
            // 질량
            float mass = rig.mass;
            // 자동차의 반지름
            float r = GetComponent<SphereCollider>().radius;
            // 상대 자동차 강체
            Rigidbody rigOther = coll.GetComponent<Rigidbody>();
            // 상대 자동차 질량
            float massOther = rigOther.mass;
            // 상대 자동차 반지름
            float rOther = coll.GetComponent<SphereCollider>().radius;
            // 상대 자동차의 속도
            float velOther = Pythagoras_theorem(rigOther.velocity.x, rigOther.velocity.z);
            // 각가속도 = 토크 / 관성모메트(mr^2)
            // 토크 = ((자동차의질량 * 가속도)=뉴턴) * 자동차의반지름

            // 뉴턴 = 자동차의 질량 * 가속도
            float newton = massOther * (velOther / Time.deltaTime);
            // 토크 = 뉴턴 * 반지름
            float torque = newton * r;
            // 각가속도 ( 토크 / mr^2 )
            angleacceleration = torque / mass * Mathf.Pow(r, 2);
        }

            return angleacceleration;
    }


    // 피타고라스정리 - 힘의 크기를 스칼라값으로 구함. 
    public float Pythagoras_theorem(float a, float b)
    {
        float c = Mathf.Sqrt(Mathf.Pow(a, 2) + Mathf.Pow(b, 2));
        return c;
    }
    // 각각의 처음속도를 넣어 v1_f 구해주는 함수
    public float get_V1_F(float v1_i, float v2_i)
    {
        // v1_f = ((v1_i + v2_i) - 탄성계수(v1_i - v2_i)) / 2 -------------------- v1_f값
        float v1_f = ((v1_i + v2_i) - (MOElasticity * (v1_i - v2_i))) / 2;
        //Debug.Log("v1_f ==== " + v1_f);
        return v1_f;
    }

    // (질량이 서로 다를 경우)각각의 처음속도와 질량을 넣어 v1_f 구해주는 함수
    public float get_V1_F(float v1i, float m1, float v2i, float m2)
    {
        // v1f(m1+m2) = m1v1i + m2v2i - m2(탄성계수(v1i-v2i)) -------------------- 에서
        // v1f = (m1v1i + m2v2i - m2(탄성계수(v1i-v2i))) / (m1+m2)
        float v1_f = (((m1 * v1i) + (m2 * v2i)) - (m2 * (MOElasticity * (v1i - v2i)))) / (m1 + m2);
        //Debug.Log("v1_f ==== " + v1_f);
        return v1_f;
    }

    public Vector3 getVelocity()
    {
       // Debug.Log(rig.velocity);
        return rig.velocity;
    }
    
    IEnumerator AngleDicrease()
    {
        while(true)
        {
 //             Debug.Log(this.name + "rotationVisualSpeed = " + rotationVisualSpeed);
            yield return new WaitForSeconds(rotationVisualSpeed);
            if(isCrash)
            {
                if (angleAcceleration < -0.1f || angleAcceleration > 0.1f)
                {
                    angleAcceleration = angleAcceleration * 0.8f;
                    newAngle = newAngle + angleAcceleration;
                    //Debug.Log("y = " + tr.rotation.y);
                    //Debug.Log(angleAcceleration);
                    tr.rotation = Quaternion.Euler(new Vector3(0.0f, tr.rotation.y + newAngle, 0.0f));
                    // angleAcceleration = angleAcceleration * 0.9f;
                    // tr.Rotate(new Vector3(0.0f, angleAcceleration, 0.0f));
                }else
                {
                    isCrash = false;
                }
            }
           
        }
        

      
    }

}

/*

        else if(col.tag == "wall")
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, rigidbody.velocity.normalized,out hit,Mathf.Infinity,LayerMask.GetMask("wall")))
            {
                Vector3 n = hit.normal;
                Vector3 vi = rigidbody.velocity;
                float pLength = Vector3.Dot(-vi,n);
                Vector3 p = pLength * n;
                Vector3 vf = 2 * p + vi;
                rigidbody.velocity = vf;
            }
        }
 
*/

/*
 
        Transform hitted = hittedBall.Find(delegate (Transform ball) { return ball == col.transform; });
        if (col.tag == "ball")
        {
            if (hitted == null)
            {
                Rigidbody targetRigid = col.attachedRigidbody;
                float moe = BillardManager.instance.MOE;
                Vector3 v1i = rigidbody.velocity;
                Vector3 v2i = targetRigid.velocity;

                Vector3 v1f = ((v1i + v2i) - moe * (v1i - v2i)) / 2;
                Vector3 v2f = v1i + v2i - v1f;

                rigidbody.velocity = v1f;
                targetRigid.velocity = v2f;
                BillardBall target = col.GetComponent<BillardBall>();
                target.SetCollision(transform);
            }
            else
            {
                hittedBall.Remove(hitted);
            }
        }
     
     */


/*

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BillardBall : MonoBehaviour {

new Rigidbody rigidbody;
List<Transform> hittedBall=new List<Transform>();


void Start()
{
    transform.tag = "ball";
    gameObject.layer = LayerMask.NameToLayer("ball");
    rigidbody = GetComponent<Rigidbody>();
}
public void SetCollision(Transform ball)
{
    hittedBall.Add(ball);
}
void OnTriggerEnter(Collider col)
{
    Transform hitted = hittedBall.Find(delegate (Transform ball) { return ball == col.transform; });
    if (col.tag == "ball")
    {
        if (hitted == null)
        {
            Rigidbody targetRigid = col.attachedRigidbody;
            float moe = BillardManager.instance.MOE;
            Vector3 v1i = rigidbody.velocity;
            Vector3 v2i = targetRigid.velocity;

            Vector3 v1f = ((v1i + v2i) - moe * (v1i - v2i)) / 2;
            Vector3 v2f = v1i + v2i - v1f;

            rigidbody.velocity = v1f;
            targetRigid.velocity = v2f;
            BillardBall target = col.GetComponent<BillardBall>();
            target.SetCollision(transform);
        }
        else
        {
            hittedBall.Remove(hitted);
        }
    }
    else if(col.tag == "wall")
    {
        RaycastHit hit;
        Vector3 point = col.bounds.ClosestPoint(transform.position);
        Debug.DrawLine(transform.position, point, Color.green, 3.0f);
        if (Physics.Raycast(transform.position, point - transform.position,out hit,Mathf.Infinity,LayerMask.GetMask("wall")))
        {
            Vector3 n = hit.normal;
            Vector3 vi = rigidbody.velocity;
            float pLength = Vector3.Dot(-vi,n);
            Vector3 p = pLength * n;
            Vector3 vf = 2 * p + vi;
            rigidbody.velocity = vf;
        }
    }
}

public void ShootBall(Vector3 _dir, float _power)
{
    rigidbody.velocity += _dir.normalized * _power;
}

}

 */
