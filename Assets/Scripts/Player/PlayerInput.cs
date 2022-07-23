using UnityEngine;

public static class PlayerInput
{
    public static Vector3 movementInput(Transform transform)
    {
        float ver = Input.GetAxisRaw("Vertical");
        float hor = Input.GetAxisRaw("Horizontal");
        return transform.forward * ver + transform.right * hor;
    }
}