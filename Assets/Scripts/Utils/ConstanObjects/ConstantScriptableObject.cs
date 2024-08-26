
using UnityEngine;

/// <summary>
/// This class represents scriptable objects that can generate constants scripts for a IConstant Collection.
/// </summary>
/// <remarks>
/// And therefore for it to work properly there must be a collection of IConstant objects defined in the ScriptableObject.
/// For example this type of scriptable object is used to generate the constants of popups types.
/// </remarks>
public abstract class ConstantScriptableObject : ScriptableObject
{
    [HideInInspector]
    public string ClassName;
    [HideInInspector]
    public string PathFileConstants = ConstantsGenerator.DEFAULT_CONST_FILE_PATH;

    public abstract IConstant[] GetConstants();
}