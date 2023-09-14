using System;
using UnityEngine;
using System.Reflection;
using System.Collections.Generic;

namespace Qnject
{
    public static class SceneResolver
    {
        private const BindingFlags bindingAttrs = BindingFlags.Instance |
                                                  BindingFlags.Public |
                                                  BindingFlags.NonPublic;

        private static List<Container> _containers = new List<Container>();

        public static void Resolve()
        {
            for (int i = 0; i < UnityEngine.SceneManagement.SceneManager.GetActiveScene().rootCount; i++)
            {
                foreach (var item in UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects()[i].transform.GetComponentsInChildren<MonoBehaviour>())
                {
                    MonoBehaviour[] monobesh = item.GetComponents<MonoBehaviour>();

                    if (monobesh != null && monobesh.Length > 0)
                    {
                        foreach (var monoComponent in monobesh)
                        {
                            ResolveObject(monoComponent);
                        }
                    }
                }
            }
        }

        private static void ResolveObject(MonoBehaviour target)
        {
            Type targetType = target.GetType();
            FieldInfo[] fields = targetType.GetFields(bindingAttrs);

            foreach (FieldInfo fi in fields)
            {
                IEnumerable<Attribute> attributes = fi.GetCustomAttributes();
                foreach (Attribute attribute in attributes)
                {
                    if (attribute is not Inject)
                    {
                        continue;
                    }

                    fi.SetValue(target, TryGetObjFromContainers(fi.FieldType));
                }
            }
        }

        public static void RegisterContainer(Container container)
        {
            _containers.Add(container);
        }

        public static void UnregisterContainer(Container container)
        {
            _containers.Remove(container);
        }

        private static object TryGetObjFromContainers(Type fieldType)
        {
            foreach (var container in _containers)
            {
                if (container.Get(fieldType) != null)
                {
                    return container.Get(fieldType);
                }
            }

            return null;
        }
    }
}