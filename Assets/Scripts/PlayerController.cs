using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    //player move speed
    [SerializeField] private float speed;
    //player's hands point, warehouse GameObject
    [SerializeField] private GameObject playerHands, warehouse;
    //is player on the warehouse
    private bool isOnWarehouse;
    //the box
    private GameObject box;
    //is the player on the shelf, is player alive
    public bool isOnShelf, isAlive = true;
    //Input System
    private NewInput PI;
    private void Awake()
    {
        PI = new NewInput();
        PI.Gameplay.Use.performed += context => Use();
        PI.Gameplay.Exit.performed += context => SceneManager.LoadScene("Menu");
    }
    private void OnEnable()
    { PI.Enable(); }
    private void OnDisable()
    { PI.Disable(); }
    void FixedUpdate()
    {
        //if the player is alive
        if (isAlive)
        { 
            //reading pressed keys
            float moveX = PI.Gameplay.MoveX.ReadValue<float>();
            float moveY = PI.Gameplay.MoveY.ReadValue<float>();
            //player rotation
            if(moveX != 0)
            {
                if (moveX == 1) transform.rotation = Quaternion.Euler(0, 0, -90);
                else if (moveX == -1) transform.rotation = Quaternion.Euler(0, 0, 90);
            }
            if (moveY != 0)
            { 
                if(moveY == 1) transform.rotation = Quaternion.Euler(0, 0, 0);
                else if (moveY == -1) transform.rotation = Quaternion.Euler(0, 0, 180);
            }
            //player movement
            Vector2 move = new Vector2(moveX * speed, moveY * speed);
            gameObject.GetComponent<Rigidbody2D>().velocity = move;
            //box moves with player
            if (box != null) box.transform.position = playerHands.transform.position;
        }
    }
    //everything that is activated vith E button
    private void Use()
    {
        //if the player is on the warehouse
        if (isOnWarehouse)
        {
            //get the box
            box = warehouse.GetComponent<WarehouseController>().currentBox;
        }
        //if the player is on the shelf
        if (isOnShelf)
        {
            //delete the box, spawn new one
            Destroy(box.gameObject);
            warehouse.GetComponent<WarehouseController>().SpawnBox();
        }
        //if the palyer is dead restart the level
        if (!isAlive) SceneManager.LoadScene("Gameplay");
    }
    //box drop
    private void DropBox()
    {
        if (box != null)
        {
            //delete the box, spawn new one
            Destroy(box);
            warehouse.GetComponent<WarehouseController>().SpawnBox();
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        //warehouse collision
        if (collision.gameObject.CompareTag("Warehouse")) isOnWarehouse = true;
        //customer collision
        if (collision.gameObject.CompareTag("Customer"))
        { 
            //drop the box
            DropBox();
            //loose hp
            gameObject.GetComponent<HealthController>().LooseHp();
        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        //player leaves the warehouse
        if (collision.gameObject.CompareTag("Warehouse")) isOnWarehouse = false;
    }
}
