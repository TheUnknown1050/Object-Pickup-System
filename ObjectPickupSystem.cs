using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ObjectPickupSystem : MonoBehaviour
{
    public float pickUpRange;
    public float maxThrowForce;
    public float throwForceIncreaseSpeed;

    public Transform objectHolder;
    public LayerMask pickUpLayer;

    public GameObject throwBarGameObject;
    // Make sure to include UnityEngine.UI
    public Slider throwBar;

    public GameObject pickUpGameObject;
    // Make sure to include TMPro (if using text mesh pro)
    public TextMeshProUGUI pickUpText;

    private GameObject currentObject;
    private Rigidbody currentObjectRb;

    private float currentThrowForce;

    private void Start()
    {
        throwBar.maxValue = maxThrowForce;
        throwBarGameObject.SetActive(false);
        pickUpGameObject.SetActive(false);
    }

    private void Update()
    {
        PickUp();
        Throw();
    }

    private void PickUp()
    {
        if (currentObject != null)
        {
            return;
        }

        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
        RaycastHit hitInfo;

        if (Physics.Raycast(ray, out hitInfo, pickUpRange, pickUpLayer))
        {
            if (currentObject == null)
            {
                pickUpGameObject.SetActive(true);
                pickUpText.text = hitInfo.collider.gameObject.name;
            }

            if (Input.GetKeyDown(KeyCode.E))
            {
                currentObject = hitInfo.collider.gameObject;
                currentObjectRb = currentObject.GetComponent<Rigidbody>();

                currentObject.transform.parent = objectHolder;
                currentObject.transform.localPosition = Vector3.zero;
                currentObject.transform.localEulerAngles = new Vector3(0, 0, 0);

                foreach (Collider collider in currentObject.GetComponents<Collider>())
                {
                    collider.enabled = false;
                }

                currentObjectRb.isKinematic = true;
            }
        }
        else
        {
            pickUpGameObject.SetActive(false);
            pickUpText.text = "";
        }
    }

    private void Throw()
    {
        if (currentObject == null)
        {
            return;
        }

        if (Input.GetKey(KeyCode.Q))
        {
            throwBarGameObject.SetActive(true);

            if (currentThrowForce <= maxThrowForce)
            {
                currentThrowForce += throwForceIncreaseSpeed * Time.deltaTime;
                throwBar.value = currentThrowForce;
            }
        }

        if (currentThrowForce > maxThrowForce)
        {
            currentThrowForce = maxThrowForce;
        }

        if (Input.GetKeyUp(KeyCode.Q))
        {
            currentObject.transform.parent = null;

            currentObjectRb.isKinematic = false;
            currentObjectRb.AddForce(currentObject.transform.forward * currentThrowForce, ForceMode.Impulse);

            foreach (Collider collider in currentObject.GetComponents<Collider>())
            {
                collider.enabled = true;
            }

            currentObject = null;
            currentObjectRb = null;

            currentThrowForce = 0;
            throwBar.value = 0;
            throwBarGameObject.SetActive(false);
        }
    }
}
