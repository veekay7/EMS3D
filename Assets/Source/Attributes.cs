using System;
using UnityEngine;

/// <summary>
/// A property attribute which makes a public or serialized private variable
/// appear as read-only in the Unity inspector.
/// </summary>
[AttributeUsage(AttributeTargets.Field)]
public class ReadOnlyVarAttribute : PropertyAttribute { }


/// <summary>
/// Mark a method with an integer argument with this to display the argument as an enum popup in the UnityEvent
/// drawer. Use: [EnumAction(typeof(SomeEnumType))]
/// https://forum.unity.com/threads/ability-to-add-enum-argument-to-button-functions.270817/#post-2785739
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public class EnumActionAttribute : PropertyAttribute
{
    public Type EnumType;


    public EnumActionAttribute(Type enumType)
    {
        this.EnumType = enumType;
    }
}


/// <summary>
/// Reorderable list attribute
/// </summary>
[AttributeUsage(AttributeTargets.Field)]
public class ReorderableListAttribute : PropertyAttribute
{
    public float r, g, b;

    public bool disableAdding;

    public bool disableRemoving;

    public bool disableAddingAndRemoving
    {
        get { return disableAdding && disableRemoving; }
        set { disableAdding = disableRemoving = value; }
    }

    public bool disableDragging;

    public bool elementsAreSubassets;

    public string elementHeaderFormat;

    public string listHeaderFormat;

    public bool hideFooterButtons;

    public string[] parallelListNames;

    public enum ParallelListLayout { Rows, Columns };

    public ParallelListLayout parallelListLayout;


    public ReorderableListAttribute() { }

    public ReorderableListAttribute(params string[] parallelListNames)
    {
        this.parallelListNames = parallelListNames;
    }

    public const string SingularPluralBlockBegin = "{{";
    public const string SingularPluralBlockSeparator = "|";
    public const string SingularPluralBlockEnd = "}}";

    public string singularListHeaderFormat
    {
        get
        {
            if (listHeaderFormat == null)
                return null;

            var value = listHeaderFormat;
            while (value.Contains(SingularPluralBlockBegin))
            {
                int beg = value.IndexOf(SingularPluralBlockBegin);
                int end = value.IndexOf(SingularPluralBlockEnd, beg);
                
                if (end < 0)
                    break;
                
                end += SingularPluralBlockEnd.Length;
                
                int blockLen = end - beg;
                var block = value.Substring(beg, blockLen);
                int sep = value.IndexOf(SingularPluralBlockSeparator, beg);
                
                if (sep < 0)
                {
                    value = value.Replace(block, "");
                }
                else
                {
                    beg += SingularPluralBlockBegin.Length;
                    int singularLen = (sep - beg);
                    var singular = value.Substring(beg, singularLen);
                    value = value.Replace(block, singular);
                }
            }

            return value;
        }
    }

    public string pluralListHeaderFormat
    {
        get
        {
            if (listHeaderFormat == null)
                return null;

            var value = listHeaderFormat;
            while (value.Contains(SingularPluralBlockBegin))
            {
                int beg = value.IndexOf(SingularPluralBlockBegin);
                int end = value.IndexOf(SingularPluralBlockEnd, beg);
                
                if (end < 0)
                    break;

                end += SingularPluralBlockEnd.Length;
                
                int blockLen = end - beg;
                var block = value.Substring(beg, blockLen);
                int sep = value.IndexOf(SingularPluralBlockSeparator, beg);
                
                if (sep < 0)
                {
                    beg += SingularPluralBlockBegin.Length;
                    end -= SingularPluralBlockEnd.Length;
                    int pluralLen = (end - beg);
                    var plural = value.Substring(beg, pluralLen);
                    value = value.Replace(block, plural);
                }
                else
                {
                    sep = sep + SingularPluralBlockSeparator.Length;
                    end -= SingularPluralBlockEnd.Length;
                    int pluralLen = (end - sep);
                    var plural = value.Substring(beg, pluralLen);
                    value = value.Replace(block, plural);
                }
            }

            return value;
        }
    }
}
