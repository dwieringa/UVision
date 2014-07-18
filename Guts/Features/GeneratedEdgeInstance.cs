using System;
using System.Collections.Generic;
using System.Text;

namespace NetCams
{
    /* COMMENTED OUT "TEMPORARILLY" 4/16/09 SO I CAN COMPILE A NEW TOOL.  I QUIT WORKING ON THIS FINDEDGES STUFF A FEW WEEKS AGO WHEN I GOT STUCK AND RAN OUT OF TIME
    public class GeneratedEdgeInstance
    {
        public GeneratedEdgeInstance(IEdgeGeneratorDefinition theGenerator, int theEdgeNumber)
            : base(theGenerator.LineGroupDefinition, theGenerator.TestExecution())
        {
            EdgeNumber = theEdgeNumber;
            mTool = theTool;
        }

        private ValueBasedLineDecorationInstance mLineDecoration;
        private GeneratedValueInstance mEdgeLocation;

        private FindEdgesInstance mTool;
        public int EdgeNumber = -1;
        public Surface leadingSurface;
        public Transition transition;
        public Surface trailingSurface;
        public int FindEdgeBasedOnVariation(RowAnalysis[] rows)
        {
            if (leadingSurface == null || trailingSurface == null || transition == null)
            {
                throw new ArgumentException("asldji231dilasj");
            }
            int rowWithBiggestChange = -1;
            double biggestChange = 0;
            for (int rowNdx = transition.StartPos - 1; rowNdx <= transition.EndPos + 1; rowNdx++)
            {
                double absVariation = Math.Abs(rows[rowNdx].VariationFromPrevRow_raw);
                if (absVariation > biggestChange)
                {
                    biggestChange = absVariation * 1.2; // "*1.4" = we want the first big one...following ones need to beat the first by 40%
                    rowWithBiggestChange = rowNdx;
                }
            }
            return rowWithBiggestChange - 1;
        }
        public int FindEdgeBasedOnIntensity(RowAnalysis[] rows)
        {
            if (leadingSurface == null || trailingSurface == null || transition == null)
            {
                throw new ArgumentException("asldjiajdilasj");
            }
            for (int rowNdx = transition.StartPos - 1; rowNdx <= transition.EndPos + 1; rowNdx++)
            {
                double rowIntensity = rows[rowNdx].AverageValue;
                if (Math.Abs(trailingSurface.AverageIntensity - rowIntensity) < Math.Abs(leadingSurface.AverageIntensity - rowIntensity))
                {
                    // if this row is closer in intensity to the trailing surface (the one we're searching TOWARD) than the leading surface (search FROM), consider the PREVIOUS row the edge of surface 1
                    return rowNdx - 1;
                }
            }
            return transition.EndPos;
        }
        public double FindSubPixelEdgeBasedOnIntensity(RowAnalysis[] rows, double targetSurfaceTransitionPercentage, int searchDirOffset, int searchDirIncrement)
        {
            if (leadingSurface == null || trailingSurface == null || transition == null)
            {
                throw new ArgumentException("asldjiajdilasj");
            }
            double targetIntensity = leadingSurface.AverageIntensity + ((trailingSurface.AverageIntensity - leadingSurface.AverageIntensity) * (targetSurfaceTransitionPercentage / 100.0));
            double rowIntensity;
            double lastRowIntensity = rows[transition.StartPos - 1].AverageValue;
            for (int rowNdx = transition.StartPos; rowNdx <= transition.EndPos + 1; rowNdx++)
            {
                rowIntensity = rows[rowNdx].AverageValue;
                if (( lastRowIntensity <= targetIntensity && targetIntensity <= rowIntensity) ||
                    ( lastRowIntensity >= targetIntensity && targetIntensity >= rowIntensity)
                    )
                {
                    double subPixelComponent = Math.Abs(targetIntensity - lastRowIntensity) / Math.Abs(rowIntensity - lastRowIntensity);
                    double edgeLocation = searchDirOffset + ((rowNdx - 1) * searchDirIncrement) + (subPixelComponent * searchDirIncrement);
                    return edgeLocation;
                }
                lastRowIntensity = rowIntensity;
            }
            return searchDirOffset + (transition.EndPos * searchDirIncrement);
        }

    }

    public class Section
    {
        public Section()
        {
        }

        public int Size() { return EndPos - StartPos + 1; }
        public void ComputeValues(RowAnalysis[] rows)
        {
            double intensity = 0;
            double absVariation = 0;
            double variation = 0;
            double sumIntensity = 0;
            double sumVariationScores = 0;
            int numValues = 0;
            for (int rowNdx = StartPos; rowNdx <= EndPos; rowNdx++)
            {
                intensity = rows[rowNdx].AverageValue;
                variation = rows[rowNdx].VariationFromPrevRow_raw;
                absVariation = Math.Abs(variation);
                if (intensity < MinIntensity) MinIntensity = intensity;
                if (intensity > MaxIntensity) MaxIntensity = intensity;
                if (absVariation < MinVariation) MinVariation = absVariation;
                if (absVariation > MaxVariation) MaxVariation = absVariation;
                sumIntensity += intensity;
                SumVariation += variation;
                SumAbsVariation += absVariation;
                sumVariationScores += rows[rowNdx].VariationScore;
                numValues++;
            }
            AverageIntensity = sumIntensity / numValues;
            AverageVariation = SumVariation / numValues;
            AverageAbsVariation = SumAbsVariation / numValues;
            AverageVariationScore = sumVariationScores / numValues;
        }

        public int StartPos = -1;
        public int EndPos = -1;
        public double AverageIntensity = -1;
        public double AverageAbsVariation = -1;
        public double AverageVariation = -1;
        public double MinVariation = 999999;
        public double MaxVariation = -1;
        public double MinIntensity = 999999;
        public double MaxIntensity = -1;
        public double SumAbsVariation = -1;
        public double SumVariation = -1;
        public double AverageVariationScore = -1;
    }

    public class Surface : Section
    {
        public Surface(int theSurfaceNumber)
        {
            SurfaceNumber = theSurfaceNumber;
        }

        public int SurfaceNumber = -1;
    }

    public class Transition : Section
    {
        public Transition(int theSurfaceNumber)
        {
            TransitionNumber = theSurfaceNumber;
        }

        public int TransitionNumber = -1;
    }

    public class RowAnalysis
    {
        public RowAnalysis(int theSearchIndex)
        {
            SearchIndex = theSearchIndex;
        }

        // indicates the row this data belows to
        public int SearchIndex = -1;

        // stats for pixels within this particular row
        //public double AverageVariation = 0;
        public double AverageValue = -1; // renamed to AverageValue from AverageIntensity 10/22/08 since we're actually (currently) using gray value
        public double StdDevOfVariation = -1;
        public double VariationScore = -1;

        public double VariationFromPrevRow_raw; // this row's avgVal - previous row's avgVal
        public double VariationFromPrevRow_used = 0; // the variation value used within algorithms...this may be identical to "_raw" or it may be filtered (e.g. 3RowAverage)

        // Before 10/22/08, I used a 3Row Average of Variation for the FindEdge algorithms, but this didn't work well for SHORT surfaces (eg small 3 row black gaps on sides of DS's UGDO buttons),
        // so I decided to make the 3Row Avearage optional.  To acomplish this, I changed AverageVariation_3Row to VariationFromPrevRow_used and depending on settings I either write the raw values in or compute a 3Row average
        // filters out noise and highlights edges by sticking out "merging" variations on neighboring rows

        // 3Row Average works well when you have fuzzy/slow transitions which occur over multiple rows, since the slow variations get "accumulated".
        // 3Row Average DOESN'T work well if SMALL surfaces are present with sharp transitions, since the transitions "blur" into the small surface and create noise...and prevent the variation from settling out long enough for the surface to be detected.

        // _used is used for detecting edges/transitions.  This may hold a copy of the raw values or filtered values such as the 3Row Average.
        // Regardless of filtering chosen, _raw is used within surface/edge analysis once surfaces are chosen.

        // I originally tested a 2Row Average, but I didn't like the 2row average since it skewed the transition a pixel
    }
     */
}
