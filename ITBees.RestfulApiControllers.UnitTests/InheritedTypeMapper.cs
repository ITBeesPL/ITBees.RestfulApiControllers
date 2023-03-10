using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Nelibur.ObjectMapper;

namespace ITBees.RestfulApiControllers.UnitTests
{
    public class InheritedTypeMapper
    {
        public static object Map<T, T2>(List<T2> vms) where T : List<T> where T2 : List<T2>
        {
            T map = TinyMapper.Map<T>(vms);

            return map;
        }

        public static void MapAndModifyUsingReflection<TInput, TOutput>(List<TInput> input, ref List<TOutput> output) where TOutput : new()
        {
            output = new List<TOutput>();

            var mapped = TinyMapper.Map<List<TOutput>>(input);

            output.AddRange(mapped);
            Console.WriteLine($">>> {output.First().GetType().Name}");
            var pi = GetPropertyWithClassType(output.First());
            for (int i = 0; i < input.Count; i++)
            {
                var value = input[i].GetType().Name;
                Console.WriteLine("ff+ " + value);
                pi.SetValue(output[i], value);
            }
        }

        public static void MapAndModify<TInput, TOutput>(List<TInput> input, ref List<TOutput> output) where TOutput : IVmWithPropertyType, new()
        {
            output = new List<TOutput>();

            var mapped = TinyMapper.Map<List<TOutput>>(input);

            output.AddRange(mapped);

            for (int i = 0; i < input.Count; i++)
            {
                output[i].SetTargetVmType(input[i].GetType().Name);
            }
        }

        private static PropertyInfo GetPropertyWithClassType<T>(T o) where T : new()
        {
            var typePropertyName = o.GetType().Name + "Type";
            var propertyInfo = o.GetType().GetProperties().FirstOrDefault(x => x.Name == typePropertyName);
            return propertyInfo;
        }
    }
}