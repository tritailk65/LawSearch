using LawSearch_Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LawSearch_Core.Common
{
    public class TF_IDF_Improved
    {
        public static List<KeyPhraseResult> FindNearestNeighbors(List<KeyPhraseResult> lstResult, List<KeyPhrase> lstKeyPhrases_Searched, double minDis, int itop)
        {
            // Thêm list keyphrase câu hỏi vào cuối danh sách các điều ứng viên
            lstResult.Add(new KeyPhraseResult { ID = -1, keys = lstKeyPhrases_Searched });

            // Tạo từ điển thể hiện tần suất hiện keyphrase trong toàn bộ văn bản
            SortedDictionary<string, int> vocabulary = new();
            for (int i = 0; i < lstResult.Count; i++)
            {
                for (int j = 0; j < lstResult[i].keys.Count; j++)
                {
                    // Keyphrase
                    string sKey = lstResult[i].keys[j].Key;

                    if (!string.IsNullOrEmpty(sKey))
                    {
                        // So sánh keyphrase có trong từ điển không -> Không, thêm vào với tần suất là 1 -> Có thì tăng tần suất lên 1
                        if (!vocabulary.ContainsKey(sKey))
                            vocabulary.Add(sKey, 1);
                        else
                            vocabulary[sKey]++;
                    }
                }
            }

            // Tạo từ điển chứa tần suất thể hiện IDF của 1 keyphrase
            Dictionary<string, double> _vocabularyIDF = new();
            foreach (var term in vocabulary.Keys)
            {
                // Số tài liệu mà keyphrase xuất hiện trong đó
                double numberOfDocsContainingTerm = lstResult.Count(
                    d => d.keys.Exists(
                        x => string.Compare(x.Key, term, StringComparison.InvariantCultureIgnoreCase) == 0)
                );
                // IDF:
                _vocabularyIDF[term] = Math.Log((double)lstResult.Count / ((double)1 + numberOfDocsContainingTerm));
            }

            // Chuyển đổi các điều luật thành các vector tf-idf
            List<List<double>> vectors = new();
            foreach (var myKey in lstResult)
            {
                List<double> vector = new();

                foreach (var vocab in _vocabularyIDF)
                {
                    // So sánh keyphrase trong từ điển có trong keyphrase danh sách các điều luật 
                    KeyPhrase? key = myKey.keys.Where(
                        d => string.Compare(
                            d.Key, vocab.Key, StringComparison.InvariantCultureIgnoreCase) == 0
                        ).FirstOrDefault();

                    // Prepare
                    // TF
                    double n = 2;
                    double tf = Math.Pow(key == null ? 0 : key.Count, 1.0 / n);
                    // IDF
                    double idf = vocab.Value;
                    // bt
                    double bt = key == null ? 0 : key.WordClassWeight;
                    // Pt
                    double pt = key == null ? 0 : key.PositionWeight;
                    // TF-IDF
                    double tfidf = Math.Pow(tf, 1.0 / n) * idf * bt * pt;

                    vector.Add(tfidf);
                }

                vectors.Add(vector);
            }

            // Chuẩn hoá vector L2-Norm
            double[][] inputs = vectors.Select(v => v.ToArray()).ToArray();
            inputs = Normalize(inputs);
            var show2 = inputs;
            // Đưa các vector vào lại danh sách điều luật
            for (int i = 0; i < lstResult.Count; i++)
            {
                lstResult[i].vector = inputs[i];
            }

            // Vector cần search
            double[] search_vector = lstResult[lstResult.Count - 1].vector;

            for (int i = 0; i < lstResult.Count - 1; i++)
            {
                // Tính độ tương đồng cosine
                lstResult[i].distance = ComputeCosineSimilarity(search_vector, lstResult[i].vector);
            }
            // Sắp xếp danh sách giảm dần độ tương đồng cosine với điều kiện độ tương đồng > minDis và chỉ lấy itop thành phần đầu tiên
            List<KeyPhraseResult> lstReturn = lstResult.OrderByDescending(x => x.distance).Where(x => x.distance > minDis).Take(itop).ToList();
            return lstReturn;
        }

        public static double ComputeCosineSimilarity(double[] vector1, double[] vector2)
        {
            if (vector1.Length != vector2.Length)
                throw new Exception("DIFER LENGTH");

            // Công thức độ tương đồng consine: cs(a, b) = tích vô hướng vector a và vector b / (length vector a * length vector b)
            double denom = (VectorLength(vector1) * VectorLength(vector2));
            if (denom == 0F)
                return 0F;
            else
                return (InnerProduct(vector1, vector2) / denom);

        }

        public static double InnerProduct(double[] vector1, double[] vector2)
        {
            // Trường hợp lỗi vì đã chuẩn hoá L2 Norm mà vẫn còn khác độ dài
            if (vector1.Length != vector2.Length)
                throw new Exception("DIFFER LENGTH ARE NOT ALLOWED");

            // Công thức tích vô hướng: A[1]*B[1] + A[2]*B[2] + ... + A[n]*B[n]
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
            List<double> result = new();

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
