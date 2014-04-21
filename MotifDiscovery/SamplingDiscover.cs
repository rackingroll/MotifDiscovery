using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MotifDiscovery
{
    /// <summary>
    /// This Algorithm is an implementation of the following paper:
    /// Catalano, Joe, Tom Armstrong, and Tim Oates. "Discovering patterns in real-valued time series". Springer Berlin Heidelberg, 2006.
    /// PS: May be it is not the best method of motif discover.
    /// </summary>
    class SamplingDiscover
    {
        /// <summary>
        /// In every interation, we use redction the candidate set, using the ratio of SubDuctionParameter
        /// </summary>
        private double SubDuctionParameter = 0.9;

        /// <summary>
        /// Construction Method
        /// </summary>
        public SamplingDiscover() {}

        /// <summary>
        /// The Main Method of Finding Motif in a Time Series
        /// Return the Motif of the Time Series
        /// </summary>
        /// <param name="TimeSeries">Resource Time Series</param>
        /// <param name="WMax">The Max length of the Motif</param>
        /// <param name="WMin">The Min length of the Motif</param>
        /// <param name="n"> The iteration of sampling the Candidate set and Conparison set, it will effect the dimension of these two set</param>
        /// <param name="Alpha">Used for calculate the thershold of the rejecting the candidate motif</param>
        /// <param name="iterations"> The number of iteration for the main algorithm</param>
        /// <returns></returns>
        public List<double> Sampling (List <double> TimeSeries, int WMax, int WMin, int n, double Alpha, int iterations)
        {
            // Initialization the Three Sub Set
            List<SubTimeSeries> CandidateSet = SamplingSubSet(TimeSeries, WMax, WMin, n);
            List<SubTimeSeries> ComparisonSet = SamplingSubSet(TimeSeries, WMax, WMin, n);
            List<SubTimeSeries> NoiseSet = SamplingNoiseSet(TimeSeries, WMax, WMin);

            // Iterations to Extract Candidate Set
            for (int i = 0; i < iterations; i++)
            {
                int k = (int) SubDuctionParameter * CandidateSet.Count ;
                CandidateSet = CompareSubWindowPair(CandidateSet, ComparisonSet, k);

                // If the CandateSet becomes 0. That means the motif dose not discovered.
                if (CandidateSet.Count == 0) return null;

                ComparisonSet = SamplingSubSet(TimeSeries, WMax, WMin, n);
                RemoveReject(Alpha, CandidateSet, NoiseSet);
            }

            // return the first one, better result must combine all the sub series in the candidate set, here is a easier representation
            return CandidateSet[0].TimeSeries;
        }

        /// <summary>
        /// Sampling sub set in a time series
        /// </summary>
        /// <param name="TimeSeries"></param>
        /// <param name="WMax"></param>
        /// <param name="WMin"></param>
        /// <param name="n"></param>
        /// <returns></returns>
        private List<SubTimeSeries> SamplingSubSet(List<double> TimeSeries, int WMax, int WMin, int n)
        {
            List<SubTimeSeries> SubSet = new List<SubTimeSeries>();
            
            for (int i = 0; i < n; i++)
            {
                SubTimeSeries SubSequence = SamplingSubSequence(TimeSeries, WMax);
                List<SubTimeSeries> SlidingWindowSet = SlidingWindowSampling(SubSequence, WMin);
                foreach (SubTimeSeries SlidingSeries in SlidingWindowSet)
                {
                    // add the group information to the subsequence.
                    SlidingSeries.group = i;
                    SubSet.Add(SlidingSeries);
                }
            }
            return SubSet;
        }

        /// <summary>
        /// Samping the Noise Set
        /// </summary>
        /// <param name="TimeSeries"></param>
        /// <param name="WMax"></param>
        /// <param name="WMin"></param>
        /// <returns></returns>
        private List<SubTimeSeries> SamplingNoiseSet(List<double> TimeSeries, int WMax, int WMin)
        {
            List<SubTimeSeries> SubSet = new List<SubTimeSeries>();
            Random rand = new Random();

            SubTimeSeries SubSequence = new SubTimeSeries();
            for (int i = 0; i < WMax; i++)
            {
                int index = rand.Next(TimeSeries.Count);
                SubSequence.TimeSeries.Add(TimeSeries[index]);
            }

            List<SubTimeSeries> SlidingWindowSet = SlidingWindowSampling(SubSequence, WMin);
            foreach (SubTimeSeries SlidingSeries in SlidingWindowSet)
            {
                SubSet.Add(SlidingSeries);
            }
            return SubSet;
        }

        /// <summary>
        /// Sample one sub sequence give a length w
        /// </summary>
        /// <param name="TimeSeries"></param>
        /// <param name="w"></param>
        /// <returns></returns>
        private SubTimeSeries SamplingSubSequence(List<double> TimeSeries, int w)
        {
            Random rand = new Random();
            int index = rand.Next(TimeSeries.Count);
            while (index + w - 1 >= 1000)
                index = rand.Next(TimeSeries.Count);
            SubTimeSeries SubSequence = new SubTimeSeries();

            for (int i = index; i < index + w - 1; i++)
            {
                SubSequence.TimeSeries.Add(TimeSeries[i]);
            }
            SubSequence.index = index;

            return SubSequence;
        }

        /// <summary>
        /// Obtain all the sliding window sub seqences given a time series and a length w
        /// </summary>
        /// <param name="TimeSeries"></param>
        /// <param name="w"></param>
        /// <returns></returns>
        private List<SubTimeSeries> SlidingWindowSampling(SubTimeSeries TimeSeries, int w)
        {
            List<SubTimeSeries> SlidingTimeSeriesSet = new List<SubTimeSeries>();
            for (int i = 0; i < TimeSeries.TimeSeries.Count; i++)
            {
                if (i + w < TimeSeries.TimeSeries.Count)
                {
                    SubTimeSeries subSequence = new SubTimeSeries();
                    for (int j = 0; j < w; j++)
                    {
                        subSequence.TimeSeries.Add(TimeSeries.TimeSeries[i + j]);
                    }
                    subSequence.index = i + TimeSeries.index;
                    SlidingTimeSeriesSet.Add(subSequence);
                }
            }
            return SlidingTimeSeriesSet;
        }

        /// <summary>
        /// Remove some candidate time series in candidate set using the noise set
        /// </summary>
        /// <param name="Alpha"></param>
        /// <param name="CandidateSet"></param>
        /// <param name="NoiseSet"></param>
        private void RemoveReject(double Alpha, List<SubTimeSeries> CandidateSet, List<SubTimeSeries> NoiseSet)
        {
            int Gama = (int)(Alpha * NoiseSet.Count);
            List<SubTimeSeries> CandidateNoiseSet = CompareSubWindowPair(NoiseSet, CandidateSet, NoiseSet.Count);
            double Dist = 0.0;
            for (int i = 0; i < Gama; i++)
            {
                Dist += CandidateNoiseSet[i].dist;
            }
            double Thershold = Dist / Gama;

            // Remove the rubish candidate set that 
            for (int i = 0; i < CandidateSet.Count; i++)
            {
                if (CandidateSet[i].dist > Thershold)
                    CandidateSet.Remove(CandidateSet[i]);
            }
        }

        /// <summary>
        /// Compair sub window pairs and return the candidate set after filtering
        /// </summary>
        /// <param name="SubSetCandidate"></param>
        /// <param name="SubSetComparison"></param>
        /// <param name="k"></param>
        /// <returns></returns>
        private List<SubTimeSeries> CompareSubWindowPair(List<SubTimeSeries> SubSetCandidate, List<SubTimeSeries> SubSetComparison, int k)
        {
            // Top K candidate Sub Sequence
            List<SubTimeSeries> CandidateSet = new List<SubTimeSeries>(k);
            for (int i = 0; i < SubSetCandidate.Count; i++)
            {
                double distance = 0.0;
                for (int j = 0; j < SubSetComparison.Count; j++)
                {
                    distance += DTWDistance(SubSetCandidate[i].TimeSeries, SubSetComparison[j].TimeSeries);
                }
                SubSetCandidate[i].dist = distance/SubSetComparison.Count ;
                InsertInto(ref CandidateSet, SubSetCandidate[i]);
            }
            return CandidateSet;
        }

        /// <summary>
        /// Dynamic Time Warping Distance
        /// </summary>
        /// <param name="seriesOne"></param>
        /// <param name="serieTwo"></param>
        /// <returns></returns>
        private double DTWDistance(List<double> seriesOne, List<double> serieTwo)
        {
            int length = seriesOne.Count;
            double[,] D = new double[length, length];
            for (int i = 0; i < length; i++)
            {
                D[i, 0] = (seriesOne[i] - serieTwo[0]) * (seriesOne[i] - serieTwo[0]);
                D[0, i] = (seriesOne[0] - serieTwo[i]) * (seriesOne[0] - serieTwo[i]);
            }
            for (int i = 1; i < length; i++)
            {
                for (int j = 1; j < length; j++)
                {
                    D[i, j] = Math.Min(Math.Min(D[i - 1, j - 1], D[i, j - 1]), D[i - 1, j]) + (seriesOne[i] - serieTwo[j]) * (seriesOne[i] - serieTwo[j]);
                }

            }

            return Math.Sqrt(D[length - 1, length - 1]);
        }

        /// <summary>
        /// A tool method used for insert sorting.
        /// </summary>
        /// <param name="CandidateSet"></param>
        /// <param name="CandidateSequence"></param>
        private void InsertInto (ref List<SubTimeSeries> CandidateSet, SubTimeSeries CandidateSequence)
        {
            for (int i =0; i < CandidateSet.Count; i++)
            {
                if (CandidateSequence.dist < CandidateSet[i].dist)
                {
                    for (int j = CandidateSet.Count - 1; j > i; j--)
                    {
                        CandidateSet[j] = CandidateSet[j - 1];
                    }
                    CandidateSet[i] = CandidateSequence;
                }
            }
        }
    }

    /// <summary>
    /// Sub Time Series Class used for restore the Information of a sue time series
    /// </summary>
    class SubTimeSeries
    {
        // The resorce of the time series
        public List<double> TimeSeries = new List<double>();

        // The rank of the time series in the corresponding set: candidate set or Noise set or Comparison Set
        public int rank = 0;

        // The index of the sub sequence
        public int index = 0;

        // Denoting with group it belongs to
        public int group = 0;

        // The average warping distance of the time series
        public double dist = double.MaxValue;
    }




}
