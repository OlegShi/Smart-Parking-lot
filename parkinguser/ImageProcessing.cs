using System;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Drawing.Imaging;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace parking
{
    class ImageProcessing
    {
        public byte[] data;

        // Computer Vision key
        // https://docs.microsoft.com/en-us/azure/cognitive-services/computer-vision/vision-api-how-to-topics/howtosubscribe
        const string ocrUriBase = "https://westeurope.api.cognitive.microsoft.com/vision/v1.0/ocr";
        // Ocr result from Azure (json response)
        public string jsonOcrResult = "";
        // ocr Result Number of the car with dots and other wrong cars
        public string hardResult = "";
        // ocr result Number cleaned
        public string cleanResult = "";
        // bouding box
        public int boxx;
        public int boxy;
        public int boxwidth;
        public int boxheigth;

        // Constructor nothing
        public ImageProcessing()
        {
        }

        // Constructor, load image from file and store content in byte[] data
        public ImageProcessing(string finename)
        {
            FileStream stream = new FileStream(finename, FileMode.Open, FileAccess.Read);
            BinaryReader reader = new BinaryReader(stream);

            data = reader.ReadBytes((int)stream.Length);

            reader.Close();
            stream.Close();
        }

        // Save Binary image in file result
        public void saveToFile(string destfile, byte[] binImage)
        {
            File.WriteAllBytes(destfile, binImage);
        }

        // load image from file to show
        public void showImageFile(string filename)
        {
            Process.Start(filename);
        }

        // Convert Binary to Hexa string
        public string Bin2Hex(byte[] buffer)
        {
            var hex = new StringBuilder(buffer.Length * 2);
            foreach (byte b in buffer)
                hex.AppendFormat("{0:x2}", b);
            return hex.ToString();
        }

        // Convert Hexa string image to Binary
        public byte[] Hex2Bin(string hexString)
        {
            int bytesCount = (hexString.Length) / 2;
            byte[] bytes = new byte[bytesCount];
            for (int x = 0; x < bytesCount; ++x)
            {
                bytes[x] = Convert.ToByte(hexString.Substring(x * 2, 2), 16);
            }

            return bytes;
        }

        // Convert image to byte array
        public byte[] imageToByteArray(Image imageIn)
        {
            MemoryStream ms = new MemoryStream();
            imageIn.Save(ms, System.Drawing.Imaging.ImageFormat.Gif);
            return ms.ToArray();
        }

        // Convert byte array to image
        public Image convertToImage(byte[] byteArrayIn)
        {
            MemoryStream ms = new MemoryStream(byteArrayIn);
            Image returnImage = Image.FromStream(ms);
            return returnImage;
        }

        public byte[] cutRegion(byte[] byteArrayIn, int x, int y, int width, int height)
        {
            MemoryStream ms = new MemoryStream(byteArrayIn);
            Image returnImage = Image.FromStream(ms);
            Bitmap source = new Bitmap(returnImage);
            Bitmap CroppedImage = source.Clone(new System.Drawing.Rectangle(x, y, width, height), source.PixelFormat);

            data = (byte[])TypeDescriptor.GetConverter(CroppedImage).ConvertTo(CroppedImage, typeof(byte[]));
            /*
            ms = new MemoryStream();
            CroppedImage.Save(ms, ImageFormat.Jpeg);
            byte[] bmpBytes = ms.GetBuffer();
            CroppedImage.Dispose();
            ms.Close();
            */
            return data;
        }

        // Convert hexa array to image
        public Image convertToImage(string hexString)
        {
            MemoryStream ms = new MemoryStream(Hex2Bin(hexString));
            Image returnImage = Image.FromStream(ms);
            return returnImage;
        }

        // Request OCR to Azure
        public void getOcrFromAzure(byte[] byteData, string key)
        {
            // Json result
            jsonOcrResult = Task.Run(() => MakeOCRRequestToAzure(byteData, key)).Result;
            // ocr Result Number of the car with dots and other wrong cars
            hardResult = getNumberOfCarFromJson(byteData, jsonOcrResult);
        }

        // Request OCR to Azure
        public async Task<string> MakeOCRRequestToAzure(byte[] byteData, string key)
        {
            string res = "";
            try
            {
                HttpClient client = new HttpClient();
                // Request headers.
                client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", key);
                // Request parameters. language fr works 
                string requestParameters = "language=fr&detectOrientation=true";

                // Assemble the URI for the REST API Call.
                string uri = ocrUriBase + "?" + requestParameters;

                HttpResponseMessage response;

                using (ByteArrayContent content = new ByteArrayContent(byteData))
                {
                    // The data requested is the image so we specify octet-stream as type of value
                    content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                    // Make the REST API call.
                    response = await client.PostAsync(uri, content);
                }

                // Get the JSON response.
                res = await response.Content.ReadAsStringAsync();
                // Example = @"{ ""language"":""fr"",""orientation"":""Up"",""textAngle"":-0.020943951023931283,""regions"":[{""boundingBox"":""6,7,237,120"",""lines"":[{""boundingBox"":""6,7,234,53"",""words"":[{""boundingBox"":""6,7,234,53"",""text"":""IAH-522-RVB""}]},{""boundingBox"":""6,73,237,54"",""words"":[{""boundingBox"":""6,73,237,54"",""text"":""IBS.029•NX9""}]}]}]}";
            }
            catch (Exception)
            {
            }
            return res;
        }

        public class matchBox
        {
            public string boundingBox { get; set; }
            public string text { get; set; }
        }

        public void ocrTest()
        {
            string test = @"{""language"":""fr"",""orientation"":""Up"",""textAngle"":0.0,""regions"":[{""boundingBox"":""1012,1184,1156,269"",""lines"":[{""boundingBox"":""1176,1184,992,208"",""words"":[{""boundingBox"":""1176,1184,992,208"",""text"":""244:19 - 901""}]},{""boundingBox"":""1012,1419,614,34"",""words"":[{""boundingBox"":""1012,1422,252,31"",""text"":""09 - 8846150""},{""boundingBox"":""1276,1424,108,25"",""text"":""n•am""},{""boundingBox"":""1404,1419,222,29"",""text"":""HONDA""}]}]}]}";
            MatchCollection match = Regex.Matches(test, @"""boundingBox""\:""([^""]*)"",""text""\:""([^""]*)""}", RegexOptions.IgnoreCase);

            for (int i = 0; i < match.Count; i++)
            {
                string res = "{"+match[i].Value;
                matchBox msgDataPoint = JsonConvert.DeserializeObject<matchBox>(res);
                boxx = boxy = boxwidth;boxheigth = 0;
            }
        }
        // Get Number of the car from json response from azure
        public string getNumberOfCarFromJson(byte[] byteData, string azureOcrresult)
        {
            string result = "";

            // We take the result with minimum 7 chars 
            // We suppose we can get severl responses, so we match all occurences
            MatchCollection match = Regex.Matches(azureOcrresult, @"""boundingBox""\:""([^""]*)"",""text""\:""([^""]*)""}", RegexOptions.IgnoreCase);

            for (int i=0; i < match.Count; i++)
            {
                string res = "{" + match[i].Value;
                matchBox msgDataPoint = JsonConvert.DeserializeObject<matchBox>(res);
                boxx = boxy = boxwidth; boxheigth = 0;

                // We take the first or the last having minimum 7 chars and not a phone number
                if (result == "" || msgDataPoint.text.Length >= 7 && msgDataPoint.text[0] != '0')
                {
                    // save region
                    string[] region = msgDataPoint.boundingBox.Split(',');
                    boxx = int.Parse(region[0]);
                    boxy = int.Parse(region[1]);
                    boxwidth = int.Parse(region[2]);
                    boxheigth = int.Parse(region[3]);
                    if (boxx >= 20) { boxx -= 20; boxwidth += 20; }
                    if (boxy >= 20) { boxy -= 20; boxheigth += 20; }
                    // Should test height, width of original
                    boxwidth += 0; boxheigth += 0;

                    // Cut the region where the number was found
                    cutRegion(byteData, boxx, boxy, boxwidth, boxheigth);
                    // ocr result Number cleaned
                    result = cleanResult = hardResult = msgDataPoint.text.Replace("'", "");
                    cleanResult = cleanResult.Replace("-", "").Replace(".", "").Replace(":", "").Replace("'", "");
                }
            }
            return result;
        }
    }
}
