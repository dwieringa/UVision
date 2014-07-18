// Copyright (c) 2004-2008 Userwise Solutions LLC.  All rights reserved.

using System;

namespace NetCams
{
	/// <summary>
	/// 
	/// </summary>
	public class CellImageToolArray : SourceGrid2.Cells.Virtual.CellVirtual
	{
		private ToolDefinition[,] m_Array;
		public CellImageToolArray(ToolDefinition[,] p_Array):base(typeof(string))
		{
			m_Array = p_Array;
		}
		public override object GetValue(SourceGrid2.Position p_Position)
		{
			if( m_Array[p_Position.Row, p_Position.Column] == null ) return "x";
			return m_Array[p_Position.Row, p_Position.Column].Name;
		}
		public override void SetValue(SourceGrid2.Position p_Position, object p_Value)
		{
			m_Array[p_Position.Row, p_Position.Column].Name = (string)p_Value;
			OnValueChanged(new SourceGrid2.PositionEventArgs(p_Position, this));
		}
		public void SetObjectPosition(int row, int col, ToolDefinition tool)
		{
			// TODO: throw error or adjust position if value isn't null
			m_Array[row, col] = tool;
		}
	}
}
