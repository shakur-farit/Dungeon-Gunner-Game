using System;
using System.Collections;
using UnityEngine;

public static class HelperUtilities
{
    public static Camera mainCamera;

    public static Vector3 GetMouseWorldPosition()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;

        Vector3 mouseScreenPosition = Input.mousePosition;

        mouseScreenPosition.x = Mathf.Clamp(mouseScreenPosition.x, 0f, Screen.width);
        mouseScreenPosition.y = Mathf.Clamp(mouseScreenPosition.y, 0f, Screen.height);

        Vector3 worldPosition = mainCamera.ScreenToWorldPoint(mouseScreenPosition);

        worldPosition.z = 0;

        return worldPosition;
    }

    public static void CameraWorldPositionBounds(out Vector2Int cameraWorldPositionLowerBounds,
        out Vector2Int cameraWorldPositionUpperBounds, Camera camera)
    {
        Vector3 worldPositionViewportBottomLeft = camera.ViewportToWorldPoint(new Vector3(0f, 0f, 0f));
        Vector3 worldPositionViewportTopRight = camera.ViewportToWorldPoint(new Vector3(1f, 1f, 0f));

        cameraWorldPositionLowerBounds = new Vector2Int((int)worldPositionViewportBottomLeft.x,
            (int)worldPositionViewportBottomLeft.y);
        cameraWorldPositionUpperBounds = new Vector2Int((int)worldPositionViewportTopRight.x,
            (int)worldPositionViewportTopRight.y);
    }

    public static float GetAngelFromVector(Vector3 vector)
    {
        float radians = Mathf.Atan2(vector.y, vector.x);

        float degress = radians * Mathf.Rad2Deg;

        return degress;
    }

    public static Vector3 GetDirectionVectorFromAngle(float angle)
    {
        Vector3 directionVector = new Vector3(Mathf.Cos(Mathf.Deg2Rad * angle), Mathf.Sin(Mathf.Deg2Rad * angle), 0f);
        return directionVector;
    }

    public static AimDirection GetAimDerection(float angleDegree)
    {
        angleDegree = (angleDegree + 360) % 360; // Normalize angle to be positive

        string[] directionMap = { "Right", "UpRight", "Up", "UpLeft", "Left", "Down", "Right" };

        int index = (int)((angleDegree + 22.5) % 360) / 45;
        index = (index == 7) ? 0 : index; // Wrap around for 315-360 degrees

        return (AimDirection)Enum.Parse(typeof(AimDirection), directionMap[index]);
    }

    public static float LinearToDecibles(int linear)
    {
        float linearScaleRange = 20f;

        return Mathf.Log10((float)linear / linearScaleRange) * 20f;
    }

    public static bool ValidateCheckEmptyString(UnityEngine.Object thisObject, string fieldName, string stringToCheck)
    {
        if(stringToCheck == "")
        {
            Debug.Log(fieldName + " is empty and must contain a value object " + thisObject.name.ToString());
            return true;
        }
        return false;
    }

    public static bool ValidateCheckNullValue(UnityEngine.Object thisObject, string fieldName, UnityEngine.Object objectToCheck)
    {
        if (objectToCheck ==  null)
        {
            Debug.Log(fieldName + " is null and must contain a value object " + thisObject.name.ToString());
            return true;
        }
        return false;
    }

    public static bool ValidateCheckEnumerableValues(UnityEngine.Object thisObject, string filedName, IEnumerable enumerableObjectToCheck)
    {
        bool error = false;
        int count = 0;

        if(enumerableObjectToCheck == null)
        {
            Debug.Log(filedName + " is null in object " + thisObject.name.ToString());
            return true;
        }

        foreach (var item in enumerableObjectToCheck)
        {
            if(item == null)
            {
                Debug.Log(filedName + " has null values in object " + thisObject.name.ToString());
                error = true;
                continue;
            }

            count++;
        }

        if(count == 0)
        {
            Debug.Log(filedName + " has no values in object " + thisObject.name.ToString());
            error = true;
        }

        return error;
    }

    public static bool ValidateCheckPositiveValue(UnityEngine.Object thisObject, string fieldName, int valueToCheck, bool isZeroAllowed)
    {
        bool error = false;

        if (isZeroAllowed && valueToCheck < 0)
        {
            Debug.Log(fieldName + " must contain a positive value or zero in object " + thisObject.name.ToString());
            error = true;
            return error;

        }
        
        if(!isZeroAllowed && valueToCheck <= 0)
        {
            Debug.Log(fieldName + " must contain a positive value in object " + thisObject.name.ToString());
            error = true;
            Debug.Log("Here");
            return error;
        }

        return error;
    }

    public static bool ValidateCheckPositiveValue(UnityEngine.Object thisObject, string fieldName, float valueToCheck, bool isZeroAllowed)
    {
        bool error = false;

        if (isZeroAllowed && valueToCheck < 0)
        {
            Debug.Log(fieldName + " must contain a positive value or zero in object " + thisObject.name.ToString());
            error = true;
            return error;
            
        }

        if (!isZeroAllowed && valueToCheck <= 0)
        {
            Debug.Log(fieldName + " must contain a positive value in object " + thisObject.name.ToString());
            error = true;
            return error;
        }

        return error;
    }

    public static bool ValidateCheckPositiveRange(UnityEngine.Object thisObject, string fieldNameMinimum, float valueToCheckMinimum,
        string fieldNameMaximum, float valueToCheckMaximum, bool isZeroAllowed)
    {
        bool error = false;

        if(valueToCheckMinimum > valueToCheckMaximum)
        {
            Debug.Log(fieldNameMinimum + " must be less than or equal to " + fieldNameMaximum + " in object " +
                thisObject.name.ToString());
            error = true;
        }

        if (ValidateCheckPositiveValue(thisObject, fieldNameMinimum, valueToCheckMinimum, isZeroAllowed))
            error = true;

        if(ValidateCheckPositiveValue(thisObject,fieldNameMaximum, valueToCheckMaximum, isZeroAllowed))
            error = true;

        return error;
    }

    public static Vector3 GetSpawnPositionNearestToPlayer(Vector3 playerPosition)
    {
        Room currentRoom = GameManager.Instance.GetCurrentRoom;

        Grid grid = currentRoom.instantiatedRoom.grid;

        Vector3 nearestSpawnPosition = new Vector3(10000f, 10000f, 0);

        foreach (Vector2Int spawnPositionGrid in currentRoom.spawnPositionArray)
        {
            Vector3 spawnPositionWorld = grid.CellToWorld((Vector3Int)spawnPositionGrid);

            if(Vector3.Distance(spawnPositionWorld, playerPosition) < Vector3.Distance(nearestSpawnPosition, playerPosition))
                nearestSpawnPosition = spawnPositionWorld;
        }

        return nearestSpawnPosition;
    }
}
