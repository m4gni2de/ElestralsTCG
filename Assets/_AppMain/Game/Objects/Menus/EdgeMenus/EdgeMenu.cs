using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class EdgeMenu : MonoBehaviour
{
    private bool _isOpen; 
    public bool IsOpen { get { return _isOpen; } }

    
    public void ToggleOpen()
    {
        if (!_isOpen)
        {
            Open();
        }
        else
        {
            Close();
        }
    }
    protected virtual void Open()
    {
        _isOpen = true;
        GameManager.OpenEdgeMenu(this);
    }
    protected virtual void Close()
    {
        _isOpen = false;
        GameManager.OpenEdgeMenu();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
