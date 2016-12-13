using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UIControl : MonoBehaviour {

    // 메인 차
    GameObject MainCar;
    // 메인 질량
    float MainCarMess;
    // 메인 질량 UI
    GameObject InputMyMessUI;
    // 메인 속도 UI
    GameObject MySpeedUI;
    // 메인 릿지드바디
    Rigidbody MainCarRig;

    /////////////////////

    // 일반차
    GameObject Car;
    float CarMess;
    GameObject InputMessUI;
    GameObject SpeedUI;
    Rigidbody CarRig;

    // Use this for initialization
    void Start () {
        MainCar = GameObject.Find("MainCar");
        Car = GameObject.Find("Car");
        MainCarRig = MainCar.GetComponent<Rigidbody>();
        CarRig = Car.GetComponent<Rigidbody>();
        // 메인 질량 초기화
        MainCarMess = MainCar.GetComponent<Rigidbody>().mass;
        CarMess = Car.GetComponent<Rigidbody>().mass;

        InputMyMessUI = GameObject.Find("InputMyMessUI");
        InputMessUI = GameObject.Find("InputMessUI");

        InputMyMessUI.GetComponent<InputField>().text = MainCarMess.ToString();
        InputMessUI.GetComponent<InputField>().text = CarMess.ToString();
        // 메인 속도
        MySpeedUI = GameObject.Find("MyCarSpeedText");
        SpeedUI = GameObject.Find("CarSpeedText");
        // 메인 질량 체크 코루틴 시작
        StartCoroutine(MainCarMessCheck());
        StartCoroutine(CarMessCheck());
    }

    // Update is called once per frame
    void Update()
    {
        if(MainCarRig.velocity.x > 0.1f || MainCarRig.velocity.z > 0.1f)
        {
            MySpeedUI.GetComponent<Text>().text = MainCar.GetComponent<BallEnergy>().Pythagoras_theorem(MainCarRig.velocity.x, MainCarRig.velocity.z).ToString();
        }else
        {
            MySpeedUI.GetComponent<Text>().text = "0";
        }
        
        if(CarRig.velocity.x > 0.1f || CarRig.velocity.z > 0.1f)
        {
            SpeedUI.GetComponent<Text>().text = Car.GetComponent<BallEnergy>().Pythagoras_theorem(CarRig.velocity.x, CarRig.velocity.z).ToString();
        }else
        {
            SpeedUI.GetComponent<Text>().text = "0";
        }
        
    }

    IEnumerator MainCarMessCheck()
    {
        while(true)
        {
            yield return new WaitForSeconds(0.5f);

            if(float.Parse(InputMyMessUI.GetComponent<InputField>().text) != MainCarMess)
            {
                MainCarMess = float.Parse(InputMyMessUI.GetComponent<InputField>().text);
                MainCar.GetComponent<Rigidbody>().mass = MainCarMess;
            }
            else
            {
            }
        }
    }

    IEnumerator CarMessCheck()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.5f);

            if (float.Parse(InputMessUI.GetComponent<InputField>().text) != CarMess)
            {
                CarMess = float.Parse(InputMessUI.GetComponent<InputField>().text);
                Car.GetComponent<Rigidbody>().mass = CarMess;
            }
            else
            {
            }
        }
    }


}
