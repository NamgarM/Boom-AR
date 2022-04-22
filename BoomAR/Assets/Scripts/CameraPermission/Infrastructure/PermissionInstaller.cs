using System;
using Zenject;

namespace BoomAR.Permission.Infrastructure
{
    public class PermissionInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            SignalBusInstaller.Install(Container);
            BindSignals();
            BindControllers();
        }

        private void BindControllers()
        {
            PermissionController permissionController = Container
                .Instantiate<PermissionController>();

            Container
                .Bind<PermissionController>()
                .FromInstance(permissionController)
                .AsSingle();
        }

        private void BindSignals()
        {
            BindPermissionWasGrantedSignal();
        }

        private void BindPermissionWasGrantedSignal()
        {
            Container
                .DeclareSignal<PermissionSignals
                .PermissionWasGrantedSignal>();
            Container.BindSignal<PermissionSignals
                .PermissionWasGrantedSignal>()
                .ToMethod<PermissionController>(x => x.ApplyPermission)
                .FromResolve();
        }
    }
}