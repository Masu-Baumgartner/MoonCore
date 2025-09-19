namespace MoonCore.Helpers;

public static class PermissionHelper
{
    /// <summary>
    /// Check if the required permission is contained in the provided permission array.
    /// It uses the LuckPerms permission syntax.
    /// More details <see href="https://luckperms.net/wiki/Usage">here</see>
    /// </summary>
    /// <param name="permissions">Array of permissions to look in</param>
    /// <param name="requiredPermission">Permission to check for</param>
    /// <returns>True if the permission was found, False if the permission was not found</returns>
    public static bool HasPermission(string[] permissions, string requiredPermission)
    {
        // Check for wildcard permission
        if (permissions.Contains("*"))
            return true;

        var requiredSegments = requiredPermission.Split('.');

        // Check if the user has the exact permission or a wildcard match
        foreach (var permission in permissions)
        {
            var permissionSegments = permission.Split('.');

            // Iterate over the segments of the required permission
            for (var i = 0; i < requiredSegments.Length; i++)
            {
                // If the current segment matches or is a wildcard, continue to the next segment
                if (i < permissionSegments.Length && requiredSegments[i] == permissionSegments[i] ||
                    permissionSegments[i] == "*")
                {
                    // If we've reached the end of the permissionSegments array, it means we've found a match
                    if (i == permissionSegments.Length - 1)
                        return true; // Found an exact match or a wildcard match
                }
                else
                {
                    // If we reach here, it means the segments don't match and we break out of the loop
                    break;
                }
            }
        }

        // No matching permission found
        return false;
    }
}