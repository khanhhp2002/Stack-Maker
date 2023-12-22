using UnityEngine;
using DG.Tweening;
using System;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour
{
    #region Fields
    [SerializeField] private Rigidbody _rigidbody;
    [SerializeField] private float _speedPerBlock;
    [SerializeField] private Transform _bricksHolder;
    [SerializeField] private LayerMask _layerMask;

    [Header("Drag Threshold"), Space(5f)]
    [SerializeField] private float _verticalThreshold = 300f;
    [SerializeField] private float _horizontalThreshold = 150f;
    [SerializeField] private float _endTime = 3f;
    private Stack<GameObject> _bricks = new Stack<GameObject>();
    private Vector3 _holderPos = Vector3.zero;
    private int _bricksCount = 0;
    private Vector3 _startPos;
    #endregion

    void FixedUpdate()
    {
        if (!DOTween.IsTweening(_rigidbody))
        {
#if UNITY_EDITOR
            SwiftWithMouse();
            ControlWithKeyBoard();
#endif
            SwiftByTouch();
        }
    }

    void SwiftWithMouse()
    {
        if (Input.GetMouseButtonDown(0))
        {
            _startPos = Input.mousePosition;
        }
        else if (Input.GetMouseButton(0))
        {
            Vector3 offset = Input.mousePosition - _startPos;
            if (Mathf.Abs(offset.y) >= _verticalThreshold)
            {
                if (offset.y > 0)
                {
                    FindDestination(Vector3.forward);
                }
                else
                {
                    FindDestination(Vector3.back);
                }
            }
            else if (Mathf.Abs(offset.x) >= _horizontalThreshold)
            {
                if (offset.x > 0)
                {
                    FindDestination(Vector3.right);
                }
                else
                {
                    FindDestination(Vector3.left);
                }
            }
        }
    }

    void ControlWithKeyBoard()
    {
        if (Input.GetKey(KeyCode.W))
        {
            FindDestination(Vector3.forward);
        }
        else if (Input.GetKey(KeyCode.A))
        {
            FindDestination(Vector3.left);
        }
        else if (Input.GetKey(KeyCode.S))
        {
            FindDestination(Vector3.back);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            FindDestination(Vector3.right);
        }
    }

    void SwiftByTouch()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                _startPos = touch.position;
            }
            else if (touch.phase == TouchPhase.Moved)
            {
                Vector2 offset = touch.position - new Vector2(_startPos.x, _startPos.y);
                if (Mathf.Abs(offset.y) >= _verticalThreshold)
                {
                    if (offset.y > 0)
                    {
                        FindDestination(Vector3.forward);
                    }
                    else
                    {
                        FindDestination(Vector3.back);
                    }
                }
                else if (Mathf.Abs(offset.x) >= _horizontalThreshold)
                {
                    if (offset.x > 0)
                    {
                        FindDestination(Vector3.right);
                    }
                    else
                    {
                        FindDestination(Vector3.left);
                    }
                }
            }
        }
    }

    void FindDestination(Vector3 direction)
    {
        RaycastHit hitInfo;

        if (Physics.Raycast(transform.position, direction, out hitInfo, 100f, _layerMask))
        {
            GameObject hitObject = hitInfo.collider.gameObject;
            Vector3 destination = hitObject.transform.position - direction;
            destination = new Vector3(destination.x, transform.position.y, destination.z);
            Debug.Log(destination);
            if (destination == transform.position)
            {
                Debug.Log("Next to wall");
                return;
            }
            float distance = Math.Abs(destination.x - transform.position.x) + Math.Abs(destination.z - transform.position.z);
            _rigidbody.DOMove(destination, distance * _speedPerBlock).SetEase(Ease.Linear);
        }
        else
        {
            Debug.Log("Not found destination");
        }
    }

    void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.layer == 7)
        {
            _bricks.Push(collider.gameObject);
            collider.GetComponent<BoxCollider>().isTrigger = false;
            collider.GetComponent<BoxCollider>().enabled = false;
            collider.transform.parent = _bricksHolder;
            collider.transform.localPosition = _holderPos;
            _holderPos += new Vector3(0, 0.5f, 0);
            _bricksCount++;
        }
        else if (collider.gameObject.layer == 8)
        {
            if (_bricksCount != 0)
            {
                collider.GetComponent<BoxCollider>().enabled = false;
                GameObject tmpBrick = _bricks.Pop();
                tmpBrick.transform.parent = GameManager.Instance.MapLoadPosition;
                tmpBrick.transform.position = collider.transform.position + Vector3.down * 0.75f;
                tmpBrick.GetComponent<BoxCollider>().enabled = true;
                _bricksCount--;
                _holderPos -= new Vector3(0, 0.5f, 0);
            }
        }
        else if (collider.gameObject.layer == 9)
        {
            // finish
            GameManager.Instance.DestroyPlayer(gameObject, _endTime);
        }
    }
}
