using System.IO;
using BitMiracle.LibTiff.Classic;

namespace ExportProcess.Utilities
{
    public class TiffConversion
    {

        public static byte[] ConvertTiff(string sourceFile)
        {
            using (var ms = new MemoryStream())
            {
                
                var ts = new TiffStream();
                
                using (var input = Tiff.Open(sourceFile, "r"))
                {
                    var totalPages = input.NumberOfDirectories();
                    var pageNumber = 0;
                    using (var output = Tiff.ClientOpen("someName", Constants.LibTiff.FileMode.Write, ms, ts))
                    {
                         
                        do
                        {
                            AddPageToTiff(input, output, pageNumber, totalPages);

                            pageNumber += 1;
                        } while (input.ReadDirectory());

                        ms.Position = 0;
                        var result = new byte[ts.Size(ms)];
                        ts.Read(ms, result, 0, result.Length);
                        return result;

                    }
                }
            }

        }



        private static void AddPageToTiff(Tiff sourceFile, Tiff destinationFile, int pageNumber, int totalPages)
        {

            var width = sourceFile.GetField(TiffTag.IMAGEWIDTH)[0].ToInt();
            var height = sourceFile.GetField(TiffTag.IMAGELENGTH)[0].ToInt();
            var samplesPerPixel = sourceFile.GetField(TiffTag.SAMPLESPERPIXEL)[0].ToInt();
            var bitsPerSample = sourceFile.GetField(TiffTag.BITSPERSAMPLE)[0].ToInt();
            var photo = sourceFile.GetField(TiffTag.PHOTOMETRIC)[0].ToInt();

            var scanlineSize = sourceFile.ScanlineSize();
            var buffer = new byte[height][];
            for (var i = 0; i < height; ++i)
            {
                buffer[i] = new byte[scanlineSize];
                sourceFile.ReadScanline(buffer[i], i);
            }


            destinationFile.SetField(TiffTag.IMAGEWIDTH, width);
            destinationFile.SetField(TiffTag.IMAGELENGTH, height);
            destinationFile.SetField(TiffTag.SAMPLESPERPIXEL, samplesPerPixel);
            destinationFile.SetField(TiffTag.BITSPERSAMPLE, bitsPerSample);
            destinationFile.SetField(TiffTag.ROWSPERSTRIP, destinationFile.DefaultStripSize(0));
            destinationFile.SetField(TiffTag.PHOTOMETRIC, photo);
            destinationFile.SetField(TiffTag.PLANARCONFIG, PlanarConfig.CONTIG);
            destinationFile.SetField(TiffTag.COMPRESSION, Compression.CCITT_T6);
            // change orientation of the image
            //output.SetField(TiffTag.ORIENTATION, Orientation.RIGHTBOT);
            destinationFile.SetField(TiffTag.SAMPLESPERPIXEL, 1);

            // specify that it's a page within the multipage file
            destinationFile.SetField(TiffTag.SUBFILETYPE, FileType.PAGE);
            // specify the page number
            destinationFile.SetField(TiffTag.PAGENUMBER, pageNumber, totalPages);

            for (int j = 0; j < height; ++j)
                destinationFile.WriteScanline(buffer[j], j);


            destinationFile.WriteDirectory();

        }

    }
}
