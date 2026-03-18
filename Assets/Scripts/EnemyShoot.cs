using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class EnemyShoot : MonoBehaviour
{
    [SerializeField] private float distance;
    [SerializeField] private float timer;
    [SerializeField] private Transform player;
    public WebRequest web;
    [SerializeField] private GameObject[] enemyObj;
    [SerializeField] GameObject deathPrompt;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Update()
    {
        timer += Time.deltaTime;
        transform.LookAt(player.position);
        if (timer >= 10)
        {

            GetDistance();
            
        }

        if (timer > 2)
        {
            deathPrompt.SetActive(false);
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

            web.StartUpdateDeaths();
            timer = 0;  
            deathPrompt.SetActive(true);
            foreach (var enemy in enemyObj)
            {
                enemy.gameObject.SetActive(true);
            }
            


        }
        else
        {
            Debug.DrawLine(ray.origin, ray.origin + ray.direction * distance, Color.blue);
        }
    }
    private void OnDrawGizmos()
    {
        if (player == null) return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, transform.position + transform.forward * distance);
        Gizmos.DrawSphere(transform.position + transform.forward * distance, 0.2f);
    }
}