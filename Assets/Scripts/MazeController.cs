using UnityEngine;
using UnityEngine.InputSystem;
public class MazeController : MonoBehaviour
{
    public GameObject maze;
    public float rotationSpeed = 5;
    private InputAction rotateInput; 
    void Start()
    {
        rotateInput = InputSystem.actions.FindAction("Player/Move");
    }
    
    void Update()
    {
        Vector2 rotation = rotateInput.ReadValue<Vector2>();
        //Debug.Log("rotation x: " + rotation.x + " y " + rotation.y);
        maze.transform.Rotate(
            rotation.x * rotationSpeed * Time.deltaTime,
            0,
            -rotation.y * rotationSpeed * Time.deltaTime);
    }
}