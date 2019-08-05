using SplitGrid.PainLib.Interop;
using SplitGrid.PainLib.Schema.pain_002_001_03;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace SplitGrid.PainLib
{
    public class Reader
    {
        public PaymentInfo[] GetPaymentsData(string folderPath, string filePattern = "*.xml")
        {
            if (string.IsNullOrEmpty(folderPath)) return Enumerable.Empty<PaymentInfo>().ToArray();
            if (!Directory.Exists(folderPath)) return Enumerable.Empty<PaymentInfo>().ToArray();

            var files = Directory.EnumerateFiles(folderPath, filePattern);

            if(!files.Any()) return Enumerable.Empty<PaymentInfo>().ToArray();

            return ProcessFiles(files);
        }

        private PaymentInfo[] ProcessFiles(IEnumerable<string> files)
        {
            var result = new List<PaymentInfo>();
            foreach(var file in files)
            {
                var info = GetPaymentInfo(file);
                if (info != null) result.Add(info);
            }
            return result.ToArray();
        }

        private PaymentInfo GetPaymentInfo(string file)
        {
            using(var stream = File.OpenRead(file))
            {
                try
                {
                    var serializer = new XmlSerializer(typeof(Document));
                    var doc = (Document)serializer.Deserialize(stream);
                    return BuildPaymentInfoFromDocument(doc);
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }

        private PaymentInfo BuildPaymentInfoFromDocument(Document doc)
        {
            throw new NotImplementedException();
        }
    }
}