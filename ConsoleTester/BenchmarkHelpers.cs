using BenchmarkDotNet.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleTester;
internal static class BenchmarkHelpers
{
	public static void WriteBenchmarkReturns<T>()
	{
		Console.WriteLine("Benchmark Returns");
		IEnumerable<MethodInfo> methods = typeof(T).GetMethods().Where(m => m.GetParameters().Length != 0 && m.ReturnType != typeof(void) && m.CustomAttributes.Any(c => c.AttributeType == typeof(BenchmarkAttribute)));
		object bench = typeof(T).GetConstructor(Type.EmptyTypes)!.Invoke(null);
		foreach (MethodInfo method in methods)
		{
			KeyValuePair<object, IEnumerable<KeyValuePair<string, object>>>[] rets;
			if (method.GetParameters().Length == 0)
				rets = new KeyValuePair<object, IEnumerable<KeyValuePair<string, object>>>[] { KeyValuePair.Create(method.Invoke(bench, null)!, Enumerable.Empty<KeyValuePair<string,object>>()) };
			else
			{
				string argumentsMethodName = method.GetCustomAttribute<ArgumentsSourceAttribute>()?.Name ?? throw new InvalidOperationException($"Benchmark must contain an ArgumentSource attribute");
				MethodInfo argumentsMethod = typeof(T).GetMethod(argumentsMethodName, Type.EmptyTypes) ?? throw new InvalidOperationException("ArgumentSource must be a method that takes no parameters");
				IEnumerable<object[]> argumentsEnumerable = argumentsMethod.Invoke(bench, null) as IEnumerable<object[]> ?? throw new InvalidOperationException("ArgumentSource method must return IEnumerable<object[]>");

				string?[] argumentNames = method.GetParameters().Select(p => p.Name).ToArray();

				List<KeyValuePair<object, IEnumerable<KeyValuePair<string, object>>>> returnsList = new List<KeyValuePair<object, IEnumerable<KeyValuePair<string, object>>>>();        // this is a horible type
				foreach (object[] arguments in argumentsEnumerable)
				{
					object ivk = method.Invoke(bench, arguments)!;
					IEnumerable<KeyValuePair<string, object>> nameValue = argumentNames.Zip(arguments, (name, arg) => KeyValuePair.Create(name!, arg));
					returnsList.Add(KeyValuePair.Create(ivk, nameValue));
				}
				rets = returnsList.ToArray();
			}

			foreach(KeyValuePair<object, IEnumerable<KeyValuePair<string, object>>> returnValue in rets)
			{
				string parameters = string.Join(", ", returnValue.Value.Select(v => $"{v.Key}: {v.Value}"));
				string output = $"{method.Name}({parameters}): {returnValue.Key}";
				Console.WriteLine(output);
			}
			Console.WriteLine();
		}
	}
}
