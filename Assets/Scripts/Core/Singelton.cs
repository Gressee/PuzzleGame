using UnityEngine;

namespace Core.Singleton
{
    public class Singleton<T> : MonoBehaviour where T : Component
    {
        private static T instance;
        public static T Instance {
            get 
            {
                if (instance == null) 
                {
                    instance = FindObjectOfType<T> ();
                    if (instance == null)
                    {
                        GameObject obj = new GameObject ();
                        obj.name = typeof(T).Name;
                        instance = obj.AddComponent<T>();
                    }
                }
            return instance;
            }
        }

        // Instead of 'Awake' call 'AwakeSingelton' in a derived class
        protected virtual void AwakeSingelton()
        {

        }

        public void Awake ()
        {
            if (instance == null)
            {
                instance = this as T;
                AwakeSingelton();
                //DontDestroyOnLoad (this.gameObject);
            }
            else
            {
                Destroy (gameObject);
            }
        }
    }

    /*
    public class Singleton<T> : MonoBehaviour
        where T : Component
    {
        private static T _instance;
        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    var objs = FindObjectsOfType(typeof(T)) as T[];
                    if (objs.Length > 0)
                        _instance = objs[0];
                    if (objs.Length > 1)
                    {
                        Debug.LogError("There is more than one " + typeof(T).Name + " in the scene.");
                    }
                    if (_instance == null)
                    {
                        GameObject obj = new GameObject();
                        obj.name = string.Format("_{0}", typeof(T).Name);
                        _instance = obj.AddComponent<T>();
                    }
                }
                return _instance;
            }
        }

        void Awake()
        {
            if (_instance != null)
            {
                Destroy(gameObject);
            }
        }
    }
    */
}
