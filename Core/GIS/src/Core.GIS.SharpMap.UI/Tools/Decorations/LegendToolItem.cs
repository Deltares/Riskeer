using System;
using System.Collections.Generic;
using System.Drawing;

namespace Core.GIS.SharpMap.UI.Tools.Decorations
{
    public class LegendToolItem
    {
        public LegendToolItem() {}

        public LegendToolItem(Bitmap symbol, string text, bool centered, LegendToolItem parent)
        {
            Centered = centered;
            Symbol = symbol;
            Text = text;
            Parent = parent;
        }

        public LegendToolItem AddItem(string text)
        {
            var newToolItem = new LegendToolItem(null, text, false, this);
            items.Add(newToolItem);
            return newToolItem;
        }

        public LegendToolItem AddItem(string text, bool centeredParam)
        {
            var newToolItem = new LegendToolItem(null, text, centeredParam, this);
            items.Add(newToolItem);
            return newToolItem;
        }

        public LegendToolItem AddItem(Bitmap symbolBitmap, string text)
        {
            var newToolItem = new LegendToolItem(symbolBitmap, text, false, this);
            items.Add(newToolItem);
            return newToolItem;
        }

        public LegendToolItem InsertItem(LegendToolItem predecessor, string itemText)
        {
            var newToolItem = new LegendToolItem(null, itemText, false, this);
            int index = items.IndexOf(predecessor);
            if ((index + 1) < items.Count)
            {
                items.Insert(index + 1, newToolItem);
            }
            else
            {
                items.Add(newToolItem);
            }
            return newToolItem;
        }

        /// <summary>
        /// returns new Size with Height 0 and the maximum width of both passed in sizes
        /// </summary>
        /// <param name="size1"></param>
        /// <param name="size2"></param>
        /// <returns></returns>
        private static SizeF MaxWidth(SizeF size1, SizeF size2)
        {
            return new SizeF(Math.Max(size1.Width, size2.Width), 0);
        }

        /// <summary>
        /// returns new Size with Width 0 and the maximum height of both passed in sizes
        /// </summary>
        /// <param name="size1"></param>
        /// <param name="size2"></param>
        /// <returns></returns>
        private static SizeF MaxHeight(SizeF size1, SizeF size2)
        {
            return new SizeF(0, Math.Max(size1.Height, size2.Height));
        }

        /// <summary>
        /// returns new Size with Width 0 and the maximum height of both passed in sizes
        /// </summary>
        /// <param name="sizes"></param>
        /// <returns></returns>
        private static SizeF MaxHeight(params SizeF[] sizes)
        {
            var maxHeight = new SizeF(0, 0);

            foreach (var size in sizes)
            {
                maxHeight = MaxHeight(maxHeight, size);
            }

            return maxHeight;
        }

        #region fields

        private readonly IList<LegendToolItem> items = new List<LegendToolItem>();
        private Size padding;
        private Graphics graphics;
        private Font font;

        #endregion

        #region properties (getters & setters)

        public IList<LegendToolItem> Items
        {
            get
            {
                return items;
            }
        }

        public Bitmap Symbol { set; get; }

        internal string Text { get; set; }

        public bool Centered { get; private set; }

        public LegendToolItem Parent { get; set; }

        public Graphics Graphics
        {
            get
            {
                if (Parent == null)
                {
                    return graphics;
                }

                return Parent.Graphics;
            }
            set
            {
                graphics = value;
            }
        }

        public Font Font
        {
            get
            {
                if (Parent == null)
                {
                    return font;
                }

                return Parent.Font;
            }
            set
            {
                font = value;
            }
        }

        public Size Padding
        {
            get
            {
                if (Parent == null)
                {
                    return padding;
                }

                return Parent.Padding;
            }
            set
            {
                padding = value;
            }
        }

        /// <summary>
        /// Compute size of this LegendToolItem only (i.e. excluding any children)
        /// </summary>
        public SizeF InternalSize
        {
            get
            {
                SizeF internalSize = new SizeF(0, 0);
                List<SizeF> sizes = new List<SizeF>();
                if (Symbol != null)
                {
                    sizes.Add(Symbol.Size);
                }
                if (Text != null)
                {
                    sizes.Add(Graphics.MeasureString(Text, Font));
                }
                if (Text != null && Symbol != null)
                {
                    sizes.Add(Padding);
                }

                foreach (SizeF size in sizes)
                {
                    internalSize.Width += size.Width;
                }

                internalSize.Height = MaxHeight(sizes.ToArray()).Height;

                return internalSize;
            }
        }

        public SizeF Size
        {
            get
            {
                SizeF maxWidthSize = InternalSize;
                foreach (var item in items)
                {
                    maxWidthSize.Width = MaxWidth(maxWidthSize, item.Size).Width;
                }

                SizeF heightSize = InternalSize;
                heightSize.Height += Padding.Height;

                foreach (var item in items)
                {
                    heightSize.Height += item.Size.Height;
                }

                return new SizeF(maxWidthSize.Width, heightSize.Height);
            }
        }

        public int Depth
        {
            get
            {
                int maxDepth = -1;
                foreach (var child in Items)
                {
                    int childDepth = child.Depth;
                    if (childDepth > maxDepth)
                    {
                        maxDepth = childDepth;
                    }
                }
                return maxDepth + 1;
            }
        }

        public LegendToolItem Root
        {
            get
            {
                if (Parent != null)
                {
                    return Parent.Root;
                }

                return this;
            }
        }

        #endregion
    }
}