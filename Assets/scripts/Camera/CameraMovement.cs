using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [SerializeField]
    private Camera cam;

    private Vector3 dragOrigin;
    public Vector2 panLimit;
    [SerializeField]
    private float zoomSpeed, minCamSize, maxCamSize, panSpeed = 100f, targetSize, panBortherThickness = 10f;

    private float mapMinX, mapMaxX, mapMinY, mapMaxY;

    MeshRenderer terrain;

    private void Start()
    {
        terrain = GameObject.Find("terrainR").GetComponent<MeshRenderer>();
        mapMinX = terrain.transform.position.x - terrain.bounds.size.x /2f;
        mapMaxX = terrain.transform.position.x + terrain.bounds.size.x /2f;

        mapMinY = terrain.transform.position.y - terrain.bounds.size.y /2f;
        mapMaxY = terrain.transform.position.y + terrain.bounds.size.y /2f;
    }
    private void Update()
    {
        Vector3 pos = transform.position;
        if (Input.GetKey("w") /*|| Input.mousePosition.y >= Screen.height - panBortherThickness*/)
        {
            pos.y += panSpeed * Time.deltaTime;
        }
        if (Input.GetKey("s") /*|| Input.mousePosition.y <= panBortherThickness*/)
        {
            pos.y -= panSpeed * Time.deltaTime;
        }
        if (Input.GetKey("d") /*|| Input.mousePosition.x >= Screen.width - panBortherThickness*/)
        {
            pos.x += panSpeed * Time.deltaTime;
        }
        if (Input.GetKey("a") /*|| Input.mousePosition.x <= panBortherThickness*/)
        {
            pos.x -= panSpeed * Time.deltaTime;
        }
        
        pos = ClampCam(pos);
        
        
        transform.position = pos;
        //PanCamera();
        if (Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            ZoomIn();
        }
        if (Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            ZoomOut();
        }
        //ClampCam();


    }

    void PanCamera()
    {
        if (Input.GetMouseButtonDown(2))
        {
            dragOrigin = cam.ScreenToWorldPoint(Input.mousePosition);
        }
        if (Input.GetMouseButton(2))
        {
            Vector3 difference = dragOrigin - cam.ScreenToWorldPoint(Input.mousePosition);
            //Debug.Log("origin " + dragOrigin + " new pos " + cam.ScreenToWorldPoint(Input.mousePosition) + " =difference " + difference);
            cam.transform.position = ClampCam(cam.transform.position + difference);
            
            //cam.transform.position = new Vector3(Mathf.Clamp(difference.x, 400f, 600f), Mathf.Clamp(difference.y, 150f, 850f), -10);
        }
    }

    public void ZoomIn()
    {
        float newSize = cam.orthographicSize - zoomSpeed;
        //Camera childCam = cam.transform.GetChild(0).gameObject.GetComponent<Camera>();
        cam.orthographicSize = Mathf.Clamp(newSize, minCamSize, maxCamSize);
        
        //childCam.orthographicSize = Mathf.Clamp(newSize, minCamSize, maxCamSize);
        cam.transform.position = ClampCam(cam.transform.position);

    }

    public void ZoomOut()
    {
        //Camera childCam = cam.transform.GetChild(0).gameObject.GetComponent<Camera>();
        float newSize = cam.orthographicSize + zoomSpeed;
        cam.orthographicSize = Mathf.Clamp(newSize, minCamSize, maxCamSize);
        //childCam.orthographicSize = Mathf.Clamp(newSize, minCamSize, maxCamSize);
        cam.transform.position = ClampCam(cam.transform.position);
    }

    public Vector3 ClampCam(Vector3 targetPos)
    {
        float cameHeight = cam.orthographicSize;
        float camWidth = cam.orthographicSize * cam.aspect;
        float minX = mapMinX + camWidth;
        float maxX = mapMaxX - camWidth;
        float minY = mapMinY + cameHeight;
        float maxY = mapMaxY - cameHeight;

        float newX = Mathf.Clamp(targetPos.x, minX, maxX);
        float newY = Mathf.Clamp(targetPos.y, minY, maxY);
        return new Vector3(newX, newY, targetPos.z);

    }
    //private void OnTriggerEnter2D(Collider2D collision)
    //{
    //    if(collision.gameObject.layer == LayerMask.NameToLayer("Walls"))
    //    {
    //        collision.gameObject.SetActive(true);
    //    }
    //}

    //private void OnTriggerExit2D(Collider2D collision)
    //{
    //    if (collision.gameObject.layer == LayerMask.NameToLayer("Walls"))
    //    {
    //        collision.gameObject.SetActive(false);
    //    }
    //}

}
