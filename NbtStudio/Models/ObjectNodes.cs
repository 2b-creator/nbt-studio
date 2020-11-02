﻿using fNbt;
using NbtStudio;
using NbtStudio.SNBT;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NbtStudio
{
    public interface INode
    {
        string Description { get; }
        bool CanDelete { get; }
        void Delete();
        bool CanSort { get; }
        void Sort();
        bool CanCopy { get; }
        string Copy();
        bool CanPaste { get; }
        IEnumerable<INode> Paste(string data);
        bool CanRename { get; }
        bool CanEdit { get; }
    }

    public static class NodeExtractions
    {
        public static INbtTag GetNbtTag(this INode node)
        {
            if (node is NbtTagNode nbt)
                return nbt.Tag;
            if (node is NbtFileNode file)
                return file.File.RootTag;
            if (node is ChunkNode chunk)
                return chunk.Chunk.Data;
            return null;
        }

        public static ISaveable GetSaveable(this INode node)
        {
            if (node is NbtFileNode file)
                return file.File;
            if (node is RegionFileNode region)
                return region.Region;
            return null;
        }

        public static NbtFile GetNbtFile(this INode node)
        {
            if (node is NbtFileNode file)
                return file.File;
            return null;
        }

        public static RegionFile GetRegionFile(this INode node)
        {
            if (node is RegionFileNode region)
                return region.Region;
            return null;
        }

        public static Chunk GetChunk(this INode node)
        {
            if (node is ChunkNode chunk)
                return chunk.Chunk;
            return null;
        }

        public static NbtFolder GetNbtFolder(this INode node)
        {
            if (node is FolderNode folder)
                return folder.Folder;
            return null;
        }
    }

    public abstract class NotifyNode : INode
    {
        protected readonly NbtTreeModel Tree;
        protected readonly object SourceObject;
        protected NotifyNode(NbtTreeModel tree, object source)
        {
            Tree = tree;
            SourceObject = source;
        }

        protected virtual void Notify()
        {
            Tree.Notify(SourceObject);
        }
        protected virtual void Notify(object obj)
        {
            Tree.Notify(obj);
        }
        protected void PerformAction(UndoableAction action)
        {
            Tree.PerformAction(action);
        }

        public static NotifyNode Create(NbtTreeModel tree, object obj)
        {
            if (obj is NbtTag tag)
                return new NbtTagNode(tree, tag);
            if (obj is NbtFile file)
                return new NbtFileNode(tree, file);
            if (obj is RegionFile region)
                return new RegionFileNode(tree, region);
            if (obj is Chunk chunk)
                return new ChunkNode(tree, chunk);
            if (obj is NbtFolder folder)
                return new FolderNode(tree, folder);
            throw new ArgumentException($"Can't create node from {obj.GetType()}");
        }

        public virtual string Description => "unknown node";
        public virtual bool CanDelete => false;
        public virtual void Delete() { }
        public virtual bool CanSort => false;
        public virtual void Sort() { }
        public virtual bool CanCopy => false;
        public virtual string Copy() => null;
        public virtual bool CanPaste => false;
        public virtual IEnumerable<INode> Paste(string data) => Enumerable.Empty<INode>();
        public virtual bool CanRename => false;
        public virtual bool CanEdit => false;
    }

    public static class NbtNodeOperations
    {
        public static string Copy(INbtTag tag)
        {
            return tag.ToSnbt(include_name: true);
        }

        public static bool CanEdit(INbtTag tag)
        {
            return true;
        }

        public static bool CanPaste(INbtTag tag)
        {
            return tag is INbtContainer;
        }

        public static bool CanRename(INbtTag tag)
        {
            return !(tag.Parent is INbtList);
        }

        public static bool CanSort(INbtTag tag)
        {
            return tag is INbtCompound;
        }

        public static void Sort(INbtTag tag)
        {
            if (tag is INbtCompound compound)
                NbtUtil.Sort(compound, new NbtUtil.TagTypeSorter(), true);
        }

        public static IEnumerable<NbtTag> ParseTags(string text)
        {
            var snbts = text.Split('\n');
            foreach (var nbt in snbts)
            {
                if (SnbtParser.TryParse(nbt, true, out NbtTag tag) || SnbtParser.TryParse(nbt, false, out tag))
                    yield return tag;
            }
        }
    }

    public class NbtTagNode : NotifyNode
    {
        public readonly NotifyNbtTag Tag;
        public NbtTagNode(NbtTreeModel tree, NbtTag tag) : base(tree, tag)
        {
            Tag = NotifyNbtTag.CreateFrom(tag);
            Tag.Changed += Tag_Changed;
            Tag.ActionPrepared += Tag_ActionPrepared;
        }

        private void Tag_Changed(object sender, EventArgs e)
        {
            Notify((NotifyNbtTag)sender);
        }

        private void Tag_ActionPrepared(object sender, UndoableAction action)
        {
            PerformAction(action);
        }

        public override string Description => Tag.TagDescription();

        public override bool CanCopy => true;
        public override string Copy() => NbtNodeOperations.Copy(Tag);
        public override bool CanDelete => true;
        public override void Delete()
        {
            Tag.Parent?.Remove(Tag);
        }
        public override bool CanEdit => NbtNodeOperations.CanEdit(Tag);
        public override bool CanPaste => NbtNodeOperations.CanPaste(Tag);
        public override bool CanRename => NbtNodeOperations.CanRename(Tag);
        public override bool CanSort => NbtNodeOperations.CanSort(Tag);
        public override void Sort() => NbtNodeOperations.Sort(Tag);
    }

    public class NbtFileNode : NotifyNode
    {
        public readonly NbtFile File;
        public NbtFileNode(NbtTreeModel tree, NbtFile file) : base(tree, file)
        {
            File = file;
            File.RootTag.Changed += RootTag_Changed;
            File.RootTag.ActionPrepared += RootTag_ActionPrepared;
        }

        private void RootTag_Changed(object sender, EventArgs e)
        {
            Notify((NotifyNbtTag)sender);
        }

        private void RootTag_ActionPrepared(object sender, UndoableAction action)
        {
            PerformAction(action);
        }

        public override string Description => Path.GetFileName(File.Path);

        public override bool CanCopy => true;
        public override string Copy() => NbtNodeOperations.Copy(File.RootTag);
        public override bool CanDelete => true;
        public override void Delete()
        {
            if (File.Path != null)
                System.IO.File.Delete(File.Path);
        }
        public override bool CanEdit => false;
        public override bool CanPaste => NbtNodeOperations.CanPaste(File.RootTag);
        public override bool CanRename => false;
        public override bool CanSort => NbtNodeOperations.CanSort(File.RootTag);
        public override void Sort() => NbtNodeOperations.Sort(File.RootTag);
    }

    public class ChunkNode : NotifyNode
    {
        public readonly Chunk Chunk;
        public ChunkNode(NbtTreeModel tree, Chunk chunk) : base(tree, chunk)
        {
            Chunk = chunk;
            Chunk.Data.Changed += RootTag_Changed;
            Chunk.Data.ActionPrepared += RootTag_ActionPrepared;
        }

        public NotifyNbtCompound AccessChunkData()
        {
            if (!Chunk.IsLoaded)
                Chunk.Load();
            return Chunk.Data;
        }

        private void RootTag_Changed(object sender, EventArgs e)
        {
            Notify((NotifyNbtTag)sender);
        }

        private void RootTag_ActionPrepared(object sender, UndoableAction action)
        {
            PerformAction(action);
        }

        public override string Description => NbtUtil.ChunkDescription(Chunk);

        public override bool CanCopy => true;
        public override string Copy() => NbtNodeOperations.Copy(AccessChunkData());
        public override bool CanDelete => true;
        public override void Delete()
        {
            Chunk.Region.RemoveChunk(Chunk.X, Chunk.Z);
        }
        public override bool CanEdit => true;
        public override bool CanPaste => true;
        public override bool CanRename => true;
        public override bool CanSort => true;
        public override void Sort() => NbtNodeOperations.Sort(AccessChunkData());
    }

    public class RegionFileNode : NotifyNode
    {
        public readonly RegionFile Region;
        public RegionFileNode(NbtTreeModel tree, RegionFile region) : base(tree, region)
        {
            Region = region;
        }

        public override string Description => Path.GetFileName(Region.Path);

        public override bool CanCopy => false;
        public override bool CanDelete => true;
        public override void Delete()
        {
            if (Region.Path != null)
                File.Delete(Region.Path);
        }
        public override bool CanEdit => false;
        public override bool CanPaste => true;
        public override bool CanRename => false;
        public override bool CanSort => false;
    }

    public class FolderNode : NotifyNode
    {
        public readonly NbtFolder Folder;
        public FolderNode(NbtTreeModel tree, NbtFolder folder) : base(tree, folder)
        {
            Folder = folder;
        }

        public override string Description => Path.GetFileName(Folder.Path);

        public override bool CanCopy => true;
        public override bool CanDelete => true;
        public override void Delete()
        {
            if (Folder.Path != null)
                Directory.Delete(Folder.Path, true);
        }
        public override bool CanEdit => false;
        public override bool CanPaste => true;
        public override bool CanRename => false;
        public override bool CanSort => false;
    }
}
