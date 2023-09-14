using UnityEngine;

namespace Qnject
{
    [DefaultExecutionOrder(-130)]
    public abstract class ProjectInstaller : Installer
    {
        protected override void Awake()
        {
            base.Awake();

            gameObject.name = "[ProjectContext]";

            DontDestroyOnLoad(gameObject);
        }

        public override void Bind() {}

        private void OnDestroy()
        {
            UnloadLocalDependencies();
        }

        protected void UnloadLocalDependencies()
        {
            MonoObjectsResolver.UnregisterContainer(_container);
            _container.Clear();
        }
    }
}