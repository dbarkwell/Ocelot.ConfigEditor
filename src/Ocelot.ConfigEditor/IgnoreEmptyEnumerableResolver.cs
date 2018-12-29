using System.Collections;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Ocelot.ConfigEditor
{
    public class IgnoreEmptyEnumerableResolver : DefaultContractResolver
    {
        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            var property = base.CreateProperty(member, memberSerialization);

            if (property.PropertyType != typeof(string) &&
                typeof(IEnumerable).IsAssignableFrom(property.PropertyType))
            {
                property.ShouldSerialize = instance =>
                {
                    var enumerable = GetEnumerable(member, instance);
                    return enumerable?.GetEnumerator().MoveNext() ?? true;
                };
            }

            return property;
        }

        private static IEnumerable GetEnumerable(MemberInfo member, object instance)
        {
            switch (member.MemberType)
            {
                case MemberTypes.Property:
                    return instance
                        .GetType()
                        .GetProperty(member.Name)
                        .GetValue(instance) as IEnumerable;
                case MemberTypes.Field:
                    return instance
                        .GetType()
                        .GetField(member.Name)
                        .GetValue(instance) as IEnumerable;
                default:
                    return null;
            }
        }
    }
}