﻿using Aga.Controls.Tree;
using Aga.Controls.Tree.NodeControls;
using fNbt;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NbtStudio.UI
{
    public class NbtIcon : NodeControl
    {
        public IconSource IconSource;
        public override void Draw(TreeNodeAdv node, DrawContext context)
        {
            NbtText.DrawSelection(node, context);
            var image = GetIcon(node);
            if (image != null)
            {
                float ratio = Math.Min((float)context.Bounds.Width / (float)image.Width, (float)context.Bounds.Height / (float)image.Height);
                var rectangle = new Rectangle();
                rectangle.Width = (int)(image.Width * ratio);
                rectangle.Height = (int)(image.Height * ratio);
                rectangle.X = context.Bounds.X + (context.Bounds.Width - rectangle.Width) / 2;
                rectangle.Y = context.Bounds.Y + (context.Bounds.Height - rectangle.Height) / 2;
                if (context.Bounds.Width < image.Width || context.Bounds.Height < image.Height)
                    context.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                else
                    context.Graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
                context.Graphics.DrawImage(image, rectangle);
            }
        }

        public override Size MeasureSize(TreeNodeAdv node, DrawContext context)
        {
            var image = GetIcon(node);
            int height = node.Tree.RowHeight - 4;
            return image == null ? Size.Empty : new Size((int)(((float)height / image.Height) * image.Width), height);
        }

        private Image GetIcon(TreeNodeAdv node)
        {
            return GetImage(node.Tag as INode);
        }

        private Image GetImage(INode node)
        {
            if (IconSource == null)
                return null;
            if (node is NbtFileNode)
                return IconSource.GetImage(IconType.File).Image;
            if (node is FolderNode)
                return IconSource.GetImage(IconType.Folder).Image;
            if (node is RegionFileNode)
                return IconSource.GetImage(IconType.Region).Image;
            if (node is ChunkNode)
                return IconSource.GetImage(IconType.Chunk).Image;
            if (node is NbtTagNode tag)
                return NbtUtil.TagTypeImage(IconSource, tag.Tag.TagType).Image;
            return null;
        }
    }

    public class NbtText : NodeControl
    {
        public override void Draw(TreeNodeAdv node, DrawContext context)
        {
            DrawSelection(node, context);
            DrawOrMeasure(node, context, draw: true);
        }

        private SizeF DrawOrMeasure(TreeNodeAdv node, DrawContext context, bool draw)
        {
            var (name, value) = GetText(node);
            var boldfont = new Font(context.Font, FontStyle.Bold);
            context.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            context.Graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
            SizeF size = SizeF.Empty;
            var rectangle = context.Bounds;
            var format = TextFormatFlags.PreserveGraphicsClipping | TextFormatFlags.PreserveGraphicsTranslateTransform | TextFormatFlags.VerticalCenter;

            if (name != null)
            {
                if (draw)
                    TextRenderer.DrawText(context.Graphics, name, boldfont, rectangle, Parent.ForeColor, format);
                var name_size = TextRenderer.MeasureText(context.Graphics, name, boldfont, rectangle.Size);
                size = AppendSizes(size, name_size);
                rectangle.X += (int)name_size.Width;
            }
            if (value != null)
            {
                if (draw)
                    TextRenderer.DrawText(context.Graphics, value, context.Font, rectangle, Parent.ForeColor, format);
                var value_size = TextRenderer.MeasureText(context.Graphics, value, context.Font, rectangle.Size);
                size = AppendSizes(size, value_size);
            }
            return size;
        }

        private static SizeF AppendSizes(SizeF size1, SizeF size2)
        {
            return new SizeF(size1.Width + size2.Width, Math.Max(size1.Height, size2.Height));
        }

        public override Size MeasureSize(TreeNodeAdv node, DrawContext context)
        {
            var size = DrawOrMeasure(node, context, draw: false);
            return new Size((int)Math.Ceiling(size.Width), (int)Math.Ceiling(size.Height));
        }

        public static void DrawSelection(TreeNodeAdv node, DrawContext context)
        {
            // selected nodes are not "active" while dragging
            // hovered nodes are "active" while dragging
            if (context.DrawSelection == DrawSelectionMode.Active || (node.IsSelected && !node.Tree.Focused))
                context.Graphics.FillRectangle(new SolidBrush(Util.SelectionColor), context.Bounds);
            else if (node.IsSelected)
                context.Graphics.FillRectangle(Brushes.LightYellow, context.Bounds);
        }

        public override string GetToolTip(TreeNodeAdv node)
        {
            return PreviewTooltip(node.Tag as INode);
        }

        private (string name, string value) GetText(TreeNodeAdv node)
        {
            var (name, value) = PreviewNameAndValue(node);
            return (Flatten(name), Flatten(value));
        }

        private (string name, string value) PreviewNameAndValue(TreeNodeAdv node)
        {
            string prefix = null;
            string name = PreviewName(node);
            string value = PreviewValue(node);
            if (node.Tag is INode inode)
            {
                var saveable = inode.Get<ISaveable>();
                var chunk = inode.Get<Chunk>();
                if ((saveable != null && saveable.HasUnsavedChanges) || (chunk != null && chunk.HasUnsavedChanges))
                    prefix = "* ";
            }
            if (name == null)
                return (prefix, value);
            return (prefix + name + ":", value);
        }

        public static string PreviewName(TreeNodeAdv node) => PreviewName(node.Tag as INode);
        public static string PreviewValue(TreeNodeAdv node) => PreviewValue(node.Tag as INode);

        public static string PreviewName(INode node)
        {
            if (node is NbtFileNode file)
                return Path.GetFileName(file.File.Path);
            if (node is FolderNode folder)
                return Path.GetFileName(folder.Folder.Path);
            if (node is RegionFileNode region)
                return Path.GetFileName(region.Region.Path);
            if (node is ChunkNode chunk)
            {
                string text = $"Chunk [{chunk.Chunk.X}, {chunk.Chunk.Z}]";
                var coords = chunk.Chunk.Region.Coords;
                if (coords == null)
                    return text;
                var world = coords.WorldChunk(chunk.Chunk);
                return $"{text} in world at ({world.x}, {world.z})";
            }
            if (node is NbtTagNode tag)
                return tag.Tag.Name;
            return null;
        }

        public static string PreviewValue(INode node)
        {
            if (node is NbtFileNode file)
                return NbtUtil.PreviewNbtValue(file.File.RootTag);
            if (node is FolderNode folder_node)
            {
                var folder = folder_node.Folder;
                if (folder.HasScanned)
                {
                    if (folder.Subfolders.Any())
                        return $"[{Util.Pluralize(folder.Subfolders.Count, "folder")}, {Util.Pluralize(folder.Files.Count, "file")}]";
                    else
                        return $"[{Util.Pluralize(folder.Files.Count, "file")}]";
                }
                else
                    return "(open to load)";
            }
            if (node is RegionFileNode region)
                return $"[{Util.Pluralize(region.Region.ChunkCount, "chunk")}]";
            if (node is ChunkNode chunk_node)
            {
                var chunk = chunk_node.Chunk;
                if (chunk.IsLoaded)
                    return NbtUtil.PreviewNbtValue(chunk.Data);
                else if (chunk.IsExternal)
                    return "(saved externally)";
                else
                    return "(open to load)";
            }
            if (node is NbtTagNode tag)
                return NbtUtil.PreviewNbtValue(tag.Tag);
            return null;
        }

        private static string PreviewTooltip(INode node)
        {
            if (node is ChunkNode chunk)
            {
                var coords = chunk.Chunk.Region.Coords;
                if (coords == null)
                    return null;
                var blocks = coords.WorldBlocks(chunk.Chunk);
                return $"Contains blocks between ({blocks.x_min}, {blocks.z_min}) and ({blocks.x_max}, {blocks.z_max})";
            }
            return null;
        }

        private static string Flatten(string text)
        {
            if (text == null) return null;
            return text.Replace("\n", "⏎").Replace("\r", "⏎");
        }
    }
}
