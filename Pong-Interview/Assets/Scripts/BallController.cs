using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallController : Photon.PunBehaviour
{
    #region Public Variables
    
    public float MoveSpeed = 10.0f;
    #endregion

    #region Private Variables

    private Rigidbody RigidBody;
    private int XDirection;
    private int ZDirection;
    #endregion

    #region Public Methods
    #endregion

    #region Private Methods
    #endregion

    #region MonoBehavior CallBacks

    private void Start()
    {
        RigidBody = GetComponent<Rigidbody>();
        
        RigidBody.velocity = new Vector3(MoveSpeed, 0, MoveSpeed);

        XDirection = 1;
        ZDirection = 1;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.name == "Net")
        {

        }
        else
        {
            RigidBody = this.GetComponent<Rigidbody>();
            
            // If we hit a wall, reverse 'vertical' movement
            if(collision.gameObject.name == "Wall")
            {
                ZDirection *= -1;
                
            }
            else
            {
                // If we hit a player, reverse 'horizontal' movement
                XDirection *= -1;
            }
            
            RigidBody.velocity = new Vector3(MoveSpeed * XDirection, 0, MoveSpeed * ZDirection);
        }
    }
    #endregion

    #region Photon CallBacks
    #endregion
}
