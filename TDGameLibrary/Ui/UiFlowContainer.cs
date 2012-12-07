using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace TDGameLibrary.Ui
{
    public enum FlowDirection
    {
        LeftToRight_TopToBottom,
        TopToBottom_LeftToRight
    }

    public enum CellAlignmentVertical
    {
        Bottom,
        Middle,
        Top
    }

    public enum CellAlignmentHorizontal
    {
        Left,
        Center,
        Right
    }


    /**
     * UiFlowContainer will automatically position its controls in left to right or top to bottom positions
     * A flag can determine if the flow container is left to right or top to bottom naturally
     * Overflow pushes the control to the next row or column
     * If controls exceed the container limit in overflow, they will be TODO?
     */
    public class UiFlowContainer : UiContainer
    {
        public UiFlowContainer(Rectangle displayRectangle, UiControlManager controlManager)
            : base(displayRectangle, controlManager)
        {
            FlowDirection = FlowDirection.TopToBottom_LeftToRight;
            
            CellAlignmentVertical = CellAlignmentVertical.Top;
            
            CellAlignmentHorizontal = CellAlignmentHorizontal.Left;

            Margin = Vector2.Zero;
            
            RelativePositionVector = Vector2.Zero;

            RelativePositionRectangle = new Rectangle(0, 0, 0, 0);
        }


        public FlowDirection FlowDirection;
        public CellAlignmentVertical CellAlignmentVertical;
        public CellAlignmentHorizontal CellAlignmentHorizontal;
        public Vector2 Margin;
        protected Vector2 RelativePositionVector;
        protected Rectangle RelativePositionRectangle;


        public override void AddChildControl(UiControl control)
        {
            Vector2 relativePosition = ResolvePosition(control);

            if (!UiControlPositions.ContainsKey(control))
            {
                UiControlPositions.Add(control, relativePosition);
            }

            if (!UiControls.Contains(control))
            {
                UiControls.Add(control);
            }

            ControlManager.Add(control);
        }


        protected Vector2 ResolvePosition(UiControl control)
        {
            // The relativePositionPointer is the upper-left coordinate of the last control


            switch(FlowDirection)
            {
                case FlowDirection.LeftToRight_TopToBottom :
                
                    bool isNewRow = false;

                    // If we have not applied a margin to our Y
                    if (RelativePositionRectangle.Y == 0)
                    {
                        RelativePositionRectangle.Y += (int)Margin.Y;
                    }

                    // move X to the right
                    RelativePositionRectangle.X += RelativePositionRectangle.Width;
                    RelativePositionRectangle.X += (int)Margin.X;

                    // if we will exceed our horizontal max
                    if ((RelativePositionRectangle.X + RelativePositionRectangle.Width) > DisplayRectangle.Width)
                    {
                        isNewRow = true;

                        // reset X
                        RelativePositionRectangle.X = DisplayRectangle.X;
                        RelativePositionRectangle.X += (int)Margin.X;

                        // move Y down
                        RelativePositionRectangle.Y += RelativePositionRectangle.Height;
                        RelativePositionRectangle.Y += (int)Margin.Y;

                        // if we will exceed our vertical max
                        if ((RelativePositionRectangle.Y + RelativePositionRectangle.Height) > DisplayRectangle.Height)
                        {
                            // reposition at zero
                            RelativePositionRectangle.X = DisplayRectangle.X;
                            RelativePositionRectangle.Y = DisplayRectangle.Y;
                        }
                    }

                    // relativePositionRectangle.X and .Y are at the correct coordinate now


                    // relativePositionRectangle width always flows to last control's width
                    RelativePositionRectangle.Width = control.DisplayRectangle.Width;

                    // but their height may need to be adjusted
                    if (isNewRow)
                    {
                        // start new row height at current control height
                        RelativePositionRectangle.Height = control.DisplayRectangle.Height;
                    }
                    else
                    {
                        // perhaps the row height should grow
                        RelativePositionRectangle.Height = Math.Max(control.DisplayRectangle.Height, RelativePositionRectangle.Height);
                    }

                    break;



                case FlowDirection.TopToBottom_LeftToRight :

                    bool isNewColumn = false;

                    // If we have not applied a margin to our X
                    if (RelativePositionRectangle.X == 0)
                    {
                        RelativePositionRectangle.X += (int)Margin.X;
                    }

                    // move Y down
                    RelativePositionRectangle.Y += RelativePositionRectangle.Height;
                    RelativePositionRectangle.Y += (int)Margin.Y;

                    // if we will exceed our vertical max
                    if ((RelativePositionRectangle.Y + RelativePositionRectangle.Height) > DisplayRectangle.Height)
                    {
                        isNewColumn = true;

                        // reset Y
                        RelativePositionRectangle.Y = DisplayRectangle.Y;
                        RelativePositionRectangle.Y += (int)Margin.Y;

                        // move X right
                        RelativePositionRectangle.X += RelativePositionRectangle.Width;
                        RelativePositionRectangle.X += (int)Margin.X;

                        // if we will exceed our vertical max
                        if ((RelativePositionRectangle.X + RelativePositionRectangle.Width) > DisplayRectangle.Width)
                        {
                            // reposition at zero
                            RelativePositionRectangle.X = DisplayRectangle.X;
                            RelativePositionRectangle.Y = DisplayRectangle.Y;
                        }
                    }

                    // relativePositionRectangle.X and .Y are at the correct coordinate now


                    // relativePositionRectangle height always flows to last control's width
                    RelativePositionRectangle.Height = control.DisplayRectangle.Height;

                    // but their width may need to be adjusted
                    if (isNewColumn)
                    {
                        // start new row height at current control height
                        RelativePositionRectangle.Width = control.DisplayRectangle.Width;
                    }
                    else
                    {
                        // perhaps the row height should grow
                        RelativePositionRectangle.Width = Math.Max(control.DisplayRectangle.Width, RelativePositionRectangle.Width);
                    }

                    break;
            }


            // update our relativePositionPointer and return it
            RelativePositionVector.X = RelativePositionRectangle.X;
            RelativePositionVector.Y = RelativePositionRectangle.Y;

            switch (CellAlignmentHorizontal)
            {
                case CellAlignmentHorizontal.Left:

                    RelativePositionVector.X = RelativePositionRectangle.X;

                    break;

                case CellAlignmentHorizontal.Center:

                    RelativePositionVector.X += (RelativePositionRectangle.Width - control.DisplayRectangle.Width) / 2;

                    break;

                case CellAlignmentHorizontal.Right:

                    RelativePositionVector.X += RelativePositionRectangle.Width - control.DisplayRectangle.Width;

                    break;
            }

            switch (CellAlignmentVertical)
            {
                case CellAlignmentVertical.Top:

                    RelativePositionVector.Y = RelativePositionRectangle.Y;

                    break;

                case CellAlignmentVertical.Middle:

                    RelativePositionVector.Y += (RelativePositionRectangle.Height - control.DisplayRectangle.Height) / 2;

                    break;

                case CellAlignmentVertical.Bottom:

                    RelativePositionVector.Y += RelativePositionRectangle.Height - control.DisplayRectangle.Height;

                    break;
            }

            return RelativePositionVector;
        }
    }
}