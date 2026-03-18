using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Raycast : MonoBehaviour
{
    [SerializeField] float distance;
    public Camera cam;
    public float defaultFov = 60;
    float zoomSpeed = 10f;  
    float minFov = 10f;
    float maxFov = 60f;
    public bool isZoomed;
    public WebRequest web;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            GetDistance();
        }

        if (Input.GetMouseButtonDown(1))
        {
            isZoomed = !isZoomed;

            if (isZoomed)
            {
                cam.fieldOfView = (minFov + maxFov)/2;
            }
            else
            {
                cam.fieldOfView = defaultFov;
            }
        }

        if (isZoomed)
        {
            float scroll = Input.GetAxis("Mouse ScrollWheel");
            if (Mathf.Abs(scroll) > 0.01f)
            {
                cam.fieldOfView -= scroll * zoomSpeed * 10f;
                cam.fieldOfView = Mathf.Clamp(cam.fieldOfView, minFov, maxFov);
            }
        }
    }

    public void GetDistance()
    {
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hitInfo;

        if (Physics.Raycast(ray, out hitInfo, distance))
        {
            MeshRenderer renderer = hitInfo.transform.GetComponent<MeshRenderer>();
            renderer.material.color = Color.red;
            Debug.DrawLine(ray.origin, hitInfo.point, Color.green);
            Debug.Log(hitInfo.distance);

            hitInfo.transform.gameObject.SetActive(false);
            web.StartUpdateKills();

        }
        else
        {
            Debug.DrawLine(ray.origin, ray.origin + ray.direction * distance, Color.blue);
        }
    }
}
