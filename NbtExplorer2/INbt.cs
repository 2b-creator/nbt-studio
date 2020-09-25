﻿using Aga.Controls.Tree;
using fNbt;
using NbtExplorer2.SNBT;
using NbtExplorer2.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NbtExplorer2
{
    // provides psuedo-interfaces to NBT tags as methods
    public static class INbt
    {
        // everything except End and Unknown
        public static IEnumerable<NbtTagType> NormalTagTypes()
        {
            yield return NbtTagType.Byte;
            yield return NbtTagType.Short;
            yield return NbtTagType.Int;
            yield return NbtTagType.Long;
            yield return NbtTagType.Float;
            yield return NbtTagType.Double;
            yield return NbtTagType.String;
            yield return NbtTagType.ByteArray;
            yield return NbtTagType.IntArray;
            yield return NbtTagType.LongArray;
            yield return NbtTagType.Compound;
            yield return NbtTagType.List;
        }

        // tags with simple primitive values
        // excludes lists, compounds, and arrays
        public static bool IsValueType(NbtTagType type)
        {
            switch (type)
            {
                case NbtTagType.Byte:
                case NbtTagType.Short:
                case NbtTagType.Int:
                case NbtTagType.Long:
                case NbtTagType.Float:
                case NbtTagType.Double:
                case NbtTagType.String:
                    return true;
                default:
                    return false;
            }
        }

        public static bool IsArrayType(NbtTagType type)
        {
            switch (type)
            {
                case NbtTagType.ByteArray:
                case NbtTagType.IntArray:
                case NbtTagType.LongArray:
                    return true;
                default:
                    return false;
            }
        }

        public static NbtTag GetNbt(object obj)
        {
            if (obj is NbtFile file)
                return file.RootTag;
            if (obj is NbtTag tag)
                return tag;
            return null;
        }

        public static void SetValue(INbtTag tag, object value)
        {
            if (tag is INbtByte tag_byte && value is byte b)
                tag_byte.Value = b;
            else if (tag is INbtShort tag_short && value is short s)
                tag_short.Value = s;
            else if (tag is INbtInt tag_int && value is int i)
                tag_int.Value = i;
            else if (tag is INbtLong tag_long && value is long l)
                tag_long.Value = l;
            else if (tag is INbtFloat tag_float && value is float f)
                tag_float.Value = f;
            else if (tag is INbtDouble tag_double && value is double d)
                tag_double.Value = d;
            else if (tag is INbtString tag_string && value is string str)
                tag_string.Value = str;
        }

        public static object ParseValue(string value, NbtTagType type)
        {
            switch (type)
            {
                case NbtTagType.Byte:
                    return (byte)sbyte.Parse(value);
                case NbtTagType.Short:
                    return short.Parse(value);
                case NbtTagType.Int:
                    return int.Parse(value);
                case NbtTagType.Long:
                    return long.Parse(value);
                case NbtTagType.Float:
                    return float.Parse(value, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture);
                case NbtTagType.Double:
                    return double.Parse(value, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture);
                case NbtTagType.String:
                    return value;
                default:
                    return null;
            }
        }

        // clears any existing data in the tag's array
        public static void SetSize(INbtTag tag, int size)
        {
            if (tag is INbtByteArray tag_byte_array)
                tag_byte_array.Value = new byte[size];
            else if (tag is INbtIntArray tag_int_array)
                tag_int_array.Value = new int[size];
            else if (tag is INbtLongArray tag_long_array)
                tag_long_array.Value = new long[size];
        }

        public static NbtTag CreateTag(NbtTagType type)
        {
            switch (type)
            {
                case NbtTagType.Byte:
                    return new NbtByte();
                case NbtTagType.Short:
                    return new NbtShort();
                case NbtTagType.Int:
                    return new NbtInt();
                case NbtTagType.Long:
                    return new NbtLong();
                case NbtTagType.Float:
                    return new NbtFloat();
                case NbtTagType.Double:
                    return new NbtDouble();
                case NbtTagType.String:
                    return new NbtString();
                case NbtTagType.ByteArray:
                    return new NbtByteArray();
                case NbtTagType.IntArray:
                    return new NbtIntArray();
                case NbtTagType.LongArray:
                    return new NbtLongArray();
                case NbtTagType.Compound:
                    return new NbtCompound();
                case NbtTagType.List:
                    return new NbtList();
                default:
                    throw new ArgumentException($"Can't create a tag from {type}");
            }
        }

        public static Tuple<string, string> MinMaxFor(NbtTagType type)
        {
            switch (type)
            {
                case NbtTagType.Byte:
                    return Tuple.Create(sbyte.MinValue.ToString(), sbyte.MaxValue.ToString());
                case NbtTagType.Short:
                    return Tuple.Create(short.MinValue.ToString(), short.MaxValue.ToString());
                case NbtTagType.Int:
                    return Tuple.Create(int.MinValue.ToString(), int.MaxValue.ToString());
                case NbtTagType.Long:
                    return Tuple.Create(long.MinValue.ToString(), long.MaxValue.ToString());
                case NbtTagType.Float:
                    return Tuple.Create(Util.FloatToString(float.MinValue), Util.FloatToString(float.MaxValue));
                case NbtTagType.Double:
                    return Tuple.Create(Util.DoubleToString(double.MinValue), Util.DoubleToString(double.MaxValue));
                default:
                    throw new ArgumentException($"{type} isn't numeric, has no min and max");
            }
        }

        public static IEnumerable<object> GetChildren(object obj)
        {
            if (obj is NbtFolder folder)
                return folder.Subfolders.Concat<object>(folder.Files);
            if (obj is NbtFile file)
                return file.RootTag.Tags;
            if (obj is NbtCompound compound)
                return compound.Tags;
            if (obj is NbtList list)
                return list;
            return Enumerable.Empty<object>();
        }

        public static Tuple<string, string> PreviewNameAndValue(object obj)
        {
            string name = PreviewName(obj);
            string value = PreviewValue(obj);
            if (name == null)
            {
                if (value == null)
                    return null;
                return Tuple.Create((string)null, value);
            }
            return Tuple.Create(name + ": ", value);
        }

        public static string PreviewName(object obj)
        {
            if (obj is NbtFile file)
                return Path.GetFileName(file.Path);
            if (obj is NbtFolder folder)
                return Path.GetFileName(folder.Path);
            if (obj is NbtTag tag)
                return tag.Name;
            return null;
        }

        public static string PreviewValue(object obj)
        {
            if (obj is NbtFile file)
                return PreviewNbtValue(file);
            if (obj is NbtFolder folder)
                return PreviewNbtValue(folder);
            if (obj is NbtTag tag)
                return PreviewNbtValue(tag.Adapt());
            return null;
        }

        public static string PreviewNbtValue(INbtTag tag)
        {
            if (tag is INbtCompound compound)
                return $"[{Util.Pluralize(compound.Count, "entry", "entries")}]";
            else if (tag is INbtList list)
            {
                if (list.Count == 0)
                    return $"[0 entries]";
                return $"[{Util.Pluralize(list.Count, TagTypeName(list.ListType).ToLower())}]";
            }
            else if (tag is INbtByteArray byte_array)
                return $"[{Util.Pluralize(byte_array.Value.Length, "byte")}]";
            else if (tag is INbtIntArray int_array)
                return $"[{Util.Pluralize(int_array.Value.Length, "int")}]";
            else if (tag is INbtLongArray long_array)
                return $"[{Util.Pluralize(long_array.Value.Length, "long")}]";
            return tag.ToSnbt(expanded: false, delimit: false);
        }

        public static string PreviewNbtValue(NbtFile file) => PreviewNbtValue(file.RootTag.Adapt());
        public static string PreviewNbtValue(NbtFolder folder) => $"[{Util.Pluralize(folder.Files.Count, "file")}]";

        public static string TagTypeName(NbtTagType type)
        {
            if (type == NbtTagType.ByteArray)
                return "Byte Array";
            if (type == NbtTagType.IntArray)
                return "Int Array";
            if (type == NbtTagType.LongArray)
                return "Long Array";
            return type.ToString();
        }

        public class TagNameSorter : IComparer<INbtTag>
        {
            public int Compare(INbtTag x, INbtTag y)
            {
                return x.Name.CompareTo(y.Name);
            }
        }

        public class TagTypeSorter : IComparer<INbtTag>
        {
            private static readonly Dictionary<NbtTagType, int> TypeOrder = new Dictionary<NbtTagType, int>
            {
                {NbtTagType.Compound, 1},
                {NbtTagType.List, 2},
                {NbtTagType.Byte, 3},
                {NbtTagType.Short, 4},
                {NbtTagType.Int, 5},
                {NbtTagType.Long, 6},
                {NbtTagType.Float, 7},
                {NbtTagType.Double, 8},
                {NbtTagType.String, 9},
                {NbtTagType.ByteArray, 9},
                {NbtTagType.IntArray, 10},
                {NbtTagType.LongArray, 11},
            };
            public int Compare(INbtTag x, INbtTag y)
            {
                int compare = TypePriority(x.TagType).CompareTo(TypePriority(y.TagType));
                if (compare != 0)
                    return compare;
                return x.Name.CompareTo(y.Name);
            }
            private int TypePriority(NbtTagType type)
            {
                if (TypeOrder.TryGetValue(type, out int result))
                    return result;
                return int.MaxValue;
            }
        }

        public static void Sort(INbtCompound compound, IComparer<INbtTag> sorter, bool recursive)
        {
            var tags = compound.Tags.OrderBy(x => x, sorter).ToList();
            compound.Clear();
            foreach (var tag in tags)
            {
                if (recursive)
                {
                    if (tag is INbtCompound sub)
                        Sort(sub, sorter, true);
                    else if (tag is INbtList list)
                        SortChildren(list, sorter);
                }
                tag.AddTo(compound);
            }
        }

        private static void SortChildren(INbtList list, IComparer<INbtTag> sorter)
        {
            if (list.ListType == NbtTagType.Compound)
            {
                foreach (INbtCompound item in list)
                {
                    Sort(item, sorter, true);
                }
            }
            else if (list.ListType == NbtTagType.List)
            {
                foreach (INbtList item in list)
                {
                    SortChildren(item, sorter);
                }
            }
        }

        public static Tuple<INbtContainer, int> GetInsertionLocation(INbtTag target, NodePosition position)
        {
            if (position == NodePosition.Inside)
            {
                var container = target as INbtContainer;
                return Tuple.Create(container, container?.Count ?? 0);
            }
            else
            {
                var parent = target.Parent;
                int index = target.Index;
                if (position == NodePosition.After)
                    index++;
                return Tuple.Create(parent, index);
            }
        }

        public static void TransformInsert(INbtTag tag, INbtContainer destination, int index)
        {
            if (tag.IsInside(destination) && index > tag.Index)
                index--;
            tag.Remove();
            if (destination is INbtCompound compound)
                tag.Name = GetAutomaticName(tag, compound);
            else if (destination is INbtList)
                tag.Name = null;
            tag.InsertInto(destination, index);
        }

        public static string GetAutomaticName(INbtTag tag, INbtCompound parent)
        {
            if (tag.Name != null && !parent.Contains(tag.Name))
                return tag.Name;
            string basename = tag.Name ?? INbt.TagTypeName(tag.TagType).ToLower().Replace(' ', '_');
            for (int i = 1; i < 999999; i++)
            {
                string name = basename + i.ToString();
                if (!parent.Contains(name))
                    return name;
            }
            throw new InvalidOperationException("This compound really contains 999999 similarly named tags?!");
        }

        public static void TransformAdd(INbtTag tag, INbtContainer destination) => TransformInsert(tag, destination, destination.Count);

        public static bool CanAddAll(IEnumerable<INbtTag> tags, INbtContainer destination)
        {
            // check if you're trying to add items of different types to a list
            if (destination is INbtList list && tags.Select(x => x.TagType).Distinct().Skip(1).Any())
                return false;
            // check if you're trying to add an item to its own descendent
            var ancestors = Ancestors(destination);
            foreach (var ancestor in ancestors)
            {
                if (tags.Any(x => x.IsInside(ancestor)))
                    return false;
            }
            return tags.All(x => destination.CanAdd(x.TagType));
        }

        public static List<INbtContainer> Ancestors(INbtTag tag)
        {
            var ancestors = new List<INbtContainer>();
            var parent = tag.Parent;
            while (parent != null)
            {
                ancestors.Add(parent);
                parent = parent.Parent;
            }
            return ancestors;
        }

        public static Image Image(object obj)
        {
            if (obj is NbtFile)
                return Properties.Resources.file_image;
            if (obj is NbtFolder)
                return Properties.Resources.folder_image;
            if (obj is NbtTag tag)
                return TagTypeImage(tag.TagType);
            return null;
        }

        public static Image TagTypeImage(NbtTagType type)
        {
            switch (type)
            {
                case NbtTagType.Byte:
                    return Properties.Resources.tag_byte_image;
                case NbtTagType.Short:
                    return Properties.Resources.tag_short_image;
                case NbtTagType.Int:
                    return Properties.Resources.tag_int_image;
                case NbtTagType.Long:
                    return Properties.Resources.tag_long_image;
                case NbtTagType.Float:
                    return Properties.Resources.tag_float_image;
                case NbtTagType.Double:
                    return Properties.Resources.tag_double_image;
                case NbtTagType.ByteArray:
                    return Properties.Resources.tag_byte_array_image;
                case NbtTagType.String:
                    return Properties.Resources.tag_string_image;
                case NbtTagType.List:
                    return Properties.Resources.tag_list_image;
                case NbtTagType.Compound:
                    return Properties.Resources.tag_compound_image;
                case NbtTagType.IntArray:
                    return Properties.Resources.tag_int_array_image;
                case NbtTagType.LongArray:
                    return Properties.Resources.tag_long_array_image;
                default:
                    return null;
            }
        }

        public static Icon TagTypeIcon(NbtTagType type)
        {
            switch (type)
            {
                case NbtTagType.Byte:
                    return Properties.Resources.tag_byte_icon;
                case NbtTagType.Short:
                    return Properties.Resources.tag_short_icon;
                case NbtTagType.Int:
                    return Properties.Resources.tag_int_icon;
                case NbtTagType.Long:
                    return Properties.Resources.tag_long_icon;
                case NbtTagType.Float:
                    return Properties.Resources.tag_float_icon;
                case NbtTagType.Double:
                    return Properties.Resources.tag_double_icon;
                case NbtTagType.ByteArray:
                    return Properties.Resources.tag_byte_array_icon;
                case NbtTagType.String:
                    return Properties.Resources.tag_string_icon;
                case NbtTagType.List:
                    return Properties.Resources.tag_list_icon;
                case NbtTagType.Compound:
                    return Properties.Resources.tag_compound_icon;
                case NbtTagType.IntArray:
                    return Properties.Resources.tag_int_array_icon;
                case NbtTagType.LongArray:
                    return Properties.Resources.tag_long_array_icon;
                default:
                    return null;
            }
        }
    }
}
