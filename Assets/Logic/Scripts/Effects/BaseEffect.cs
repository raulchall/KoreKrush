using UnityEngine;


public abstract class BaseEffect : MonoBehaviour
{
    public abstract void Apply();
}

public abstract class PreEffect : BaseEffect
{
    
} 

public abstract class RemovableEffect : BaseEffect
{
    public abstract void Remove();
}

public abstract class OnConnectEffect : RemovableEffect
{
    
}

public abstract class OnDestroyEffect : BaseEffect
{
    
}
