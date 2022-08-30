using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

public static class AttributeExtension
{
  /// <summary>
  /// Returns true if the attribute whose type is specified by the generic argument is defined on this member
  /// </summary>
  public static bool IsDefined<T>(this ICustomAttributeProvider member, bool inherit) where T : Attribute
  {
    try
    {
      return member.IsDefined(typeof(T), inherit);
    }
    catch
    {
      return false;
    }
  }

  /// <summary>
  /// Returns true if the attribute whose type is specified by the generic argument is defined on this member
  /// </summary>
  public static bool IsDefined<T>(this ICustomAttributeProvider member) where T : Attribute =>
    member.IsDefined<T>(false);

  /// <summary>
  /// Returns the first found custom attribute of type T on this member
  /// Returns null if none was found
  /// </summary>
  public static T GetAttribute<T>(this ICustomAttributeProvider member, bool inherit) where T : Attribute
  {
    T[] array = member.GetAttributes<T>(inherit).ToArray<T>();
    return array != null && array.Length != 0 ? array[0] : default(T);
  }

  /// <summary>
  /// Returns the first found non-inherited custom attribute of type T on this member
  /// Returns null if none was found
  /// </summary>
  public static T GetAttribute<T>(this ICustomAttributeProvider member) where T : Attribute =>
    member.GetAttribute<T>(false);

  /// <summary>Gets all attributes of the specified generic type.</summary>
  /// <param name="member">The member.</param>
  public static IEnumerable<T> GetAttributes<T>(this ICustomAttributeProvider member) where T : Attribute =>
    member.GetAttributes<T>(false);

  /// <summary>Gets all attributes of the specified generic type.</summary>
  /// <param name="member">The member.</param>
  /// <param name="inherit">If true, specifies to also search the ancestors of element for custom attributes.</param>
  public static IEnumerable<T> GetAttributes<T>(
    this ICustomAttributeProvider member,
    bool inherit)
    where T : Attribute
  {
    try
    {
      return member.GetCustomAttributes(typeof(T), inherit).Cast<T>();
    }
    catch
    {
      return (IEnumerable<T>) new T[0];
    }
  }

  /// <summary>
  /// Gets all attribute instances defined on a MemeberInfo.
  /// </summary>
  /// <param name="member">The member.</param>
  public static Attribute[] GetAttributes(this ICustomAttributeProvider member)
  {
    try
    {
      return member.GetAttributes<Attribute>().ToArray<Attribute>();
    }
    catch
    {
      return new Attribute[0];
    }
  }

  /// <summary>Gets all attribute instances on a MemberInfo.</summary>
  /// <param name="member">The member.</param>
  /// <param name="inherit">If true, specifies to also search the ancestors of element for custom attributes.</param>
  public static Attribute[] GetAttributes(
    this ICustomAttributeProvider member,
    bool inherit)
  {
    try
    {
      return member.GetAttributes<Attribute>(inherit).ToArray<Attribute>();
    }
    catch
    {
      return new Attribute[0];
    }
  }
}