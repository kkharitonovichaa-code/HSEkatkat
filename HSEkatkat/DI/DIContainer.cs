namespace HSEkatkat.DI
{
    using System;
    using System.Collections.Generic;
    public class DiContainer
    {
        private readonly Dictionary<Type, Type> _registrations = new();
        private readonly Dictionary<Type, object> _instances = new();

        // регистрация
        public void Register<TInterface, TImplementation>() where TImplementation : TInterface
        {
            _registrations[typeof(TInterface)] = typeof(TImplementation);
        }
        
        public T Resolve<T>()
        {
            return (T)Resolve(typeof(T));
        }
        private object Resolve(Type type)
        {
            if (_instances.TryGetValue(type, out var cachedInstance))
            {
                return cachedInstance;
            }
            
            if (!_registrations.TryGetValue(type, out var implementationType))
            {
                throw new InvalidOperationException($"Тип {type.Name} не зарегистрирован");
            }
            var constructors = implementationType.GetConstructors();
            if (constructors.Length == 0)
            {
                throw new InvalidOperationException($"У типа {implementationType.Name} нет публичного конструктора");
            }
            var constructor = constructors[0];
            var parameters = constructor.GetParameters();
            var dependencies = new object[parameters.Length];

            // зависимости зависимостей
            for (int i = 0; i < parameters.Length; i++)
            {
                dependencies[i] = Resolve(parameters[i].ParameterType);
            }
            // создаем и сохраняем объект 
            var instance = Activator.CreateInstance(implementationType, dependencies);
            _instances[type] = instance;
            return instance;
        }
    }
}