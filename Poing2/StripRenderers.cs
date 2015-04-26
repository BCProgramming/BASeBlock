using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

namespace BASeBlock
{

        public class ButtonStripRenderer : ToolStripRenderer
        {
            protected override void OnRenderItemImage(ToolStripItemImageRenderEventArgs e)
            {
                Rectangle destRect = e.ImageRectangle;
                Image normalImage = e.Image;
                if ((destRect != Rectangle.Empty) && (normalImage != null))
                {
                    bool shouldDisposeImage = false;
                    if (!e.Item.Enabled)
                    {
                        normalImage = CreateDisabledImage(normalImage);
                        shouldDisposeImage = true;
                    }
                    if (e.Item.ImageScaling == ToolStripItemImageScaling.None)
                    {
                        e.Graphics.DrawImage(normalImage, destRect, new Rectangle(Point.Empty, destRect.Size), GraphicsUnit.Pixel);
                    }
                    else
                    {
                        e.Graphics.DrawImage(normalImage, destRect);
                    }
                    if (shouldDisposeImage)
                    {
                        normalImage.Dispose();
                    }
                }
            }

            protected override void OnRenderButtonBackground(ToolStripItemRenderEventArgs e)
            {
                ToolBarState buttonState = GetToolBarState(e.Item);
                Rectangle buttonBounds = new Rectangle(Point.Empty, e.Item.Size);

                PushButtonState pbs = ConvertToolBarStateToPushButtonState(buttonState);
                ButtonRenderer.DrawButton(e.Graphics, buttonBounds, pbs);
            }

            private static PushButtonState ConvertToolBarStateToPushButtonState(ToolBarState buttonState)
            {
                PushButtonState pbs = PushButtonState.Normal;

                switch (buttonState)
                {
                    case ToolBarState.Normal:
                        pbs = PushButtonState.Normal;
                        break;
                    case ToolBarState.Hot:
                        pbs = PushButtonState.Hot;
                        break;
                    case ToolBarState.Pressed:
                        pbs = PushButtonState.Pressed;
                        break;
                    case ToolBarState.Disabled:
                        pbs = PushButtonState.Disabled;
                        break;

                    case ToolBarState.Checked:
                    case ToolBarState.HotChecked:
                        break;
                }

                return pbs;
            }

            protected override void InitializeItem(ToolStripItem item)
            {
                base.InitializeItem(item);
                item.Height = Math.Max(23, item.Height);
            }

            private static ToolBarState GetToolBarState(ToolStripItem item)
            {
                ToolBarState normal = ToolBarState.Normal;
                if (item != null)
                {
                    if (!item.Enabled)
                    {
                        normal = ToolBarState.Disabled;
                    }
                    if ((item is ToolStripButton) && ((ToolStripButton)item).Checked)
                    {
                        return ToolBarState.Checked;
                    }
                    if (item.Pressed)
                    {
                        return ToolBarState.Pressed;
                    }
                    if (item.Selected)
                    {
                        normal = ToolBarState.Hot;
                    }
                }
                return normal;
            }
        
    }
}
