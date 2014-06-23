using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using BitMiracle.LibTiff.Classic;

using BitMiracle;
namespace ExportProcess
{
    public class TestTiffConverter
    {
        


        public void TestConvertTiffImage()
        {
            //http://bitmiracle.com/libtiff/help/convert-color-tiff-to-a-32-bit-system.drawing.bitmap.aspx

            //  I set the script to only convert image compression to CCITT (Group Fax 4) format with a bit depth of 1
            //  The final outputted image files will have their color removed (be rendered in black and white).  I don’t think this will be a problem for Acclaris.

            //image.SetField(TiffTag.COMPRESSION, Compression.CCITT_T4);
            //compression = image.GetField(TiffTag.COMPRESSION);

            //image.SetField(TiffTag.SAMPLESPERPIXEL, 1);

            //http://bitmiracle.com/libtiff/help/read-all-tiff-tags.aspx


            //File.Copy(@"C:\Temp\PBM-JECO-2014042200014\147496_6041302.tif", @"C:\Temp\PBM-JECO-2014042200014\Converted.tif", true);

            //Test1();
            

            var files = System.IO.Directory.GetFiles(@"C:\Temp\Source","*.tif");
            foreach (var file in files)
            {
                var bytes = ConvertTiff(file);

                File.WriteAllBytes(@"c:\temp\image1.tiff", bytes);
                //Test2(file);
            }
        }


        public static byte[] ConvertTiff(string sourceFile)
        {
            var ms = new MemoryStream();
            //using (var fs = File.OpenRead(sourceFile))
            //{
            //    fs.CopyTo(ms);
            //}

            var ts = new TiffStream();

            //var destination = Path.Combine(@"c:\temp\converted", Path.GetFileName(sourceFile));
            var tiff = Tiff.ClientOpen("someName", Constants.LibTiff.FileMode.Write, ms, ts);


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


        private void Test1()
        {
            var sourceFile = @"C:\Temp\PBM-JECO-2014042200014\File 1.tif";
            var destination = @"C:\Temp\PBM-JECO-2014042200014\Converted.tif";

            using (Tiff input = Tiff.Open(sourceFile, "r"))
            {
                int width = input.GetField(TiffTag.IMAGEWIDTH)[0].ToInt();
                int height = input.GetField(TiffTag.IMAGELENGTH)[0].ToInt();
                int samplesPerPixel = input.GetField(TiffTag.SAMPLESPERPIXEL)[0].ToInt();
                int bitsPerSample = input.GetField(TiffTag.BITSPERSAMPLE)[0].ToInt();
                int photo = input.GetField(TiffTag.PHOTOMETRIC)[0].ToInt();

                int scanlineSize = input.ScanlineSize();
                byte[][] buffer = new byte[height][];
                for (int i = 0; i < height; ++i)
                {
                    buffer[i] = new byte[scanlineSize];
                    input.ReadScanline(buffer[i], i);
                }

                using (Tiff output = Tiff.Open(destination, "w"))
                {
                    output.SetField(TiffTag.IMAGEWIDTH, width);
                    output.SetField(TiffTag.IMAGELENGTH, height);
                    output.SetField(TiffTag.SAMPLESPERPIXEL, samplesPerPixel);
                    output.SetField(TiffTag.BITSPERSAMPLE, bitsPerSample);
                    output.SetField(TiffTag.ROWSPERSTRIP, output.DefaultStripSize(0));
                    output.SetField(TiffTag.PHOTOMETRIC, photo);
                    output.SetField(TiffTag.PLANARCONFIG, PlanarConfig.CONTIG);
                    output.SetField(TiffTag.COMPRESSION, Compression.CCITT_T4);
                    // change orientation of the image
                    //output.SetField(TiffTag.ORIENTATION, Orientation.RIGHTBOT);
                    output.SetField(TiffTag.SAMPLESPERPIXEL, 1);
                    for (int i = 0; i < height; ++i)
                        output.WriteScanline(buffer[i], i);
                }
            }

            
        }


        private void Test2(string sourceFile)
        {
            //var sourceFile = @"C:\Temp\PBM-JECO-2014042200014\File 1.tif";

            var destination = Path.Combine(@"c:\temp\converted", Path.GetFileName(sourceFile));


            
            using (Tiff input = Tiff.Open(sourceFile, "r"))
            {
                var totalPages = input.NumberOfDirectories();
                var pageNumber = 0;
                using (Tiff output = Tiff.Open(destination, "w"))
                {
                    do
                    {
                        AddPageToTiff(input, output, pageNumber, totalPages);


                        pageNumber += 1;
                    } while (input.ReadDirectory());
                    

                }

                
            }

        }

        private void OLD_AddPageToTiff(Tiff sourceFile, Tiff destinationFile, int pageNumber, int totalPages)
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

        public void Test3()
        {
            var input = @"C:\Temp\PBM-JECO-2014042200014\Converted.tif";

            //create an object that we can use to examine an image file
            System.Drawing.Image img = System.Drawing.Image.FromFile(input);

            //rotate the picture by 90 degrees
            //img.RotateFlip(RotateFlipType.Rotate90FlipNone);

            // load into a bitmap to save with proper compression
            var myBitmap = new Bitmap(img);
            myBitmap.SetResolution(img.HorizontalResolution, img.VerticalResolution);

            // get the tiff codec info
            var myImageCodecInfo = GetEncoderInfo("image/tiff");

            // Create an Encoder object based on the GUID for the Compression parameter category
            var myEncoder = System.Drawing.Imaging.Encoder.Compression;

            // create encode parameters
            var myEncoderParameters = new EncoderParameters(1);
            var myEncoderParameter = new EncoderParameter(myEncoder, (long)EncoderValue.CompressionCCITT4);
            myEncoderParameters.Param[0] = myEncoderParameter;

            // save as a tiff
            myBitmap.Save(@"C:\Temp\PBM-JECO-2014042200014\Converted2.tiff", myImageCodecInfo, myEncoderParameters);


        }
        // get encoder info for specified mime type
        private static ImageCodecInfo GetEncoderInfo(String mimeType)
        {
            int j;
            ImageCodecInfo[] encoders;
            encoders = ImageCodecInfo.GetImageEncoders();
            for (j = 0; j < encoders.Length; ++j)
            {
                if (encoders[j].MimeType == mimeType)
                    return encoders[j];
            }
            return null;
        }

    }
}
