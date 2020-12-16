﻿using fNbt;
using NbtStudio.SNBT;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace NbtStudio
{
    // represents a loadable and saveable NBT file
    // uses fNbt.NbtFile to do the work reading/writing binary data to disk, but can also read/write SNBT without using one
    public class NbtFile : ISaveable
    {
        public string Path { get; private set; }
        public bool IsFolder => false;
        public event EventHandler OnSaved;
        public NbtCompound RootTag { get; private set; }
        public ExportSettings ExportSettings { get; private set; }
        public bool CanSave => Path != null && ExportSettings != null;
        public bool HasUnsavedChanges { get; private set; } = false;

        private NbtFile(string path, NbtCompound root, ExportSettings settings)
        {
            Path = path;
            SetRoot(root);
            ExportSettings = settings;
        }

        public NbtFile() : this(new NbtCompound(""))
        { }

        public NbtFile(NbtCompound root)
        {
            if (root.Name == null)
                root.Name = "";
            SetRoot(root);
            Path = null;
            ExportSettings = null;
            HasUnsavedChanges = true;
        }

        private void SetRoot(NbtCompound root)
        {
            RootTag = root;
            RootTag.Changed += (s, e) => HasUnsavedChanges = true;
        }

        private static bool LooksSuspicious(NbtTag tag)
        {
            foreach (var ch in tag.Name)
            {
                if (Char.IsControl(ch))
                    return true;
            }
            return false;
        }

        public static Failable<NbtFile> TryCreate(string path)
        {
            var methods = new Func<Failable<NbtFile>>[]
            {
                () => TryCreateFromSnbt(path), // SNBT
                () => TryCreateFromNbt(path, NbtCompression.AutoDetect, big_endian: true), // java files
                () => TryCreateFromNbt(path, NbtCompression.AutoDetect, big_endian: false), // bedrock files
                () => TryCreateFromNbt(path, NbtCompression.AutoDetect, big_endian: false, bedrock_header: true) // bedrock level.dat files
            };
            return TryVariousMethods(methods, x => LooksSuspicious(x.RootTag));
        }

        public static Failable<NbtFile> TryVariousMethods(IEnumerable<Func<Failable<NbtFile>>> methods, Predicate<NbtFile> suspicious)
        {
            // try loading a file a few different ways
            // if loading fails or looks suspicious, try a different way
            // if all loads are suspicious, choose the first that didn't fail
            var attempted = new List<Failable<NbtFile>>();
            foreach (var method in methods)
            {
                var result = method();
                if (!result.Failed && !suspicious(result.Result))
                    return result;
                attempted.Add(result);
            }
            // everything was suspicious, pick the first that didn't fail
            foreach (var attempt in attempted)
            {
                if (!attempt.Failed)
                    return attempt;
            }
            // everything failed, sob!
            return attempted.First();
        }

        public static Failable<NbtFile> TryCreateFromSnbt(string path)
        {
            return new Failable<NbtFile>(() => CreateFromSnbt(path));
        }

        public static NbtFile CreateFromSnbt(string path)
        {
            using (var stream = File.OpenRead(path))
            using (var reader = new StreamReader(stream, Encoding.UTF8))
            {
                char[] firstchar = new char[1];
                reader.ReadBlock(firstchar, 0, 1);
                if (firstchar[0] != '{') // optimization to not load in huge files
                    throw new FormatException("File did not begin with a '{'");
                var text = firstchar[0] + reader.ReadToEnd();
                var tag = SnbtParser.Parse(text, named: false);
                if (!(tag is NbtCompound compound))
                    throw new FormatException("File did not contain an NBT compound");
                compound.Name = "";
                var file = new fNbt.NbtFile(compound);
                return new NbtFile(path, file.RootTag, ExportSettings.AsSnbt(!text.Contains("\n"), System.IO.Path.GetExtension(path) == ".json"));
            }
        }

        public static Failable<NbtFile> TryCreateFromNbt(string path, NbtCompression compression, bool big_endian = true, bool bedrock_header = false)
        {
            return new Failable<NbtFile>(() => CreateFromNbt(path, compression, big_endian, bedrock_header));
        }

        public static NbtFile CreateFromNbt(string path, NbtCompression compression, bool big_endian = true, bool bedrock_header = false)
        {
            var file = new fNbt.NbtFile();
            file.BigEndian = big_endian;
            using (var reader = File.OpenRead(path))
            {
                if (bedrock_header)
                {
                    var header = new byte[8];
                    reader.Read(header, 0, header.Length);
                }
                file.LoadFromStream(reader, compression);
            }
            if (file.RootTag == null)
                throw new FormatException("File had no root tag");

            return new NbtFile(path, file.RootTag, ExportSettings.AsNbt(file.FileCompression, big_endian, bedrock_header));
        }

        public void Save()
        {
            ExportSettings.Export(Path, RootTag);
            HasUnsavedChanges = false;
            OnSaved?.Invoke(this, EventArgs.Empty);
        }

        public void SaveAs(string path)
        {
            Path = path;
            Save();
        }

        public void SaveAs(string path, ExportSettings settings)
        {
            Path = path;
            ExportSettings = settings;
            Save();
        }

        public void Move(string path)
        {
            if (Path != null)
            {
                File.Move(Path, path);
                Path = path;
            }
        }
    }

    public interface IHavePath
    {
        string Path { get; }
        bool IsFolder { get; }
        void Move(string path);
    }

    public interface ISaveable : IHavePath
    {
        event EventHandler OnSaved;
        bool HasUnsavedChanges { get; }
        bool CanSave { get; }
        void Save();
        void SaveAs(string path);
    }
}
