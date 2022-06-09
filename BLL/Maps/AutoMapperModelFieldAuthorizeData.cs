using Domain.Entities;

namespace BLL.Maps;

public class AutoMapperModelFieldAuthorizeData
{
    /// <summary>
    /// This is a permission of an Subject that initiates an action.
    /// Must be a PermissionType.Value
    /// </summary>
    public Permission PermissionComparable { get; set; }
    
    /// <summary>
    /// This is a permission of an Object's Subject over which the action is performed.
    /// Must be any of [PermissionType.ValueNeededSelf, PermissionType.ValueNeededOthers, PermissionType.ValueNeededAny]
    /// </summary>
    public Permission PermissionCompared { get; set; }
    
    /// <summary>
    /// This is a permission of a System.
    /// Must be a PermissionType.ValueNeededSystem
    /// </summary>
    public PermissionValue PermissionValueSystemCompared { get; set; }
    
    /// <summary>
    /// Custom value
    /// </summary>
    public byte[] CustomValueCompared { get; set; }
}