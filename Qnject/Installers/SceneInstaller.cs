using UnityEngine;

namespace Qnject
{
    [DefaultExecutionOrder(-10)]
    public abstract class SceneInstaller : MonoBehaviour
    {
        protected Container _container;

        private void Awake()
        {
            _container = new Container();

            SceneResolver.RegisterContainer(_container);

            Bind();
            ResolveScene();
        }

        private void OnDestroy()
        {
            UnloadLocalDependencies();
        }

        public abstract void Bind();

        protected void UnloadLocalDependencies()
        {
            SceneResolver.UnregisterContainer(_container);
            _container.Clear();
        }

        private void ResolveScene()
        {
            SceneResolver.Resolve();
        }
    }
}