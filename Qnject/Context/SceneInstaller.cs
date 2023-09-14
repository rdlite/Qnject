using UnityEngine;

namespace Qnject
{
    [DefaultExecutionOrder(-120)]
    public abstract class SceneInstaller : Installer
    {
        protected override void Awake()
        {
            ContextsCreator.TryCreateProjectContextFromResources();

            base.Awake();

            ResolveScene();
        }

        private void OnDestroy()
        {
            UnloadLocalDependencies();
        }

        protected void UnloadLocalDependencies()
        {
            MonoObjectsResolver.UnregisterContainer(_container);
            _container.Clear();
        }

        private void ResolveScene()
        {
            MonoObjectsResolver.ResolveCurrentScene();
        }
    }
}