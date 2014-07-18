using System;
using System.Collections.Generic;
using System.Text;

namespace NetCams
{
    /* COMMENTED OUT "TEMPORARILLY" 4/16/09 SO I CAN COMPILE A NEW TOOL.  I QUIT WORKING ON THIS FINDEDGES STUFF A FEW WEEKS AGO WHEN I GOT STUCK AND RAN OUT OF TIME

    public interface IEdgeGeneratorInstance : IToolInstance
    {
    }

    public class EdgeCollectionInstance : DataInstance, IDecorationInstance
    {
        public EdgeCollectionInstance(EdgeCollectionDefinition theDefinition, TestExecution testExecution, IEdgeGeneratorInstance theGenerator)
            : base(theDefinition, testExecution)
        {
            testExecution.DecorationRegistry.RegisterObject(this);
        }

        public int Count
        {
            get { return mEdges.Count; }
        }

        public void Add(GeneratedEdgeInstance theNewEdge)
        {
            mEdges.Add(theNewEdge);
        }

        private List<GeneratedEdgeInstance> mEdges = new List<GeneratedEdgeInstance>();

        public override bool IsComplete()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #region IDecorationInstance Members

        public void Draw(System.Drawing.Graphics g, PictureBoxScale scale)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion
    }
     */
}
