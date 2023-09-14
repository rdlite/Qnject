using System;
using UnityEngine;
using System.Reflection;
using System.Collections.Generic;
using System.Reflection.Emit;

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
                            ResolveFields(monoComponent);
                            ResolveMethods(monoComponent);
                        }
                    }
                }
            }
        }
        
        private static void ResolveMethods(MonoBehaviour target)
        {
            Type targetType = target.GetType();
            MethodInfo[] methods = targetType.GetMethods(bindingAttrs);

            foreach (MethodInfo method in methods)
            {
                foreach (Attribute attribute in method.GetCustomAttributes())
                {
                    if (attribute is not Inject)
                    {
                        continue;
                    }

                    int it = 0;
                    object[] parameters = new object[method.GetParameters().Length];

                    foreach (var parameter in method.GetParameters())
                    {
                        parameters[it] = TryGetObjFromContainers(parameter.ParameterType);
                        it++;
                    }

                    method.Invoke(target, parameters);
                }
            }
        }

        private static void ResolveFields(MonoBehaviour target)
        {
            Type targetType = target.GetType();
            FieldInfo[] fields = targetType.GetFields(bindingAttrs);

            foreach (FieldInfo fi in fields)
            {
                foreach (Attribute attribute in fi.GetCustomAttributes())
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