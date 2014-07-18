using System;
using System.Collections.Generic;
using System.Text;

namespace NetCams
{
    public interface IEdgeGeneratorDefinition : IToolDefinition
    {
    }

    /// <summary>
    /// An EdgeCollection is intended to hold a group of edges that were created from the same source (FindEdges or EdgeCollectionFilter tools)
    /// </summary>
    public class EdgeCollectionDefinition : DataDefinition, IDecorationDefinition
    {
        public EdgeCollectionDefinition(TestSequence testSequence, OwnerLink theOwner)
            : base(testSequence)
        {
            SetOwnerLink(theOwner);
            testSequence.DecorationRegistry.RegisterObject(this);
        }

        public override void Dispose_UVision()
        {
            TestSequence().DecorationRegistry.UnRegisterObject(this);
            base.Dispose_UVision();
        }

        public override void CreateInstance(TestExecution theExecution)
        {
            throw new Exception("CreateInstance should not be called for EdgeCollections."); // because we need to set the creator...these always have an owner
        }

        public override bool IsDependentOn(IObjectDefinition theOtherObject)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override int ToolMapRow
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }


    }
}
