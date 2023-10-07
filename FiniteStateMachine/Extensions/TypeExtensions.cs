namespace FiniteStateMachine.Extensions;

internal static class TypeExtensions
{
    internal static string GetParametersTypes(this Type? type) =>
        type is null
            ? "No Parameters"
            : type.IsGenericType
                ? string.Join(",", type.GenericTypeArguments.Select(g => g.Name))
                : type.Name;
}