using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.Pkcs;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using GemBox.Pdf;
using GemBox.Pdf.Forms;

namespace PDFTest
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            //InitializeComponent();
            Höpölöpö();
        }

        /*public void Höpälööpää()
        {
            // If using Professional version, put your serial key below.
            ComponentInfo.SetLicense("FREE-LIMITED-KEY");

            using (var document = PdfDocument.Load("Reading.pdf"))
            {
                // Add a visible signature field to the first page of the PDF document.
                var PdfSignatureField = document.Form.Fields.AddSignature(document.Pages[0], 300, 500, 250, 50);

                // Retrieve signature appearance settings to customize it.
                var signatureAppearance = PdfSignatureField.Appearance;

                // Show 'Reason' label and value.
                signatureAppearance.Reason = "Legal agreement";
                // Show 'Location' label and value.
                signatureAppearance.Location = "New York, USA";
                // Do not show 'Date' label nor value.
                signatureAppearance.DateFormat = string.Empty;
                string str = "GemBoxExampleExplorer.pfx";
                byte[] data = Encoding.ASCII.GetBytes(str);
                Func<StreamGeometry, byte[]>;
                // Initiate signing of a PDF file with the specified digital ID file and the password.
                PdfSignatureField.Sign("GemBoxExampleExplorer.pfx", "GemBoxPassword", 5);

                // Finish signing of a PDF file.
                document.Save("Visible Digital Signature.pdf");
            }
        }*/

        static void Höpölöpö()
        {
            // If using Professional version, put your serial key below.
            ComponentInfo.SetLicense("FREE-LIMITED-KEY");

            using (var document = PdfDocument.Load(@"/root/SignTest/PDFTest/Hello World.pdf"))
            {
                // Add an invisible signature field to the PDF document.
                var signatureField = document.Form.Fields.AddSignature();

                // Initiate signing of a PDF file with the specified signer delegate.
                signatureField.Sign(pdfFileStream =>
                {
                    // Create a signed CMS object using the content that should be signed,
                    // but not included in the signed CMS object (detached: true).
                    var content = new byte[pdfFileStream.Length];
                    pdfFileStream.Read(content, 0, content.Length);
                    var signedCms = new SignedCms(new ContentInfo(content), detached: true);

                    X509Certificate2 certificate = null;
                    try
                    {
                        // Compute the signature using the specified digital ID file and the password.
                        certificate = new X509Certificate2(@"/root/SignTest/PDFTest/GemBoxExampleExplorer.pfx", "GemBoxPassword");
                        var cmsSigner = new CmsSigner(certificate);
                        cmsSigner.DigestAlgorithm = new Oid("2.16.840.1.101.3.4.2.1"); // SHA256
                        signedCms.ComputeSignature(cmsSigner);
                    }
                    finally
                    {
                        // Starting with the .NET Framework 4.6, this type implements the IDisposable interface.
                        (certificate as IDisposable)?.Dispose();
                    }

                    // Return the signature encoded into a CMS/PKCS #7 message.
                    return signedCms.Encode();

                }, PdfSignatureFormat.PKCS7, 2199);

                // Finish signing of a PDF file.
                document.Save(@"C:\users\joose\External Digital Signature.pdf");
            }
        }
    }
}
