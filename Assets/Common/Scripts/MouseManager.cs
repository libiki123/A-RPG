using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MouseManager : MonoBehaviour
{
    public LayerMask clickableLayer;        // layermask used to isolate raycasts against clickable layers

    public Texture2D pointer;       // normal mouse pointer
    public Texture2D target;        // target mouse pointer
    public Texture2D doorway;       // doorway mouse pointer
    public Texture2D sword;         // attack mouse pointer

    public EventVector3 OnClickEnvironment;         // create an click event in the Inspector
    public EventVector3 OnRightClickEnvironment;    // create an click event in the Inspector
    public EventGameObject OnClickAttackable;       // create an click event in the Inspector

    private bool _useDefaultCursor = false;

    private void Awake()
    {
        if(GameManager.Instance != null)
            GameManager.Instance.OnGameStateChanged.AddListener(HandleGameStateChanged);
    }

    void HandleGameStateChanged(GameManager.GameState currentState, GameManager.GameState previousState)
    {
        _useDefaultCursor = (currentState != GameManager.GameState.RUNNING);
    }

    void Update()
    {
        // Set cursor
        if (_useDefaultCursor)
        {
            return;
        }

        // Raycast into scene
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 50, clickableLayer.value))      // Check mouse position on scene
        {
            bool door = false;
            bool isAttackable = hit.collider.GetComponent(typeof(IAttackable)) != null; // check of the object have IAttackable

            if (hit.collider.gameObject.tag == "Doorway")
            {
                Cursor.SetCursor(doorway, new Vector2(16, 16), CursorMode.Auto);        // change the mouse icon to doorway
                door = true;
            }
            else if (hit.collider.gameObject.tag == "Chest")
            {
                Cursor.SetCursor(pointer, new Vector2(16, 16), CursorMode.Auto);         // change mouse icon to target
            }
            else if (isAttackable)
            {
                Cursor.SetCursor(sword, new Vector2(16, 16), CursorMode.Auto);         // change mouse icon to target
            }
            else
            {
                // Override cursor
                Cursor.SetCursor(target, new Vector2(16, 16), CursorMode.Auto);
            }

            // If environment surface is clicked, invoke callbacks.
            if (Input.GetMouseButtonDown(0))
            {
                if (door)
                {
                    Transform doorway = hit.collider.gameObject.transform;
                    OnClickEnvironment.Invoke(doorway.position + doorway.forward * 10);
                }
                else if (isAttackable)
                {
                    GameObject attackable = hit.collider.gameObject;
                    OnClickAttackable.Invoke(attackable);
                }
                else
                {
                    OnClickEnvironment.Invoke(hit.point);
                }
            }
            else if (Input.GetMouseButtonDown(1))
            {
                if (!door)
                {
                    OnRightClickEnvironment.Invoke(hit.point);
                }
            }
        }
        else
        {
            Cursor.SetCursor(pointer, Vector2.zero, CursorMode.Auto);
        }
    }
}

[System.Serializable]
public class EventVector3 : UnityEvent<Vector3> { }

[System.Serializable]
public class EventGameObject : UnityEvent<GameObject> {}
