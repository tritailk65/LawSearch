using LawSearch_Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LawSearch_Core.Common
{
    public class TF_IDF
    {
        public static List<KeyPhraseResult> FindNearestNeighbors(List<KeyPhraseResult> lstResult, List<KeyPhrase> lstKeyPhrases_Searched, double minDis, int itop)
        {
            lstResult.Add(new KeyPhraseResult { ID = -1, keys = lstKeyPhrases_Searched });

            SortedDictionary<string, int> vocabulary = new SortedDictionary<string, int>();
            for (int i = 0; i < lstResult.Count; i++)
            {
                for (int j = 0; j < lstResult[i].keys.Count; j++)
                {
                    string sKey = lstResult[i].keys[j].Key;
                    if (!vocabulary.ContainsKey(sKey))
                        vocabulary.Add(lstResult[i].keys[j].Key, 1);
                    else
                        vocabulary[sKey]++;
                }
            }
            Dictionary<string, double> _vocabularyIDF = new Dictionary<string, double>();
            foreach (var term in vocabulary.Keys)
            {
                double numberOfDocsContainingTerm = lstResult.Count(d => d.keys.Exists(x => string.Compare(x.Key, term, StringComparison.InvariantCultureIgnoreCase) == 0));
                _vocabularyIDF[term] = Math.Log((double)lstResult.Count / ((double)1 + numberOfDocsContainingTerm));
            }
            // Transform each document into a vector of tfidf values.
            List<List<double>> vectors = new List<List<double>>();
            foreach (var myKey in lstResult)
            {
                List<double> vector = new List<double>();

                foreach (var vocab in _vocabularyIDF)
                {
                    // Term frequency = count how many times the term appears in this document.

                    KeyPhrase key = myKey.keys.Where(d => string.Compare(d.Key, vocab.Key, StringComparison.InvariantCultureIgnoreCase) == 0).FirstOrDefault();
                    double tf = key == null ? 0 : key.Count;
                    double tfidf = tf * vocab.Value;

                    vector.Add(tfidf);
                }

                vectors.Add(vector);
            }
            double[][] inputs = vectors.Select(v => v.ToArray()).ToArray();
            inputs = Normalize(inputs);

            for (int i = 0; i < lstResult.Count; i++)
            {
                lstResult[i].vector = inputs[i];
            }

            double[] search_vector = lstResult[lstResult.Count - 1].vector;

            for (int i = 0; i < lstResult.Count - 1; i++)
            {
                lstResult[i].distance = ComputeCosineSimilarity(search_vector, lstResult[i].vector);
            }
            List<KeyPhraseResult> lstReturn = lstResult.OrderByDescending(x => x.distance).Where(x => x.distance > minDis).Take(itop).ToList();
            return lstReturn;
        }

        public static double ComputeCosineSimilarity(double[] vector1, double[] vector2)
        {
            if (vector1.Length != vector2.Length)
                throw new Exception("DIFER LENGTH");


            double denom = (VectorLength(vector1) * VectorLength(vector2));
            if (denom == 0F)
                return 0F;
            else
                return (InnerProduct(vector1, vector2) / denom);

        }

        public static double InnerProduct(double[] vector1, double[] vector2)
        {

            if (vector1.Length != vector2.Length)
                throw new Exception("DIFFER LENGTH ARE NOT ALLOWED");


            double result = 0F;
            for (int i = 0; i < vector1.Length; i++)
                result += vector1[i] * vector2[i];

            return result;
        }

        public static double VectorLength(double[] vector)
        {
            double sum = 0.0F;
            for (int i = 0; i < vector.Length; i++)
                sum = sum + (vector[i] * vector[i]);

            return (double)Math.Sqrt(sum);
        }

        public static double[][] Normalize(double[][] vectors)
        {
            // Normalize the vectors using L2-Norm.
            List<double[]> normalizedVectors = new List<double[]>();
            foreach (var vector in vectors)
            {
                var normalized = Normalize(vector);
                normalizedVectors.Add(normalized);
            }

            return normalizedVectors.ToArray();
        }

        public static double[] Normalize(double[] vector)
        {
            List<double> result = new List<double>();

            double sumSquared = 0;
            foreach (var value in vector)
            {
                sumSquared += value * value;
            }


            double SqrtSumSquared = Math.Sqrt(sumSquared);

            foreach (var value in vector)
            {
                // L2-norm: Xi = Xi / Sqrt(X0^2 + X1^2 + .. + Xn^2)
                result.Add(SqrtSumSquared == 0 ? 0 : value / SqrtSumSquared);
            }

            return result.ToArray();
        }
    }
}
