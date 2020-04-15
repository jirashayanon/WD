using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ReadWriteCsv;
using System.IO;
using PhotoLibrary.Model;

namespace PhotoLibrary.Repository
{
    public class CsvDefectsData : ICsvDefectsData
    {
        /// <summary>
        /// Save Defects to csv file.
        /// </summary>
        public void SaveDefectsToCsv(string name, List<DefectDataOnImage> defectDataOnImage, string currentPath)
        {
            int count = 1;

            string fullPath = Path.Combine(Directory.GetCurrentDirectory(), name + ".csv");
            string fileNameOnly = Path.GetFileNameWithoutExtension(fullPath);
            string extension = Path.GetExtension(fullPath);
            string path = Path.GetDirectoryName(fullPath);
            string newFullPath = fullPath;

            while (File.Exists(newFullPath))
            {
                string tempFileName = string.Format("{0}({1})", fileNameOnly, count++);
                newFullPath = Path.Combine(path, tempFileName + extension);
            }

            using (CsvFileWriter writer = new CsvFileWriter(newFullPath))
            {
                // Write CurrentPath
                CsvRow row = new CsvRow();
                row.Add("Image Path");
                row.Add(currentPath);
                writer.WriteRow(row);

                // Write Header
                row = new CsvRow();
                row.Add("Round");
                row.Add("Order");
                row.Add("Picture No.");
                row.Add("View");
                row.Add("Defects");
                row.Add("Time");
                row.Add("Defect Data");
                row.Add("Filename");
                writer.WriteRow(row);

                for (int i = 0; i < defectDataOnImage.Count; i++)
                {
                    row = new CsvRow();
                    row.Add(defectDataOnImage[i].Round.ToString());
                    row.Add(defectDataOnImage[i].Order.ToString());
                    row.Add(defectDataOnImage[i].PictureNo.ToString());
                    row.Add(defectDataOnImage[i].View);
                    row.Add(defectDataOnImage[i].Defect);
                    row.Add(defectDataOnImage[i].Time.ToString());
                    row.Add(defectDataOnImage[i].DefectData);
                    row.Add(defectDataOnImage[i].filename);
                    writer.WriteRow(row);
                }
            }
        }

        /// <summary>
        /// Load Defects from csv file.
        /// </summary>
        public void LoadDefectsFromCsv(string name, List<DefectDataOnImage> defectDataOnImage)
        {
            throw new NotImplementedException();
        }
    }
}
