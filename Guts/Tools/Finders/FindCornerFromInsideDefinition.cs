// Copyright (c) 2004-2008 Userwise Solutions LLC.  All rights reserved.

using System;
using System.ComponentModel;
using System.Drawing;
using System.Text;

namespace NetCams
{
    public enum Direction
    {
        NotDefined = 0,
        Up = 1,
        Down = 2,
        Left = 3,
        Right = 4
    }

    public class FindCornerFromInsideDefinition : NetCams.ToolDefinition
    {
        public FindCornerFromInsideDefinition(TestSequence testSequence)
            : base(testSequence)
		{
            mResultX = new GeneratedValueDefinition(testSequence, OwnerLink.newLink(this, "ResultX"));
            mResultX.Type = DataType.IntegerNumber;
            mResultX.AddDependency(this);
            mResultX.Name = "ResultX";

            mResultY = new GeneratedValueDefinition(testSequence, OwnerLink.newLink(this, "ResultY"));
            mResultY.Type = DataType.IntegerNumber;
            mResultY.AddDependency(this);
            mResultY.Name = "ResultY";

            /*
            mSearchEndX = new GeneratedValueDefinition(testSequence, OwnerLink.newLink(this, "SearchEndX"));
            mSearchEndX.Type = DataType.IntegerNumber;
            mSearchEndX.AddDependency(this);
            mSearchEndX.Name = "SearchEndX";

            mSearchEndY = new GeneratedValueDefinition(testSequence, OwnerLink.newLink(this, "SearchEndY"));
            mSearchEndY.Type = DataType.IntegerNumber;
            mSearchEndY.AddDependency(this);
            mSearchEndY.Name = "SearchEndY";

            mSearchPath = new ToolLineDecorationDefinition(testSequence, OwnerLink.newLink(this, "SearchPath"));
            mSearchPath.Name = "SearchPath";
            mSearchPath.SetStartX(mStartX);
            mSearchPath.SetStartY(mStartY);
            mSearchPath.SetEndX(mSearchEndX);
            mSearchPath.SetEndY(mSearchEndY);
             */
        }

		public override void CreateInstance(TestExecution theExecution)
		{
            new FindCornerFromInsideInstance(this, theExecution);
		}

		public override bool IsDependentOn(IObjectDefinition theOtherObject)
		{
            if (theOtherObject == this) return true;
            if (mSourceImage != null && mSourceImage.IsDependentOn(theOtherObject)) return true;
            if (mStartX != null && mStartX.IsDependentOn(theOtherObject)) return true;
            if (mStartY != null && mStartY.IsDependentOn(theOtherObject)) return true;
            if (mFollowingEdgeColorDefinition != null && mFollowingEdgeColorDefinition.IsDependentOn(theOtherObject)) return true;
            if (mTargetEdgeColorDefinition != null && mTargetEdgeColorDefinition.IsDependentOn(theOtherObject)) return true;
            if (mSearchBackgroundColorDefinition != null && mSearchBackgroundColorDefinition.IsDependentOn(theOtherObject)) return true;
            if (mTargetEdgeWidth != null && mTargetEdgeWidth.IsDependentOn(theOtherObject)) return true;
            return base.IsDependentOn(theOtherObject);
		}

		public override int ToolMapRow
		{
			get
			{
                int result = base.ToolMapRow - 1;
                if (mSourceImage != null) result = Math.Max(result, mSourceImage.ToolMapRow);
                if (mStartX != null) result = Math.Max(result, mStartX.ToolMapRow);
                if (mStartY != null) result = Math.Max(result, mStartY.ToolMapRow);
                if (mSearchBackgroundColorDefinition != null) result = Math.Max(result, mSearchBackgroundColorDefinition.ToolMapRow);
                if (mFollowingEdgeColorDefinition != null) result = Math.Max(result, mFollowingEdgeColorDefinition.ToolMapRow);
                if (mTargetEdgeColorDefinition != null) result = Math.Max(result, mTargetEdgeColorDefinition.ToolMapRow);
                if (mTargetEdgeWidth != null) result = Math.Max(result, mTargetEdgeWidth.ToolMapRow);
                return result + 1;
			}
		}

        private ColorMatchDefinition mFollowingEdgeColorDefinition;
        [CategoryAttribute("Parameters"),
        DescriptionAttribute("Definition of the color of the edge to search along")]
        public ColorMatchDefinition FollowingEdgeColorDefinition
        {
            get
            {
                return mFollowingEdgeColorDefinition;
            }
            set
            {
                HandlePropertyChange(this, "FollowingEdgeColorDefinition", mFollowingEdgeColorDefinition, value);
                mFollowingEdgeColorDefinition = value;
            }
        }

        private ColorMatchDefinition mTargetEdgeColorDefinition;
        [CategoryAttribute("Parameters"),
        DescriptionAttribute("Definition of the color of the perpendicular edge to search for")]
        public ColorMatchDefinition TargetEdgeColorDefinition
        {
            get
            {
                return mTargetEdgeColorDefinition;
            }
            set
            {
                HandlePropertyChange(this, "TargetEdgeColorDefinition", mTargetEdgeColorDefinition, value);
                mTargetEdgeColorDefinition = value;
            }
        }

        private ColorMatchDefinition mSearchBackgroundColorDefinition;
        [CategoryAttribute("Parameters"),
        DescriptionAttribute("Definition of the background color to search through for the edge")]
        public ColorMatchDefinition SearchBackgroundColorDefinition
        {
            get
            {
                return mSearchBackgroundColorDefinition;
            }
            set
            {
                HandlePropertyChange(this, "SearchBackgroundColorDefinition", mSearchBackgroundColorDefinition, value);
                mSearchBackgroundColorDefinition = value;
            }
        }

        private DataValueDefinition mStartX;
        [CategoryAttribute("Parameters"),
        DescriptionAttribute("Position on x-axis to start searching. This should be on the Following Edge.")]
        public DataValueDefinition StartX
        {
            get { return mStartX; }
            set
            {
                HandlePropertyChange(this, "StartX", mStartX, value);
                mStartX = value;
                //mSearchPath.SetStartX(value);
            }
        }

        private DataValueDefinition mStartY;
        [CategoryAttribute("Parameters"),
        DescriptionAttribute("Position on y-axis to start searching. This should be on the Following Edge.")]
        public DataValueDefinition StartY
        {
            get { return mStartY; }
            set
            {
                HandlePropertyChange(this, "StartY", mStartY, value);
                mStartY = value;
                //mSearchPath.SetStartY(value);
            }
        }

        private Direction mSearchDirection = Direction.NotDefined;
        [CategoryAttribute("Parameters"),
        DescriptionAttribute("The approximate direction to search along")]
        public Direction SearchDirection
        {
            get { return mSearchDirection; }
            set
            {
                if (((mTargetEdgeDirection == Direction.Up || mTargetEdgeDirection == Direction.Down) && (value == Direction.Up || value == Direction.Down))
                    ||
                    ((mTargetEdgeDirection == Direction.Left || mTargetEdgeDirection == Direction.Right) && (value == Direction.Left || value == Direction.Right))
                    )
                {
                    mTargetEdgeDirection = Direction.NotDefined;
                }
                HandlePropertyChange(this, "SearchDirection", mSearchDirection, value);
                mSearchDirection = value;
            }
        }

        private Direction mTargetEdgeDirection = Direction.NotDefined;
        [CategoryAttribute("Parameters"),
        DescriptionAttribute("The approximate direction of the perpendicular target edge.")]
        public Direction TargetEdgeDirection
        {
            get { return mTargetEdgeDirection; }
            set
            {
                if (((mSearchDirection == Direction.Up || mSearchDirection == Direction.Down) && (value == Direction.Up || value == Direction.Down))
                    ||
                    ((mSearchDirection == Direction.Left || mSearchDirection == Direction.Right) && (value == Direction.Left || value == Direction.Right))
                    )
                {
                    throw new ArgumentException("Your Target Edge Direction must be perpendicular to your Search Direction.");
                }
                HandlePropertyChange(this, "TargetEdgeDirection", mTargetEdgeDirection, value);
                mTargetEdgeDirection = value;
            }
        }

        private DataValueDefinition mTargetEdgeWidth;
        [CategoryAttribute("Parameters"),
        DescriptionAttribute("The height/width of the perpendicular edge to search for (in pixels).  It must be at least this high to be considered found.")]
        public DataValueDefinition TargetEdgeWidth
        {
            get { return mTargetEdgeWidth; }
            set
            {
                HandlePropertyChange(this, "TargetEdgeWidth", mTargetEdgeWidth, value);
                mTargetEdgeWidth = value; 
            }
        }

		private ImageDefinition mSourceImage = null;
		[CategoryAttribute("Input")]
		public ImageDefinition SourceImage
		{
			get { return mSourceImage; }
            set
            {
                HandlePropertyChange(this, "SourceImage", mSourceImage, value);
                mSourceImage = value;
            }
        }

        private GeneratedValueDefinition mResultX = null;
        [CategoryAttribute("Output")]
        public GeneratedValueDefinition ResultX
        {
            get { return mResultX; }
        }

        private GeneratedValueDefinition mResultY = null;
        [CategoryAttribute("Output")]
        public GeneratedValueDefinition ResultY
        {
            get { return mResultY; }
        }

        /*
        private ToolLineDecorationDefinition mSearchPath = null;
        [CategoryAttribute("Debug Options")]
        public ToolLineDecorationDefinition SearchPath // TODO: needed as property??? would be nice if it's properties could be shown in tree
        {
            get { return mSearchPath; }
        }

        [CategoryAttribute("Debug Options"),
        DescriptionAttribute("Shows the path that was searched.")]
        public Color SearchPathColor
        {
            get { return mSearchPath.Color; }
            set { mSearchPath.Color = value; }
        }
         */
    }
}
