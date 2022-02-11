using BoomAR.Permission.Infrastructure;
using System;

namespace BoomAR.Permission
{
    public class PermissionController
    {
        public Action PermissionWasGranted;

        public void ApplyPermission(PermissionSignals
                .PermissionWasGrantedSignal permissionWasGranted)
        {
            PermissionWasGranted.Invoke();
        }
    }
}
